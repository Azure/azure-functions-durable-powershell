//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member 'member'

namespace DurableEngine.Commands
{
    using DurableEngine;
    using System;
    using System.Collections;
    using System.Management.Automation;

    /// <summary>
    /// Start the Durable Functions timer
    /// </summary>
    /*internal class StartDurableTimerCommand : DFCommand
    {
        public StartDurableTimerCommand(TimeSpan duration, SwitchParameter noWait, Hashtable privateData) : base(noWait, privateData)
        {
            Duration = duration;
        }

        /// <summary>
        /// Gets and sets the duration of the Durable Timer.
        /// </summary>
        internal TimeSpan Duration { get; set; }

        internal override DurableSDKTask GetTask()
        {
            var context = getDTFxContext();
            var context2 = getOrchestrationContext();
            DateTime fireAt = context2.CurrentUtcDateTime.Add(Duration);
            var task = new DurableTimerTask(fireAt, context, context2);
            return task;
        }

    }*/
}
