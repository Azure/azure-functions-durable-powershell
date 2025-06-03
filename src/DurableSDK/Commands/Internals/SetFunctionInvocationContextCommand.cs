//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member 'member'


namespace DurableSDK.Commands.Internals
{
    using System;
    using System.Collections;
    using System.Management.Automation;
    using DurableEngine;
    using DurableEngine.Models;
    using Newtonsoft.Json;

    /// <summary>
    /// Sets either the orchestration context or the durableClient in the privateData of this module or clears both.
    /// </summary>
    [Cmdlet("Set", "FunctionInvocationContext")]
    public class SetFunctionInvocationContextCommand : PSCmdlet
    {
        private const string ContextKey = "OrchestrationContext";
        private const string DurableClientKey = "DurableClient";

        /// <summary>
        /// The orchestration context.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = ContextKey)]
        public string OrchestrationContext { get; set; }

        /// <summary>
        /// The orchestration client.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = DurableClientKey)]
        public object DurableClient { get; set; }

        /// <summary>
        /// Whether or not to clear the privateData of this module.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "Clear")]
        public SwitchParameter Clear { get; set; }


        protected override void EndProcessing()
        {
            var privateData = (Hashtable)MyInvocation.MyCommand.Module.PrivateData;
            switch (ParameterSetName)
            {
                case ContextKey:
                    // De-serialize the orchestration context
                    JsonSerializerSettings serializerSettings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.None
                    };

                    var context = JsonConvert.DeserializeObject<OrchestrationContext>(OrchestrationContext, serializerSettings);

                    // save orchestration context to privateData
                    privateData[ContextKey] = context;

                    // construct and return orchestrator invoker that will utilize the de-serialized context
                    OrchestrationInvoker orchestrationInvoker = new OrchestrationInvoker(privateData);
                    Func<PowerShell, object> invokerFunction = orchestrationInvoker.CreateInvokerFunction();
                    WriteObject(invokerFunction);
                    break;

                case DurableClientKey:
                    privateData[DurableClientKey] = DurableClient;
                    break;

                default:
                    if (Clear.IsPresent)
                    {
                        privateData.Remove(ContextKey);
                        privateData.Remove(DurableClientKey);
                    }
                    break;
            }
        }
    }
}