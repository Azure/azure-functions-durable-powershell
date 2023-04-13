//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

namespace DurableEngine.Actions
{
    internal class CallSubOrchestratorAction : OrchestrationAction
    {
        /// <summary>
        /// The sub-orchestrator function name.
        /// </summary>
        public readonly string FunctionName;

        /// <summary>
        /// The sub-orchestrator instanceId.
        /// </summary>
        public readonly string InstanceId;

        /// <summary>
        /// The input to the sub-orchestrator function.
        /// </summary>
        public readonly object Input;

        internal CallSubOrchestratorAction(string functionName, object input, string instanceId)
            : base(ActionType.CallSubOrchestrator)
        {
            FunctionName = functionName;
            Input = input;
            InstanceId = instanceId;
        }
    }
}
