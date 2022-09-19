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
    using System.Collections;
    using System.Management.Automation;
    using DurableEngine.Models;

    public class ActivityInvocationTask : DurableTask
    {
        internal string FunctionName { get; }

        internal object Input { get; }

        private RetryOptions RetryOptions { get; }

        public ActivityInvocationTask(string functionName, object functionInput, RetryOptions retryOptions, SwitchParameter noWait,
            Hashtable privateData) : base(noWait, privateData)
        {
            FunctionName = functionName;
            Input = JsonConvert.SerializeObject(functionInput);
            RetryOptions = retryOptions;
        }

        internal override Task CreateDTFxTask()
        {
            if (RetryOptions != null)
            {
                throw new NotImplementedException();
            }
            else
            {
                return this.TaskOrchestrationContext.CallActivityAsync<object>(FunctionName, Input);

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
