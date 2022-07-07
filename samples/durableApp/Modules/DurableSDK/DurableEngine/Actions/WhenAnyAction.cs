//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

namespace DurableEngine
{

    internal class WhenAnyAction : OrchestrationAction
    {

        public readonly OrchestrationAction[] CompoundActions;

        internal WhenAnyAction(OrchestrationAction[] compoundActions)
            : base(ActionType.WhenAny)
        {
            CompoundActions = compoundActions;
        }
    }
}
