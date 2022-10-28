param($Context)

$output = @()

# TEST CASE 0: Basic Invoke-DurableActivity
Write-Host "TEST 0: Basic Invoke-DurableActivity"
Write-Host "Beginning first activity function"
$output += Invoke-DurableActivityE -FunctionName 'Hello' -Input 'Seattle'
Write-Host "Passed first activity function"

# TEST CASE 1: Basic Start-DurableTimer
Write-Host "TEST 1: Basic Start-DurableTimer"
Write-Host "Started durable timer at $($Context.CurrentUtcDateTime)"
Start-DurableTimerE -Duration (New-Timespan -Seconds 5)
Write-Host "Finished awaiting timer at $($Context.CurrentUtcDateTime)"

# TEST CASE 2: WaitAll for two activity functions and two timers
Write-Host "TEST 2: WaitAll for two activity functions and two timers"
$activity1 = Invoke-DurableActivityE -FunctionName "Hello" -Input "Test 2 Activity 1" -NoWait
$activity2 = Invoke-DurableActivityE -FunctionName "Hello" -Input "Test 2 Activity 2" -NoWait
$timer1 = Start-DurableTimerE -Duration (New-Timespan -Seconds 10) -NoWait
Write-Host "Started durable timer 1 at $($Context.CurrentUtcDateTime)"
$timer2 = Start-DurableTimerE -Duration (New-Timespan -Seconds 5) -NoWait
Write-Host "Started durable timer 2 $($Context.CurrentUtcDateTime)"
$output += Wait-DurableTaskE -Task @($activity1, $activity2, $timer1, $timer2)
Write-Host "Finished awaiting timers at $($Context.CurrentUtcDateTime)"

# TEST CASE 3: WaitAny for two timers
Write-Host "TEST 3: WaitAny for two timers"
$timer1 = Start-DurableTimerE -Duration (New-Timespan -Seconds 10) -NoWait
Write-Host "Started durable timer 1 at $($Context.CurrentUtcDateTime)"
$timer2 = Start-DurableTimerE -Duration (New-Timespan -Seconds 5) -NoWait
Write-Host "Started durable timer 2 $($Context.CurrentUtcDateTime)"
$winner = Wait-DurableTaskE -Task @($timer1, $timer2) -Any
Write-Host "$($winner -eq $timer2)"
Write-Host "Finished awaiting timers at $($Context.CurrentUtcDateTime)"
Stop-DurableTimerTaskE -Task $timer1
Write-Host 'Stopped durable timer'

$output
