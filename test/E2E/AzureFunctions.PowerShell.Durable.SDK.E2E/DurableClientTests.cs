// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AzureFunctions.PowerShell.Durable.SDK.Tests.E2E;
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
                (dynamic initialResponseBody) =>
                {
                    Assert.NotNull(initialResponseBody.id);
                    var statusQueryGetUri = (string)initialResponseBody.statusQueryGetUri;
                    Assert.Equal(location?.ToString(), statusQueryGetUri);
                    Assert.NotNull(initialResponseBody.sendEventPostUri);
                    Assert.NotNull(initialResponseBody.purgeHistoryDeleteUri);
                    Assert.NotNull(initialResponseBody.terminatePostUri);
                    Assert.NotNull(initialResponseBody.rewindPostUri);
                },
                (dynamic intermediateStatusResponseBody) =>
                {
                    var runtimeStatus = (string)intermediateStatusResponseBody.runtimeStatus;
                    Assert.True(
                        runtimeStatus == "Running" || runtimeStatus == "Pending",
                        $"Unexpected runtime status: {runtimeStatus}");
                },
                (dynamic finalStatusResponseBody) =>
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
                (dynamic initialStatusResponseBody) =>
                {
                    Assert.NotNull(initialStatusResponseBody.id);
                    var statusQueryGetUri = (string)initialStatusResponseBody.statusQueryGetUri;
                    Assert.Equal(location?.ToString(), statusQueryGetUri);
                    Assert.NotNull(initialStatusResponseBody.sendEventPostUri);
                    Assert.NotNull(initialStatusResponseBody.purgeHistoryDeleteUri);
                    Assert.NotNull(initialStatusResponseBody.terminatePostUri);
                    Assert.NotNull(initialStatusResponseBody.rewindPostUri);
                },
                (dynamic intermediateStatusResponseBody) =>
                {
                    var runtimeStatus = (string)intermediateStatusResponseBody.runtimeStatus;
                    Assert.True(
                        runtimeStatus == "Running" || runtimeStatus == "Pending",
                        $"Unexpected runtime status: {runtimeStatus}");
                },
                (dynamic finalStatusResponseBody) =>
                {
                    Assert.Equal("Completed", (string)finalStatusResponseBody.runtimeStatus);
                    Assert.Equal("Hello Tokyo", finalStatusResponseBody.output[0].ToString());
                    Assert.Equal("Hello Seattle", finalStatusResponseBody.output[1].ToString());
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
                null,
                (dynamic intermediateStatusResponseBody) =>
                {
                    var runtimeStatus = (string)intermediateStatusResponseBody.runtimeStatus;
                    Assert.True(
                        runtimeStatus == "Running" || runtimeStatus == "Pending",
                        $"Unexpected runtime status: {runtimeStatus}");
                },
                (dynamic finalStatusResponseBody) =>
                {
                    Assert.Equal("Terminated", (string)finalStatusResponseBody.runtimeStatus);
                    Assert.Equal("Terminated intentionally", (string)finalStatusResponseBody.output);
                });
        }
    }
}