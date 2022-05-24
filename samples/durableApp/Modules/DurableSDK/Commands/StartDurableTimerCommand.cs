//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member 'member'

namespace Microsoft.DurableTask.Commands
{
    using System;
    using System.Management.Automation;
    using DurableSDK.Commands;
    using Microsoft.DurableTask.Tasks;

    /// <summary>
    /// Start the Durable Functions timer
    /// </summary>
    [Cmdlet("Start", "DurableTimer")]
    public class StartDurableTimerCommand : DFCmdlet
    {
        /// <summary>
        /// Gets and sets the duration of the Durable Timer.
        /// </summary>
        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public TimeSpan Duration { get; set; }

        [Parameter]
        public SwitchParameter NoWait { get; set; }

        internal override DurableSDKTask GetTask()
        {
            var context = getDTFxContext();
            var context2 = getOrchestrationContext();
            DateTime fireAt = context2.CurrentUtcDateTime.Add(Duration);
            var task = new DurableTimerTask(fireAt, context, context2);
            return task;
        }

    }
}
