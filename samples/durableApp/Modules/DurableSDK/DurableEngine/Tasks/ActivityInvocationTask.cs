//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // "Missing XML comments for publicly visible type"

namespace DurableEngine
{
    using Newtonsoft.Json;
    using Microsoft.DurableTask;
    using System.Threading.Tasks;
    using System;
    using DurableEngine.Tasks;

    internal class ActivityInvocationTask : AtomicTask
    {
        internal string FunctionName { get; }

        internal object Input { get; }

        private RetryOptions RetryOptions { get; }

        private TaskOrchestrationContext context;

        internal ActivityInvocationTask(string functionName, object functionInput, RetryOptions retryOptions, TaskOrchestrationContext context, OrchestrationContext sdkContext) : base(sdkContext)
        {
            FunctionName = functionName;
            Input = JsonConvert.SerializeObject(functionInput);
            RetryOptions = retryOptions;
            this.context = context;
        }

        internal override Task createDTFxTask()
        {
            if (RetryOptions != null)
            {
                throw new NotImplementedException();
            }
            else
            {
                return this.context.CallActivityAsync<object>(FunctionName, Input);

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
