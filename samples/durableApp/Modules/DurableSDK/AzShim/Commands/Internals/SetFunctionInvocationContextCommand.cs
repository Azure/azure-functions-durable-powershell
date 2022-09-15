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
    using Newtonsoft.Json;

    /// <summary>
    /// Set the orchestration context.
    /// </summary>
    [Cmdlet("Set", "FunctionInvocationContext")]
    public class SetFunctionInvocationContextCommand : PSCmdlet
    {
        // We define ContextKey in the DurableEngine because we want to ensure that the key used to access the OrchestrationContext
        // matches the key used to set the OrchestrationContext without hard-coding it in two separate places
        public const string ContextKey = DurableEngine.OrchestrationInvoker.ContextKey;
        private const string DurableClientKey = "DurableClient";

        [Parameter(Mandatory = true, ParameterSetName = ContextKey)]
        public string OrchestrationContext { get; set; }

        /// <summary>
        /// The orchestration client.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = DurableClientKey)]
        public object DurableClient { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "Clear")]
        public SwitchParameter Clear { get; set; }

        protected override void EndProcessing()
        {
            var privateData = (Hashtable)MyInvocation.MyCommand.Module.PrivateData;
            switch (ParameterSetName)
            {
                case ContextKey:
                    // MICHAELPENG TOASK: How does deserialization work here? It is not sufficient to set IsReplaying: we need to generate an appropriate DTFxContext
                    // so that the getter agrees with the underlying value
                    // ANSWER: No context passed to the host; we expose the context to the user here
                    // Deserialize the OrchestratorContext data string
                    JsonSerializerSettings serializerSettings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    };
                    var context = JsonConvert.DeserializeObject<OrchestrationContext>(OrchestrationContext, serializerSettings);


                    // context = (TaskOrchestrationContext)orchestrationContext;
                    privateData[ContextKey] = context;
                    IOrchestrationInvoker orchestrationInvoker = new OrchestrationInvoker(privateData);
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
