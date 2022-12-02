//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // "Missing XML comments for publicly visible type"

namespace DurableEngine.Tasks
{
    using System;
    using System.Collections;
    using System.Management.Automation;
    using System.Threading;
    using System.Threading.Tasks;

    // Returned by the Start-DurableTimer cmdlet if the NoWait flag is present, representing a timeout task
    // All DurableTimerTasks must be complete or canceled for the orchestration to complete
    public class DurableTimerTask : DurableTask
    {
        internal TimeSpan Duration { get; }
        private DateTime FireAt { get; }
        private CreateDurableTimerAction Action { get; set; }
        private readonly CancellationTokenSource _cancelationTokenSource = new CancellationTokenSource();

        // Only incomplete, uncanceled DurableTimerTasks should be created
        public DurableTimerTask(
            TimeSpan duration,
            SwitchParameter noWait,
            Hashtable privateData) : base(noWait, privateData)
        {
            Duration = duration;
            FireAt = OrchestrationContext.CurrentUtcDateTime.Add(Duration);
        }

        internal override Task CreateDTFxTask()
        {
            var dtfxContext = OrchestrationContext.DTFxContext;
            return dtfxContext.CreateTimer(Duration, _cancelationTokenSource.Token);
        }

        internal override OrchestrationAction CreateOrchestrationAction()
        {
            Action = new CreateDurableTimerAction(FireAt);
            return Action;
        }

        internal override bool HasResult()
        {
            // To replicate the behavior of Start-Sleep, Start-DurableTimer should never
            // have a result to the output pipe.
            return false;
        }

        public void Cancel()
        {
            Action.IsCanceled = true;
            _cancelationTokenSource.Cancel();
        }
    }
}