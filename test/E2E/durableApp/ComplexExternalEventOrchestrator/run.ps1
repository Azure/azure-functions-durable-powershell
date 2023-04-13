using namespace System.Net

param($Context)

$ErrorActionPreference = 'Stop'

Write-Host "ExternalEventOrchestrator started."

$output = @()

$firstDuration = New-TimeSpan -Seconds $Context.Input.FirstDuration
$secondDuration = New-TimeSpan -Seconds $Context.Input.SecondDuration

$firstTimeout = Start-DurableTimerE -Duration $firstDuration -NoWait
$firstExternalEvent = Start-DurableExternalEventListenerE -EventName "FirstExternalEvent" -NoWait
$firstCompleted = Wait-DurableTaskE -Task @($firstTimeout, $firstExternalEvent) -Any

if ($firstCompleted -eq $firstTimeout) {
    $output += "FirstTimeout"
}
else {
    $output += "FirstExternalEvent"
    Stop-DurableTimerTaskE -Task $firstTimeout
}

$secondTimeout = Start-DurableTimerE -Duration $secondDuration -NoWait
$secondExternalEvent = Start-DurableExternalEventListenerE -EventName "SecondExternalEvent" -NoWait
$secondCompleted = Wait-DurableTaskE -Task @($secondTimeout, $secondExternalEvent) -Any

if ($secondCompleted -eq $secondTimeout) {
    $output += "SecondTimeout"
}
else {
    $output += "SecondExternalEvent"
    # If Stop-DurableTimerTask does not work as intended, then the orchestration will time out
    Stop-DurableTimerTaskE -Task $secondTimeout
}

return $output
