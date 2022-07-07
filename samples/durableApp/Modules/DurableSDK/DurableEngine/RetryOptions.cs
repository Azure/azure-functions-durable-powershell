//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member 'member'

using System;

namespace DurableEngine
{
    public class RetryOptions
    {
        internal TimeSpan FirstRetryInterval { get; }

        internal int MaxNumberOfAttempts { get; }

        internal double? BackoffCoefficient { get; }

        internal TimeSpan? MaxRetryInterval { get; }

        internal TimeSpan? RetryTimeout { get; }

        internal RetryOptions(
            TimeSpan firstRetryInterval,
            int maxNumberOfAttempts,
            double? backoffCoefficient,
            TimeSpan? maxRetryInterval,
            TimeSpan? retryTimeout)
        {
            this.FirstRetryInterval = firstRetryInterval;
            this.MaxNumberOfAttempts = maxNumberOfAttempts;
            this.BackoffCoefficient = backoffCoefficient;
            this.MaxRetryInterval = maxRetryInterval;
            this.RetryTimeout = retryTimeout;
        }
    }
}
