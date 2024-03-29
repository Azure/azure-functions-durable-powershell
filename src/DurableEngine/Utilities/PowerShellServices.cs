//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

namespace DurableEngine.Utilities

{
    using System;
    using System.Management.Automation;
    using DurableEngine.Models;

    /// <summary>
    /// Utilities to interact with a PowerShell runspace.
    /// </summary>
    internal class PowerShellServices : IPowerShellServices
    {
        private const string SetFunctionInvocationContextCommand =
            "Microsoft.Azure.Functions.PowerShellWorker\\Set-FunctionInvocationContext";

        private readonly PowerShell _pwsh;
        private bool _hasSetOrchestrationContext = false;

        internal PowerShellServices(PowerShell pwsh)
        {
            _pwsh = pwsh;
        }

        public void AddParameter(string paramName, object paramValue)
        {
            _pwsh.AddParameter(paramName, paramValue);
        }

        public void TracePipelineObject()
        {
            _pwsh.AddCommand("Microsoft.Azure.Functions.PowerShellWorker\\Trace-PipelineObject");
        }

        public void SetDurableClient(object durableClient)
        {
            _pwsh.AddCommand(SetFunctionInvocationContextCommand)
                .AddParameter("DurableClient", durableClient)
                .InvokeAndClearCommands();

            _hasSetOrchestrationContext = true;
        }

        public void SetOrchestrationContext(OrchestrationContext orchestrationContext)
        {
            _pwsh.AddCommand(SetFunctionInvocationContextCommand)
                .AddParameter("OrchestrationContext", orchestrationContext)
                .InvokeAndClearCommands();

            _hasSetOrchestrationContext = true;
        }

        public void ClearOrchestrationContext()
        {
            if (_hasSetOrchestrationContext)
            {
                _pwsh.AddCommand(SetFunctionInvocationContextCommand)
                    .AddParameter("Clear", true)
                    .InvokeAndClearCommands();
            }
        }

        public IAsyncResult BeginInvoke(PSDataCollection<object> output)
        {
            return _pwsh.BeginInvoke<object, object>(input: null, output);
        }

        public void EndInvoke(IAsyncResult asyncResult)
        {
            _pwsh.EndInvoke(asyncResult);
        }

        public void StopInvoke()
        {
            _pwsh.Stop();
        }

        public void ClearStreamsAndCommands()
        {
            _pwsh.Streams.ClearStreams();
            _pwsh.Commands.Clear();
        }
    }
}