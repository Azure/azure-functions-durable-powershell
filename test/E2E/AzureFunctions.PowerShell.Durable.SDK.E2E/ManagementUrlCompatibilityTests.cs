// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Management.Automation.Runspaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using Ps = System.Management.Automation.PowerShell;

namespace AzureFunctions.PowerShell.Durable.SDK.E2E
{
    public class ManagementUrlCompatibilityTests
    {
        private const string BaseUrl = "https://notification.test/runtime/webhooks/durabletask";

        [Theory]
        [InlineData("code=synthetic-INSTANCEID%2Bkey%2F%3D")]
        [InlineData("CoDe=synthetic-INSTANCEID%2Bkey%2F%3D&site=value%2Bone")]
        [InlineData("%63ode=synthetic-INSTANCEID%2Bkey%2F%3D&CODE=duplicate")]
        [InlineData("taskHub=hub&connection=storage")]
        [InlineData("taskHub=hub&connection=storage&metadata=https://notification.test/info")]
        [InlineData("code=synthetic-INSTANCEID%2Bkey%2F%3D&metadata=https://notification.test/INSTANCEID")]
        [InlineData("")]
        public void ResponsePreservesHostQueryAndPayloadShape(string query)
        {
            JObject binding = Binding(query);
            JObject response = Invoke(binding, "response");
            JObject payload = Assert.IsType<JObject>(response["Body"]);

            Assert.Equal(202, (int)response["StatusCode"]!);
            Assert.Equal("instance", (string?)payload["id"]);
            Assert.Null(payload["requiredHeaders"]);
            Assert.Equal((string?)payload["statusQueryGetUri"], (string?)response["Headers"]?["Location"]);
            const string notificationOrigin = "https://notification.test";
            foreach (JProperty property in payload.Properties())
            {
                string value = Assert.IsType<string>(Assert.IsType<JValue>(property.Value).Value);
                string original = (string)binding["managementUrls"]![property.Name]!;
                string expected = original.StartsWith(notificationOrigin, StringComparison.Ordinal)
                    ? "https://frontend.test" + original[notificationOrigin.Length..]
                    : original;
                int index = expected.IndexOf("INSTANCEID", StringComparison.Ordinal);
                expected = expected[..index] + "instance" + expected[(index + "INSTANCEID".Length)..];
                Assert.Equal(expected, value);
                int queryIndex = original.IndexOf('?');
                if (queryIndex >= 0)
                {
                    Assert.Equal(original[(queryIndex + 1)..], value[(value.IndexOf('?') + 1)..]);
                }
                Assert.DoesNotContain("starter-key", value);
            }
            Assert.Contains("{eventName}", (string?)payload["sendEventPostUri"]);
        }

        [Theory]
        [InlineData(
            "https://notification.test/https://notification.test/path?metadata=https://notification.test/info#https://notification.test/fragment",
            "http://frontend.test:8080/https://notification.test/path?metadata=https://notification.test/info#https://notification.test/fragment")]
        [InlineData(
            "https://notification.test?metadata=https://notification.test/info",
            "http://frontend.test:8080?metadata=https://notification.test/info")]
        [InlineData(
            "HTTPS://NOTIFICATION.test:443/runtime/instances/INSTANCEID?%63ode=key%2b&metadata=https://notification.test/INSTANCEID",
            "http://frontend.test:8080/runtime/instances/instance?%63ode=key%2b&metadata=https://notification.test/INSTANCEID")]
        public void ResponseReplacesOnlySchemeAndAuthority(string managementUrl, string expectedUrl)
        {
            JObject binding = Binding("");
            binding["managementUrls"]!["statusQueryGetUri"] = managementUrl;

            JObject response = Invoke(binding, "response",
                "http://frontend.test:8080/api/start?code=starter-key#request-fragment");
            JObject payload = Assert.IsType<JObject>(response["Body"]);

            Assert.Equal(expectedUrl, (string?)payload["statusQueryGetUri"]);
            Assert.Equal("instance", (string?)payload["id"]);
        }

        [Theory]
        [InlineData("https://INSTANCEID.example.test/api/start", BaseUrl,
            "https://instanceid.example.test/runtime/webhooks/durabletask")]
        [InlineData("https://frontend.test/api/start",
            "https://notification.test/INSTANCEID-base/runtime/webhooks/durabletask",
            "https://frontend.test/INSTANCEID-base/runtime/webhooks/durabletask")]
        public void ResponseReplacesOnlyTheInstancePathSegment(string requestUrl, string baseUrl, string expectedBase)
        {
            const string query = "?taskHub=hub&code=synthetic-INSTANCEID";
            JObject binding = Binding("");
            binding["managementUrls"] = new JObject
            {
                ["id"] = "INSTANCEID",
                ["statusQueryGetUri"] = baseUrl + "/instances/INSTANCEID" + query,
                ["sendEventPostUri"] = baseUrl + "/instances/INSTANCEID/raiseEvent/{eventName}" + query,
            };

            JObject payload = Assert.IsType<JObject>(Invoke(binding, "response", requestUrl)["Body"]);

            Assert.Equal("instance", (string?)payload["id"]);
            Assert.Equal(expectedBase + "/instances/instance" + query, (string?)payload["statusQueryGetUri"]);
            Assert.Equal(expectedBase + "/instances/instance/raiseEvent/{eventName}" + query,
                (string?)payload["sendEventPostUri"]);
        }

        [Theory]
        [InlineData("code=synthetic-key")]
        [InlineData("CoDe=synthetic%2Bkey")]
        [InlineData("taskHub=hub&connection=storage")]
        [InlineData("")]
        public void FrontendStartPreservesHostQueryAndDefaultRequestBehavior(string query)
        {
            JObject request = Invoke(Binding(query), "start");
            string expected = BaseUrl + "/orchestrators/Allowed/instance"
                + (query.Length == 0 ? "?" : "?" + query + "&") + "version=2.0";

            Assert.Equal(expected, (string?)request["Uri"]);
            Assert.Null(request["Headers"]?["x-functions-key"]);
            Assert.Equal(-1, (int)request["MaximumRedirection"]!);
        }

        [Theory]
        [InlineData("code=synthetic-key")]
        [InlineData("")]
        public void RpcDoesNotReceivePublicKeyCredentials(string query)
        {
            JObject request = Invoke(Binding(query, rpc: true), "start");

            Assert.Equal("http://rpc.test/durabletask/orchestrators/Allowed/instance?version=2.0",
                (string?)request["Uri"]);
            Assert.Empty(Assert.IsType<JObject>(request["Headers"]));
            Assert.Equal(-1, (int)request["MaximumRedirection"]!);
        }

        private static JObject Binding(string query, bool rpc = false)
        {
            string suffix = query.Length == 0 ? "" : "?" + query;
            string reason = "?reason={text}" + (query.Length == 0 ? "" : "&" + query);
            return new JObject
            {
                ["managementUrls"] = new JObject
                {
                    ["id"] = "INSTANCEID",
                    ["statusQueryGetUri"] = BaseUrl + "/instances/INSTANCEID" + suffix,
                    ["sendEventPostUri"] = BaseUrl + "/instances/INSTANCEID/raiseEvent/{eventName}" + suffix,
                    ["terminatePostUri"] = BaseUrl + "/instances/INSTANCEID/terminate" + reason,
                    ["rewindPostUri"] = BaseUrl + "/instances/INSTANCEID/rewind" + reason,
                    ["purgeHistoryDeleteUri"] = BaseUrl + "/instances/INSTANCEID" + suffix,
                },
                ["creationUrls"] = new JObject
                {
                    ["createNewInstancePostUri"] = BaseUrl + "/orchestrators/{functionName}[/{instanceId}]" + suffix,
                },
                ["rpcBaseUrl"] = rpc ? "http://rpc.test/durabletask/" : null,
            };
        }

        private static JObject Invoke(JObject binding, string operation,
            string requestUrl = "https://frontend.test/api/start?code=starter-key")
        {
            InitialSessionState sessionState = InitialSessionState.CreateDefault2();
            if (OperatingSystem.IsWindows())
            {
                // Embedded Windows hosts otherwise block the local module under Restricted policy.
                sessionState.ExecutionPolicy = Microsoft.PowerShell.ExecutionPolicy.RemoteSigned;
            }
            using Ps shell = Ps.Create(sessionState);
            shell.AddScript("""
                if (-not ('HttpResponseContext' -as [type])) {
                    Add-Type -TypeDefinition @'
                public class HttpResponseContext {
                    public System.Net.HttpStatusCode StatusCode { get; set; }
                    public object Body { get; set; }
                    public System.Collections.IDictionary Headers { get; set; }
                }
                '@
                }
                """);
            shell.Invoke();
            Assert.False(shell.HadErrors, string.Join(Environment.NewLine, shell.Streams.Error));
            shell.Commands.Clear();
            shell.AddCommand("Import-Module")
                .AddParameter("Name", Path.Combine(AppContext.BaseDirectory, "AzureFunctions.PowerShell.Durable.SDK.psm1"))
                .AddParameter("Force");
            shell.Invoke();
            Assert.False(shell.HadErrors, string.Join(Environment.NewLine, shell.Streams.Error));
            shell.Commands.Clear();
            shell.AddScript("""
                param($bindingJson, $operation, $requestUrl)
                $ErrorActionPreference = 'Stop'
                $client = ConvertFrom-Json -InputObject $bindingJson -AsHashtable
                $global:CapturedRequest = $null
                function global:Invoke-RestMethod {
                    param($Uri, $Method, $ContentType, $Body, $Headers, [int]$MaximumRedirection = -1)
                    $global:CapturedRequest = @{
                        Uri = [string]$Uri
                        Headers = $Headers
                        MaximumRedirection = $MaximumRedirection
                    }
                }
                if ($operation -eq 'response') {
                    $response = New-DurableOrchestrationCheckStatusResponse `
                        -Request @{ Url = $requestUrl } `
                        -InstanceId instance -DurableClient $client
                    @{ StatusCode = [int]$response.StatusCode; Body = $response.Body; Headers = $response.Headers } |
                        ConvertTo-Json -Depth 100
                } else {
                    $null = Start-DurableOrchestration -FunctionName Allowed -InstanceId instance `
                        -DurableClient $client -Version '2.0'
                    $global:CapturedRequest | ConvertTo-Json -Depth 100
                }
                """)
                .AddArgument(binding.ToString(Formatting.None))
                .AddArgument(operation)
                .AddArgument(requestUrl);
            var output = shell.Invoke();
            Assert.False(shell.HadErrors, string.Join(Environment.NewLine, shell.Streams.Error));
            return JObject.Parse(Assert.Single(output).ToString());
        }
    }
}
