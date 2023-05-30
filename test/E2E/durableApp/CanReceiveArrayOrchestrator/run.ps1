using namespace System.Net

param($Context)

$ErrorActionPreference = 'Stop'

$output = Invoke-DurableActivity -FunctionName "GetArrayActivity"
return $output
