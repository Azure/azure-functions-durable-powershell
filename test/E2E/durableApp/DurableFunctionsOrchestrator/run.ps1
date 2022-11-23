param($Context)

$output = @()

Write-Output($Context.IsReplaying)
Invoke-DurableActivity -FunctionName 'HelloActivityFunction' -Input 'Seattle'
$timer1 = Start-DurableTimer -Duration (New-Timespan -Seconds 10) -nowait
Write-Host 'Started durable timer1'
$timer2 = Start-DurableTimer -Duration (New-Timespan -Seconds 5) -nowait
Write-Host "Started durable timer 2"
Wait-DurableTask -Task $timer2
Write-Host "Waited durable task"
Stop-DurableTimerTask -Task $timer1
Write-Host 'stopped durable timer'
Invoke-DurableActivity -FunctionName 'HelloActivityFunction' -Input 'Tokyo'
Write-Host($Context.IsReplaying)
Invoke-DurableActivity -FunctionName 'HelloActivityFunction' -Input $Context.InstanceId
Write-Host "Success"
$output
