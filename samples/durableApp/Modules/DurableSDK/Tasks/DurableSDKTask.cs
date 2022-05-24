//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // "Missing XML comments for publicly visible type"

namespace Microsoft.DurableTask.Tasks
{
    using Microsoft.DurableTask.Actions;
    using System.Threading.Tasks;

    public abstract class DurableSDKTask
    {
        internal abstract OrchestrationAction CreateOrchestrationAction();
        internal virtual object Result {
            get
            {
                return this.dtfxTask.Result;
            }
        }
        internal abstract Task getDTFxTask();
        internal Task<object> dtfxTask;
        internal virtual bool hasResult()
        {
            return this.dtfxTask.IsCompleted;
        }


    }
}
