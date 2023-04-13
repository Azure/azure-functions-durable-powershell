// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AzureFunctions.PowerShell.Durable.SDK.Tests.E2E;
using System.Net;
using Xunit;

namespace AzureFunctions.PowerShell.Durable.SDK.E2E
{
    [Collection(Constants.DurableAppCollectionName)]
    public class ActivityTests : DurableTests
    {
        public ActivityTests(DurableAppFixture fixture) : base(fixture) {}

        //[Fact]
        //public async Task ActivityCanHaveQueueBinding()
        //{
        //    await StorageHelpers.ClearQueue();
        //    var initialResponse = await Utilities.GetHttpStartResponse("DurableOrchestratorWriteToQueue");
        //    Assert.Equal(HttpStatusCode.Accepted, initialResponse.StatusCode);

        //    await ValidateDurableWorkflowResults(
        //        initialResponse,
        //        null,
        //        null,
        //        async (dynamic finalStatusResponseBody) =>
        //        {
        //            Assert.Equal("Completed", (string)finalStatusResponseBody.runtimeStatus);
        //            var queueMessage = await StorageHelpers.ReadFromQueue();
        //            Assert.Equal("QueueData", queueMessage);
        //        });
        //}


        [Fact]
        public async Task ActivityExceptionIsPropagatedThroughOrchestrator()
        {
            var initialResponse = await Utilities.GetHttpStartResponse("DurableOrchestratorWithException");
            Assert.Equal(HttpStatusCode.Accepted, initialResponse.StatusCode);

            await ValidateDurableWorkflowResults(
                initialResponse,
                null,
                null,
                (dynamic finalStatusResponseBody) =>
                {
                    Assert.Equal("Failed", (string)finalStatusResponseBody.runtimeStatus);
                    var output = finalStatusResponseBody.output.ToString();
                    Assert.Contains("Orchestrator function 'DurableOrchestratorWithException' failed", output);
                    Assert.Contains("Activity function 'DurableActivityWithException' failed", output);
                    Assert.Contains("Intentional exception (Name)", output);
                });
        }
    }
}