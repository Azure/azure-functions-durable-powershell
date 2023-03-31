using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DurableEngine.Actions
{
    internal class CallSubOrchestratorWithRetryAction : OrchestrationAction
    {
        /// <summary>
        /// The activity function name.
        /// </summary>
        public readonly string FunctionName;

        /// <summary>
        /// The sub-orchestrator instanceId.
        /// </summary>
        public readonly string InstanceId;

        /// <summary>
        /// The input to the sub-orchestrator.
        /// </summary>
        public readonly object Input;

        /// <summary>
        /// Retry options.
        /// </summary>
        public readonly Dictionary<string, object> RetryOptions;

        internal CallSubOrchestratorWithRetryAction(string functionName, object input, string instanceId, RetryOptions retryOptions)
            : base(ActionType.CallSubOrchestratorWithRetry)
        {
            FunctionName = functionName;
            InstanceId = instanceId;
            Input = input;
            RetryOptions = retryOptions.RetryOptionsDictionary;
        }
    }
}
