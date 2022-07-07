using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace DurableSDK
{
    internal class DependencyAssemblyLoadContext : AssemblyLoadContext
    {
        private static readonly string s_psHome = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        private static readonly ConcurrentDictionary<string, DependencyAssemblyLoadContext> s_dependencyLoadContexts = new ConcurrentDictionary<string, DependencyAssemblyLoadContext>();

        internal static DependencyAssemblyLoadContext GetForDirectory(string directoryPath)
        {
            return s_dependencyLoadContexts.GetOrAdd(directoryPath, (path) => new DependencyAssemblyLoadContext(path));
        }

        private readonly string _dependencyDirPath;

        public DependencyAssemblyLoadContext(string dependencyDirPath)
            : base(nameof(DependencyAssemblyLoadContext))
        {
            _dependencyDirPath = dependencyDirPath;
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            string assemblyFileName = $"{assemblyName.Name}.dll";

            // Now try to load the assembly from the dependency directory
            string dependencyAsmPath = Path.Join(_dependencyDirPath, assemblyFileName);
            if (File.Exists(dependencyAsmPath))
            {
                return LoadFromAssemblyPath(dependencyAsmPath);
            }

            return null;
        }
 
    }
}
