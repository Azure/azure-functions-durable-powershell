using Microsoft.DurableTask;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DurableEngine.Utilities
{
    internal class DTFxUtilities
    {
        /// <summary>
        /// DataConverter class needed to instantiate a DTFx orchestrator executor.
        /// </summary>
        internal class JsonDataConverter : DataConverter
        {
            internal JsonDataConverter()
            {
            }

            /// <inheritdoc/>
            public override string Serialize(object value)
            {
                return value != null ? JsonConvert.SerializeObject(value) : null;
            }

            /// <inheritdoc/>
            public override object Deserialize(string data, Type targetType)
            {
                return data != null ? JsonConvert.DeserializeObject(data, targetType) : null;
            }
        }

        /// <summary>
        /// Represents a DTFx orchestrator state.
        /// </summary>
        internal sealed class OrchestratorState
        {
            internal string InstanceId { get; set; }

            internal IList<DurableTask.Core.History.HistoryEvent> PastEvents { get; set; }

            internal IList<DurableTask.Core.History.HistoryEvent> NewEvents { get; set; }

            internal int? UpperSchemaVersion { get; set; }
        }

    }
}