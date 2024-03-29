//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member 'member'

namespace DurableEngine
{
    /// <summary>
    /// Base class that represents an orchestration action.
    /// </summary>
    internal abstract class OrchestrationAction
    {
        internal OrchestrationAction(ActionType actionType)
        {
            ActionType = actionType;
        }

        public readonly ActionType ActionType;
    }
}