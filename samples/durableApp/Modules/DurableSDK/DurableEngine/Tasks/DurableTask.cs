using DurableEngine.Models;
using Microsoft.DurableTask;
using System;
using System.Collections;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

namespace DurableEngine.Tasks
{
    public abstract class DurableTask
    {
        public DurableTask(SwitchParameter noWait, Hashtable privateData)
        {
            NoWait = noWait;
            PrivateData = privateData;
            OrchestrationContext = (OrchestrationContext)privateData[OrchestrationInvoker.ContextKey];
            TaskOrchestrationContext = OrchestrationContext.DTFxContext;
        }

        internal OrchestrationContext OrchestrationContext { get; set; }
        internal TaskOrchestrationContext TaskOrchestrationContext { get; set; }

        public SwitchParameter NoWait { get; set; }

        public Hashtable PrivateData { get; set; }

        internal OrchestrationContext getOrchestrationContext()
        {
            //var privateData = (Hashtable)MyInvocation.MyCommand.Module.PrivateData;
            var privateData = PrivateData;
            var context = (OrchestrationContext)privateData["OrchestrationContext"];
            return context;
        }

        internal TaskOrchestrationContext getDTFxContext()
        {
            //var privateData = (Hashtable)MyInvocation.MyCommand.Module.PrivateData;
            var privateData = PrivateData;
            var context2 = getOrchestrationContext();
            TaskOrchestrationContext context = context2.DTFxContext;
            return context;
        }

        public void Execute(Action<object> write, Action<ErrorRecord> writeErr)
        {
            DurableTask task = this;

            if (NoWait)
            {
                write(task);
            }
            else
            {
                var context = getOrchestrationContext();

                context.SharedMemory.currTask = task;
                context.SharedMemory.Add(task.CreateOrchestrationAction());

                // signal to orchestration thread to await
                context.SharedMemory.YieldToInvokerThread();

                var sdkTask = context.SharedMemory.currTask;
                if (sdkTask.HasResult())
                {
                    object result = null;
                    if (sdkTask.IsFaulted())
                    {
                        var errorMessage = context.SharedMemory.currTask.Exception.Message;
                        const string ErrorId = "Functions.Durable.ActivityFailure";
                        var exception = new ActivityFailureException(errorMessage);
                        var errorRecord = new ErrorRecord(exception, ErrorId, ErrorCategory.NotSpecified, null);
                        writeErr(errorRecord);
                    }
                    else
                    {
                        result = context.SharedMemory.currTask.Result;
                        write(result);
                    }
                }
                // Reset the current Durable Engine task to null once the invocation completes
                context.SharedMemory.currTask = null;
                //context.OrchestrationActionCollector.BlockInvokerThread(); //TODO: ensure no race condition here. Maybe use semaphore?


            }
        }

        public void Stop()
        {
            var context = getOrchestrationContext();
            context.SharedMemory.userCodeThreadTurn.Set();
        }

        internal virtual object Result
        {
            get { return ((Task<object>)DTFxTask).Result; }
        }

        internal virtual Exception Exception
        {
            get { return DTFxTask.Exception; }
        }

        internal abstract Task CreateDTFxTask();

        public Task GetDTFxTask()
        {
            if (DTFxTask == null)
            {
                DTFxTask = CreateDTFxTask();
                OrchestrationContext.SharedMemory.taskMap.Add(DTFxTask, this);
            }
            return DTFxTask;
        }

        internal Task DTFxTask;

        internal virtual bool HasResult()
        {
            return DTFxTask != null && DTFxTask.IsCompleted;
        }

        internal virtual bool IsFaulted()
        {
            return DTFxTask != null && DTFxTask.IsFaulted;
        }

        internal abstract OrchestrationAction CreateOrchestrationAction();

    }
}
