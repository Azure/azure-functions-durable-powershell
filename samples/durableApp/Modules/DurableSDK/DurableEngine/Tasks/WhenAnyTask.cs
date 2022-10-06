//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // "Missing XML comments for publicly visible type"

namespace DurableEngine.Tasks
{
    using System.Collections;
    using System.Linq;
    using System.Management.Automation;
    using System.Threading.Tasks;
    using DurableEngine.Models;

    public class WhenAnyTask : DurableTask
    {
        internal DurableTask[] Tasks { get; set; }
        private RetryOptions RetryOptions { get; }

        public WhenAnyTask(
            DurableTask[] tasks,
            RetryOptions retryOptions,
            // Should this task also be able to be awaited?
            SwitchParameter noWait,
            Hashtable privateData)
            : base(noWait, privateData)
        {
            Tasks = tasks;
            RetryOptions = retryOptions;
        }

        internal override object Result {
            get
            {
                var firstDTFxTaskToComplete = ((Task<Task>)GetDTFxTask()).Result;
                OrchestrationContext.SharedMemory.taskMap.TryGetValue(firstDTFxTaskToComplete, out DurableTask firstDurableTaskToComplete);
                return firstDurableTaskToComplete.Result;
            }
        }

        internal override bool HasResult()
        {
            foreach (var task in Tasks)
            {
                if (task.HasResult())
                {
                    return true;
                }
            }
            return false;
        }

        internal override OrchestrationAction CreateOrchestrationAction()
        {
            var compoundActions = Tasks.Select((task) => task.CreateOrchestrationAction()).ToArray();
            var action = new WhenAnyAction(compoundActions);
            return action;
        }

        internal override Task CreateDTFxTask()
        {
            var DTFxTasks = Tasks.Select(task => task.GetDTFxTask());
            var task = Task.WhenAny(DTFxTasks);
            return task;
        }
    }
}

//         internal override object Result
//         {
//             get
//             {
//                 var task = (Task<Task>) this.dtfxTask;
//                 Task winner = task.Result;
//                 sdkContext.OrchestrationActionCollector.taskMap.TryGetValue(winner, out var result);
//                 return result;
//             }
//         }
//     }
// }
