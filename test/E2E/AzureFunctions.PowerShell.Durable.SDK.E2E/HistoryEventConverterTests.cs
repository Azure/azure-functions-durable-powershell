// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using DurableSDK.Converters;
using DurableTask.Core.History;
using Newtonsoft.Json;
using Xunit;

namespace AzureFunctions.PowerShell.Durable.SDK.E2E
{
    public class HistoryEventConverterTests
    {
        public static TheoryData<EventType, Type> SupportedEventTypes => new()
        {
            { EventType.ExecutionStarted, typeof(ExecutionStartedEvent) },
            { EventType.ExecutionCompleted, typeof(ExecutionCompletedEvent) },
            { EventType.ExecutionTerminated, typeof(ExecutionTerminatedEvent) },
            { EventType.TaskScheduled, typeof(TaskScheduledEvent) },
            { EventType.TaskCompleted, typeof(TaskCompletedEvent) },
            { EventType.TaskFailed, typeof(TaskFailedEvent) },
            { EventType.SubOrchestrationInstanceCreated, typeof(SubOrchestrationInstanceCreatedEvent) },
            { EventType.SubOrchestrationInstanceCompleted, typeof(SubOrchestrationInstanceCompletedEvent) },
            { EventType.SubOrchestrationInstanceFailed, typeof(SubOrchestrationInstanceFailedEvent) },
            { EventType.TimerCreated, typeof(TimerCreatedEvent) },
            { EventType.TimerFired, typeof(TimerFiredEvent) },
            { EventType.OrchestratorStarted, typeof(OrchestratorStartedEvent) },
            { EventType.OrchestratorCompleted, typeof(OrchestratorCompletedEvent) },
            { EventType.EventSent, typeof(EventSentEvent) },
            { EventType.EventRaised, typeof(EventRaisedEvent) },
            { EventType.ContinueAsNew, typeof(ContinueAsNewEvent) },
            { EventType.GenericEvent, typeof(GenericEvent) },
            { EventType.HistoryState, typeof(HistoryStateEvent) },
            { EventType.ExecutionSuspended, typeof(ExecutionSuspendedEvent) },
            { EventType.ExecutionResumed, typeof(ExecutionResumedEvent) },
            { EventType.ExecutionRewound, typeof(ExecutionRewoundEvent) },
        };

        [Theory]
        [MemberData(nameof(SupportedEventTypes))]
        public void DeserializesLegacyHistoryEvents(EventType eventType, Type expectedType)
        {
            string json = $$"""
                {
                  "EventType": {{(int)eventType}},
                  "EventId": 1,
                  "IsPlayed": false,
                  "Timestamp": "2026-08-05T00:00:00Z"
                }
                """;

            HistoryEvent historyEvent = JsonConvert.DeserializeObject<HistoryEvent>(
                json,
                CreateSerializerSettings())!;

            Assert.IsType(expectedType, historyEvent);
            Assert.Equal(1, historyEvent.EventId);
        }

        [Fact]
        public void DeserializesTypePreservingHistoryEvents()
        {
            var original = new TaskScheduledEvent(7, "Hello", string.Empty, "{}");
            string json = JsonConvert.SerializeObject(
                original,
                typeof(HistoryEvent),
                new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

            HistoryEvent historyEvent = JsonConvert.DeserializeObject<HistoryEvent>(
                json,
                CreateSerializerSettings())!;

            var taskScheduled = Assert.IsType<TaskScheduledEvent>(historyEvent);
            Assert.Equal(7, taskScheduled.EventId);
            Assert.Equal("Hello", taskScheduled.Name);
            Assert.Equal("{}", taskScheduled.Input);
        }

        [Theory]
        [InlineData("{}")]
        [InlineData("""{ "EventType": 999 }""")]
        [InlineData("""{ "EventType": 2 }""")]
        public void RejectsHistoryEventsWithoutConcreteTypes(string json)
        {
            Assert.Throws<JsonSerializationException>(
                () => JsonConvert.DeserializeObject<HistoryEvent>(json, CreateSerializerSettings()));
        }

        private static JsonSerializerSettings CreateSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                Converters = { new HistoryEventConverter() },
            };
        }
    }
}
