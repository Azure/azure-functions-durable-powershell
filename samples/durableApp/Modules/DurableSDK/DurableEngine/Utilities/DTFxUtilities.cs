using Microsoft.DurableTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DurableEngine.Utilities
{
    internal class DTFxUtilities
    {
        /// <summary>
        /// DataConverter class needed to instantiate a DTFx orchestrator executor.
        /// </summary>
        internal class JsonDataConverter : DataConverter
        {
            // WARNING: Changing default serialization options could potentially be breaking for in-flight orchestrations.
            static readonly JsonSerializerOptions DefaultOptions = new()
            {
                IncludeFields = true,
            };

            /// <summary>
            /// An instance of the <see cref="JsonDataConverter"/> with default configuration.
            /// </summary>
            internal static JsonDataConverter Default { get; } = new JsonDataConverter();

            readonly JsonSerializerOptions? options;

            JsonDataConverter(JsonSerializerOptions? options = null)
            {
                if (options != null)
                {
                    this.options = options;
                }
                else
                {
                    this.options = DefaultOptions;
                }
            }

            /// <inheritdoc/>
            public override string? Serialize(object? value)
            {
                return value != null ? System.Text.Json.JsonSerializer.Serialize(value, this.options) : null;
            }

            /// <inheritdoc/>
            public override object? Deserialize(string? data, Type targetType)
            {
                return data != null ? System.Text.Json.JsonSerializer.Deserialize(data, targetType, this.options) : null;
            }
        }

        /// <summary>
        /// Represents a DTFx orchestrator state.
        /// </summary>
        internal sealed class OrchestratorState
        {
            internal string? InstanceId { get; set; }

            internal IList<global::DurableTask.Core.History.HistoryEvent>? PastEvents { get; set; }

            internal IList<global::DurableTask.Core.History.HistoryEvent>? NewEvents { get; set; }

            internal int? UpperSchemaVersion { get; set; }
        }

    }
}
