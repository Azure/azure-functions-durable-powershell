//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace DurableSDK.AssemblyLoader
{
    internal class DependencyAssemblyLoadContext : AssemblyLoadContext
    {
        private readonly string _dependencyDirPath;

        public DependencyAssemblyLoadContext(string dependencyDirPath)
            : base(nameof(DependencyAssemblyLoadContext))
        {
            _dependencyDirPath = dependencyDirPath;
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            // Try to load the assembly with the given name from the dependency directory
            string assemblyPath = Path.Join(_dependencyDirPath, $"{assemblyName.Name}.dll");
            if (File.Exists(assemblyPath))
            {
                return LoadFromAssemblyPath(assemblyPath);
            }
            // Return null for other assemblies to allow assembly resolution to continue
            return null;
        }

    }
}
