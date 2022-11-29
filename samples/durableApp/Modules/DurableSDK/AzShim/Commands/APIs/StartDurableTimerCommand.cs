//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member 'member'

namespace DurableSDK.Commands.APIs
{
    using DurableEngine.Tasks;
    using System;
    using System.Collections;
    using System.Management.Automation;

    /// <summary>
    /// Start the Durable Functions timer
    /// </summary>
    [Cmdlet("Start", "DurableTimerE")]
    public class StartDurableTimerCommand : DurableSDKCmdlet
    {
        /// <summary>
        /// Gets and sets the duration of the Durable Timer.
        /// </summary>
        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public TimeSpan Duration { get; set; }

        [Parameter]
        public SwitchParameter NoWait { get; set; }

        internal override DurableTask CreateDurableTask()
        {
            var privateData = (Hashtable)MyInvocation.MyCommand.Module.PrivateData;
            var task = new DurableTimerTask(Duration, NoWait, privateData);
            return task;
        }
    }
}
