//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member 'member'

namespace Microsoft.Azure.Functions.PowerShellWorker.Durable.Commands
{
    using System.Collections;
    using System.Management.Automation;
    using Microsoft.Azure.Functions.PowerShellWorker.Durable.Tasks;
    using Newtonsoft.Json;
    using System;
    using Microsoft.Azure.Functions.Worker;
    using DurableTask;
    using System.Threading.Tasks;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Invoke a durable activity.
    /// </summary>
    [Cmdlet("Invoke", "DurableActivity")]
    public class InvokeDurableActivityCommand : PSCmdlet
    {
        /// <summary>
        /// Gets and sets the activity function name.
        /// </summary>
        [Parameter(Mandatory = true)]
        public string FunctionName { get; set; }

        /// <summary>
        /// Gets and sets the input for an activity function.
        /// </summary>
        [Parameter]
        [ValidateNotNull]
        public object Input { get; set; }

        [Parameter]
        public SwitchParameter NoWait { get; set; }

        [Parameter]
        [ValidateNotNull]
        public RetryOptions RetryOptions { get; set; }

        private readonly DurableTaskHandler _durableTaskHandler = new DurableTaskHandler();

        private OrchestrationContext myContext = null;

        protected override async void EndProcessing()
        {
            var privateData = (Hashtable)MyInvocation.MyCommand.Module.PrivateData;
            var context = (OrchestrationContext)privateData[SetFunctionInvocationContextCommand.ContextKey];
            var dtfxContext = (TaskOrchestrationContext)privateData["dtfx"];


            // add to actions
            var sdkTask = new ActivityInvocationTask(FunctionName, Input, RetryOptions);
            context.OrchestrationActionCollector.Add(sdkTask.CreateOrchestrationAction());
            context.OrchestrationActionCollector.NextBatch();

            // signal to orchestration thread to await
            context.OrchestrationActionCollector.hasToAwait.Set();

            // wait for result
            var handlers = new[] { context.OrchestrationActionCollector.cancelationToken }; //, taskHasResult };
            WaitHandle.WaitAny(handlers);

            // clear IAsync signals
            context.OrchestrationActionCollector.currTask = null;
            context.OrchestrationActionCollector.hasToAwait.Reset();

            // send result to user code
            WriteObject("tempResultValue");
            context.OrchestrationActionCollector.startOfNewCmdLet.Set();
        }

        protected override void StopProcessing()
        {
            var privateData = (Hashtable)MyInvocation.MyCommand.Module.PrivateData;
            var context = (OrchestrationContext)privateData[SetFunctionInvocationContextCommand.ContextKey];
            context.OrchestrationActionCollector.cancelationToken.Set();
        }
    }
}
