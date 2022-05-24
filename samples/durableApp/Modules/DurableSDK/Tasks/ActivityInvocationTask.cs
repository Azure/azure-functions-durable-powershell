//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // "Missing XML comments for publicly visible type"

namespace Microsoft.DurableTask.Tasks
{
    using Newtonsoft.Json;
    using Microsoft.DurableTask;
    using Microsoft.DurableTask.Actions;
    using System.Threading.Tasks;
    using global::DurableTask;
    using System;

    public class ActivityInvocationTask : DurableSDKTask
    {
        internal string FunctionName { get; }

        internal object Input { get; }

        private RetryOptions RetryOptions { get; }

        private TaskOrchestrationContext context;
        private OrchestrationContext context2;

        internal ActivityInvocationTask(string functionName, object functionInput, RetryOptions retryOptions, TaskOrchestrationContext context, OrchestrationContext context2)
        {
            FunctionName = functionName;
            Input = JsonConvert.SerializeObject(functionInput);
            RetryOptions = retryOptions;
            this.context = context;
            this.context2 = context2;
        }

        internal override Task getDTFxTask()
        {
            if (this.dtfxTask != null)
            {
                return this.dtfxTask;
            }
            else if (RetryOptions != null)
            {
                throw new NotImplementedException();
            }
            else
            {
                this.dtfxTask = this.context.CallActivityAsync<object>(FunctionName, Input);
                context2.OrchestrationActionCollector.taskMap.Add(dtfxTask, this);
                return dtfxTask;
                
            }
        }

        internal override OrchestrationAction CreateOrchestrationAction()
        {
            return RetryOptions == null
                ? new CallActivityAction(FunctionName, Input)
                : new CallActivityWithRetryAction(FunctionName, Input, RetryOptions);
        }

        /*internal static void ValidateTask(ActivityInvocationTask task, IEnumerable<AzFunctionInfo> loadedFunctions)
        {
            var functionInfo = loadedFunctions.FirstOrDefault(fi => fi.FuncName == task.FunctionName);
            if (functionInfo == null)
            {
                var message = string.Format(PowerShellWorkerStrings.FunctionNotFound, task.FunctionName);
                throw new InvalidOperationException(message);
            }

            var activityTriggerBinding = functionInfo.InputBindings.FirstOrDefault(
                                            entry => DurableBindings.IsActivityTrigger(entry.Value.Type)
                                                     && entry.Value.Direction == BindingInfo.Types.Direction.In);
            if (activityTriggerBinding.Key == null)
            {
                var message = string.Format(PowerShellWorkerStrings.FunctionDoesNotHaveProperActivityFunctionBinding, task.FunctionName);
                throw new InvalidOperationException(message);
            }
        }*/
    }
}
