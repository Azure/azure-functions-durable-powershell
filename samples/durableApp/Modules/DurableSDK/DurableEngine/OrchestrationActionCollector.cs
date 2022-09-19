//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

namespace DurableEngine
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    internal class OrchestrationActionCollector
    {
        internal readonly List<List<OrchestrationAction>> _actions = new();

        internal readonly AutoResetEvent cancelationToken = new AutoResetEvent(initialState: false);
        internal readonly AutoResetEvent hasToAwait = new AutoResetEvent(initialState: false);
        internal DurableSDKTask currTask = null;
        internal Dictionary<Task, DurableSDKTask> taskMap = new ();
        internal Hashtable data = null;


        internal readonly AutoResetEvent startOfNewCmdLet = new AutoResetEvent(initialState: false);

        private bool _nextBatch = true;

        internal void Add(OrchestrationAction action)
        {
            if (_nextBatch)
            {
                _actions.Add(new List<OrchestrationAction>());
                _nextBatch = false;
            }

            _actions.Last().Add(action);
            NextBatch();
        }

        internal void NextBatch()
        {
            _nextBatch = true;
        }

        public void WaitForActivityResult()
        {
            hasToAwait.Set();
            cancelationToken.WaitOne();
        }

        public void ResumeInvoker()
        {
            hasToAwait.Reset();
            startOfNewCmdLet.Set();

        }
    }
}
