//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member 'member'

namespace DurableEngine
{
    using Microsoft.DurableTask;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represent the context of an execution of an orchestration function.
    /// </summary>
    [DataContract]
    public class OrchestrationContext
    {
        [DataMember]
        public object Input { get;  set; }

        [DataMember]
        public string InstanceId { get; set; }

        [DataMember]
        public string ParentInstanceId { get; set; }

        [DataMember]
        public bool IsReplaying
        {
            get => this.DTFxContext.IsReplaying; 
        }

        [DataMember]
        internal global::DurableTask.Core.History.HistoryEvent[] History { get; set; }

        public DateTime CurrentUtcDateTime
        {
            get => this.DTFxContext.CurrentUtcDateTime;
        }

        internal OrchestrationActionCollector OrchestrationActionCollector { get; } = new OrchestrationActionCollector();

        public object CustomStatus { get; set; }
        internal TaskOrchestrationContext DTFxContext;
    }
}
