using Microsoft.DurableTask;
using System;
using System.Collections;
using System.Linq;
using System.Management.Automation;

namespace DurableEngine
{
    public abstract class DFCmdlet
    {
        public DFCmdlet(SwitchParameter noWait, Hashtable privateData)
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
            TaskOrchestrationContext context = (TaskOrchestrationContext)privateData["dtfx"];
            return context;
        }

        internal abstract DurableSDKTask GetTask();

        public void Exec(Action<object> write, Action<ErrorRecord> writeErr)
        {
            DurableSDKTask task = GetTask();
            object result = null;

            if (NoWait)
            {
                result = task;
                write(result);
            }
            else
            {
                var context = getOrchestrationContext();

                context.OrchestrationActionCollector.currTask = task;
                /*if (task is WhenAllTask compositeTask)
                {

                    var innerActions = compositeTask.tasks.Select((task) => {
                        //var dtfxTask = task.getDTFxTask();
                        //context.OrchestrationActionCollector.taskMap.Add(dtfxTask, task);
                        return task.CreateOrchestrationAction();
                    });
                    foreach (var action in innerActions)
                    {
                        context.OrchestrationActionCollector.Add(action);
                    }
                }*/
                if (task is WhenAnyTask compositeTask2)
                {
                    var innerActions = compositeTask2.tasks.Select((task) => {
                        //var dtfxTask = task.getDTFxTask();
                        //context.OrchestrationActionCollector.taskMap.Add(dtfxTask, task);
                        return task.CreateOrchestrationAction();
                    });
                    foreach (var action in innerActions)
                    {
                        context.OrchestrationActionCollector.Add(action);

                    }
                }
                else
                {
                    context.OrchestrationActionCollector.Add(task.CreateOrchestrationAction());

                }
                context.OrchestrationActionCollector.NextBatch();
                //var dtfxTask = task.getDTFxTask();
                //context.OrchestrationActionCollector.taskMap.Add(dtfxTask, task);


                // signal to orchestration thread to await
                context.OrchestrationActionCollector.hasToAwait.Set();

                // wait for result
                //var handlers = new[] { context.OrchestrationActionCollector.cancelationToken }; //, taskHasResult };
                //WaitHandle.WaitAny(handlers);
                context.OrchestrationActionCollector.cancelationToken.WaitOne();

                // logic (this wil lbock cuz of .Result) do this in invoker!
                var sdkTask = context.OrchestrationActionCollector.currTask;
                if (sdkTask.hasResult())
                {  // works for failed?
                    if (sdkTask.isFaulted())
                    {
                        var errorMessage = context.OrchestrationActionCollector.currTask.Exception.Message;
                        const string ErrorId = "Functions.Durable.ActivityFailure";
                        var exception = new ActivityFailureException(errorMessage);
                        var errorRecord = new ErrorRecord(exception, ErrorId, ErrorCategory.NotSpecified, null);

                        // clear IAsync signals
                        context.OrchestrationActionCollector.currTask = null;
                        context.OrchestrationActionCollector.hasToAwait.Reset();

                        // send result to user code
                        context.OrchestrationActionCollector.startOfNewCmdLet.Set();
                        writeErr(errorRecord);
                    }
                    else
                    {
                        result = context.OrchestrationActionCollector.currTask.Result;

                        // clear IAsync signals
                        context.OrchestrationActionCollector.currTask = null;
                        context.OrchestrationActionCollector.hasToAwait.Reset();

                        // send result to user code
                        context.OrchestrationActionCollector.startOfNewCmdLet.Set();
                        write(result);
                    }
                }
            }
            //write(result);
        }

        public void Stop()
        {
            var context = getOrchestrationContext();
            context.OrchestrationActionCollector.cancelationToken.Set();
        }

    }
}
