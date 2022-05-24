//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

namespace Microsoft.DurableTask
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.DurableTask.Actions;

    internal class OrchestrationActionCollector
    {
        public readonly List<List<OrchestrationAction>> _actions = new();

        internal readonly AutoResetEvent cancelationToken = new AutoResetEvent(initialState: false);
        internal readonly AutoResetEvent hasToAwait = new AutoResetEvent(initialState: false);
        internal Tasks.DurableSDKTask currTask = null;
        internal Dictionary<Task, Tasks.DurableSDKTask> taskMap = new ();


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

        public void NextBatch()
        {
            _nextBatch = true;
        }

    }
}
