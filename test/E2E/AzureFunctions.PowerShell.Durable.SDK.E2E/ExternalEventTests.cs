// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AzureFunctions.PowerShell.Durable.SDK.Tests.E2E;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AzureFunctions.PowerShell.Durable.SDK.E2E
{
    [Collection(Constants.DurableAppCollectionName)]
    public class ExternalEventTests : DurableTests
    {
        public ExternalEventTests(DurableAppFixture fixture) : base(fixture) {}

        [Fact]
        public async Task ExternalEventReturnsData()
        {
            var initialResponse = await Utilities.GetHttpStartResponse("BasicExternalEventOrchestrator");
            Assert.Equal(HttpStatusCode.Accepted, initialResponse.StatusCode);

            await ValidateDurableWorkflowResults(
                initialResponse,
                sendExternalEvents: async (HttpClient httpClient) =>
                {
                    var initialResponseBodyString = await initialResponse.Content.ReadAsStringAsync();
                    dynamic initialResponseBody = JsonConvert.DeserializeObject(initialResponseBodyString);
                    var raiseEventUri = (string)initialResponseBody.sendEventPostUri;

                    raiseEventUri = raiseEventUri.Replace("{eventName}", "TESTEVENTNAME");

                    // Send external event payload
                    var json = JsonConvert.SerializeObject("helloWorld!");
                    var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    await httpClient.PostAsync(raiseEventUri, httpContent);
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
                    Assert.Equal("helloWorld!", finalStatusResponseBody.output.ToString());
                });
        }

        [Fact]
        private async Task SendDurableExternalEventsRaisesEvent()
        {
            var initialResponse = await Utilities.GetHttpStartResponse("SendDurableExternalEventOrchestrator");

            await ValidateDurableWorkflowResults(
                initialResponse,
                validateFinalResponse: (dynamic finalStatusResponseBody) =>
                {
                    Assert.Equal("Completed", (string)finalStatusResponseBody.runtimeStatus);
                    Assert.Equal("FirstTimeout", finalStatusResponseBody.output[0].ToString());
                    Assert.Equal("SecondExternalEvent", finalStatusResponseBody.output[1].ToString());
                });
        }
    }
}