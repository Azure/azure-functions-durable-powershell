//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

namespace DurableSDK.Commands.APIs
{
    using DurableEngine.Tasks;
    using System;
    using System.Management.Automation;

    [Cmdlet("Get", "DurableTaskResult")]
    public class GetDurableTaskResultCommand : PSCmdlet
    {
        /// <summary>
        /// Gets and sets the Task whose result to obtain
        /// </summary>
        [Parameter(Mandatory = true)]
        [ValidateNotNull]
        public DurableTask Task { get; set; }

        protected override void EndProcessing()
        {
            Console.WriteLine("START");
            Task.NoWait = false; // force result to materialize
            Task.Execute(WriteObject, WriteError);
        }

        protected override void StopProcessing()
        {
            Console.WriteLine("STOP");
            Task.Stop();
        }
    }
}