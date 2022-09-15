//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

namespace DurableEngine
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;

    using System.Threading.Tasks;
    using System.Text.Json;
    using Microsoft.Extensions.Logging.Abstractions;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.DurableTask;
    using DurableTask.Core;
    using DurableTask.Core.Command;

    public class OrchestrationInvoker : IOrchestrationInvoker
    {
        public const string ContextKey = "OrchestrationContext";
        private OrchestrationContext _orchestrationContext;
        private bool failed = false;
        private object dfOutput = null;
        private Exception dfEx = null;

        public OrchestrationInvoker(Hashtable privateData)
        {
            _orchestrationContext = (OrchestrationContext)privateData[ContextKey];
        }

        private sealed class OrchestratorState
        {
            internal string? InstanceId { get; set; }

            internal IList<global::DurableTask.Core.History.HistoryEvent>? PastEvents { get; set; }

            internal IList<global::DurableTask.Core.History.HistoryEvent>? NewEvents { get; set; }

            internal int? UpperSchemaVersion { get; set; }
        }

        public Func<PowerShell, object> CreateInvokerFunction()
        {
            // return (pwsh) => Invoke(new PowerShellServices(pwsh, _orchestrationContext));
            return (pwsh) => Invoke(new PowerShellServices(pwsh));
        }

        public Hashtable Invoke(IPowerShellServices powerShellServices)
        {
            
            // Is wrapping the user's orchestration function with this orchestration function necessary?
            Func<TaskOrchestrationContext, object, Task<object>> orchestratorFunction = async (TaskOrchestrationContext taskOrchestrationContext, object _) =>
            {
                // MICHAELPENG TOASK: How can we ensure that the taskOrchestrationContext fields (IsReplaying) agrees with the OrchestrationContext that is passed in?
                _orchestrationContext.DTFxContext = taskOrchestrationContext;
                // MICHAELPENG TOASK: Double check to make sure that _orchestrationContext is not able to be changed by the user at runtime
                powerShellServices.AddParameter("Context", _orchestrationContext);
                // MICHAELPENG TOASK: What does this do, exactly?
                powerShellServices.TracePipelineObject();
                var outputBuffer = new PSDataCollection<object>();
                var asyncResult = powerShellServices.BeginInvoke(outputBuffer);

                while (true)
                {
                    var (shouldStop, actions) = _orchestrationContext.OrchestrationActionCollector.WaitForActions(asyncResult.AsyncWaitHandle);

                    // The orchestrator should only await when we reach a new Durable activity; at this point, the thread corresponding to execution
                    // of the user code should be waiting
                    if (shouldStop)
                    {
                        // Stop the user code
                        powerShellServices.StopInvoke();
                        var task = _orchestrationContext.OrchestrationActionCollector.CurrentDurableEngineCommand;
                        object taskResult = null;

                        // MICHAELPENG TOASK: Ask how do these resources ever get freed if the DTFxTask never reaches a final value
                        // This does not schedule the activity; returning actions at the end schedules the invocation of activities (worker-powered)

                        // Try to read and return the result if the task has been completed
                        taskResult = await task.GetDTFxTask();
                        return taskResult;
                    }
                    // The orchestration has completed
                    else
                    {
                        try
                        {
                            // MICHAELPENG TOASK: Why do we need to call EndInvoke if the asyncResult has already completed?
                            // Cross-reference this with what is present in the PS Worker
                            powerShellServices.EndInvoke(asyncResult);
                            return CreateReturnValueFromFunctionOutput(outputBuffer);
                        }
                        catch (Exception e)
                        {
                            // The orchestrator code has thrown an unhandled exception: this should be treated as an entire orchestration failure
                            // MICHAELPENG TOASK: How can we ensure that this exception is handled properly upon being returned?
                            throw new OrchestrationFailureException(actions, _orchestrationContext.CustomStatus, e);
                        }
                    }
                }
            };

            var durableTaskExecutor = CreateOrchestrationExecutor(_orchestrationContext, orchestratorFunction);
            // var durableTaskExecutor = CreateOrchestrationExecutor(_orchestrationContext, powershellServices.BeginInvoke());

            OrchestratorExecutionResult orchestratorExecutionResult = durableTaskExecutor.Execute();
            
            return CreateOrchestrationResult(orchestratorExecutionResult);
        }

        private Hashtable CreateOrchestrationResult(OrchestratorExecutionResult result)
        {
            bool isDone = result.Actions.All((x) => x.OrchestratorActionType == OrchestratorActionType.OrchestrationComplete);

            var actions = _orchestrationContext.OrchestrationActionCollector._actions;
            if (failed)
            {
                throw new OrchestrationFailureException(actions, _orchestrationContext.CustomStatus, dfEx);
            }

            return CreateOrchestrationResult(isDone: isDone, actions, output: dfOutput, _orchestrationContext.CustomStatus);
        }

        private TaskOrchestrationExecutor CreateOrchestrationExecutor(
            OrchestrationContext context,
            Func<TaskOrchestrationContext, object, Task<object>> orchestratorFunction)
        {
            OrchestratorState state = new OrchestratorState();
            state.NewEvents = context.History;  // Simplfying assumption
            state.InstanceId = context.InstanceId;
            state.UpperSchemaVersion = 2;

            // Re-construct the orchestration state from the history.
            OrchestrationRuntimeState runtimeState = new(state.PastEvents);
            foreach (global::DurableTask.Core.History.HistoryEvent newEvent in state.NewEvents)
            {
                runtimeState.AddEvent(newEvent);
            }

            IServiceProvider emptyServiceProvider = new ServiceCollection().BuildServiceProvider();
            JsonDataConverter dataConverter = JsonDataConverter.Default;
            var logger = NullLoggerFactory.Instance.CreateLogger("gg"); //Extensions.Logging.Abstractions.NullLoggerFactory.Instance.CreateLogger("PowerShell DF: ");
            //ILogger logger2 = new NullLogger();

            WorkerContext workerContext = new(dataConverter, logger, emptyServiceProvider);


            TaskName orchestratorName = new TaskName(runtimeState.Name, runtimeState.Version);
            FuncTaskOrchestrator<object, object> f = new(orchestratorFunction);
            TaskOrchestrationShim orchestrator = new(workerContext, orchestratorName, f);
            return new(runtimeState, orchestrator, BehaviorOnContinueAsNew.Carryover);
        }

        internal static object CreateReturnValueFromFunctionOutput(IList<object> pipelineItems)
        {
            if (pipelineItems == null || pipelineItems.Count <= 0)
            {
                return null;
            }

            return pipelineItems.Count == 1 ? pipelineItems[0] : pipelineItems.ToArray();
        }

        private static Hashtable CreateOrchestrationResult(
            bool isDone,
            List<List<OrchestrationAction>> actions,
            object output,
            object customStatus)
        {
            var orchestrationMessage = new OrchestrationMessage(isDone, actions, output, customStatus);
            return new Hashtable { { "$return", orchestrationMessage } };
        }


        internal class JsonDataConverter : DataConverter
        {
            // WARNING: Changing default serialization options could potentially be breaking for in-flight orchestrations.
            static readonly JsonSerializerOptions DefaultOptions = new()
            {
                IncludeFields = true,
            };

            /// <summary>
            /// An instance of the <see cref="JsonDataConverter"/> with default configuration.
            /// </summary>
            internal static JsonDataConverter Default { get; } = new JsonDataConverter();

            readonly JsonSerializerOptions? options;

            JsonDataConverter(JsonSerializerOptions? options = null)
            {
                if (options != null)
                {
                    this.options = options;
                }
                else
                {
                    this.options = DefaultOptions;
                }
            }

            /// <inheritdoc/>
            public override string? Serialize(object? value)
            {
                return value != null ? System.Text.Json.JsonSerializer.Serialize(value, this.options) : null;
            }

            /// <inheritdoc/>
            public override object? Deserialize(string? data, Type targetType)
            {
                return data != null ? System.Text.Json.JsonSerializer.Deserialize(data, targetType, this.options) : null;
            }
        }
    }
}
