// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AzureFunctions.PowerShell.Durable.SDK.Tests.E2E;
using Newtonsoft.Json;
using System.Net;
using Xunit;

namespace AzureFunctions.PowerShell.Durable.SDK.E2E
{
    [Collection(Constants.DurableAppCollectionName)]
    public class ActivityTests : DurableTests
    {
        public ActivityTests(DurableAppFixture fixture) : base(fixture) {}

        // TODO: Rewrite this to use a version of ValidateDurableWorkflow tha accepts asynchronous
        // validation procedures
        [Fact]
        public async Task ActivityCanHaveQueueBinding()
        {
            await StorageHelpers.ClearQueue();
            var initialResponse = await Utilities.GetHttpStartResponse("DurableOrchestratorWriteToQueue");
            Assert.Equal(HttpStatusCode.Accepted, initialResponse.StatusCode);

            var initialResponseBodyString = await initialResponse.Content.ReadAsStringAsync();
            dynamic initialResponseBody = JsonConvert.DeserializeObject(initialResponseBodyString);
            var statusQueryGetUri = (string)initialResponseBody.statusQueryGetUri;

            var startTime = DateTime.UtcNow;

            using (var httpClient = new HttpClient())
            {
                while (true)
                {
                    var statusResponse = await httpClient.GetAsync(statusQueryGetUri);
                    var statusResponseBody = await Utilities.GetResponseBodyAsync(statusResponse);

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
                                Assert.Equal("Completed", (string)statusResponseBody.runtimeStatus);
                                var queueMessage = await StorageHelpers.ReadFromQueue();
                                Assert.Equal("QueueData", queueMessage);
                                return;
                            }

                        default:
                            Assert.True(false, $"Unexpected orchestration status code: {statusResponse.StatusCode}");
                            break;
                    }
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
                validateFinalResponse: async (dynamic finalStatusResponseBody) => await Task.Run(() =>
                {
                    Assert.Equal("Failed", (string)finalStatusResponseBody.runtimeStatus);
                    var output = finalStatusResponseBody.output.ToString();
                    Assert.Contains("Orchestrator function 'DurableOrchestratorWithException' failed", output);
                    Assert.Contains("Activity function 'DurableActivityWithException' failed", output);
                    Assert.Contains("Intentional exception (Name)", output);
                }));
        }
    }
}