//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member 'member'

using System;
using System.Collections.Generic;
using Microsoft.DurableTask;

namespace DurableEngine
{
    /// <summary>
    /// Defines retry policies that can be passed as parameters to various operations.
    /// </summary>
    public class RetryOptions : RetryPolicy
    {
        internal Dictionary<string, object> RetryOptionsDictionary { get; set; }

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
            // We maintain a dictionary representation of the retry policies to pass onto DTFx through 
            // the CallActivityWithRetryAction. This dictionary should be initialized when this class is
            // initialized because we must only pass on non-null, sensible parameter values to DTFx: the
            // parameter values of the underlying RetryPolicy class are all non-nullable, with unsuitable
            // defaults for the optional MaxRetryInterval and RetryTimeOut parameters. Furthermore,
            // of RetryOptions cannot be modified after construction, so it is safe to do this eager
            // serialization in the constructor.
            RetryOptionsDictionary = new Dictionary<string, object>()
                            {
                                { "firstRetryIntervalInMilliseconds", ToIntMilliseconds(firstRetryInterval) },
                                { "maxNumberOfAttempts", maxNumberOfAttempts },
                                { "backoffCoefficient", backoffCoefficient }
                            };
            AddOptionalValue(RetryOptionsDictionary, "maxRetryIntervalInMilliseconds", maxRetryInterval, ToIntMilliseconds);
            AddOptionalValue(RetryOptionsDictionary, "maxRetryIntervalInMilliseconds", retryTimeout, ToIntMilliseconds);
        }

        private static void AddOptionalValue<T>(
            Dictionary<string, object> dictionary,
            string name,
            T? nullable,
            Func<T, object> transformValue) where T : struct
        {
            if (nullable.HasValue)
            {
                dictionary.Add(name, transformValue(nullable.Value));
            }
        }

        private static object ToIntMilliseconds(TimeSpan timespan)
        {
            return (int)timespan.TotalMilliseconds;
        }
    }
}