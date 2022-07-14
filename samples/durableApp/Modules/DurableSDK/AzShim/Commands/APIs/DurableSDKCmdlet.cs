using DurableEngine;
using System.Management.Automation;

namespace DurableSDK.Commands
{
    public abstract class DurableSDKCmdlet : PSCmdlet
    {
        internal abstract DurableEngineCommand GetCommand();
        private DurableEngineCommand cmd;

        protected override void EndProcessing()
        {
            cmd = GetCommand();
            cmd.Exec(WriteObject, WriteError);
        }

        protected override void StopProcessing()
        {
            cmd.Stop();
        }
    }
}
