//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System;
using System.IO;
using System.Management.Automation;
using System.Reflection;
using System.Runtime.Loader;

namespace DurableSDK.AssemblyLoader
{
    // This project component separates the dependencies of the Durable SDK from those of the PowerShell worker.
    // See https://docs.microsoft.com/en-us/powershell/scripting/dev-cross-plat/resolving-dependency-conflicts?view=powershell-7.2
    // for more detail.
    public class ModuleInitializer : IModuleAssemblyInitializer, IModuleAssemblyCleanup
    {
        /// <summary>
        /// Location of assemblies to load in our custom Assembly Load Context (A:C).
        /// </summary>
        private static string sharedDependenciesPath = Path.GetFullPath(
            Path.Combine(
                Path.GetDirectoryName(typeof(ModuleInitializer).Assembly.Location),
            "Dependencies"));


        /// <summary>
        /// Lazy ALC initializer, used to instantiate a singleton ALC in the dependencies path.
        /// </summary>
        private static Lazy<DependencyAssemblyLoadContext> lazyALC = new(() => new DependencyAssemblyLoadContext(sharedDependenciesPath));

        /// <summary>
        /// Singleton ALC.
        /// </summary>
        private static DependencyAssemblyLoadContext singletonALC
        {
            get
            {
                return lazyALC.Value;
            }
        }

        public void OnImport()
        {
            // Add the Resolving event handler here
            AssemblyLoadContext.Default.Resolving += ResolveDurableEngineAssembly;
        }

        public void OnRemove(PSModuleInfo psModuleInfo)
        {
            // Remove the Resolving event handler here
            AssemblyLoadContext.Default.Resolving -= ResolveDurableEngineAssembly;
        }

        private static Assembly ResolveDurableEngineAssembly(
            AssemblyLoadContext assemblyLoadContext,
            AssemblyName assemblyName)
        {
            // We only want to resolve the DurableEngine.dll assembly, which will be loaded into
            // the custom ALC.
            if (!assemblyName.Name.Equals("DurableEngine"))
            {
                return null;
            }

            // We load the Durable Engine assembly through the Dependency ALC, the context in which
            // all of its dependencies will be resolved (preventing potential conflicts with the
            // PowerShell worker's dependencies).
            return singletonALC.LoadFromAssemblyName(assemblyName);
        }
    }
}
