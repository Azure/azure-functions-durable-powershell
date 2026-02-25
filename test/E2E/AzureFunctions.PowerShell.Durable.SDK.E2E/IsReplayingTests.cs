// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AzureFunctions.PowerShell.Durable.SDK.Tests.E2E;
using System.Net;
using Xunit;

namespace AzureFunctions.PowerShell.Durable.SDK.E2E
{
    [Collection(Constants.DurableAppCollectionName)]
    public class IsReplayingTests : DurableTests
    {
        public IsReplayingTests(DurableAppFixture fixture) : base(fixture) {}

        /// <summary>
        /// Verifies that IsReplaying is True before a completed activity (during replay)
        /// and False after the last scheduled task completes.
        /// </summary>
        [Fact]
        public async Task IsReplayingIsTrueBeforeActivityAndFalseAfter()
        {
            var initialResponse = await Utilities.GetHttpStartResponse("IsReplayingSingleActivityOrchestrator");
            Assert.Equal(HttpStatusCode.Accepted, initialResponse.StatusCode);

            await ValidateDurableWorkflowResults(
                initialResponse,
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

                    // On the final replay: IsReplaying is True before the (already completed) activity
                    Assert.Equal("True", finalStatusResponseBody.output[0].ToString());
                    // Activity result
                    Assert.Equal("Hello ReplayTest", finalStatusResponseBody.output[1].ToString());
                    // After the last task, no more events to replay, so IsReplaying is False
                    Assert.Equal("False", finalStatusResponseBody.output[2].ToString());
                });
        }

        /// <summary>
        /// Verifies that IsReplaying remains True while replaying multiple completed activities,
        /// and only becomes False after the final activity completes.
        /// </summary>
        [Fact]
        public async Task IsReplayingTransitionsCorrectlyAcrossMultipleActivities()
        {
            var initialResponse = await Utilities.GetHttpStartResponse("IsReplayingMultipleActivitiesOrchestrator");
            Assert.Equal(HttpStatusCode.Accepted, initialResponse.StatusCode);

            await ValidateDurableWorkflowResults(
                initialResponse,
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

                    // On the final replay all three activities have completed, so:
                    // Before activity 1 (replaying) -> True
                    Assert.Equal("True", finalStatusResponseBody.output[0].ToString());
                    Assert.Equal("Hello First", finalStatusResponseBody.output[1].ToString());

                    // Before activity 2 (still replaying) -> True
                    Assert.Equal("True", finalStatusResponseBody.output[2].ToString());
                    Assert.Equal("Hello Second", finalStatusResponseBody.output[3].ToString());

                    // Before activity 3 (still replaying) -> True
                    Assert.Equal("True", finalStatusResponseBody.output[4].ToString());
                    Assert.Equal("Hello Third", finalStatusResponseBody.output[5].ToString());

                    // After last activity, nothing left to replay -> False
                    Assert.Equal("False", finalStatusResponseBody.output[6].ToString());
                });
        }

        /// <summary>
        /// Verifies that IsReplaying behaves consistently when a sub-orchestrator is invoked
        /// alongside a regular activity call.
        /// </summary>
        [Fact]
        public async Task IsReplayingTransitionsCorrectlyWithSubOrchestrator()
        {
            var initialResponse = await Utilities.GetHttpStartResponse("IsReplayingWithSubOrchestratorOrchestrator");
            Assert.Equal(HttpStatusCode.Accepted, initialResponse.StatusCode);

            await ValidateDurableWorkflowResults(
                initialResponse,
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

                    // Before sub-orchestrator (replaying) -> True
                    Assert.Equal("True", finalStatusResponseBody.output[0].ToString());

                    // After sub-orchestrator, before activity (still replaying) -> True
                    Assert.Equal("True", finalStatusResponseBody.output[1].ToString());

                    // Activity result
                    Assert.Equal("Hello AfterSubOrchestrator", finalStatusResponseBody.output[2].ToString());

                    // After last task, nothing left to replay -> False
                    Assert.Equal("False", finalStatusResponseBody.output[3].ToString());
                });
        }
    }
}
