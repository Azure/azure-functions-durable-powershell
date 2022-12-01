//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member 'member'

namespace DurableSDK.Commands.APIs
{
    using DurableEngine.Tasks;
    using System.Management.Automation;

    /// <summary>
    /// Stop the Durable Timer
    /// </summary>
    [Cmdlet("Stop", "DurableTimerTaskE")]
    public class StopDurableTimerCommand : PSCmdlet
    {
        /// <summary>
        /// Gets and sets the timer to be stopped.
        /// </summary>
        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public DurableTimerTask Task { get; set; }

        protected override void EndProcessing()
        {
            Task.Cancel();
        }
    }
}