//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // "Missing XML comments for publicly visible type"

namespace DurableEngine.Tasks
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Threading.Tasks;
    using DurableEngine.Models;

    public class WhenAllTask : DurableTask
    {
        internal DurableTask[] Tasks { get; set; }
        private RetryOptions RetryOptions { get; }
        private List<object> allResults = new List<object>();

        public WhenAllTask(
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

        internal override Task CreateDTFxTask()
        {
            var DTFxTasks = Tasks.Select(task => task.GetDTFxTask());
            var task = Task.WhenAll(DTFxTasks);
            return task;
        }

        internal override OrchestrationAction CreateOrchestrationAction()
        {
            var compoundActions = Tasks.Select((task) => task.CreateOrchestrationAction()).ToArray();
            var orchestrationAction = new WhenAllAction(compoundActions);
            return orchestrationAction;
        }

        internal override object Result {
            get
            {
                return allResults;
            }
        }

        internal override bool HasResult()
        {
            if (DTFxTask.IsCompleted)
            {
                return false;
            }
            foreach (var task in Tasks)
            {
                if (task.HasResult())
                {
                    allResults.Add(task.Result);
                }
            }
            return allResults.Count > 0;
        }
    }
}
