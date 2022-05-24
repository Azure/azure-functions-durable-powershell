using DurableTask;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Commands;
using Microsoft.DurableTask.Tasks;
using System.Collections;
using System.Linq;
using System.Management.Automation;

namespace DurableSDK.Commands
{
    public abstract class DFCmdlet : PSCmdlet
    {
        [Parameter]
        public SwitchParameter NoWait { get; set; }

        internal OrchestrationContext getOrchestrationContext()
        {
            var privateData = (Hashtable)MyInvocation.MyCommand.Module.PrivateData;
            var context = (OrchestrationContext)privateData[SetFunctionInvocationContextCommand.ContextKey];
            return context;
        }

        internal TaskOrchestrationContext getDTFxContext()
        {
            var privateData = (Hashtable)MyInvocation.MyCommand.Module.PrivateData;
            TaskOrchestrationContext context = (TaskOrchestrationContext)privateData["dtfx"];
            return context;
        }

        internal abstract DurableSDKTask GetTask();

        protected override void EndProcessing()
        {
            DurableSDKTask task = GetTask();
            object result = null;

            if (NoWait)
            {
                result = task;
            }
            else
            {

                var context = getOrchestrationContext();

                context.OrchestrationActionCollector.currTask = task;
                if (task is WhenAllTask compositeTask)
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
                }
                else if (task is WhenAnyTask compositeTask2)
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
                if (sdkTask.hasResult()) {  // works for failed?

                    // clear IAsync signals
                    result = context.OrchestrationActionCollector.currTask.Result;
                    context.OrchestrationActionCollector.currTask = null;
                    context.OrchestrationActionCollector.hasToAwait.Reset();

                    // send result to user code
                    context.OrchestrationActionCollector.startOfNewCmdLet.Set();
                }

            }
            WriteObject(result);
        }

        protected override void StopProcessing()
        {
            var context = getOrchestrationContext();
            context.OrchestrationActionCollector.cancelationToken.Set();
        }

    }
}
