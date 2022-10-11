param($Context)

$output = @()

#1/0
#throw "err"
# $Context.IsReplaying
# $Context.CurrentUtcDateTime
# Write-Host "Beginning first activity function"
# $res1 = Invoke-DurableActivityE -FunctionName 'Hello' -Input 'Seattle' -NoWait
# Write-Host "Passed first activity function"

# Write-Output("got that task")
# Wait-DurableTaskE -Task $task
# $Context.CurrentUtcDateTime
# $Context.IsReplaying
# Write-Output("got that track 2 replay")

# 1/0
# $res2 = Invoke-DurableActivityE -FunctionName 'Hello' -Input 'London' -NoWait
# $res3 = Invoke-DurableActivityE -FunctionName 'Sleep' -Input 'Dreaming' -NoWait
# Write-Output($res2)
# Write-Output("Stage 2")
# $output += Wait-DurableTaskE -Task @($res2, $res3)
# $output += $res3
# Write-Output("Stage 3")
# Write-Output($res3)
# $res4 = Wait-Durabletask -Task @($res3)
# Write-Output($res4)


#$duration = New-TimeSpan -Seconds 30
#Start-DurableTimer -Duration $duration
#Write-Output("Stage 4")
#Start-DurableExternalEventListener -EventName "hello"
#Write-Output("Stage 5")

$timer1 = Start-DurableTimer -Duration (New-Timespan -Seconds 10) -nowait
Write-Host "Started durable timer1 at $($Context.CurrentUtcDateTime)"
$timer2 = Start-DurableTimer -Duration (New-Timespan -Seconds 5) -nowait
Write-Host "Started durable timer 2 $($Context.CurrentUtcDateTime)"
Wait-DurableTask -Task @($timer1, $timer2) -Any
Write-Host "Finished awaiting timers at $($Context.CurrentUtcDateTime)"
Stop-DurableTimerTask -Task $timer1
Write-Host 'Stopped durable timer'
# Invoke-DurableActivity -FunctionName 'HelloActivityFunction' -Input 'Tokyo'
# Write-Host($Context.IsReplaying)
# Invoke-DurableActivity -FunctionName 'HelloActivityFunction' -Input $Context.InstanceId

$output
