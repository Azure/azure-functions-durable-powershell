//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

namespace DurableEngine
{

    internal class WhenAllAction : OrchestrationAction
    {

        public readonly OrchestrationAction[] CompoundActions;

        internal WhenAllAction(OrchestrationAction[] compoundActions)
            : base(ActionType.WhenAll)
        {
            CompoundActions = compoundActions;
        }
    }
}
