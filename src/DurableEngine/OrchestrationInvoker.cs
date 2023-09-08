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
    using Microsoft.DurableTask;
    using DurableEngine.Utilities;
    using DurableEngine.Models;
    using OrchestrationContext = Models.OrchestrationContext;
    using static DurableEngine.Utilities.DTFxUtilities;
    using DurableTask.Core;
    using DurableTask.Core.Command;

    using Microsoft.DurableTask.Worker.Shims;
    using Microsoft.DurableTask.Worker;

    public class OrchestrationInvoker
    {
        public const string ContextKey = "OrchestrationContext";
        private OrchestrationContext context;

        private bool orchestratorFailed = false;
        private object orchestratorOutput = null;
        private Exception orchestratorException = null;
        private readonly DurableTaskShimFactory shimFactory;


        public OrchestrationInvoker(Hashtable privateData)
        {
            context = (OrchestrationContext)privateData[ContextKey];
            DurableTaskWorkerOptions workerOptions = new DurableTaskWorkerOptions { DataConverter = new JsonDataConverter() };
            this.shimFactory = new DurableTaskShimFactory(options: workerOptions);
        }

        public Func<PowerShell, object> CreateInvokerFunction()
        {
            return (pwsh) => Invoke(new PowerShellServices(pwsh));
        }

        /// <summary>
        /// Manages the execution of the DF orchestrator.
        /// </summary>
        internal Hashtable Invoke(IPowerShellServices powerShellServices)
        {
            // A C# orchestrator Function that translates PowerShell DF APIs into C# DF APIs.
            // We need this function because DTFx expects an orchestrator implemented in C#. Therefore, this is that orchestrator.
            // This function receives DTFx Tasks to `await` on behalf of the PS orchestrator, which runs on a parallel thread.
            //
            // This function will block its thread until the user-code thread (the PS orchestrator) returns or requests the result of
            // DF API.
            // Similarly, the user code thread / PS orchestrator will block its own thread until this function is done `await`'ing
            // the requested APIs.
            Func<TaskOrchestrationContext, object, Task<object>> apiInvokerFunction = async (TaskOrchestrationContext DTFxContext, object _) =>
            {
                context.DTFxContext = DTFxContext;

                // Parameterize user code thread with DF context object
                powerShellServices.AddParameter("Context", context);
                powerShellServices.TracePipelineObject();

                // Start user-code thread, which contains the "actual" PS orchestrator
                var outputBuffer = new PSDataCollection<object>();

                var asyncResult = powerShellServices.BeginInvoke(outputBuffer);
                var orchestratorReturnedHandle = asyncResult.AsyncWaitHandle;

                // waiting for tasks to await
                while (true)
                {
                    // block this thread until user-code thread (the PS orchestrator) invokes a DF CmdLet or completes.
                    var orchestratorReturned = context.SharedMemory.WaitForInvokerThreadTurn(orchestratorReturnedHandle);
                    if (orchestratorReturned)
                    {
                        // The PS orchestrator has a return value, there's no more DF APIs to await.
                        try
                        {
                            // Collect the result from the user-code thread.
                            powerShellServices.EndInvoke(asyncResult);
                            orchestratorOutput = CreateReturnValueFromFunctionOutput(outputBuffer);
                            return null;
                        }
                        catch (Exception e)
                        {
                            // The orchestrator code has thrown an unhandled exception.
                            // We record it and return.
                            orchestratorException = e;
                            orchestratorFailed = true;
                            return null;
                        }
                    }

                    // The PS orchestrator is requesting a DF Task to be awaited.
                    var task = context.SharedMemory.currTask;

                    try
                    {
                        // Invoke the DTFx API corresponding to this Task.
                        // If there's already a result in the History, then the loop will continue.
                        // If there's no result in the History, DTFx will terminate this thread and will return from its
                        // `Execute` method.
                        await task.GetDTFxTask();
                    } // Exceptions are ignored at this point, they will be re-surfaced by the PS code if left unhandled.
                    catch { }

                    // Wake up user-code thread. For a small moment, both the user code thread and the invoker thread
                    // will be running at the same time.
                    // However, the invoker thread will block itself again at the start of the next loop until the user-code
                    // thread yields control.
                    context.SharedMemory.WakeUserCodeThread();
                }
            };

            // Construct and then invoke DTFx executor, which will replay for us.
            TaskOrchestrationExecutor executor = CreateTaskOrchestrationExecutor(apiInvokerFunction);
            var DTFxResult = executor.Execute();

            // After the DTFx executor completes, we can terminate the user-code thread and construct
            // the orchestrator output payload
            powerShellServices.StopInvoke();
            var result = CreateOrchestrationResult(DTFxResult);
            return result;
        }

        /// <summary>
        /// Creates a DTFx orchestrator executor, which allows DTFx to manage orchestration replay.
        /// Most of the code here is unavoidable boilerplate to prepare DTFx for invocation.
        /// </summary>
        /// <param name="apiInvokerFunction">A C# Function that calls DF APIs.</param>
        /// <returns>An orchestrator executor implementing DF replay.</returns>
        private TaskOrchestrationExecutor CreateTaskOrchestrationExecutor(Func<TaskOrchestrationContext, object, Task<object>> apiInvokerFunction)
        {
            // Construct the OrchestratorState object. The key here is to correctly distinguish new events from past ones.
            OrchestratorState state = new OrchestratorState();
            var lastPlayedIndex = Array.FindLastIndex(context.History, (historyEvent) => historyEvent.IsPlayed == true);
            var newEventsIndex = lastPlayedIndex + 1;
            state.PastEvents = context.History[..newEventsIndex];
            state.NewEvents = context.History[newEventsIndex..];
            state.InstanceId = context.InstanceId;
            state.UpperSchemaVersion = 2;

            // Re-construct the runtime state from the history.
            OrchestrationRuntimeState runtimeState = new(state.PastEvents);
            foreach (global::DurableTask.Core.History.HistoryEvent newEvent in state.NewEvents)
            {
                runtimeState.AddEvent(newEvent);
            }

            // construct orchestration shim, a DTFx concept.
            TaskName orchestratorName = new TaskName(runtimeState.Name);
            var orchestratorShim = shimFactory.CreateOrchestration(orchestratorName, apiInvokerFunction);

            // construct executor
            TaskOrchestrationExecutor executor = new(runtimeState, orchestratorShim, BehaviorOnContinueAsNew.Carryover);
            return executor;
        }


        /// <summary>
        /// Extract the orchestrator return-value from the PS pipeline.
        /// </summary>
        /// <param name="pipelineItems">The pipeline items.</param>
        /// <returns>The elements of the pipeline if any exist. Otherwise null.</returns>
        private object CreateReturnValueFromFunctionOutput(IList<object> pipelineItems)
        {
            if (pipelineItems == null || pipelineItems.Count <= 0)
            {
                return null;
            }

            return pipelineItems.Count == 1 ? pipelineItems[0] : pipelineItems.ToArray();
        }

        /// <summary>
        /// Create the orchestrator result payload that the PS worker will communicate to the DF Extension.
        /// </summary>
        /// <param name="result">The DTFx executor result.</param>
        /// <returns>The return payload, if the orchestrator was successful.</returns>
        /// <exception cref="OrchestrationFailureException">A formatted exception, if the orchestrator failed.</exception>
        private Hashtable CreateOrchestrationResult(OrchestratorExecutionResult result)
        {
            // If the orchestrator is complete, then DTFx result will be a single action of type OrchestrationComplete
            bool isDone = result.Actions.All((x) => x.OrchestratorActionType == OrchestratorActionType.OrchestrationComplete);

            // For legacy reasons, the DF extension expects the actions array to be a doubly-nested list.
            var actions = context.SharedMemory.actions;
            var extensionActions = new List<List<OrchestrationAction>>();
            extensionActions.Add(actions);

            if (orchestratorFailed)
            {
                throw new OrchestrationFailureException(extensionActions, context.CustomStatus, orchestratorException);
            }

            var orchestrationMessage = new OrchestrationMessage(isDone, extensionActions, orchestratorOutput, context.CustomStatus);
            return new Hashtable { { "$return", orchestrationMessage } };
        }

    }
}