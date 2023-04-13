// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AzureFunctions.PowerShell.Durable.SDK.Tests.E2E;
using System.Net;
using Xunit;

namespace AzureFunctions.PowerShell.Durable.SDK.E2E
{
    [Collection(Constants.DurableAppCollectionName)]
    public class OrchestrationTests : DurableTests
    {
        public OrchestrationTests(DurableAppFixture fixture) : base(fixture) {}

        [Fact]
        public async Task OrchestratationCanAlwaysObtainTaskResult()
        {
            var initialResponse = await Utilities.GetHttpStartResponse("GetDurableTaskResultOrchestrator");
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
                    Assert.Equal("Completed", (string)finalStatusResponseBody.runtimeStatus);
                    Assert.Equal("Hello world", finalStatusResponseBody.output.ToString());
                });
        }

        [Fact]
        public async Task LegacyDurableCommandNamesStillWork()
        {
            var initialResponse = await Utilities.GetHttpStartResponse("LegacyNamesOrchestrator");
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
                    Assert.Equal("Completed", (string)finalStatusResponseBody.runtimeStatus);
                    Assert.Equal("Hello Tokyo", finalStatusResponseBody.output[0].ToString());
                });
        }

        [Fact]
        public async Task OrchestratationContextHasAllExpectedProperties()
        {
            var initialResponse = await Utilities.GetHttpStartResponse(
                "DurableOrchestratorAccessContextProps",
                clientRoute: "contextOrchestrators");
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
                    Assert.Equal("Completed", (string)finalStatusResponseBody.runtimeStatus);
                    Assert.Equal("True", finalStatusResponseBody.output[0].ToString());
                    Assert.Equal("Hello myInstanceId", finalStatusResponseBody.output[1].ToString());
                    Assert.Equal("False", finalStatusResponseBody.output[2].ToString());
                });
        }
    }
}

