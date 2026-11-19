// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace AzureFunctions.PowerShell.Durable.SDK.Tests.E2E
{
    using System;
    using System.Diagnostics;
    using Xunit;
    using Xunit.Abstractions;
    using Xunit.Sdk;
    
    public class DurableAppFixture : IDisposable
    {
        private IMessageSink _diagnosticMessageSink;
        private bool _disposed;
        private Process? _funcProcess;

        public DurableAppFixture(IMessageSink diagnosticMessageSink)
        {
            _diagnosticMessageSink = diagnosticMessageSink;
            if (bool.TryParse(Environment.GetEnvironmentVariable(
                    "AzureFunctionsJobHost__extensions__durableTask__httpSettings__excludeManagementKeyFromUrls"),
                    out bool excludeKey) && excludeKey
                && string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("E2E_TEST_DURABLE_SYSTEM_KEY")))
            {
                throw new InvalidOperationException(
                    "E2E_TEST_DURABLE_SYSTEM_KEY is required when management URL key exclusion is enabled.");
            }
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FunctionAppUrl")))
            {
                if (!Uri.TryCreate(Constants.FunctionsHostUrl, UriKind.Absolute, out var host)
                    || (host.Scheme != Uri.UriSchemeHttp && host.Scheme != Uri.UriSchemeHttps))
                {
                    throw new InvalidOperationException("FunctionAppUrl must be an absolute HTTP or HTTPS URL.");
                }
                WriteDiagnosticMessage("Using the configured external Functions host.");
                return;
            }
            // Kill any existing func processes
            WriteDiagnosticMessage("Shutting down any running Functions hosts.");
            FixtureHelpers.KillExistingFuncProcesses();

            // Start func processes
            WriteDiagnosticMessage("Starting Functions hosts.");
            _funcProcess = FixtureHelpers.GetFuncProcess();

            FixtureHelpers.StartProcessWithLogging(_funcProcess);
            Thread.Sleep(TimeSpan.FromSeconds(30));
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                WriteDiagnosticMessage("DurableAppFixture disposing.");
                if (_funcProcess != null)
                {
                    WriteDiagnosticMessage($"Shutting down Functions host for {Constants.DurableAppCollectionName}");
                    _funcProcess.Kill();
                    _funcProcess.Dispose();
                }
            }
            _disposed = true;
        }

        private bool WriteDiagnosticMessage(string message)
        {
            return _diagnosticMessageSink.OnMessage(new DiagnosticMessage(message));
        }

        [CollectionDefinition(Constants.DurableAppCollectionName)]
        public class DurableAppCollection : ICollectionFixture<DurableAppFixture>
        {
            // This class has no code, and is never created. Its purpose is simply
            // to be the place to apply [CollectionDefinition] and all the
            // ICollectionFixture<> interfaces.
        }
    }
}