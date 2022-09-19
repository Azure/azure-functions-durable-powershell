using Microsoft.DurableTask;
using System;
using System.Collections;
using System.Linq;
using System.Management.Automation;

namespace DurableEngine
{
    public abstract class DFCommand
    {
        public DFCommand(SwitchParameter noWait, Hashtable privateData)
        {
            NoWait = noWait;
            PrivateData = privateData;
        }

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

        internal abstract DurableSDKTask GetTask();

        public void Exec(Action<object> write, Action<ErrorRecord> writeErr)
        {
            DurableSDKTask task = GetTask();

            if (NoWait)
            {
                write(task);
            }
            else
            {
                var context = getOrchestrationContext();

                context.OrchestrationActionCollector.currTask = task;
                context.OrchestrationActionCollector.Add(task.CreateOrchestrationAction());

                // signal to orchestration thread to await
                context.OrchestrationActionCollector.WaitForActivityResult();

                var sdkTask = context.OrchestrationActionCollector.currTask;
                if (sdkTask.hasResult())
                {
                    object result = null;
                    if (sdkTask.isFaulted())
                    {
                        var errorMessage = context.OrchestrationActionCollector.currTask.Exception.Message;
                        const string ErrorId = "Functions.Durable.ActivityFailure";
                        var exception = new ActivityFailureException(errorMessage);
                        var errorRecord = new ErrorRecord(exception, ErrorId, ErrorCategory.NotSpecified, null);
                        writeErr(errorRecord);
                    }
                    else
                    {
                        result = context.OrchestrationActionCollector.currTask.Result;
                        write(result);
                    }
                }
                // Reset the current Durable Engine task to null once the invocation completes
                context.OrchestrationActionCollector.currTask = null;
                context.OrchestrationActionCollector.ResumeInvoker(); //TODO: ensure no race condition here. Maybe use semaphore?


            }
        }

        public void Stop()
        {
            var context = getOrchestrationContext();
            context.OrchestrationActionCollector.cancelationToken.Set();
        }

    }
}
