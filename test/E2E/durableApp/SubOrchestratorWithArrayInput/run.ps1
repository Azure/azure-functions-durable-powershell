using namespace System.Net

param($Context)

$ErrorActionPreference = 'Stop'

$output = @()
$output += Invoke-DurableSubOrchestrator -FunctionName "FanOutOnInputOrchestrator" -Input @("Tokyo", "Seattle")

return $output
