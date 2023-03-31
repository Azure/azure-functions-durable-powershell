using DurableEngine.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DurableEngine.Tasks
{
    /// <summary>
    /// An SDK-level Durable Functions Task.
    /// </summary>
    public abstract class DurableTask
    {
        public DurableTask(SwitchParameter noWait, Hashtable privateData)
        {
            NoWait = noWait;
            OrchestrationContext = (OrchestrationContext)privateData[OrchestrationInvoker.ContextKey];
        }

        /// <summary>
        /// The orchestrator context.
        /// </summary>
        internal OrchestrationContext OrchestrationContext { get; set; }

        /// <summary>
        /// If present, the Task does not need to be awaited.
        /// </summary>
        public SwitchParameter NoWait { get; set; }

        /// <summary>
        /// Resolves the Task to a value depending on how it was invoked
        /// and feeds it to the pipeline.
        /// 
        /// If the NoWait flag is present, the value is the Task itself.
        /// Otherwise, the value is the result of `await`'ing the Task.
        /// </summary>
        /// <param name="write">Function to write a value to the pipeline.</param>
        /// <param name="writeErr">Function to write an exception to the pipeline.</param>
        public void Execute(Action<object> write, Action<ErrorRecord> writeErr)
        {
            DurableTask task = this;

            if (NoWait)
            {
                // Task doesn't need to be awaited, just feed it to pipeline
                write(task);
            }
            else
            {
                // Flag this task as the current "task-to-await"
                OrchestrationContext.SharedMemory.currTask = task;
                OrchestrationContext.SharedMemory.Add(task.CreateOrchestrationAction());

                // Signal orchestration thread to await the Task.
                // This is necessary for DTFx to determine if a result exists for the Task.
                OrchestrationContext.SharedMemory.YieldToInvokerThread();

                var sdkTask = OrchestrationContext.SharedMemory.currTask;
                if (sdkTask.HasResult())
                {
                    if (sdkTask.IsFaulted())
                    {
                        // Feed formatted exception to pipeline
                        var errorMessage = OrchestrationContext.SharedMemory.currTask.Exception.Message;
                        const string ErrorId = "Functions.Durable.ActivityFailure";
                        var exception = new ActivityFailureException(errorMessage);
                        var errorRecord = new ErrorRecord(exception, ErrorId, ErrorCategory.NotSpecified, null);
                        writeErr(errorRecord);
                    }
                    else
                    {
                        var result = OrchestrationContext.SharedMemory.currTask.Result;

                        // TODO: improve this de-serialization logic. Might need C#-isolated support
                        // We need to re-serialize to the raw string and then de-serialize via Newtonsoft to get a proper type
                        if  (result is List<object> list) {
                            result = list.Select(e => e is JsonElement res ? JsonConvert.DeserializeObject(res.GetRawText()) : e);
                        }
                        if (result is JsonElement res)
                        {
                            result = JsonConvert.DeserializeObject(res.GetRawText());
                        }

                        // Feed result to pipeline
                        write(result);
                    }
                }
                // Reset the current task-to-await once the invocation completes
                OrchestrationContext.SharedMemory.currTask = null;
            }
        }

        /// <summary>
        /// Stop the execution of this Task
        /// </summary>
        public void Stop()
        {
            // We unblock the user-code thread to avoid a deadlock.
            OrchestrationContext.SharedMemory.userCodeThreadTurn.Set();
        }

        /// <summary>
        /// The result of this Task, if applicable.
        /// </summary>
        internal virtual object Result
        {
            get { return ((Task<object>)DTFxTask).Result; }
        }

        /// <summary>
        /// Exception thrown by this Task, if applicable.
        /// </summary>
        internal virtual Exception Exception
        {
            get { return DTFxTask.Exception; }
        }


        /// <summary>
        /// Obtain  a new DTFx Task corresponding to this SDK-level Task.
        /// This should only be invoked once per SDK-level Task.
        /// </summary>
        internal abstract Task CreateDTFxTask();

        /// <summary>
        /// Get DTFx Task corresponding to this SDK-level task.
        /// </summary>
        /// <returns>The corresponding DTFxTask.</returns>
        public Task GetDTFxTask()
        {
            // If the Task wasn't generated before
            if (DTFxTask == null)
            {
                // Create it and store it in map.
                // We need the map to convert the output of DTFx.WhenAny back to SDK tasks
                DTFxTask = CreateDTFxTask();
                OrchestrationContext.SharedMemory.taskMap.Add(DTFxTask, this);
            }
            return DTFxTask;
        }

        internal Task DTFxTask;

        /// <summary>
        /// Whether this Task finished executing.
        /// </summary>
        internal virtual bool HasResult()
        {
            return DTFxTask != null && DTFxTask.IsCompleted;
        }

        /// <summary>
        /// Whether this Task threw an Exception.
        /// </summary>
        internal virtual bool IsFaulted()
        {
            return DTFxTask != null && DTFxTask.IsFaulted;
        }

        /// <summary>
        /// Create Action object representing this Task.
        /// </summary>
        internal abstract OrchestrationAction CreateOrchestrationAction();

    }
}