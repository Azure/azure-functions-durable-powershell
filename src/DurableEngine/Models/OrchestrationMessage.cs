//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

namespace DurableEngine.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Represent an orchestration message to be sent to the Extension.
    /// </summary>
    internal class OrchestrationMessage
    {
        internal OrchestrationMessage(
            bool isDone,
            List<List<OrchestrationAction>> actions,
            object output,
            object customStatus,
            string error = null)
        {
            IsDone = isDone;
            Actions = actions;
            Output = output;
            Error = error;
            CustomStatus = customStatus;
            SchemaVersion = 1; // TODO: throw warning if lower
        }

        /// <summary>
        /// Indicate whether the orchestration is done.
        /// </summary>
        public readonly bool IsDone;

        /// <summary>
        /// Orchestration actions to be taken.
        /// </summary>
        public readonly List<List<OrchestrationAction>> Actions;

        /// <summary>
        /// The output result of the orchestration function run.
        /// </summary>
        public readonly object Output;

        /// <summary>
        /// The orchestration error message.
        /// </summary>
        public readonly string Error;

        /// <summary>
        /// Custom orchestration status.
        /// </summary>
        public readonly object CustomStatus;

        /// <summary>
        /// The DF Out-Of-Process payload-schema version being used.
        /// </summary>
        public readonly object SchemaVersion;
    }
}