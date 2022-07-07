//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member 'member'

namespace DurableEngine.Commands
{
    using System;
    using System.Collections;
    using System.Management.Automation;
    using DurableEngine;
    using Newtonsoft.Json;


    /// <summary>
    /// Set the orchestration context.
    /// </summary>
    [Cmdlet("Set", "FunctionInvocationContext22")]
    internal class SetFunctionInvocationContextCommand : PSCmdlet
    {
        internal const string ContextKey = "OrchestrationContext";
        private const string DurableClientKey = "DurableClient";

        [Parameter(Mandatory = true, ParameterSetName = ContextKey)]
        internal string OrchestrationContext { get; set; }

        /// <summary>
        /// The orchestration client.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = DurableClientKey)]
        internal object DurableClient { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "Clear")]
        internal SwitchParameter Clear { get; set; }

        protected override void EndProcessing()
        {
            var privateData = (Hashtable)MyInvocation.MyCommand.Module.PrivateData;
            switch (ParameterSetName)
            {
                case ContextKey:
                    // De-serialize DTFx History event to the original type
                    JsonSerializerSettings serializerSettings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    };
                    var context = JsonConvert.DeserializeObject<OrchestrationContext>(OrchestrationContext,serializerSettings);
                    OrchestrationInvoker orchestrationInvoker = new OrchestrationInvoker();

                    Func<PowerShell, object> invokerFunction = (pwsh) => orchestrationInvoker.InvokeExternal(context, new PowerShellServices(pwsh), privateData);
                    privateData[ContextKey] = context;
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
