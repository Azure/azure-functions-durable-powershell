//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member 'member'

namespace Microsoft.DurableTask.Commands
{
    using System.Management.Automation;
    using DurableSDK.Commands;
    using Microsoft.DurableTask.Tasks;

    [Cmdlet("Wait", "DurableTask")]
    public class WaitDurableTaskCommand : DFCmdlet
    {
        [Parameter(Mandatory = true)]
        [ValidateNotNull]
        public DurableSDKTask[] Task { get; set; }

        [Parameter]
        public SwitchParameter Any { get; set; }

        internal override DurableSDKTask GetTask()
        {
            var context = getDTFxContext();
            var context2 = getOrchestrationContext();
            DurableSDKTask task = null;
            if (Any)
            {
                task = new WhenAnyTask(Task, context, context2);
            }
            else
            {
                task = new WhenAllTask(Task, context, context2);
            }
            return task;
        }

    }
}
