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
    using System.Threading;
    using System.Text.Json;
    using Microsoft.Extensions.Logging.Abstractions;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.DurableTask;
    using DurableTask.Core;
    using DurableTask.Core.Command;
    using Newtonsoft.Json;

    public class OrchestrationInvoker //: IOrchestrationInvoker
    {
        public const string ContextKey = "OrchestrationContext";
        private OrchestrationContext context;

        public OrchestrationInvoker(Hashtable privateData)
        {
            context = (OrchestrationContext)privateData[ContextKey];
        }

        public Func<PowerShell, object> CreateInvokerFunction()
        {
            // return (pwsh) => Invoke(new PowerShellServices(pwsh, _orchestrationContext));
            return (pwsh) => InvokeExternal(new PowerShellServices(pwsh));
        }

        private sealed class OrchestratorState
        {
        internal string? InstanceId { get; set; }

        internal IList<global::DurableTask.Core.History.HistoryEvent>? PastEvents { get; set; }

        internal IList<global::DurableTask.Core.History.HistoryEvent>? NewEvents { get; set; }

        internal int? UpperSchemaVersion { get; set; }
        }

        internal Hashtable InvokeExternal(IPowerShellServices powerShellServices)
        {
            object dfOutput = null;
            Exception dfEx = null;
            bool failed = false;
            // var cSource = new TaskCompletionSource();
            Func<TaskOrchestrationContext, int, Task<object>> orchestratorFunc = async (TaskOrchestrationContext dtxContent, int _) =>
            {
                var myFunc = () => dtxContent.CallActivityAsync<object>("Hello", "Seattle");
                // start user code
                IAsyncResult asyncResult = null;
                context.DTFxContext = dtxContent;
                powerShellServices.AddParameter("Context", context);
                powerShellServices.TracePipelineObject();
                var outputBuffer = new PSDataCollection<object>();
                asyncResult = powerShellServices.BeginInvoke(outputBuffer);

                // waiting for tasks to await
                var loop = true;
                while (loop)
                {
                    var gottaAwait = context.OrchestrationActionCollector.hasToAwait;
                    var orchDone = asyncResult.AsyncWaitHandle;
                    var index = 0;
                    try
                    {
                        index = WaitHandle.WaitAny(new[] { orchDone, gottaAwait });
                    }
                    catch (Exception e)
                    {
                        return null;
                    }

                    if (index == 0)
                    {
                        try
                        {
                            powerShellServices.EndInvoke(asyncResult);
                            dfOutput = CreateReturnValueFromFunctionOutput(outputBuffer);
                            return null;
                        }
                        catch (Exception e)
                        {
                            // The orchestrator code has thrown an unhandled exception:
                            // this should be treated as an entire orchestration failure
                            //throw new OrchestrationFailureException(actions, context.CustomStatus, e);
                            dfEx = e;
                            failed = true;
                            return null;
                        }
                    }

                    var sdkTask = context.OrchestrationActionCollector.currTask;
                    //var task = sdkTask.getDTFxTask();

                    context.OrchestrationActionCollector.cancelationToken.Reset();

                    var task = context.OrchestrationActionCollector.currTask;

                    try
                    {
                        await task.GetDTFxTask();
                    }
                    catch { }
                    context.OrchestrationActionCollector.cancelationToken.Set();
                    /*var winner = await Task.WhenAny(task, cSource.Task);
                    if (winner == cSource.Task)
                    {
                        loop = false;
                        context.OrchestrationActionCollector.goodToEnd.Set();
                        return null;
                    }*/
                }

                return null;

                            // end user code thread
                            // powerShellServices.StopInvoke();
                            // return null;
            };

            OrchestratorState state = new OrchestratorState();
            state.NewEvents = context.History;  // Simplfying assumption
            state.InstanceId = context.InstanceId;
            state.UpperSchemaVersion = 2;

            //FunctionsWorkerContext workerContext = new(JsonDataConverter.Default);

            // Re-construct the orchestration state from the history.
            OrchestrationRuntimeState runtimeState = new(state.PastEvents);
            foreach (global::DurableTask.Core.History.HistoryEvent newEvent in state.NewEvents)
            {
                runtimeState.AddEvent(newEvent);
            }

            //TaskOrchestrationShim b; 

            IServiceProvider emptyServiceProvider = new ServiceCollection().BuildServiceProvider();
            JsonDataConverter dataConverter = JsonDataConverter.Default;
            var logger = NullLoggerFactory.Instance.CreateLogger("gg"); //Extensions.Logging.Abstractions.NullLoggerFactory.Instance.CreateLogger("PowerShell DF: ");
            //ILogger logger2 = new NullLogger();

            WorkerContext workerContext = new(dataConverter, logger, emptyServiceProvider);


            TaskName orchestratorName = new TaskName(runtimeState.Name, runtimeState.Version);
            FuncTaskOrchestrator<int, object> f = new(orchestratorFunc);
            TaskOrchestrationShim orchestrator = new(workerContext, orchestratorName, f);
            TaskOrchestrationExecutor executor = new(runtimeState, orchestrator, BehaviorOnContinueAsNew.Carryover);

            OrchestratorExecutionResult result2 = executor.Execute();
            powerShellServices.StopInvoke();
            bool isDone = result2.Actions.All((x) => x.OrchestratorActionType == OrchestratorActionType.OrchestrationComplete);

            var actions = context.OrchestrationActionCollector._actions;
            if (failed)
            {
                throw new OrchestrationFailureException(actions, context.CustomStatus, dfEx);
            }

            var result = CreateOrchestrationResult(isDone: isDone, actions, output: dfOutput, context.CustomStatus);
            return result;
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
