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

        internal string Version { get; }

        public SubOrchestratorTask(
            string functionName,
            string instanceId,
            object functionInput,
            RetryPolicy retryOptions,
            string version,
            SwitchParameter noWait,
            Hashtable privateData) : base(noWait, privateData)
        {
            FunctionName = functionName;
            InstanceId = instanceId;
            Input = functionInput;
            RetryOptions = retryOptions;
            Version = version;
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

            var subOrchestrationOptions = new SubOrchestrationOptions(taskOptions, InstanceId)
            {
                Version = this.Version
            };

            return DTFxContext.CallSubOrchestratorAsync<object>(FunctionName, Input, subOrchestrationOptions);
        }

        internal override OrchestrationAction CreateOrchestrationAction()
        {
            return RetryOptions == null
                ? new CallSubOrchestratorAction(FunctionName, Input, InstanceId, Version)
                : new CallSubOrchestratorWithRetryAction(FunctionName, Input, InstanceId, RetryOptions, Version);
        }
    }
}
