﻿//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

namespace DurableEngine
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// OrchestrationFailureException should be propagated back to the Host when an orchestrator function
    /// throws and does not handle an exception. The Durable Functions extension implementation requires
    /// this exception message to contain the Json-serialized orchestration replay state after a special marker.
    /// </summary>
    internal class OrchestrationFailureException : Exception
    {
        internal const string OutOfProcDataLabel = "\n\n$OutOfProcData$:";

        internal OrchestrationFailureException()
        {
        }

        internal OrchestrationFailureException(
            List<List<OrchestrationAction>> actions,
            object customStatus,
            Exception innerException)
            : base(FormatOrchestrationFailureMessage(actions, customStatus, innerException), innerException)
        {
        }

        private static string FormatOrchestrationFailureMessage(
            List<List<OrchestrationAction>> actions,
            object customStatus,
            Exception exception)
        {
            // For more details on why this message looks like this, see:
            // - https://github.com/Azure/azure-functions-durable-js/pull/145
            // - https://github.com/Azure/azure-functions-durable-extension/pull/1171
            var orchestrationMessage = new OrchestrationMessage(isDone: false, actions, output: null, customStatus, exception.Message);
            var message = $"{exception.Message}{OutOfProcDataLabel}{JsonConvert.SerializeObject(orchestrationMessage)}";
            return message;
        }
    }
}