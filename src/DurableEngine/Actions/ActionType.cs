//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

namespace DurableEngine
{
    /// <summary>
    /// Action types
    /// </summary>
    internal enum ActionType
    {
        /// <summary>
        /// Call an activity function.
        /// </summary>
        CallActivity = 0,

        /// <summary>
        /// Call an activity function with retry.
        /// </summary>
        CallActivityWithRetry = 1,

        /// <summary>
        /// Call a sub-orchestration function.
        /// </summary>
        CallSubOrchestrator = 2,

        /// <summary>
        /// Call a sub-orchestration function with retry.
        /// </summary>
        CallSubOrchestratorWithRetry = 3,

        /// <summary>
        /// Run the orchestration function as a loop.
        /// </summary>
        ContinueAsNew = 4,

        /// <summary>
        /// Create a timer.
        /// </summary>
        CreateTimer = 5,

        /// <summary>
        /// Wait for an external event.
        /// </summary>
        WaitForExternalEvent = 6,

        /// <summary>
        /// Call an entity function.
        /// </summary>
        CallEntity = 7,

        /// <summary>
        /// Make an Http call.
        /// </summary>
        CallHttp = 8,

        /// <summary>
        /// Wait for first result in a set of Tasks.
        /// </summary>
        WhenAny = 11,

        /// <summary>
        /// Wait for a set of Tasks to complete.
        /// </summary>
        WhenAll = 12
    }
}