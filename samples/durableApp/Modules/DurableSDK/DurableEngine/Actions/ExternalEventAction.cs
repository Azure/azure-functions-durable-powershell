//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

namespace DurableEngine
{
    /// <summary>
    /// An orchestration action that represents listening for an external event
    /// </summary>
    internal class ExternalEventAction : OrchestrationAction
    {
        /// <summary>
        /// The external event name.
        /// </summary>
        internal readonly string ExternalEventName;

        /// <summary>
        /// Reason for the action. This field is necessary for the Durable extension to recognize the ExternalEventAction.
        /// </summary>
        internal readonly string Reason = "ExternalEvent";

        internal ExternalEventAction(string externalEventName)
            : base(ActionType.WaitForExternalEvent)
        {
            ExternalEventName = externalEventName;
        }
    }
}
