using namespace System.Net

param($Context)

$ErrorActionPreference = 'Stop'

$output = @()
$output += Invoke-DurableSubOrchestrator -FunctionName "SimpleOrchestrator"
$output += Invoke-DurableActivity -FunctionName "Hello" -Input "Seattle"

return $output
