//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member 'member'


namespace DurableSDK.Commands.APIs
{
    using DurableEngine;
    using DurableEngine.Commands;
    using System;
    using System.Collections;
    using System.Management.Automation;

    /// <summary>
    /// Invoke a durable activity.
    /// </summary>
    [Cmdlet("Invoke", "DurableActivityE")]
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
        [ValidateNotNull]
        public RetryOptions RetryOptions { get; set; }

        [Parameter]
        public SwitchParameter NoWait { get; set; }

        internal override DFCommand CreateDurableTask()
        {
            var privateData = (Hashtable)MyInvocation.MyCommand.Module.PrivateData;
            ActivityInvocationTask task = new ActivityInvocationTask(FunctionName, Input, null, NoWait, privateData);
            return task;
        }
    }
}
