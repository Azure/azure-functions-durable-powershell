//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // "Missing XML comments for publicly visible type"

namespace DurableEngine.Tasks
{
    using System.Collections;
    using System.Management.Automation;
    using System.Threading.Tasks;
    
    public class ExternalEventTask : DurableTask
    {
        internal string ExternalEventName { get; }

        public ExternalEventTask(
            string externalEventName,
            SwitchParameter noWait,
            Hashtable privateData) : base(noWait, privateData)
        {
            ExternalEventName = externalEventName;
        }

        internal override Task CreateDTFxTask()
        {
            var dtfxContext = OrchestrationContext.DTFxContext;
            return dtfxContext.WaitForExternalEvent<object>(ExternalEventName);
        }

        internal override OrchestrationAction CreateOrchestrationAction()
        {
            return new ExternalEventAction(ExternalEventName);
        }
    }
}