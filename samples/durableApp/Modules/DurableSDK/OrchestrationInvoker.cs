//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

namespace Microsoft.DurableTask
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;

    using Microsoft.DurableTask.Actions;
    using System.Threading.Tasks;
    using System.Threading;
    using global::DurableTask;
    using global::DurableTask.Core;
    using global::DurableTask.Core.Command;

    internal class OrchestrationInvoker : IOrchestrationInvoker
    {

        private sealed class FunctionsWorkerContext : IWorkerContext
        {
            public FunctionsWorkerContext(IDataConverter dataConverter)
            {
                this.DataConverter = dataConverter;
            }

            public IDataConverter DataConverter { get; }
        }

        private sealed class OrchestratorState
        {
            public string? InstanceId { get; set; }

            public IList<global::DurableTask.Core.History.HistoryEvent>? PastEvents { get; set; }

            public IList<global::DurableTask.Core.History.HistoryEvent>? NewEvents { get; set; }

            internal int? UpperSchemaVersion { get; set; }
        }

        public Hashtable InvokeExternal(OrchestrationContext context, IPowerShellServices powerShellServices, object privateData)
        {
            // var cSource = new TaskCompletionSource();
            Func<TaskOrchestrationContext, int, Task<object>> orchestratorFunc = async (TaskOrchestrationContext dtxContent, int _) =>
            {
                var myFunc = () => dtxContent.CallActivityAsync<object>("Hello", "Seattle");
                // start user code
                IAsyncResult asyncResult = null;
                ((Hashtable)privateData)["dtfx"] = dtxContent;
                context.innercontext = dtxContent;
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
                    var index = WaitHandle.WaitAny(new[] { orchDone, gottaAwait });
                    if (index == 0)
                    {
                        powerShellServices.EndInvoke(asyncResult);
                        return null;
                    }

                    var sdkTask = context.OrchestrationActionCollector.currTask;
                    //var task = sdkTask.getDTFxTask();

                    context.OrchestrationActionCollector.cancelationToken.Reset();

                    var task = context.OrchestrationActionCollector.currTask;
                    await task.getDTFxTask();
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

            FunctionsWorkerContext workerContext = new(JsonDataConverter.Default);

            // Re-construct the orchestration state from the history.
            OrchestrationRuntimeState runtimeState = new(state.PastEvents);
            foreach (global::DurableTask.Core.History.HistoryEvent newEvent in state.NewEvents)
            {
                runtimeState.AddEvent(newEvent);
            }

            TaskName orchestratorName = new TaskName(runtimeState.Name, runtimeState.Version);
            FuncTaskOrchestrator<int, object> f = new(orchestratorFunc);
            TaskOrchestrationShim orchestrator = new(workerContext, orchestratorName, f);
            TaskOrchestrationExecutor executor = new(runtimeState, orchestrator, BehaviorOnContinueAsNew.Carryover);

            OrchestratorExecutionResult result2 = executor.Execute();
            powerShellServices.StopInvoke();

            bool isDone = result2.Actions.All((x) => x.OrchestratorActionType == OrchestratorActionType.OrchestrationComplete);
            return CreateOrchestrationResult(isDone: isDone, context.OrchestrationActionCollector._actions, output: null, context.CustomStatus);
        }

        public static object CreateReturnValueFromFunctionOutput(IList<object> pipelineItems)
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
    }
}
