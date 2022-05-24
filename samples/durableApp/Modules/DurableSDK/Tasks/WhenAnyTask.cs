using System;
using System.Linq;
using System.Threading.Tasks;
using DurableTask;
using Microsoft.DurableTask.Actions;

namespace Microsoft.DurableTask.Tasks
{
    public class WhenAnyTask : DurableSDKTask
    {
        internal DurableSDKTask[] tasks;

        private TaskOrchestrationContext context;
        private OrchestrationContext context2;


        internal WhenAnyTask(DurableSDKTask[] tasks, TaskOrchestrationContext context, OrchestrationContext context2)
        {
            this.tasks = tasks;
            this.context = context;
            this.context2 = context2;
        }

        internal override OrchestrationAction CreateOrchestrationAction()
        {
            throw new NotImplementedException();
        }

        internal override bool hasResult()
        {
            return _task != null ? _task.IsCompleted : false;
        }

        internal override object Result
        {
            get
            {
                Task winner = this._task.Result;
                context2.OrchestrationActionCollector.taskMap.TryGetValue(winner, out var result);
                return result;
            }
        }

        private Task<Task> _task;

        internal override Task getDTFxTask()
        {
            if (_task != null)
            {
                return _task;
            }

            var innertasks = this.tasks.Select(task => task.getDTFxTask());
            this._task = Task.WhenAny(innertasks);
            this.context2.OrchestrationActionCollector.taskMap.Add(_task, this);
            return this._task;
        }
    }
}
