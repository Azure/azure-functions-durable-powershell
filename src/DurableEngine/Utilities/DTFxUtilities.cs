using Microsoft.DurableTask;
using Microsoft.PowerShell.Commands;
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
                var context = new JsonObject.ConvertToJsonContext(
                    maxDepth: 100,
                    enumsAsStrings: false,
                    compressOutput: true);

                return value != null ? JsonObject.ConvertToJson(value, context) : null;
            }

            /// <inheritdoc/>
            public override object Deserialize(string data, Type targetType)
            {
                return data != null ? JsonObject.ConvertFromJson(data, error: out _) : null;
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