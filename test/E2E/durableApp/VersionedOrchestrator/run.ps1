using namespace System.Net

param($Context)

$ErrorActionPreference = 'Stop'

$reportedSubOrchestratorVersion =
    if ($Context.Input -and $Context.Input.SubOrchestratorVersion) {
        Invoke-DurableSubOrchestrator -FunctionName "VersionedSubOrchestrator" -Version $Context.Input.SubOrchestratorVersion
    } else {
        Invoke-DurableSubOrchestrator -FunctionName "VersionedSubOrchestrator"
    }

return @($Context.Version, $reportedSubOrchestratorVersion)
