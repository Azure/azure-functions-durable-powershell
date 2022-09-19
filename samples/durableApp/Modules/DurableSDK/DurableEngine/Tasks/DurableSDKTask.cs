//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // "Missing XML comments for publicly visible type"

namespace DurableEngine
{
    using System;
    using System.Threading.Tasks;
    using DurableEngine.Models;

    public abstract class DurableSDKTask
    {
        private OrchestrationContext sdkContext;

        public DurableSDKTask(OrchestrationContext sdkContext)
        {
            this.sdkContext = sdkContext;
        }

        internal abstract OrchestrationAction CreateOrchestrationAction();

        internal abstract Task createDTFxTask();
        internal Task dtfxTask;


        internal virtual bool hasResult()
        {
            return dtfxTask != null && this.dtfxTask.IsCompleted;
        }

        internal virtual bool isFaulted()
        {
            return dtfxTask != null && this.dtfxTask.IsFaulted;
        }

        internal abstract object Result
        {
            get;

        }
        internal abstract Exception Exception
        {
            get;
        }

        public Task getDTFxTask()
        {
            if (this.dtfxTask == null)
            {
                this.dtfxTask = createDTFxTask();
                // this.sdkContext.OrchestrationActionCollector.taskMap.Add(this.dtfxTask, this);

            }
            return this.dtfxTask;
        }


    }
}
