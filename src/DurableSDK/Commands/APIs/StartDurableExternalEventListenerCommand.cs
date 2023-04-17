//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member 'member'

namespace DurableSDK.Commands.APIs
{
    using DurableEngine.Models;
    using DurableEngine.Tasks;
    using System.Collections;
    using System.Management.Automation;
    
    /// <summary>
    /// Start the Durable External Event Listener
    /// </summary>
    [Cmdlet("Start", "DurableExternalEventListener")]
    public class StartDurableExternalEventListenerCommand : DurableSDKCmdlet
    {
        /// <summary>
        /// Gets and sets the EventName of the external event to listen for
        /// </summary>
        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string EventName { get; set; }

        [Parameter]
        public SwitchParameter NoWait { get; set; }

        internal override DurableTask CreateDurableTask()
        {
            var privateData = (Hashtable)MyInvocation.MyCommand.Module.PrivateData;
            var task = new ExternalEventTask(EventName, NoWait, privateData);
            return task;
        }
    }
}
