using System.Management.Automation;           // Windows PowerShell namespace.

namespace ModuleCmdlets
{
  [Cmdlet(VerbsDiagnostic.Test,"BinaryModuleCmdlet1")]
  public class TestBinaryModuleCmdlet1Command : Cmdlet
  {
    protected override void BeginProcessing()
    {
      WriteObject("BinaryModuleCmdlet1 exported by the ModuleCmdlets module.");
    }
  }

  [Cmdlet(VerbsDiagnostic.Test, "BinaryModuleCmdlet2")]
  public class TestBinaryModuleCmdlet2Command : Cmdlet
  {
      protected override void BeginProcessing()
      {
          WriteObject("BinaryModuleCmdlet2 exported by the ModuleCmdlets module.");
      }
  }

  [Cmdlet(VerbsDiagnostic.Test, "BinaryModuleCmdlet3")]
  public class TestBinaryModuleCmdlet3Command : Cmdlet
  {
      protected override void BeginProcessing()
      {
          WriteObject("BinaryModuleCmdlet3 exported by the ModuleCmdlets module.");
      }
  }



}