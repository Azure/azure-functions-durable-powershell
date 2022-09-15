using DurableEngine;
using System.Management.Automation;

namespace DurableSDK.Commands
{
    public abstract class DurableSDKCmdlet : PSCmdlet
    {
        private DurableEngineCommand durableTask;

        protected abstract DurableEngineCommand CreateDurableTask();

        protected override void EndProcessing()
        {
            durableTask = CreateDurableTask();
            durableTask.Execute(WriteObject, WriteError);
        }

        protected override void StopProcessing()
        {
            durableTask.Stop();
        }
    }
}
