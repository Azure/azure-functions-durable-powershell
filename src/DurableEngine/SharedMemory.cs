//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

namespace DurableEngine
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using DurableEngine.Tasks;

    /// <summary>
    /// Represents the shared memory that both the user-code thread and the invoker thread can access.
    /// It used to manage the execution of threads and to communicate data between them.
    /// </summary>
    internal class SharedMemory
    {
        internal readonly List<OrchestrationAction> actions = new();

        // Handles to manage (block or wake up) the execution invoker and user-code threads 
        internal readonly AutoResetEvent invokerThreadTurn = new AutoResetEvent(initialState: false);
        internal readonly AutoResetEvent userCodeThreadTurn = new AutoResetEvent(initialState: false);

        // The current DF Task to await
        internal DurableTask currTask = null;
        // A map between SDK-level tasks and DTFx-level tasks. Useful retrieving the SDK-level output of WhenAny.
        internal Dictionary<Task, DurableTask> taskMap = new ();

        internal void Add(OrchestrationAction action)
        {
            actions.Add(action);
        }

        /// <summary>
        /// Blocks User-code thread and wakes up Orchestration-invoker thread.
        /// This is usually to let the orchestration-invoker `await` some API.
        /// </summary>
        public void YieldToInvokerThread()
        {
            // Wake invoker thread.
            invokerThreadTurn.Set();
            userCodeThreadTurn.WaitOne();
        }

        /// <summary>
        /// Blocks Orchestration-invoker thread until the user-code thread completes or yields.
        /// </summary>
        /// <param name="completionHandle">The WaitHandle tracking if the user-code thread completed.</param>
        /// <returns>True if the user-code thread completed, False if it requests an API to be awaited.</returns>
        public bool WaitForInvokerThreadTurn(WaitHandle completionHandle)
        {

            // Wake up when either the user-code returns, or when we're yielded-to for `await`'ing.
            var index = WaitHandle.WaitAny(new[] { completionHandle, invokerThreadTurn });
            var shouldStop = index == 0;
            return shouldStop;
        }

        /// <summary>
        /// Wakes up the user-code thread without blocking the invoker thread.
        /// The invoker thread should block itself afterwards to prevent races.
        /// </summary>
        public void WakeUserCodeThread()
        {
            userCodeThreadTurn.Set();
        }
    }
}