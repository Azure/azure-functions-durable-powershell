//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // "Missing XML comments for publicly visible type"

namespace DurableEngine
{
    using Microsoft.DurableTask;
    using System.Collections;
    using System.Management.Automation;
    using System.Threading.Tasks;

    public class ActivityInvocationTask : DurableEngineCommand
    {
        internal string FunctionName { get; }

        internal object Input { get; }

        private RetryOptions RetryOptions { get; }

        public ActivityInvocationTask(
            string functionName,
            object input,
            RetryOptions retryOptions,
            SwitchParameter noWait,
            Hashtable privateData)
            : base(noWait, privateData)
        {
            FunctionName = functionName;
            Input = input;
            RetryOptions = retryOptions;
        }

        internal override Task<object> CreateDTFxTask()
        {
            return TaskOrchestrationContext.CallActivityAsync<object>(FunctionName, Input);
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
