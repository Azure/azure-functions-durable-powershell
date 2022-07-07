//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member 'member'

namespace DurableEngine.Commands
{
    using System.Collections;
    using System.Management.Automation;

    [Cmdlet("Set", "DurableCustomStatus")]
    internal class SetDurableCustomStatusCommand : PSCmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true)]
        internal object CustomStatus { get; set; }

        protected override void EndProcessing()
        {
            var privateData = (Hashtable)MyInvocation.MyCommand.Module.PrivateData;
            var context = (OrchestrationContext)privateData[SetFunctionInvocationContextCommand.ContextKey];
            context.CustomStatus = CustomStatus;
        }
    }
}
