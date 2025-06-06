//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member 'member'


namespace DurableEngine.Models
{
    using global::DurableTask.Core.History;
    using Microsoft.DurableTask;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represent the context object of an orchestrator, from which APIs are invoked.
    /// </summary>
    [DataContract]
    public class OrchestrationContext
    {
        /// <summary>
        /// Input to the orchestrator.
        /// </summary>
        [DataMember]
        public object Input
        {
            get => DTFxContext?.GetInput<object>();
        }

        /// <summary>
        /// Orchestrator instanceID.
        /// </summary>
        [DataMember]
        public string InstanceId { get; set; }

        /// <summary>
        /// The instanceID of the parent orchestrator, if applicable.
        /// </summary>
        [DataMember]
        public string ParentInstanceId { get; set; }

        /// <summary>
        /// Whether the orchestrator is currently replaying a previous invocation step.
        /// </summary>
        [DataMember]
        public bool IsReplaying
        {
            get => DTFxContext.IsReplaying;
        }

        /// <summary>
        /// The deterministic UTC timestamp at the step in replay / execution.
        /// </summary>
        public DateTime CurrentUtcDateTime
        {
            get => DTFxContext.CurrentUtcDateTime;
        }

        /// <summary>
        /// Shared memory between the orchestrator invoker thread and the user-code thread.
        /// </summary>
        internal SharedMemory SharedMemory { get; } = new SharedMemory();

        /// <summary>
        /// The custom status of this orchestrator.
        /// </summary>
        public object CustomStatus { get; set; }

        /// <summary>
        /// DTFx context that provides the implementation of DF APis.
        /// </summary>
        internal TaskOrchestrationContext DTFxContext;

        /// <summary>
        /// Internal orchestrator history
        /// </summary>
        [DataMember]
        internal HistoryEvent[] History { get; set; }

        /// <summary>
        /// Gets the version of the current orchestration instance, which was set when the instance was created.
        /// </summary>
        public string Version => DTFxContext?.Version;
    }
}