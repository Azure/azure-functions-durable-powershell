// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AzureFunctions.PowerShell.Durable.SDK.Tests.E2E;
using Newtonsoft.Json;
using System.Net;
using Xunit;

namespace AzureFunctions.PowerShell.Durable.SDK.E2E
{
    public class DurableTests
    {
        private readonly DurableAppFixture _fixture;
        
        private readonly TimeSpan _orchestrationCompletionTimeout = TimeSpan.FromSeconds(120);

        // Set the shared context for E2E tests
        public DurableTests(DurableAppFixture fixture)
        {
            _fixture = fixture;
        }

        // Poll the statusQueryGetUri returned in the initial HttpResponseMessage until the workflow
        // is complete and validate the final response body contents
        protected internal async Task ValidateDurableWorkflowResults(
            HttpResponseMessage initialResponse,
            Action<dynamic>? validateInitialResponse,
            Action<dynamic>? validateIntermediateResponse,
            Action<dynamic>? validateFinalResponse)
        {
            var initialResponseBodyString = await initialResponse.Content.ReadAsStringAsync();
            dynamic initialResponseBody = JsonConvert.DeserializeObject(initialResponseBodyString);
            var statusQueryGetUri = (string)initialResponseBody.statusQueryGetUri;

            await validateInitialResponse?.Invoke(initialResponseBody);

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

                            await validateIntermediateResponse?.Invoke(statusResponseBody);
                            await Task.Delay(TimeSpan.FromSeconds(2));
                            break;
                        }

                        case HttpStatusCode.OK:
                        {
                            await validateFinalResponse?.Invoke(statusResponseBody);
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