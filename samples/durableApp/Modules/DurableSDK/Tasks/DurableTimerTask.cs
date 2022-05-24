//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // "Missing XML comments for publicly visible type"

namespace Microsoft.DurableTask.Tasks
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using DurableTask;
    using global::DurableTask;
    using Microsoft.DurableTask.Actions;

    // Returned by the Start-DurableTimer cmdlet if the NoWait flag is present, representing a timeout task
    // All DurableTimerTasks must be complete or canceled for the orchestration to complete
    public class DurableTimerTask : DurableSDKTask
    {
        internal DateTime FireAt { get; }

        private CreateDurableTimerAction Action { get; }

        private TaskOrchestrationContext context;
        private OrchestrationContext context2;
        private CancellationTokenSource cancellationToken;


        // Only incomplete, uncanceled DurableTimerTasks should be created
        internal DurableTimerTask(
            DateTime fireAt, TaskOrchestrationContext context, OrchestrationContext context2)
        {
            FireAt = fireAt;
            Action = new CreateDurableTimerAction(FireAt);
            this.context = context;
            this.context2 = context2;
            this.cancellationToken = new CancellationTokenSource();
        }


        internal override OrchestrationAction CreateOrchestrationAction()
        {
            return Action;
        }

        // Indicates that the task has been canceled; without this, the orchestration will not terminate until the timer has expired
        internal void Cancel()
        {
            Action.IsCanceled = true;
            this.cancellationToken.Cancel();
        }

        private Task _task;

        internal override object Result
        {
            get
            {
                return null;
            }
        }


        internal override Task getDTFxTask()
        {
            if (this._task != null)
            {
                return this._task;
            }
            else
            {
                this._task = this.context.CreateTimer(this.FireAt, this.cancellationToken.Token);
                context2.OrchestrationActionCollector.taskMap.Add(_task, this);
                return _task;
            }
        }

        internal override bool hasResult()
        {
            return _task != null ? _task.IsCompleted : false;
        }
    }
}
