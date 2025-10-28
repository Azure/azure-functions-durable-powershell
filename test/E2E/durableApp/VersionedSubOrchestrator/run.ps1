using namespace System.Net

param($Context)

$ErrorActionPreference = 'Stop'

Write-Warning "VersionedSubOrchestrator: $($Context.Version)"

return $Context.Version
