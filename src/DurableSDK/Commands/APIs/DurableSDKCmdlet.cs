//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System.Management.Automation;

namespace DurableSDK.Commands.APIs
{
    public abstract class DurableSDKCmdlet : PSCmdlet
    {
        // Create DurableTask object that backs this API.
        internal abstract DurableEngine.Tasks.DurableTask CreateDurableTask();

        // Task object corresponding to this API
        private DurableEngine.Tasks.DurableTask durableTask;

        protected override void EndProcessing()
        {
            // Create Task corresponding to this API and execute it.
            durableTask = CreateDurableTask();
            durableTask.Execute(WriteObject, WriteError);
        }

        protected override void StopProcessing()
        {
            // Cancel the execution of this Task.
            durableTask.Stop();
        }
    }
}