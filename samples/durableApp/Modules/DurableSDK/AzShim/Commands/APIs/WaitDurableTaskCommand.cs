//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

//#pragma warning disable 1591 // Missing XML comment for publicly visible type or member 'member'


//namespace DurableSDK.Commands.APIs
//{
//    using DurableEngine;
//    using DurableEngine.Commands;
//    using System.Collections;
//    using System.Management.Automation;

//    [Cmdlet("Wait", "DurableTaskE")]
//    public class WaitDurableTaskCommand : DFCmdlet
//    {

//        [Parameter]
//        public SwitchParameter Any { get; set; }

//        [Parameter]
//        public SwitchParameter NoWait { get; set; }

//        [Parameter(Mandatory = true)]
//        [ValidateNotNull]
//        public DurableSDKTask[] Task { get; set; }

//        internal override DFCommand GetCommand()
//        {
//            var privateData = (Hashtable)MyInvocation.MyCommand.Module.PrivateData;
//            var cmd = new WaitDurableTaskCommand2(Task, Any, NoWait, privateData);
//            return cmd;
//        }
//    }
//}
