//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using DurableEngine.Actions;
using Microsoft.DurableTask;
using System.Collections;
using System.Management.Automation;
using System.Threading.Tasks;

namespace DurableEngine.Tasks
{
    public class SubOrchestratorTask : DurableTask
    {
        internal string FunctionName { get; }
        internal string InstanceId { get; }

        internal object Input { get; }

        private RetryPolicy RetryOptions { get; }

        public SubOrchestratorTask(
            string functionName,
            string instanceId,
            object functionInput,
            RetryPolicy retryOptions,
            SwitchParameter noWait,
            Hashtable privateData) : base(noWait, privateData)
        {
            FunctionName = functionName;
            InstanceId = instanceId;
            Input = functionInput;
            RetryOptions = retryOptions;
        }

        internal override Task<object> CreateDTFxTask()
        {
            var DTFxContext = OrchestrationContext.DTFxContext;
            var taskOptions = RetryOptions == null
                ? new TaskOptions() :
                TaskOptions.FromRetryPolicy(RetryOptions);

            taskOptions = InstanceId == null
                ? taskOptions :
                taskOptions.WithInstanceId(InstanceId);

            return DTFxContext.CallSubOrchestratorAsync<object>(FunctionName, Input, taskOptions);
        }

        internal override OrchestrationAction CreateOrchestrationAction()
        {
            return RetryOptions == null
                ? new CallSubOrchestratorAction(FunctionName, Input, InstanceId)
                : new CallSubOrchestratorWithRetryAction(FunctionName, Input, InstanceId, RetryOptions);
        }
    }
}
