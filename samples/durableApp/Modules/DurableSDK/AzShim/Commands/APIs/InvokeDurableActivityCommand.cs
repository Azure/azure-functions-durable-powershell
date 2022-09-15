//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member 'member'


namespace DurableSDK.Commands.APIs
{
    using DurableEngine;
    using System.Collections;
    using System.Management.Automation;

    /// <summary>
    /// Invoke a durable activity.
    /// </summary>
    [Cmdlet("Invoke", "DurableActivityExternal")]
    public class InvokeDurableActivityCommand : DurableSDKCmdlet
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
        public SwitchParameter NoWait { get; set; }

        [Parameter]
        [ValidateNotNull]
        public RetryOptions RetryOptions { get; set; }

        protected override DurableEngineCommand CreateDurableTask()
        {
            var privateData = (Hashtable)MyInvocation.MyCommand.Module.PrivateData;
            return new ActivityInvocationTask(FunctionName, Input, RetryOptions, NoWait, privateData);
        }
    }
}
