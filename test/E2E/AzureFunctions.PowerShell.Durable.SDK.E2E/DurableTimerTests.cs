// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AzureFunctions.PowerShell.Durable.SDK.Tests.E2E;
using Xunit;

namespace AzureFunctions.PowerShell.Durable.SDK.E2E
{
    [Collection(Constants.DurableAppCollectionName)]
    public class DurableTimerTests : DurableTests
    {
        public DurableTimerTests(DurableAppFixture fixture) : base(fixture) {}

        /*
            Verifies that the Durable execution model correctly replays the same collection of CurrentUtcDateTimes.
            The orchestrator writes CurrentUtcDateTime values to a temporary file. File contents are expected to
            take one of two forms:

            Case 1                      Case 2
            Line                        Line
            0     ---                   0     ---
            1     <Timestamp1>          1     <Timestamp1>    
            2     <Timestamp1>          2     <Timestamp1>
            3     ---                   3     ---
            4     <Timestamp1>          4     <Timestamp1>
            5     <Timestamp1>          5     <Timestamp1>
            6     <Timestamp2>          6     <Timestamp2>
            7     <Timestamp2>          7     <Timestamp2>
            8     <Timestamp2>          8     <Timestamp2>
            9     ---                   9     ---
            10    <Timestamp1>          10    <Timestamp1>
            11    <Timestamp1>          11    <Timestamp1>
            12    <Timestamp2>          12    <Timestamp2>
            13    <Timestamp2>          13    <Timestamp2>
            14    <Timestamp2>          14    <Timestamp2>
            15    <Timestamp3>          15    ---
                                        16    <Timestamp1>
                                        17    <Timestamp1>
                                        18    <Timestamp2>
                                        19    <Timestamp2>
                                        20    <Timestamp2>
                                        21    <Timestamp3>
        */
        [Fact]
        public async Task OrchestratorReplaysCurrentUtcDateTime()
        {
            var initialResponse = await Utilities.GetHttpStartResponse("CurrentUtcDateTimeOrchestrator", queryString: string.Empty);
            await ValidateOutput(
                initialResponse,
                (dynamic statusResponseBody) =>
                {
                    var output = statusResponseBody.output.ToString();
                    string[] lines = output.Split('\n');

                    // Expect the format to be as in Case 1
                    var delineatorLines = new int[] { 0, 3, 9 };
                    var timestamp1Lines = new int[] { 1, 2, 4, 5, 10, 11 };
                    var timestamp2Lines = new int[] { 6, 7, 8, 12, 13, 14 };
                    int timestamp3Line = 15;

                    // Updates the expected format to be Case 2 if it is not Case 1
                    if (lines[timestamp3Line] == "---")
                    {
                        delineatorLines = new int[] { 0, 3, 9, 15 };
                        timestamp1Lines = new int[] { 1, 2, 4, 5, 10, 11, 16, 17 };
                        timestamp2Lines = new int[] { 6, 7, 8, 12, 13, 14, 18, 19, 20 };
                        timestamp3Line = 21;

                        Assert.True(delineatorLines.Length == 4);
                        Assert.True(timestamp1Lines.Length == 8);
                        Assert.True(timestamp2Lines.Length == 9);
                    }

                    Assert.Equal("---", lines[delineatorLines[0]]);
                    VerifyArrayItemsAreEqual(array: lines, indices: delineatorLines);
                    VerifyArrayItemsAreEqual(array: lines, indices: timestamp1Lines);
                    VerifyArrayItemsAreEqual(array: lines, indices: timestamp2Lines);
                    // Verifies that the Timestamp3 line is not a delineator, Timestamp2, or Timestamp1 line
                    Assert.NotEqual(lines[timestamp3Line], lines[delineatorLines[0]]);
                    Assert.NotEqual(lines[timestamp3Line], lines[timestamp1Lines[0]]);
                    Assert.NotEqual(lines[timestamp3Line], lines[timestamp2Lines[0]]);
                    return;
                });
        }

        /*
            Verifies that the Start-DurableTimer cmdlet restarts the orchestrator and updates the CurrentUtcDateTime
            after the timer is fired. The orchestrator writes CurrentUtcDateTime values to a temp file. File contents
            are expected to take the following form:
            
            Line
            0     ---
            1     <Timestamp1>
            2     ---
            3     <Timestamp1>
            4     <Timestamp2>
        */
        [Fact]
        private async Task DurableTimerStopsOrchestratorAndUpdatesCurrentUtcDateTime()
        {
            var initialResponse = await Utilities.GetHttpStartResponse("DurableTimerOrchestrator", queryString: string.Empty);
            await ValidateOutput(
                initialResponse,
                (dynamic statusResponseBody) =>
                {
                    var output = statusResponseBody.output.ToString();
                    string[] lines = output.Split('\n');

                    // Expect the format to be as in Case 1
                    var delineatorLines = new int[] { 0, 2 };
                    var timestamp1Lines = new int[] { 1, 3 };
                    int timestamp2Line = 4;

                    Assert.Equal("---", lines[delineatorLines[0]]);
                    VerifyArrayItemsAreEqual(array: lines, indices: delineatorLines);
                    VerifyArrayItemsAreEqual(array: lines, indices: timestamp1Lines);
                    // Verifies that the Timestamp2 line is not a delineator or Timestamp1 line
                    Assert.NotEqual(lines[timestamp2Line], lines[delineatorLines[0]]);
                    Assert.NotEqual(lines[timestamp2Line], lines[timestamp1Lines[0]]);
                });
        }

        private void VerifyArrayItemsAreEqual(string[] array, int[] indices)
        {
            if (indices.Length > 0)
            {
                var expected = array[indices[0]];
                for (int i = 1; i < indices.Length; i++)
                {
                    Assert.True(indices[i] < array.Length, $"Array length is {array.Length} but index is {indices[i]}");
                    Assert.Equal(expected, array[indices[i]]);
                }
            }
        }
    }
}