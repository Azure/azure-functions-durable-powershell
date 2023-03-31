using namespace System.Net

param($Context)

$ErrorActionPreference = 'Stop'

$output = Invoke-DurableSubOrchestratorE -FunctionName "SimpleOrchestrator"

return $output
