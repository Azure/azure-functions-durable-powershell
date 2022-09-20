//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

namespace DurableEngine.Utilities
{
    using System;
    using System.Management.Automation;
    using DurableEngine.Models;

    public interface IPowerShellServices
    {
        void SetDurableClient(object durableClient);

        void SetOrchestrationContext(OrchestrationContext orchestrationContext);

        void ClearOrchestrationContext();

        IAsyncResult BeginInvoke(PSDataCollection<object> output);

        void EndInvoke(IAsyncResult asyncResult);

        void StopInvoke();

        void ClearStreamsAndCommands();

        void TracePipelineObject();

        void AddParameter(string paramName, object paramValue);

    }
}
