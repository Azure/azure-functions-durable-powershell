//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member 'member'


namespace DurableSDK.Commands.APIs
{
    using System;
    using System.Management.Automation;
    using DurableEngine;

    /// <summary>
    /// Creates retry plicies that can be passed as parameters to various Durable cmdlets
    /// </summary>
    [Cmdlet("New", "DurableRetryOpts")]
    public class NewDurableRetryOptions : PSCmdlet
    {
        /// <summary>
        /// The first retry interval. Must be greater than 0.
        /// </summary>
        [Parameter(Mandatory = true)]
        public TimeSpan FirstRetryInterval { get; set; }

        /// <summary>
        /// The maximum number of retry attempts. Must be 1 or greater.
        /// </summary>
        [Parameter(Mandatory = true)]
        public int MaxNumberOfAttempts { get; set; }
        
        /// <summary>
        /// The exponential backoff coefficient. Must be 1 or greater.
        /// </summary>
        [Parameter]
        public double BackoffCoefficient { get; set; } = 1;
        
        /// <summary>
        /// The maximum delay between attempts.
        /// </summary>
        [Parameter]
        public TimeSpan? MaxRetryInterval { get; set; } = null;
        
        /// <summary>
        /// The timeout for retries.
        /// </summary>
        [Parameter]
        public TimeSpan? RetryTimeout { get; set; } = null;

        protected override void EndProcessing()
        {
            WriteObject(new RetryOptions(
                MaxNumberOfAttempts,
                FirstRetryInterval,
                BackoffCoefficient,
                MaxRetryInterval,
                RetryTimeout));
        }
    }
}