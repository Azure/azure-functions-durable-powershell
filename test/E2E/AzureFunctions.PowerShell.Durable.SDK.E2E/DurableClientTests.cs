// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AzureFunctions.PowerShell.Durable.SDK.Tests.E2E;
using Newtonsoft.Json;
using System.Net;
using Xunit;

namespace AzureFunctions.PowerShell.Durable.SDK.E2E
{
    [Collection(Constants.DurableAppCollectionName)]
    public class DurableClientTests : DurableTests
    {
        public DurableClientTests(DurableAppFixture fixture) : base(fixture) {}

        [Fact]
        public async Task DurableClientFollowsAsyncPattern()
        {
            var initialResponse = await Utilities.GetHttpStartResponse("DurablePatternsOrchestrator");
            Assert.Equal(HttpStatusCode.Accepted, initialResponse.StatusCode);

            var location = initialResponse.Headers.Location;
            Assert.NotNull(location);

            await ValidateDurableWorkflowResults(
                initialResponse,
                validateInitialResponse: (dynamic initialResponseBody) =>
                {
                    Assert.NotNull(initialResponseBody.id);
                    var statusQueryGetUri = (string)initialResponseBody.statusQueryGetUri;
                    Assert.Equal(location?.ToString(), statusQueryGetUri);
                    Assert.NotNull(initialResponseBody.sendEventPostUri);
                    Assert.NotNull(initialResponseBody.purgeHistoryDeleteUri);
                    Assert.NotNull(initialResponseBody.terminatePostUri);
                    Assert.NotNull(initialResponseBody.rewindPostUri);
                },
                validateIntermediateResponse: (dynamic intermediateStatusResponseBody) =>
                {
                    var runtimeStatus = (string)intermediateStatusResponseBody.runtimeStatus;
                    Assert.True(
                        runtimeStatus == "Running" || runtimeStatus == "Pending",
                        $"Unexpected runtime status: {runtimeStatus}");
                },
                validateFinalResponse: (dynamic finalStatusResponseBody) =>
                {
                    Assert.Equal("Completed", (string)finalStatusResponseBody.runtimeStatus);
                    Assert.Equal("Hello Tokyo", finalStatusResponseBody.output[0].ToString());
                    Assert.Equal("Hello Seattle", finalStatusResponseBody.output[1].ToString());
                    Assert.Equal("Hello London", finalStatusResponseBody.output[2].ToString());
                    Assert.Equal("Hello Toronto", finalStatusResponseBody.output[3].ToString());
                    Assert.Equal("Custom status: finished", (string)finalStatusResponseBody.customStatus);
                });
        }

        [Fact]
        public async Task DurableSubOrchestratorCompletes()
        {
            var initialResponse = await Utilities.GetHttpStartResponse("SubOrchestrator");
            Assert.Equal(HttpStatusCode.Accepted, initialResponse.StatusCode);

            var location = initialResponse.Headers.Location;
            Assert.NotNull(location);

            await ValidateDurableWorkflowResults(
                initialResponse,
                validateInitialResponse: (dynamic initialStatusResponseBody) =>
                {
                    Assert.NotNull(initialStatusResponseBody.id);
                    var statusQueryGetUri = (string)initialStatusResponseBody.statusQueryGetUri;
                    Assert.Equal(location?.ToString(), statusQueryGetUri);
                    Assert.NotNull(initialStatusResponseBody.sendEventPostUri);
                    Assert.NotNull(initialStatusResponseBody.purgeHistoryDeleteUri);
                    Assert.NotNull(initialStatusResponseBody.terminatePostUri);
                    Assert.NotNull(initialStatusResponseBody.rewindPostUri);
                },
                validateIntermediateResponse: (dynamic intermediateStatusResponseBody) =>
                {
                    var runtimeStatus = (string)intermediateStatusResponseBody.runtimeStatus;
                    Assert.True(
                        runtimeStatus == "Running" || runtimeStatus == "Pending",
                        $"Unexpected runtime status: {runtimeStatus}");
                },
                validateFinalResponse: (dynamic finalStatusResponseBody) =>
                {
                    Assert.Equal("Completed", (string)finalStatusResponseBody.runtimeStatus);
                    Assert.Equal("Hello Tokyo", finalStatusResponseBody.output[0].ToString());
                    Assert.Equal("Context.Version: 1.0", finalStatusResponseBody.output[1].ToString());
                    Assert.Equal("Hello Seattle", finalStatusResponseBody.output[2].ToString());
                });
        }

        [Fact]
        public async Task DurableSubOrchestratoWithArrayInputCompletes()
        {
            var initialResponse = await Utilities.GetHttpStartResponse("SubOrchestratorWithArrayInput");
            Assert.Equal(HttpStatusCode.Accepted, initialResponse.StatusCode);

            var location = initialResponse.Headers.Location;
            Assert.NotNull(location);

            await ValidateDurableWorkflowResults(
                initialResponse,
                validateInitialResponse: (dynamic initialStatusResponseBody) =>
                {
                    Assert.NotNull(initialStatusResponseBody.id);
                    var statusQueryGetUri = (string)initialStatusResponseBody.statusQueryGetUri;
                    Assert.Equal(location?.ToString(), statusQueryGetUri);
                    Assert.NotNull(initialStatusResponseBody.sendEventPostUri);
                    Assert.NotNull(initialStatusResponseBody.purgeHistoryDeleteUri);
                    Assert.NotNull(initialStatusResponseBody.terminatePostUri);
                    Assert.NotNull(initialStatusResponseBody.rewindPostUri);
                },
                validateIntermediateResponse: (dynamic intermediateStatusResponseBody) =>
                {
                    var runtimeStatus = (string)intermediateStatusResponseBody.runtimeStatus;
                    Assert.True(
                        runtimeStatus == "Running" || runtimeStatus == "Pending",
                        $"Unexpected runtime status: {runtimeStatus}");
                },
                validateFinalResponse: (dynamic finalStatusResponseBody) =>
                {
                    Assert.Equal("Completed", (string)finalStatusResponseBody.runtimeStatus);
                    Assert.Equal("Hello Tokyo", finalStatusResponseBody.output[0].ToString());
                    Assert.Equal("Hello Seattle", finalStatusResponseBody.output[1].ToString());
                });
        }

        [Fact]
        public async Task OrchestratorCanReceiveArrayFromActivity()
        {
            var initialResponse = await Utilities.GetHttpStartResponse("CanReceiveArrayOrchestrator");
            Assert.Equal(HttpStatusCode.Accepted, initialResponse.StatusCode);

            var location = initialResponse.Headers.Location;
            Assert.NotNull(location);

            await ValidateDurableWorkflowResults(
                initialResponse,
                validateInitialResponse: (dynamic initialStatusResponseBody) =>
                {
                    Assert.NotNull(initialStatusResponseBody.id);
                    var statusQueryGetUri = (string)initialStatusResponseBody.statusQueryGetUri;
                    Assert.Equal(location?.ToString(), statusQueryGetUri);
                    Assert.NotNull(initialStatusResponseBody.sendEventPostUri);
                    Assert.NotNull(initialStatusResponseBody.purgeHistoryDeleteUri);
                    Assert.NotNull(initialStatusResponseBody.terminatePostUri);
                    Assert.NotNull(initialStatusResponseBody.rewindPostUri);
                },
                validateIntermediateResponse: (dynamic intermediateStatusResponseBody) =>
                {
                    var runtimeStatus = (string)intermediateStatusResponseBody.runtimeStatus;
                    Assert.True(
                        runtimeStatus == "Running" || runtimeStatus == "Pending",
                        $"Unexpected runtime status: {runtimeStatus}");
                },
                validateFinalResponse: (dynamic finalStatusResponseBody) =>
                {
                    Assert.Equal("Completed", (string)finalStatusResponseBody.runtimeStatus);
                    Assert.Equal("An", finalStatusResponseBody.output[0].ToString());
                    Assert.Equal("Array", finalStatusResponseBody.output[1].ToString());
                });
        }

        [Fact]
        public async Task CanReceiveDeeplyNestedClientInput()
        {
            var initialResponse = await Utilities.GetHttpStartResponse("OrchestratorReturnInput", clientRoute: "orchestratorsSendComplexInput");
            Assert.Equal(HttpStatusCode.Accepted, initialResponse.StatusCode);

            var location = initialResponse.Headers.Location;
            Assert.NotNull(location);

            await ValidateDurableWorkflowResults(
                initialResponse,
                validateInitialResponse: (dynamic initialStatusResponseBody) =>
                {
                    Assert.NotNull(initialStatusResponseBody.id);
                    var statusQueryGetUri = (string)initialStatusResponseBody.statusQueryGetUri;
                    Assert.Equal(location?.ToString(), statusQueryGetUri);
                    Assert.NotNull(initialStatusResponseBody.sendEventPostUri);
                    Assert.NotNull(initialStatusResponseBody.purgeHistoryDeleteUri);
                    Assert.NotNull(initialStatusResponseBody.terminatePostUri);
                    Assert.NotNull(initialStatusResponseBody.rewindPostUri);
                },
                validateIntermediateResponse: (dynamic intermediateStatusResponseBody) =>
                {
                    var runtimeStatus = (string)intermediateStatusResponseBody.runtimeStatus;
                    Assert.True(
                        runtimeStatus == "Running" || runtimeStatus == "Pending",
                        $"Unexpected runtime status: {runtimeStatus}");
                },
                validateFinalResponse: (dynamic finalStatusResponseBody) =>
                {
                    Assert.Equal("Completed", (string)finalStatusResponseBody.runtimeStatus);
                    // our input is a JSON 7 levels deep, with a number on each level.
                    // We check an integer for evidence of each level being preserved
                    string inputStr = finalStatusResponseBody.input.ToString();
                    Assert.Contains("1", inputStr);
                    Assert.Contains("2", inputStr);
                    Assert.Contains("3", inputStr);
                    Assert.Contains("4", inputStr);
                    Assert.Contains("5", inputStr);
                    Assert.Contains("6", inputStr);
                    Assert.Contains("7", inputStr);
                });
        }

        [Fact]
        public async Task DurableClientTerminatesOrchestration()
        {
            var initialResponse = await Utilities.GetHttpStartResponse(
                orchestratorName: "DurablePatternsOrchestratorWithExternalEvent",
                clientRoute: "terminatingOrchestrators");
            Assert.Equal(HttpStatusCode.Accepted, initialResponse.StatusCode);

            await ValidateDurableWorkflowResults(
                initialResponse,
                validateIntermediateResponse: (dynamic intermediateStatusResponseBody) =>
                {
                    var runtimeStatus = (string)intermediateStatusResponseBody.runtimeStatus;
                    Assert.True(
                        runtimeStatus == "Running" || runtimeStatus == "Pending",
                        $"Unexpected runtime status: {runtimeStatus}");
                },
                validateFinalResponse: (dynamic finalStatusResponseBody) =>
                {
                    Assert.Equal("Terminated", (string)finalStatusResponseBody.runtimeStatus);
                    Assert.Equal("Terminated intentionally", (string)finalStatusResponseBody.output);
                });
        }

        [Fact]
        public async Task DurableClientSuspendOrchestration()
        {
            var initialResponse = await Utilities.GetHttpStartResponse(
                orchestratorName: "SendDurableExternalEventOrchestrator",
                clientRoute: "suspendingOrchestrators");
            Assert.Equal(HttpStatusCode.Accepted, initialResponse.StatusCode);

            await ValidateDurableWorkflowResults(
                initialResponse,
                validateIntermediateResponse: (dynamic intermediateStatusResponseBody) =>
                {
                    var runtimeStatus = (string)intermediateStatusResponseBody.runtimeStatus;
                    Assert.True(
                        runtimeStatus == "Running" || runtimeStatus == "Suspended",
                        $"Unexpected runtime status: {runtimeStatus}");
                });

            await ValidateDurableWorkflowResults(
                initialResponse,
                validateIntermediateResponse: (dynamic intermediateStatusResponseBody) =>
                {
                    Assert.Equal("Running", (string)intermediateStatusResponseBody.runtimeStatus);
                },
                validateFinalResponse: (dynamic finalStatusResponseBody) =>
                {
                    Assert.Equal("Completed", (string)finalStatusResponseBody.runtimeStatus);
                    Assert.Equal("FirstTimeout", finalStatusResponseBody.output[0].ToString());
                    Assert.Equal("SecondExternalEvent", finalStatusResponseBody.output[1].ToString());
                });
        }
    }
}