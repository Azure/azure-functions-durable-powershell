//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member 'member'

using System;
using Microsoft.DurableTask;

namespace DurableEngine
{
    /// <summary>
    /// Defines retry policies that can be passed as parameters to various operations.
    /// </summary>
    public class RetryOptions : RetryPolicy
    {
        /// <inheritdoc/>
        public RetryOptions(
            int maxNumberOfAttempts,
            TimeSpan firstRetryInterval,
            double backoffCoefficient,
            TimeSpan? maxRetryInterval,
            TimeSpan? retryTimeout) :
            base(
                maxNumberOfAttempts,
                firstRetryInterval,
                backoffCoefficient,
                maxRetryInterval,
                retryTimeout
            )
        {
        }
    }
}