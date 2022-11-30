//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member 'member'


namespace DurableEngine.Models
{
    using System;

    /// <summary>
    /// Durable activity failure exception.
    /// </summary>
    internal class ActivityFailureException : Exception
    {
        internal ActivityFailureException()
        {
        }

        internal ActivityFailureException(string message)
            : base(message)
        {
        }
        internal ActivityFailureException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}