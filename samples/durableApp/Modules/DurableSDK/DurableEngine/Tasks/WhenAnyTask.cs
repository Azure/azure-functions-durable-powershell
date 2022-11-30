//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // "Missing XML comments for publicly visible type"

namespace DurableEngine.Tasks
{
    using System;
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
            SwitchParameter noWait,
            Hashtable privateData)
            : base(noWait, privateData)
        {
            if (!(tasks.Length > 0))
            {
                // TODO: refine the exception here
                throw new ArgumentException("The input array is empty.");
            }
            Tasks = tasks;
        }

        internal override Task CreateDTFxTask()
        {
            var DTFxTasks = Tasks.Select(task => task.GetDTFxTask());
            var task = Task.WhenAny(DTFxTasks);
            return task;
        }
        
        internal override OrchestrationAction CreateOrchestrationAction()
        {
            var compoundActions = Tasks.Select((task) => task.CreateOrchestrationAction()).ToArray();
            var action = new WhenAnyAction(compoundActions);
            return action;
        }

        internal override object Result
        {
            get
            {
                if (!HasResult())
                {
                    // TODO: Refine the exception thrown here.
                    throw new Exception("This task is not complete.");
                }
                var firstDTFxTaskToComplete = ((Task<Task>)GetDTFxTask()).Result;
                OrchestrationContext.SharedMemory.taskMap.TryGetValue(firstDTFxTaskToComplete, out DurableTask firstDurableTaskToComplete);
                // Return the first task to complete, rather than its result
                return firstDurableTaskToComplete;
            }
        }
    }
}
