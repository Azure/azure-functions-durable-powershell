//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member 'member'


namespace DurableSDK.Commands.APIs
{
    using DurableEngine.Tasks;
    using System.Collections;
    using System.Management.Automation;

    [Cmdlet("Wait", "DurableTaskE")]
    public class WaitDurableTaskCommand : DurableSDKCmdlet
    {
        [Parameter]
        public SwitchParameter Any { get; set; }

        [Parameter]
        public SwitchParameter NoWait { get; set; }

        [Parameter(Mandatory = true)]
        [ValidateNotNull]
        public DurableTask[] Task { get; set; }

        internal override DurableTask CreateDurableTask()
        {
            var privateData = (Hashtable)MyInvocation.MyCommand.Module.PrivateData;
            DurableTask task;
            if (Any)
            {
                task = new WhenAnyTask(Task, NoWait, privateData);
            }
            else
            {
                task = new WhenAllTask(Task, NoWait, privateData); 
           }
           return task;
       }
   }
}
