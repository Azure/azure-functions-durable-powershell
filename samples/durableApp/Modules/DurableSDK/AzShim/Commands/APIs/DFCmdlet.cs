using DurableEngine;
using Microsoft.PowerShell.Commands;
using System.Collections;
using System.Management.Automation;

namespace DurableSDK.Commands.APIs
{
    public abstract class DFCmdlet : PSCmdlet
    {
        internal abstract DFCommand GetCommand();
        private DFCommand cmd;

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
