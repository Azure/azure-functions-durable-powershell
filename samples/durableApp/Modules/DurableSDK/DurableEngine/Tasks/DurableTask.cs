﻿using DurableEngine.Models;
using System;
using System.Collections;
using System.Management.Automation;
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
        /// If present, the Task does not need to be awaited.
        /// </summary>
        public SwitchParameter NoWait { get; set; }

        /// <summary>
        /// The orchestrator context.
        /// </summary>
        internal OrchestrationContext OrchestrationContext { get; set; }

        /// <summary>
        /// The underlying DTFx Task.
        /// </summary>
        private Task DTFxTask;

        /// <summary>
        /// The result of this Task, if applicable.
        /// </summary>
        internal virtual object Result
        {
            get
            {
                // Getting Result for an incomplete Task is a synchronous operation,
                // so we add this check to avoid blocking on an imcomplete Task.
                if (!HasResult())
                {
                    // TODO: Refine the exception thrown here.
                    throw new Exception("This task is not complete.");
                }
                return ((Task<object>)GetDTFxTask()).Result;
            }
        }

        /// <summary>
        /// Create Action object representing this Task.
        /// </summary>
        internal abstract OrchestrationAction CreateOrchestrationAction();

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
            if (NoWait)
            {
                // Task doesn't need to be awaited, just feed it to pipeline
                write(this);
            }
            else
            {
                var sharedMemory = OrchestrationContext.SharedMemory;
                // Flag this task as the current "task-to-await"
                sharedMemory.currTask = this;
                sharedMemory.Add(CreateOrchestrationAction());

                // Signal orchestration thread to await the Task.
                // This is necessary for DTFx to determine if a result exists for the Task.
                sharedMemory.YieldToInvokerThread();

                if (IsCompleted())
                {
                    if (IsFaulted())
                    {
                        // Feed formatted exception to pipeline
                        var errorMessage = Exception.Message;
                        // TODO: Replace this error ID with something more indicative
                        const string ErrorId = "Functions.Durable.ActivityFailure";
                        var exception = new ActivityFailureException(errorMessage);
                        var errorRecord = new ErrorRecord(exception, ErrorId, ErrorCategory.NotSpecified, null);
                        writeErr(errorRecord);
                    }
                    // Some tasks (like DurableTimers may not have results). In these cases, we should continue
                    // once the task completes
                    else if (HasResult())
                    {
                        // Feed result to pipeline
                        write(Result);
                    }
                }
                // Reset the current task-to-await once the invocation completes
                sharedMemory.currTask = null;
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
        /// Exception thrown by this Task, if applicable.
        /// </summary>
        internal virtual Exception Exception
        {
            get { return GetDTFxTask().Exception; }
        }

        /// <summary>
        /// Obtain a new DTFx Task corresponding to this SDK-level Task.
        /// This should only be invoked once per SDK-level Task.
        /// </summary>
        internal abstract Task CreateDTFxTask();

        /// <summary>
        /// Gets the DTFx Task corresponding to this SDK-level task.
        /// </summary>
        /// <returns>
        /// The corresponding DTFxTask.
        /// </returns>
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

        /// <summary>
        /// Whether this Task finished executing AND has a result to write to the
        /// output pipe. This should always be called before getting Result.
        /// </summary>
        internal virtual bool HasResult()
        {
            return GetDTFxTask().IsCompleted;
        }

        /// <summary>
        /// Whether this Task is completed.
        /// </summary>
        internal virtual bool IsCompleted()
        {
            return GetDTFxTask().IsCompleted;
        }

        /// <summary>
        /// Whether this Task threw an Exception.
        /// </summary>
        internal virtual bool IsFaulted()
        {
            return GetDTFxTask().IsFaulted;
        }
    }
}