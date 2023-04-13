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

        [Fact]
        public async Task ActivityCanHaveQueueBinding()
        {
            const string queueName = "outqueue";
            await StorageHelpers.ClearQueue(queueName);
            var initialResponse = await Utilities.GetHttpTriggerResponse("DurableClient", queryString: "?FunctionName=DurableOrchestratorWriteToQueue");
            Assert.Equal(HttpStatusCode.Accepted, initialResponse.StatusCode);

            var initialResponseBody = await initialResponse.Content.ReadAsStringAsync();
            dynamic initialResponseBodyObject = JsonConvert.DeserializeObject(initialResponseBody);
            var statusQueryGetUri = (string)initialResponseBodyObject.statusQueryGetUri;

            var startTime = DateTime.UtcNow;

            using var httpClient = new HttpClient();

            while (true)
            {
                var statusResponse = await httpClient.GetAsync(statusQueryGetUri);
                switch (statusResponse.StatusCode)
                {
                    case HttpStatusCode.Accepted:
                        {
                            if (DateTime.UtcNow > startTime + _orchestrationCompletionTimeout)
                            {
                                Assert.True(false, $"The orchestration has not completed after {_orchestrationCompletionTimeout}");
                            }

                            await Task.Delay(TimeSpan.FromSeconds(2));
                            break;
                        }

                    case HttpStatusCode.OK:
                        {
                            var statusResponseBody = await GetResponseBodyAsync(statusResponse);
                            Assert.Equal("Completed", (string)statusResponseBody.runtimeStatus);

                            var queueMessage = await StorageHelpers.ReadFromQueue(queueName);
                            Assert.Equal("QueueData", queueMessage);
                            return;
                        }

                    default:
                        Assert.True(false, $"Unexpected orchestration status code: {statusResponse.StatusCode}");
                        break;
                }
            }
        }


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