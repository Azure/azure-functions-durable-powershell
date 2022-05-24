//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member 'member'

namespace Microsoft.DurableTask.Commands
{
    using System.Management.Automation;
    using Microsoft.DurableTask.Tasks;
    using DurableSDK.Commands;
    using Microsoft.DurableTask;

    /// <summary>
    /// Invoke a durable activity.
    /// </summary>
    [Cmdlet("Invoke", "DurableActivity")]
    public class InvokeDurableActivityCommand : DFCmdlet
    {
        /// <summary>
        /// Gets and sets the activity function name.
        /// </summary>
        [Parameter(Mandatory = true)]
        public string FunctionName { get; set; }

        /// <summary>
        /// Gets and sets the input for an activity function.
        /// </summary>
        [Parameter]
        [ValidateNotNull]
        public object Input { get; set; }

        [Parameter]
        [ValidateNotNull]
        public RetryOptions RetryOptions { get; set; }

        internal override DurableSDKTask GetTask()
        {
            var context = getDTFxContext();
            var context2= getOrchestrationContext();
            var task = new ActivityInvocationTask(FunctionName, Input, RetryOptions, context, context2);
            return task;
        }
    }
}
