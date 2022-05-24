//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // "Missing XML comments for publicly visible type"

namespace Microsoft.DurableTask.Tasks
{
    using System.Threading.Tasks;
    using DurableTask;
    using global::DurableTask;
    using Microsoft.DurableTask.Actions;

    public class ExternalEventTask : DurableSDKTask
    {
        internal string ExternalEventName { get; }

        private TaskOrchestrationContext context;
        private OrchestrationContext context2;

        public ExternalEventTask(string externalEventName, TaskOrchestrationContext context, OrchestrationContext context2)
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
            if (this.dtfxTask != null)
            {
                return this.dtfxTask;
            }
            else
            {
                this.dtfxTask = this.context.WaitForExternalEvent<object>(ExternalEventName);
                context2.OrchestrationActionCollector.taskMap.Add(dtfxTask, this);
                return dtfxTask;

            }
        }
    }
}
