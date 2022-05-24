//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member 'member'

namespace Microsoft.DurableTask.Commands
{
    using System.Management.Automation;
    using DurableSDK.Commands;
    using Microsoft.DurableTask.Tasks;

    /// <summary>
    /// Start the Durable External Event Listener
    /// </summary>
    [Cmdlet("Start", "DurableExternalEventListener")]
    public class StartDurableExternalEventListenerCommand : DFCmdlet
    {
        /// <summary>
        /// Gets and sets the EventName of the external event to listen for
        /// </summary>
        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string EventName { get; set; }

        [Parameter]
        public SwitchParameter NoWait { get; set; }

        internal override DurableSDKTask GetTask()
        {
            var context = getDTFxContext();
            var context2 = getOrchestrationContext();
            var task = new ExternalEventTask(EventName, context, context2);
            return task;
        }
    }
}
