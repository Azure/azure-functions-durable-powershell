// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace AzureFunctions.PowerShell.Durable.SDK.Tests.E2E
{
    using Newtonsoft.Json;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Http.Headers;

    public static class Utilities
    {
        private static string? SystemKey => Environment.GetEnvironmentVariable("E2E_TEST_DURABLE_SYSTEM_KEY");

        public static HttpClient CreateManagementHttpClient()
        {
            return new HttpClient(new HttpClientHandler
            {
                AllowAutoRedirect = string.IsNullOrEmpty(SystemKey),
            });
        }

        public static HttpRequestMessage CreateManagementRequest(HttpMethod method, string url)
        {
            var target = new Uri(url);
            var request = new HttpRequestMessage(method, target);
            string? systemKey = SystemKey;
            if (string.IsNullOrEmpty(systemKey))
            {
                return request;
            }

            var host = new Uri(Constants.FunctionsHostUrl);
            if (!string.Equals(target.GetLeftPart(UriPartial.Authority),
                               host.GetLeftPart(UriPartial.Authority),
                               StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Management headers must not be sent to another origin.");
            }
            if (!target.AbsolutePath.StartsWith("/runtime/webhooks/durabletask/", StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Management credentials require a Durable management endpoint.");
            }
            request.Headers.Add("x-functions-key", systemKey);
            return request;
        }

        public static async Task<bool> GetHttpStartResponse(
            string orchestratorName,
            string queryString,
            HttpStatusCode expectedStatusCode,
            string expectedMessage,
            int expectedCode = 0,
            string clientRoute = "orchestrators")
        {
            var response = await GetHttpStartResponse(orchestratorName, queryString, clientRoute);
            if (expectedStatusCode != response.StatusCode && expectedCode != (int)response.StatusCode)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(expectedMessage))
            {
                string actualMessage = await response.Content.ReadAsStringAsync();
                return actualMessage.Contains(expectedMessage);
            }
            return true;
        }

        public static async Task<HttpResponseMessage> GetHttpStartResponse(
            string orchestratorName,
            string queryString = "",
            string clientRoute = "orchestrators")
        {
            string uri = $"api/{clientRoute}/{orchestratorName}{queryString}";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
            string? functionKey = Environment.GetEnvironmentVariable("E2E_TEST_FUNCTION_KEY");
            if (!string.IsNullOrEmpty(functionKey))
            {
                request.Headers.Add("x-functions-key", functionKey);
            }

            using (var httpClient = new HttpClient(new HttpClientHandler
            {
                AllowAutoRedirect = string.IsNullOrEmpty(functionKey),
            }))
            {
                httpClient.BaseAddress = new Uri(Constants.FunctionsHostUrl);
                return await httpClient.SendAsync(request);
            }
        }

        public static async Task<dynamic> GetResponseBodyAsync(HttpResponseMessage response)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject(responseBody)!;
        }
    }
}