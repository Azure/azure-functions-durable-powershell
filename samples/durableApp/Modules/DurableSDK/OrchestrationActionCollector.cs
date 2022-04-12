//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

namespace Microsoft.Azure.Functions.PowerShellWorker.Durable
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Threading;
    using System.Threading.Tasks;
    using DurableSDK;
    using Microsoft.Azure.Functions.PowerShellWorker.Durable.Actions;

    internal class OrchestrationActionCollector
    {
        public readonly List<List<OrchestrationAction>> _actions = new();

        private readonly AutoResetEvent _stopEvent = new AutoResetEvent(initialState: false);
        internal readonly AutoResetEvent cancelationToken = new AutoResetEvent(initialState: false);
        internal readonly AutoResetEvent hasToAwait = new AutoResetEvent(initialState: false);

        internal readonly AutoResetEvent startOfNewCmdLet = new AutoResetEvent(initialState: false);

        private bool _nextBatch = true;

        public void Add(OrchestrationAction action)
        {
            if (_nextBatch)
            {
                _actions.Add(new List<OrchestrationAction>());
                _nextBatch = false;
            }

            _actions.Last().Add(action);
        }

        public Task currTask;

        public void NextBatch()
        {
            _nextBatch = true;
        }

        public Tuple<bool, List<List<OrchestrationAction>>> WaitForActions(WaitHandle completionWaitHandle)
        {
            var waitHandles = new[] { _stopEvent, completionWaitHandle };
            var signaledHandleIndex = WaitHandle.WaitAny(waitHandles);
            var signaledHandle = waitHandles[signaledHandleIndex];
            var shouldStop = ReferenceEquals(signaledHandle, _stopEvent);
            return Tuple.Create(shouldStop, _actions);
        }

        public void Stop()
        {
            _stopEvent.Set();
        }
    }
}
