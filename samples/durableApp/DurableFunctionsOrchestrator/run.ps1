param($Context)

$output = @()

#1/0
#throw "err"
$Context.IsReplaying
$Context.CurrentUtcDateTime
Write-Output("Stage 00")
$task = @(Invoke-DurableActivityE -FunctionName 'Hello' -Input 'Seattle' -NoWait)
Write-Output("got that task")
Wait-DurableTaskE -Task $task
$Context.CurrentUtcDateTime
$Context.IsReplaying
#Write-Output("got that track 2 replay")

#1/0
#$res2 = Invoke-DurableActivity -FunctionName 'Hello' -Input 'London' -NoWait
#Write-Output($res2)
#Write-Output("Stage 2")
#$res3 = Wait-DurableTask -Task @($res1, $res2)
#Write-Output("Stage 3")
#Write-Output($res3)
#$res4 = Wait-Durabletask -Task @($res3)
#Write-Output($res4)


#$duration = New-TimeSpan -Seconds 30
#Start-DurableTimer -Duration $duration
#Write-Output("Stage 4")
#Start-DurableExternalEventListener -EventName "hello"
#Write-Output("Stage 5")
<#
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
#>
$output
