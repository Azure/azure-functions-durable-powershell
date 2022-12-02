param($Context)

$output = @()

# # TEST CASE 0: Basic Invoke-DurableActivity
# Write-Host "TEST 0: Basic Invoke-DurableActivity"
# Write-Host "Beginning first activity function"
# $output += Invoke-DurableActivityE -FunctionName 'Hello' -Input 'Seattle'
# Write-Host "Passed first activity function"

# # TEST CASE 1: Basic Start-DurableTimer
# Write-Host "TEST 1: Basic Start-DurableTimer"
# Write-Host "Started durable timer at $($Context.CurrentUtcDateTime)"
# Start-DurableTimerE -Duration (New-Timespan -Seconds 5)
# Write-Host "Finished awaiting timer at $($Context.CurrentUtcDateTime)"

# # TEST CASE 2: WaitAll for two activity functions and two timers
# Write-Host "TEST 2: WaitAll for two activity functions and two timers"
# $activity1 = Invoke-DurableActivityE -FunctionName "Hello" -Input "Test 2 Activity 1" -NoWait
# $activity2 = Invoke-DurableActivityE -FunctionName "Hello" -Input "Test 2 Activity 2" -NoWait
# $timer1 = Start-DurableTimerE -Duration (New-Timespan -Seconds 10) -NoWait
# Write-Host "Started durable timer 1 at $($Context.CurrentUtcDateTime)"
# $timer2 = Start-DurableTimerE -Duration (New-Timespan -Seconds 5) -NoWait
# Write-Host "Started durable timer 2 $($Context.CurrentUtcDateTime)"
# $allTasks = @($activity1, $activity2, $timer1, $timer2)
# $allTaskResults = Wait-DurableTaskE -Task $allTasks
# if ($allTaskResults.Count -ne $allTasks.Count)
# {
#     throw "The WaitAll output array length is $($allTaskResults.Count) rather than $($allTasks.Count), as expected."
# }
# $output += $allTaskResults
# Write-Host "Finished awaiting timers at $($Context.CurrentUtcDateTime)"

# # TEST CASE 3: WaitAny for two timers
# Write-Host "TEST 3: WaitAny for two timers"
# $timer1 = Start-DurableTimerE -Duration (New-Timespan -Seconds 10) -NoWait
# Write-Host "Started durable timer 1 at $($Context.CurrentUtcDateTime)"
# $timer2 = Start-DurableTimerE -Duration (New-Timespan -Seconds 5) -NoWait
# Write-Host "Started durable timer 2 $($Context.CurrentUtcDateTime)"
# $winner1 = Wait-DurableTaskE -Task @($timer1, $timer2) -Any
# Write-Host "$($winner1 -eq $timer2)"
# Write-Host "Finished awaiting timers at $($Context.CurrentUtcDateTime)"
# Stop-DurableTimerTaskE -Task $timer1
# Write-Host 'Stopped durable timer'

# TEST CASE 4: Basic external event
# Write-Host "TEST 4: Basic external event"
# $approvalEvent1 = Start-DurableExternalEventListenerE -EventName "ApprovalEvent1"
# if ($approvalEvent1)
# {
#     Write-Host "Approved event 1 successfully."
#     $output += $approvalEvent1
# }
# else
# {
#     throw "Approval event 1 failed"    
# }

# TEST CASE 5: External event without waiting
Write-Host "TEST 4: Basic external event"
$approvalEvent2 = Start-DurableExternalEventListenerE -EventName "ApprovalEvent2" -NoWait
$timer3 = Start-DurableTimerE -Duration (New-Timespan -Seconds 60) -NoWait
$winner2 = Wait-DurableTaskE -Task @($approvalEvent2, $timer3) -Any
if ($approvalEvent2 -eq $winner2)
{
    Write-Host "Approved event 2 successfully within time."
    $output += $approvalEvent2
    Stop-DurableTimerTaskE -Task $timer3
}
else
{
    throw "Approval event 2 failed"    
}

$output