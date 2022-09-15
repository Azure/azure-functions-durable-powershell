//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

namespace DurableEngine

{
    using System;
    using System.Management.Automation;

    internal class PowerShellServices : IPowerShellServices
    {
        private const string SetFunctionInvocationContextCommand =
            "DurableSDK\\Set-FunctionInvocationContext";

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
            // MICHAELPENG TODO: Replace this with internal DurableSDK implementation
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

        // Invariant
        // DTFx is only able to start and stop the thread where its provided Function ran
        // any other threads (included child-threads) cannnot be controlled

        // consequence: DF orchestrators (C#) cannot be multithreaded.

        // public IAsyncResult BeginInvoke(TaskOrchestrationContext taskOrchestrationContext, object _, PSDataCollection<object> output)
        // {
        //     var privateData["Context"] = DtfxContext;
        //     return _pwsh.BeginInvoke<object, object>(input: null, output);
        // }

        // public Task<object> BeginInvoke (..the right params)
        // {  beginInvoke ^ }

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
