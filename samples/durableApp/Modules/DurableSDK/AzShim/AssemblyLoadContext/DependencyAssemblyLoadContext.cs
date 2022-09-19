//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace DurableSDK.AssemblyLoader
{
    internal class DependencyAssemblyLoadContext : AssemblyLoadContext
    {
        // To ensure there's a single ALC per dependency directory.
        private static readonly ConcurrentDictionary<string, DependencyAssemblyLoadContext> directoryToDependencyALCsMap = new ConcurrentDictionary<string, DependencyAssemblyLoadContext>();

        // The ALC is specific to a directory, so we construct them based on a directory path
        internal static DependencyAssemblyLoadContext GetForDirectory(string directoryPath)
        {
            return directoryToDependencyALCsMap.GetOrAdd(directoryPath, (path) => new DependencyAssemblyLoadContext(path));
        }

        // Path where shared dependencies reside
        private readonly string directoryPath;

        public DependencyAssemblyLoadContext(string dependencyDirPath)
            : base(nameof(DependencyAssemblyLoadContext))
        {
            directoryPath = dependencyDirPath;
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            // Try to load the assembly with the given name from the dependency directory
            string assemblyPath = Path.Join(directoryPath, $"{assemblyName.Name}.dll");
            if (File.Exists(assemblyPath))
            {
                return LoadFromAssemblyPath(assemblyPath);
            }
            // Return null for other assemblies to allow assembly resolution to continue
            return null;
        }

    }
}
