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
            var initialResponse = await Utilities.GetHttpStartResponse("DurablePatternsOrchestrator", queryString: string.Empty);
            Assert.Equal(HttpStatusCode.Accepted, initialResponse.StatusCode);

            var location = initialResponse.Headers.Location;
            Assert.NotNull(location);

            var initialResponseBody = await initialResponse.Content.ReadAsStringAsync();
            dynamic initialResponseBodyObject = JsonConvert.DeserializeObject(initialResponseBody);
            Assert.NotNull(initialResponseBodyObject.id);
            var statusQueryGetUri = (string)initialResponseBodyObject.statusQueryGetUri;
            Assert.Equal(location.ToString(), statusQueryGetUri);
            Assert.NotNull(initialResponseBodyObject.sendEventPostUri);
            Assert.NotNull(initialResponseBodyObject.purgeHistoryDeleteUri);
            Assert.NotNull(initialResponseBodyObject.terminatePostUri);
            Assert.NotNull(initialResponseBodyObject.rewindPostUri);

            await ValidateDurableWorkflowResults(
                initialResponse,
                (dynamic statusResponseBody) =>
                {
                    var runtimeStatus = (string)statusResponseBody.runtimeStatus;
                    Assert.True(
                        runtimeStatus == "Running" || runtimeStatus == "Pending",
                        $"Unexpected runtime status: {runtimeStatus}");
                    Assert.Equal("Custom status: started", (string)statusResponseBody.customStatus);
                },
                (dynamic statusResponseBody) =>
                {
                    Assert.Equal("Completed", (string)statusResponseBody.runtimeStatus);
                    Assert.Equal("Hello Tokyo", statusResponseBody.output[0].ToString());
                    Assert.Equal("Hello Seattle", statusResponseBody.output[1].ToString());
                    Assert.Equal("Hello London", statusResponseBody.output[2].ToString());
                    Assert.Equal("Hello Toronto", statusResponseBody.output[3].ToString());
                    Assert.Equal("Custom status: finished", (string)statusResponseBody.customStatus);
                });
        }

        [Fact]
        public async Task DurableClientTerminatesOrchestration()
        {
            var initialResponse = await Utilities.GetHttpStartResponse(
                clientRoute: "terminatingClientOrchestrators",
                "DurablePatternsOrchestrator",
                queryString: string.Empty);
            await ValidateDurableWorkflowResults(
                initialResponse,
                (dynamic statusResponseBody) =>
                {
                    var runtimeStatus = (string)statusResponseBody.runtimeStatus;
                    Assert.True(
                        runtimeStatus == "Running" || runtimeStatus == "Pending",
                        $"Unexpected runtime status: {runtimeStatus}");
                },
                (dynamic statusResponseBody) =>
                {
                    Assert.Equal("Terminated", (string)statusResponseBody.runtimeStatus);
                    Assert.Equal("Terminated intentionally", (string)statusResponseBody.output);
                });
        }
    }
}