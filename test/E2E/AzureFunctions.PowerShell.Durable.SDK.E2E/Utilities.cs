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

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(Constants.FunctionsHostUrl);
                return await httpClient.SendAsync(request);
            }
        }

        public static async Task<dynamic> GetResponseBodyAsync(HttpResponseMessage response)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject(responseBody);
        }
    }
}