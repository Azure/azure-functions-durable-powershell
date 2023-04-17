using namespace System.Net

param($Context)

$ErrorActionPreference = 'Stop'

$output = Invoke-DurableActivityE -FunctionName "Hello" -Input "Tokyo"

return $output
