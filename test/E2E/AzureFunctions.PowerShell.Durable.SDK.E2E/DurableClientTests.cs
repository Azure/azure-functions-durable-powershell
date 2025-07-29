// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AzureFunctions.PowerShell.Durable.SDK.Tests.E2E;
using Newtonsoft.Json;
using System.Collections.Generic;
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

        [Theory]
        // [InlineData(null, null, "1.0", "1.0")] // No version specified, should use defaultVersion from host.json for both
        // [InlineData("0.5", null, "0.5", "1.0")] // Version specified for orchestrator, orchestrator should use it, suborchestrator should use defaultVersion
        // [InlineData(null, "0.7", "1.0", "0.7")] // Version specified for suborchestrator only, orchestrator should use defaultVersion, suborchestrator should use specified version
        [InlineData("0.5", "0.7", "0.5", "0.7")] // Both versions specified, each should use their respective versions
        public async Task OrchestrationVersionIsPropagatedToContext(
            string orchestratorVersion,
            string subOrchestratorVersion,
            string expectedOrchestratorVersion,
            string expectedSubOrchestratorVersion)
        {
            var queryParams = new List<string>();
            if (orchestratorVersion != null)
                queryParams.Add($"Version={orchestratorVersion}");
            if (subOrchestratorVersion != null)
                queryParams.Add($"SubOrchestratorVersion={subOrchestratorVersion}");
            
            string queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : string.Empty;
            
            var initialResponse = await Utilities.GetHttpStartResponse("VersionedOrchestrator", queryString);
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
                    Assert.Equal(expectedOrchestratorVersion, finalStatusResponseBody.output[0].ToString());
                    Assert.Equal(expectedSubOrchestratorVersion, finalStatusResponseBody.output[1].ToString());
                });
        }
    }
}