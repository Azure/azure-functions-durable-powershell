//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

namespace DurableEngine
{
    using System;
    using System.Collections;
    using System.Management.Automation;

    public interface IOrchestrationInvoker
    {
        public Func<PowerShell, object> CreateInvokerFunction();
        internal Hashtable Invoke(IPowerShellServices pwsh);
    }
}
