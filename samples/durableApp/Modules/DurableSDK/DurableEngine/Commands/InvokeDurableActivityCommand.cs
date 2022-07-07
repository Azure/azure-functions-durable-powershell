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
    /// Invoke a durable activity.
    /// </summary>
    public class InvokeDurableActivityCommand2 : DFCommand
    {
        public InvokeDurableActivityCommand2(string functionName, object input, RetryOptions retryOptions, SwitchParameter NoWait, Hashtable privateData)
            : base(NoWait, privateData)
        {
            FunctionName = functionName;
            Input = input;
            RetryOptions = retryOptions;
        }

        /// <summary>
        /// Gets and sets the activity function name.
        /// </summary>
        public string FunctionName { get; set; }

        /// <summary>
        /// Gets and sets the input for an activity function.
        /// </summary>
        public object Input { get; set; }

        internal RetryOptions RetryOptions { get; set; }

        internal override DurableSDKTask GetTask()
        {
            var context = getDTFxContext();
            var context2= getOrchestrationContext();
            var task = new ActivityInvocationTask(FunctionName, Input, RetryOptions, context, context2);
            return task;
        }
    }
}
