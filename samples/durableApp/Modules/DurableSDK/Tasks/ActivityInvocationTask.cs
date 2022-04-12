//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // "Missing XML comments for publicly visible type"

namespace Microsoft.Azure.Functions.PowerShellWorker.Durable.Tasks
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    // using WebJobs.Script.Grpc.Messages;

    using Microsoft.Azure.Functions.PowerShellWorker;
    using Microsoft.Azure.Functions.PowerShellWorker.Durable;
    using Microsoft.Azure.Functions.PowerShellWorker.Durable.Actions;

    public class ActivityInvocationTask : DurableTask
    {
        internal string FunctionName { get; }

        private object Input { get; }

        private RetryOptions RetryOptions { get; }

        internal ActivityInvocationTask(string functionName, object functionInput, RetryOptions retryOptions)
        {
            FunctionName = functionName;
            Input = JsonConvert.SerializeObject(functionInput);
            RetryOptions = retryOptions;
        }

        internal ActivityInvocationTask(string functionName, object functionInput)
            : this(functionName, functionInput, retryOptions: null)
        {
        }

        internal override HistoryEvent GetScheduledHistoryEvent(OrchestrationContext context)
        {
            // TODO: rewrite to use DTFx context
            return null;
        }

        internal override HistoryEvent GetCompletedHistoryEvent(OrchestrationContext context, HistoryEvent scheduledHistoryEvent)
        {
            // TODO: rewrite to use DTFx context
            return null;
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
