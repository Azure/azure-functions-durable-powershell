//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

namespace DurableEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    internal class OrchestrationActionCollector
    {
        internal readonly List<List<OrchestrationAction>> _actions = new List<List<OrchestrationAction>>();
        internal readonly AutoResetEvent _orchestratorHasToAwait = new AutoResetEvent(initialState: false);
        internal readonly AutoResetEvent _userCodeHasToAwait = new AutoResetEvent(initialState: false);
        internal readonly ManualResetEvent _cancelationToken = new ManualResetEvent(initialState: false);
        public DurableEngineCommand CurrentDurableEngineCommand = null;
        private bool _nextBatch = true;

        internal void Add(OrchestrationAction action)
        {
            if (_nextBatch)
            {
                _actions.Add(new List<OrchestrationAction>());
                _nextBatch = false;
            }

            _actions.Last().Add(action);
        }

        internal void NextBatch()
        {
            _nextBatch = true;
        }

        // Puts the orchestrator thread to sleep and waits for the user orchestrator to complete/throw
        // or an activity function to wake it up.
        public Tuple<bool, List<List<OrchestrationAction>>> WaitForActions(WaitHandle completionWaitHandle)
        {
            var waitHandles = new[] { _orchestratorHasToAwait, completionWaitHandle };
            var signaledHandleIndex = WaitHandle.WaitAny(waitHandles);
            var signaledHandle = waitHandles[signaledHandleIndex];
            var shouldStop = ReferenceEquals(signaledHandle, _orchestratorHasToAwait);
            return Tuple.Create(shouldStop, _actions);
        }

        public void ResumeOrchestrator()
        {
            _orchestratorHasToAwait.Set();
        }

        public void ResumeUserCode()
        {
            _userCodeHasToAwait.Set();
        }

        public void WaitForActivityResult()
        {
            _userCodeHasToAwait.WaitOne();
        }

        public void Stop()
        {
            _cancelationToken.Set();
        }
    }
}
