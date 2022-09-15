// //
// // Copyright (c) Microsoft. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.
// //

// #pragma warning disable 1591 // "Missing XML comments for publicly visible type"

// namespace DurableEngine
// {
//     using System;
//     using System.Collections;
//     using System.Collections.Generic;
//     using System.Linq;
//     using System.Management.Automation;
//     using System.Threading.Tasks;

//     public class WhenAllTask : DurableEngineCommand
//     {
//         public DurableEngineCommand[] Task;

//         private RetryOptions RetryOptions { get; }

//         public WhenAllTask(
//             DurableEngineCommand[] task,
//             RetryOptions retryOptions,
//             SwitchParameter noWait,
//             Hashtable privateData)
//             : base(noWait, privateData)
//         {
//             Task = task;
//             RetryOptions = retryOptions;
//         }

//         internal override object Result {
//             get
//             {
//                 List<object> allResults = new List<object>();
//                 foreach (var subTask in Task)
//                 {
//                     allResults.Add(subTask.Result);
//                 }
//                 return allResults; // cache
//             }
//         }

//         internal override bool HasResult()
//         {
//             var allTasksComplete = true;
//             foreach (var subTask in Task)
//             {
//                 if (!subTask.HasResult())
//                 {
//                     allTasksComplete = false;
//                 }
//             }
//             return allTasksComplete;
//         }

//         internal override OrchestrationAction CreateOrchestrationAction()
//         {
//             var compoundActions = tasks.Select((task) => {

//                 return task.CreateOrchestrationAction();
//             }).ToArray();
//             var action = new WhenAllAction(compoundActions);
//             return action;
//         }

//         internal override Task CreateDTFxTask()
//         {
//             return TaskOrchestrationContext.CallActivityAsync<object>(FunctionName, Input);
//         }
//     }
// }
