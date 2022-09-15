// //
// // Copyright (c) Microsoft. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.
// //

// #pragma warning disable 1591 // Missing XML comment for publicly visible type or member 'member'


// namespace DurableSDK.Commands.APIs
// {
//     using DurableEngine;
//     using System.Collections;
//     using System.Management.Automation;

//     [Cmdlet("Wait", "DurableTask")]
//     public class WaitDurableTaskCommand : DurableSDKCmdlet
//     {
//         [Parameter(Mandatory = true)]
//         [ValidateNotNull]
//         public DurableEngineCommand[] Task { get; set; }

//         [Parameter]
//         public SwitchParameter Any { get; set; }

//         [Parameter]
//         public SwitchParameter NoWait { get; set; }

//         [Parameter]
//         [ValidateNotNull]
//         public RetryOptions RetryOptions { get; set; }

//         protected override DurableEngineCommand CreateDurableTask()
//         {
//             var privateData = (Hashtable)MyInvocation.MyCommand.Module.PrivateData;
//             if (Any.IsPresent)
//             {
//                 return new WaitAnyTask(Task, privateData);
//             }
//             else
//             {
//                 return new WaitAllTask(Task, privateData);
//             }
//         }
//     }
// }
