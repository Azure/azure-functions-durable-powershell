//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member 'member'

namespace DurableEngine.Commands
{
    using DurableEngine;
    using System.Collections;
    using System.Management.Automation;

    public class WaitDurableTaskCommand2 : DFCommand
    {
        public WaitDurableTaskCommand2(DurableSDKTask[] task, SwitchParameter any, SwitchParameter noWait, Hashtable privateData) : base(noWait, privateData)
        {
            Task = task;
            Any = any;
        }

        public DurableSDKTask[] Task { get; set; }

        public SwitchParameter Any { get; set; }

        internal override DurableSDKTask GetTask()
        {
            var sdkContext = getOrchestrationContext();
            DurableSDKTask task = null;
            if (Any)
            {
                task = new WhenAnyTask(Task, sdkContext);
            }
            else
            {
                task = new WhenAllTask(Task, sdkContext);
            }
            return task;
        }

    }
}
