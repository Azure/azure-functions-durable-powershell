using namespace System.Net

param($Context)

$ErrorActionPreference = 'Stop'

$output = Invoke-DurableActivity -FunctionName "Hello" -Input "Tokyo" -NoWait
$output = Start-DurableTimer -Duration (New-TimeSpan -Seconds 5) -NoWait
$output = Start-DurableExternalEventListener -EventName "TESTEVENTNAME" -NoWait
$output = Invoke-DurableSubOrchestrator -FunctionName "SimpleOrchestrator" -NoWait
Write-Output $output

#return "bye!"
