param($Context)

$output = @()

Write-Output($Context.IsReplaying)
Invoke-DurableActivityE -FunctionName 'HelloActivityFunction' -Input 'Seattle'
$timer1 = Start-DurableTimerE -Duration (New-Timespan -Seconds 10) -nowait
Write-Host 'Started durable timer1'
$timer2 = Start-DurableTimerE -Duration (New-Timespan -Seconds 5) -nowait
Write-Host "Started durable timer 2"
Wait-DurableTaskE -Task $timer2
Write-Host "Waited durable task"
Stop-DurableTimerTaskE -Task $timer1
Write-Host 'stopped durable timer'
Invoke-DurableActivityE -FunctionName 'HelloActivityFunction' -Input 'Tokyo'
Write-Host($Context.IsReplaying)
Invoke-DurableActivityE -FunctionName 'HelloActivityFunction' -Input $Context.InstanceId
Write-Host "Success"
$output
