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
        protected internal async Task ValidateOutput(
            HttpResponseMessage initialResponse,
            Action<dynamic> validateResponseBody)
        {
            Assert.Equal(HttpStatusCode.Accepted, initialResponse.StatusCode);

            var initialResponseBody = await initialResponse.Content.ReadAsStringAsync();
            dynamic initialResponseBodyObject = JsonConvert.DeserializeObject(initialResponseBody);
            var statusQueryGetUri = (string)initialResponseBodyObject.statusQueryGetUri;

            var startTime = DateTime.UtcNow;

            using (var httpClient = new HttpClient())
            {
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
                            var statusResponseBody = await Utilities.GetResponseBodyAsync(statusResponse);
                            Assert.Equal("Completed", (string)statusResponseBody.runtimeStatus);
                            validateResponseBody(statusResponseBody);
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