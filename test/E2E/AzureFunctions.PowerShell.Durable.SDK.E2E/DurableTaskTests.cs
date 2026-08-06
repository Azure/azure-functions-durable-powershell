// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using DurableEngine.Models;
using DurableEngine.Tasks;
using System.Collections;
using Xunit;

namespace AzureFunctions.PowerShell.Durable.SDK.E2E
{
    public class DurableTaskTests
    {
        [Fact]
        public void DurableTimerTaskWithoutOrchestrationContextThrowsInformativeException()
        {
            var privateData = new Hashtable();

            var exception = Assert.Throws<InvalidOperationException>(
                () => new DurableTimerTask(TimeSpan.Zero, noWait: default, privateData));

            Assert.Equal(
                "Durable orchestration cmdlets can only be used within orchestrator functions, " +
                "not in activity functions or client functions.",
                exception.Message);
        }

        [Fact]
        public void DurableTaskWithOrchestrationContextCanBeConstructed()
        {
            var orchestrationContext = new OrchestrationContext();
            var privateData = new Hashtable
            {
                [DurableEngine.OrchestrationInvoker.ContextKey] = orchestrationContext,
            };

            var task = new ExternalEventTask("TestEvent", noWait: default, privateData);

            Assert.NotNull(task);
        }
    }
}
