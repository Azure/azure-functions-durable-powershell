using System.IO;
using System.Management.Automation;
using System.Reflection;
using System.Runtime.Loader;

namespace DurableSDK
{
    // This project component separates the dependencies of the Durable SDK from those of the PowerShell worker.
    // See https://docs.microsoft.com/en-us/powershell/scripting/dev-cross-plat/resolving-dependency-conflicts?view=powershell-7.2
    // for more detail.
    public class ModuleInitializer : IModuleAssemblyInitializer, IModuleAssemblyCleanup
    {
        private static string s_binBasePath = Path.GetFullPath(
            Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "Dependencies"));

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
            // In .NET Core, PowerShell deals with assembly probing so our logic is much simpler
            // We only want to resolve the DurableEngine.dll assembly, which will be loaded into
            // the custom ALC.
            if (!assemblyName.Name.Equals("DurableEngine"))
            {
                return null;
            }

            // We load the Durable Engine assembly through the Dependency ALC, the context in which
            // all of its dependencies will be resolved (preventing potential conflicts with the
            // PowerShell worker's dependencies). The DurableEngine.dll will then be passed into
            // PowerShell's ALC.
            return DependencyAssemblyLoadContext.GetForDirectory(s_binBasePath).LoadFromAssemblyName(assemblyName);
        }
    }
}
