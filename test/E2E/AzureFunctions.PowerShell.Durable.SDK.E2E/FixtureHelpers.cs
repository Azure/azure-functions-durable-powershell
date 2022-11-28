// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace AzureFunctions.PowerShell.Durable.SDK.Tests.E2E
{
    using System.Diagnostics;

    public static class FixtureHelpers
    {
        public static Process GetFuncProcess()
        {
            var funcProcess = new Process();
            // The output directory for the corresponding .dll is AzureFunctions.PowerShell.Durable.SDK.E2E/bin/{Configuration}/{targetFramework}
            var rootDir = Path.GetFullPath(String.Format(@"..{0}..{0}..{0}..{0}..{0}..{0}", Path.DirectorySeparatorChar));

            funcProcess.StartInfo.UseShellExecute = false;
            funcProcess.StartInfo.RedirectStandardError = true;
            funcProcess.StartInfo.RedirectStandardOutput = true;
            funcProcess.StartInfo.CreateNoWindow = true;
            funcProcess.StartInfo.WorkingDirectory = Path.Combine(rootDir, String.Format(@"test{0}E2E{0}DurableApp", Path.DirectorySeparatorChar));
            funcProcess.StartInfo.FileName = Environment.GetEnvironmentVariable(Constants.FUNC_PATH);
            funcProcess.StartInfo.ArgumentList.Add("start");

            return funcProcess;
        }

        public static void StartProcessWithLogging(Process funcProcess)
        {
            funcProcess.ErrorDataReceived += (sender, e) => Console.WriteLine(e?.Data);
            funcProcess.OutputDataReceived += (sender, e) => Console.WriteLine(e?.Data);

            funcProcess.Start();

            funcProcess.BeginErrorReadLine();
            funcProcess.BeginOutputReadLine();
        }

        public static void KillExistingFuncProcesses()
        {
            foreach (var func in Process.GetProcessesByName("func"))
            {
                func.Kill();
            }
        }
    }
}