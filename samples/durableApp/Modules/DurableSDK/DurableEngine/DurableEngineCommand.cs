using Microsoft.DurableTask;
using System;
using System.Collections;
using System.Management.Automation;
using System.Threading.Tasks;

namespace DurableEngine
{
    // Wrapper class for Durable Task
    public abstract class DurableEngineCommand
    {
        internal Task<object> DTFxTask;

        public DurableEngineCommand(SwitchParameter noWait, Hashtable privateData)
        {
            NoWait = noWait.IsPresent;
            // MICHAELPENG TOASK: This OrchestrationContext should be passed by reference; does casting preserve this?
            OrchestrationContext = (OrchestrationContext)privateData[OrchestrationInvoker.ContextKey];
            TaskOrchestrationContext = OrchestrationContext.DTFxContext;
        }

        public bool NoWait { get; set; }

        internal OrchestrationContext OrchestrationContext { get; set; }

        internal TaskOrchestrationContext TaskOrchestrationContext { get; set; }

        internal abstract Task<object> CreateDTFxTask();

        internal virtual object Result
        {
            get { return DTFxTask.Result; }
        }

        internal virtual Exception Exception
        {
            get { return DTFxTask.Exception; }
        }

        public Task<object> GetDTFxTask()
        {
            if (DTFxTask == null)
            {
                DTFxTask = CreateDTFxTask();
            }
            return DTFxTask;
        }

        internal abstract OrchestrationAction CreateOrchestrationAction();

        public void Execute(Action<object> output, Action<ErrorRecord> writeError)
        {
            OrchestrationContext.OrchestrationActionCollector.CurrentDurableEngineCommand = this;
            OrchestrationContext.OrchestrationActionCollector.Add(CreateOrchestrationAction());
            var task = GetDTFxTask();

            if (NoWait)
            {
                output(task);
            }
            else
            {
                OrchestrationContext.OrchestrationActionCollector.NextBatch();

                // Output the result if the Durable cmdlet does not have NoWait and the task has been completed
                if (HasResult())
                {
                    if (IsFaulted())
                    {
                        var errorMessage = Exception.Message;
                        const string ErrorId = "Functions.Durable.ActivityFailure";
                        var exception = new ActivityFailureException(errorMessage);
                        var errorRecord = new ErrorRecord(exception, ErrorId, ErrorCategory.NotSpecified, null);

                        writeError(errorRecord);
                    }
                    else
                    {
                        output(Result);
                    }
                }
                else
                {
                    OrchestrationContext.OrchestrationActionCollector.ResumeOrchestrator();
                    OrchestrationContext.OrchestrationActionCollector.WaitForActivityResult();
                }
            }

            // Reset the current Durable Engine task to null once the invocation completes
            OrchestrationContext.OrchestrationActionCollector.CurrentDurableEngineCommand = null;
        }

        public void Stop()
        {
            OrchestrationContext.OrchestrationActionCollector._cancelationToken.Set();
        }

        internal virtual bool HasResult()
        {
            return DTFxTask != null && DTFxTask.IsCompleted;
        }

        internal virtual bool IsFaulted()
        {
            return DTFxTask != null && DTFxTask.IsFaulted;
        }
    }
}
