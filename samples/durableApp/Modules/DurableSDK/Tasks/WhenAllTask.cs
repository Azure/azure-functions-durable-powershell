using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DurableTask;
using Microsoft.DurableTask.Actions;

namespace Microsoft.DurableTask.Tasks
{
    public class WhenAllTask : DurableSDKTask
    {
        internal DurableSDKTask[] tasks;

        private TaskOrchestrationContext context;
        private object[] _result;
        private Task _task;
        private OrchestrationContext context2;

        internal override bool hasResult()
        {
            return _task != null ? _task.IsCompleted : false;
        }

        internal override object Result {
            get
            {
                List<object> allResults = new List<object>();
                foreach (var subTask in this.tasks)
                {
                    var res = subTask.Result;
                    allResults.Add(res);
                }
                return allResults; // cache
            }
        }

        internal WhenAllTask(DurableSDKTask[] tasks, TaskOrchestrationContext context, OrchestrationContext context2)
        {
            this.tasks = tasks;
            this.context = context;
            this.context2 = context2;
        }

        internal override OrchestrationAction CreateOrchestrationAction()
        {
            throw new NotImplementedException();
        }

        internal override Task getDTFxTask()
        {
            if (this._task != null)
            {
                return this._task;
            }

            var innertasks = this.tasks.Select(task => task.getDTFxTask());
            this._task = Task.WhenAll(innertasks);
            this.context2.OrchestrationActionCollector.taskMap.Add(_task, this);
            return this._task;
        }
    }
}
