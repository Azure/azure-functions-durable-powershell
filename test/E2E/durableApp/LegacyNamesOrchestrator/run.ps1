using namespace System.Net

param($Context)

$ErrorActionPreference = 'Stop'

Write-Host "DurableOrchestratorLegacyNames: started."

Invoke-ActivityFunction -FunctionName "Hello" -Input "Tokyo"

Write-Host "DurableOrchestratorLegacyNames: finished."

return $output
