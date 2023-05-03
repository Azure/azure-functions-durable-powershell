// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AzureFunctions.PowerShell.Durable.SDK.Tests.E2E;
using Newtonsoft.Json;
using System.Net;
using Xunit;

namespace AzureFunctions.PowerShell.Durable.SDK.E2E
{
    // Parent class for all Durable E2E test files. No tests should be written here, otherwise,
    // XUnit will not be able to find matching fixture data.
    public class DurableTests
    {
        private readonly DurableAppFixture _fixture;
        
        protected readonly TimeSpan _orchestrationCompletionTimeout = TimeSpan.FromSeconds(120);

        // Set the shared context for E2E tests
        public DurableTests(DurableAppFixture fixture)
        {
            _fixture = fixture;
        }

        // Durable workflows follow an asynchronous pattern, where an initial response is given
        // to the client, and a statusQueryGetUri is polled until the orchestration terminates. This
        // method allows delagates to be passed in that will validate the appropriate response is
        // given in each of the below three states:
        //  1. The orchestration has been initialized.
        //  2. The orchestration is running.
        //  3. The orchestration has terminated.
        // Validation delegates are expected to be synchronous. This method also allows for external
        // events to be sent after the workflow begins execution.
        protected internal async Task ValidateDurableWorkflowResults(
            HttpResponseMessage initialResponse,
            Func<HttpClient, Task>? sendExternalEvents = null,
            Action<dynamic>? validateInitialResponse = null,
            Action<dynamic>? validateIntermediateResponse = null,
            Action<dynamic>? validateFinalResponse = null)
        {
            var initialResponseBodyString = await initialResponse.Content.ReadAsStringAsync();
            dynamic initialResponseBody = JsonConvert.DeserializeObject(initialResponseBodyString);
            var statusQueryGetUri = (string)initialResponseBody.statusQueryGetUri;

            validateInitialResponse?.Invoke(initialResponseBody);

            var startTime = DateTime.UtcNow;

            using (var httpClient = new HttpClient())
            {
                while (true)
                {
                    var statusResponse = await httpClient.GetAsync(statusQueryGetUri);
                    var statusResponseBody = await Utilities.GetResponseBodyAsync(statusResponse);
                    if (sendExternalEvents != null)
                    {
                        await sendExternalEvents.Invoke(httpClient);
                    }

                    switch (statusResponse.StatusCode)
                    {
                        case HttpStatusCode.Accepted:
                        {
                            if (DateTime.UtcNow > startTime + _orchestrationCompletionTimeout)
                            {
                                Assert.True(false, $"The orchestration has not completed after {_orchestrationCompletionTimeout}");
                            }

                            validateIntermediateResponse?.Invoke(statusResponseBody);
                            await Task.Delay(TimeSpan.FromSeconds(2));
                            break;
                        }

                        case HttpStatusCode.OK:
                        {
                            validateFinalResponse?.Invoke(statusResponseBody);
                            return;
                        }

                        default:
                            Assert.True(false, $"Unexpected orchestration status code: {statusResponse.StatusCode}");
                            break;
                    }
                }
            }
        }
    }
}