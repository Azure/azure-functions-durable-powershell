//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

namespace DurableEngine
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An orchestration action that represents calling an activity function with retry.
    /// </summary>
    internal class CallActivityWithRetryAction : OrchestrationAction
    {
        /// <summary>
        /// The activity function name.
        /// </summary>
        public readonly string FunctionName;

        /// <summary>
        /// The input to the activity function.
        /// </summary>
        public readonly object Input;

        /// <summary>
        /// Retry options.
        /// </summary>
        public readonly Dictionary<string, object> RetryOptions;

        internal CallActivityWithRetryAction(string functionName, object input, RetryOptions retryOptions)
            : base(ActionType.CallActivityWithRetry)
        {
            FunctionName = functionName;
            Input = input;
            RetryOptions = retryOptions.RetryOptionsDictionary;
        }  
    }
}