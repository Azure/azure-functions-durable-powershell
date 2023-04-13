using namespace System.Net

param($Context)

$ErrorActionPreference = 'Stop'

$output = @()
$output += Invoke-DurableSubOrchestratorE -FunctionName "SimpleOrchestrator"
$output += Invoke-DurableActivityE -FunctionName "Hello" -Input "Seattle"

return $output
