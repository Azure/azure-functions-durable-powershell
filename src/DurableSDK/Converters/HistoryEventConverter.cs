//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

namespace DurableSDK.Converters
{
    using System;
    using DurableTask.Core.History;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    internal sealed class HistoryEventConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(HistoryEvent);
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var eventObject = JObject.Load(reader);
            var eventTypeToken = eventObject.GetValue(
                nameof(HistoryEvent.EventType),
                StringComparison.OrdinalIgnoreCase);

            if (eventTypeToken == null)
            {
                throw new JsonSerializationException(
                    $"History event is missing the required '{nameof(HistoryEvent.EventType)}' property.");
            }

            if (!Enum.TryParse(eventTypeToken.ToString(), ignoreCase: true, out EventType eventType)
                || !Enum.IsDefined(typeof(EventType), eventType))
            {
                throw new JsonSerializationException(
                    $"History event contains unsupported EventType value '{eventTypeToken}'.");
            }

            Type concreteType = GetConcreteType(eventType);
            if (concreteType == typeof(ExecutionRewoundEvent))
            {
                return DeserializeExecutionRewoundEvent(eventObject, serializer);
            }

            return eventObject.ToObject(concreteType, serializer)
                ?? throw new JsonSerializationException(
                    $"History event of type '{eventType}' could not be deserialized.");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }

        private static ExecutionRewoundEvent DeserializeExecutionRewoundEvent(
            JObject eventObject,
            JsonSerializer serializer)
        {
            int? eventId = eventObject.Value<int?>(nameof(HistoryEvent.EventId));
            if (!eventId.HasValue)
            {
                throw new JsonSerializationException(
                    $"History event is missing the required '{nameof(HistoryEvent.EventId)}' property.");
            }

            // This event has multiple parameterized constructors, so Newtonsoft cannot select one automatically.
            var historyEvent = new ExecutionRewoundEvent(eventId.Value);
            using JsonReader eventReader = eventObject.CreateReader();
            serializer.Populate(eventReader, historyEvent);
            return historyEvent;
        }

        private static Type GetConcreteType(EventType eventType)
        {
            return eventType switch
            {
                EventType.ExecutionStarted => typeof(ExecutionStartedEvent),
                EventType.ExecutionCompleted => typeof(ExecutionCompletedEvent),
                EventType.ExecutionTerminated => typeof(ExecutionTerminatedEvent),
                EventType.TaskScheduled => typeof(TaskScheduledEvent),
                EventType.TaskCompleted => typeof(TaskCompletedEvent),
                EventType.TaskFailed => typeof(TaskFailedEvent),
                EventType.SubOrchestrationInstanceCreated => typeof(SubOrchestrationInstanceCreatedEvent),
                EventType.SubOrchestrationInstanceCompleted => typeof(SubOrchestrationInstanceCompletedEvent),
                EventType.SubOrchestrationInstanceFailed => typeof(SubOrchestrationInstanceFailedEvent),
                EventType.TimerCreated => typeof(TimerCreatedEvent),
                EventType.TimerFired => typeof(TimerFiredEvent),
                EventType.OrchestratorStarted => typeof(OrchestratorStartedEvent),
                EventType.OrchestratorCompleted => typeof(OrchestratorCompletedEvent),
                EventType.EventSent => typeof(EventSentEvent),
                EventType.EventRaised => typeof(EventRaisedEvent),
                EventType.ContinueAsNew => typeof(ContinueAsNewEvent),
                EventType.GenericEvent => typeof(GenericEvent),
                EventType.HistoryState => typeof(HistoryStateEvent),
                EventType.ExecutionSuspended => typeof(ExecutionSuspendedEvent),
                EventType.ExecutionResumed => typeof(ExecutionResumedEvent),
                EventType.ExecutionRewound => typeof(ExecutionRewoundEvent),
                _ => throw new JsonSerializationException(
                    $"History event type '{eventType}' has no concrete DurableTask history event class."),
            };
        }
    }
}
