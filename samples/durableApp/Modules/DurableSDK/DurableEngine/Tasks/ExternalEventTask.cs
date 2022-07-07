//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // "Missing XML comments for publicly visible type"

namespace DurableEngine
{
    using Microsoft.DurableTask;
    using System.Threading.Tasks;


    /*internal class ExternalEventTask : DurableSDKTask
    {
        internal string ExternalEventName { get; }

        private TaskOrchestrationContext context;
        private OrchestrationContext context2;

        internal ExternalEventTask(string externalEventName, TaskOrchestrationContext context, OrchestrationContext context2)
        {
            ExternalEventName = externalEventName;
            this.context = context;
            this.context2 = context2;
        }

        internal override OrchestrationAction CreateOrchestrationAction()
        {
            return new ExternalEventAction(ExternalEventName);
        }

        internal override Task getDTFxTask()
        {
            if (this.typedDTFxTask != null)
            {
                return this.typedDTFxTask;
            }
            else
            {
                this.typedDTFxTask = this.context.WaitForExternalEvent<object>(ExternalEventName);
                context2.OrchestrationActionCollector.taskMap.Add(typedDTFxTask, this);
                return typedDTFxTask;

            }
        }
    }*/
}
