//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member 'member'

using System;

namespace DurableEngine.Models
{
    /// <summary>
    /// Defines retry policies that can be passed as parameters to various operations.
    /// </summary>
    public class RetryOptions
    {
        /// <summary>
        /// The first retry interval. Must be greater than 0.
        /// </summary>
        internal TimeSpan FirstRetryInterval { get; }

        /// <summary>
        /// The maximum number of retry attempts.
        /// </summary>
        internal int MaxNumberOfAttempts { get; }

        /// <summary>
        /// The backoff coefficient.
        /// </summary>
        internal double? BackoffCoefficient { get; }

        /// <summary>
        /// The max retry interval.
        /// </summary>
        internal TimeSpan? MaxRetryInterval { get; }

        /// <summary>
        /// The timeout for retries[
        /// </summary>
        internal TimeSpan? RetryTimeout { get; }

        internal RetryOptions(
            TimeSpan firstRetryInterval,
            int maxNumberOfAttempts,
            double? backoffCoefficient,
            TimeSpan? maxRetryInterval,
            TimeSpan? retryTimeout)
        {
            FirstRetryInterval = firstRetryInterval;
            MaxNumberOfAttempts = maxNumberOfAttempts;
            BackoffCoefficient = backoffCoefficient;
            MaxRetryInterval = maxRetryInterval;
            RetryTimeout = retryTimeout;
        }
    }
}
