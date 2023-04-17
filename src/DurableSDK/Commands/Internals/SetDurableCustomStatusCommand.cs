//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member 'member'

namespace DurableSDK.Commands.APIs
{
    using System.Collections;
    using System.Management.Automation;
    using DurableEngine.Models;

    [Cmdlet("Set", "DurableCustomStatusE")]
    public class SetDurableCustomStatusCommand : PSCmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true)]
        public object CustomStatus { get; set; }

        protected override void EndProcessing()
        {
            var privateData = (Hashtable)MyInvocation.MyCommand.Module.PrivateData;
            var context = (OrchestrationContext)privateData["OrchestrationContext"];
            context.CustomStatus = CustomStatus;
        }
    }
}
