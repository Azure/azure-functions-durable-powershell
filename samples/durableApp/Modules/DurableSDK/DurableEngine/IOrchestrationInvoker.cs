//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

namespace DurableEngine
{
    using System.Collections;

    public interface IOrchestrationInvoker
    {
        internal Hashtable InvokeExternal(OrchestrationContext context, IPowerShellServices pwsh, object privateData);
    }
}
