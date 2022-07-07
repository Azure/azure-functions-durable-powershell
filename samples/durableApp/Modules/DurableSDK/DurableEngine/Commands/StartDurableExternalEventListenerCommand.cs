//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member 'member'

namespace DurableEngine.Commands
{
    using DurableEngine;
    using System.Collections;
    using System.Management.Automation;

    /// <summary>
    /// Start the Durable External Event Listener
    /// </summary>
    /*internal class StartDurableExternalEventListenerCommand : DFCommand
    {
        public StartDurableExternalEventListenerCommand(string eventName, SwitchParameter noWait, Hashtable privateData) : base(noWait, privateData)
        {
            EventName = eventName;
        }

        /// <summary>
        /// Gets and sets the EventName of the external event to listen for
        /// </summary>
        internal string EventName { get; set; }

        internal override DurableSDKTask GetTask()
        {
            var context = getDTFxContext();
            var context2 = getOrchestrationContext();
            var task = new ExternalEventTask(EventName, context, context2);
            return task;
        }
    }*/
}
