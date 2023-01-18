//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member 'member'


namespace DurableSDK.Commands.APIs
{
    using DurableEngine;
    using DurableEngine.Tasks;
    using System.Collections;
    using System.Management.Automation;

    /// <summary>
    /// Invoke a durable activity.
    /// </summary>
    [Cmdlet("Invoke", "DurableActivityE")]
    public class InvokeDurableActivityCommand : DurableSDKCmdlet
    {
        /// <summary>
        /// Name of the Activity to invoke.
        /// </summary>
        [Parameter(Mandatory = true)]
        public string FunctionName { get; set; }

        /// <summary>
        /// The input for the Activity.
        /// </summary>
        [Parameter]
        [ValidateNotNull]
        public object Input { get; set; }

        /// <summary>
        /// Retry configuration for the Activity.
        /// </summary>
        [Parameter]
        [ValidateNotNull]
        public RetryOptions RetryOptions { get; set; }

        /// <summary>
        /// If provided, the Task will block and be scheduled immediately.
        /// Otherwise, a Task object is returned and the Task is not scheduled yet.
        /// </summary>
        [Parameter]
        public SwitchParameter NoWait { get; set; }

        internal override DurableTask CreateDurableTask()
        {
            var privateData = (Hashtable)MyInvocation.MyCommand.Module.PrivateData;
            ActivityInvocationTask task = new ActivityInvocationTask(FunctionName, Input, RetryOptions, NoWait, privateData);
            return task;
        }
    }
}