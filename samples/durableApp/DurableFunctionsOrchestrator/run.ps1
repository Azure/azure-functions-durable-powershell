param($Context)

$output = @()

Write-Output($Context.IsReplaying)
Invoke-DurableActivityExternal -FunctionName 'HelloActivityFunction' -Input 'Seattle'
# $timer1 = Start-DurableTimer -Duration (New-Timespan -Seconds 10) -nowait
# Write-Output 'Started durable timer1'
# # $timer2 = Start-DurableTimer -Duration (New-Timespan -Seconds 5) -nowait
# # Write-Output "Started durable timer 2"
# # Wait-DurableTask -Task $timer2
# # Write-Output "Waited durable task"
# Stop-DurableTimerTask -Task $timer1
# Write-Output 'stopped durable timer'
# Invoke-DurableActivityExternal -FunctionName 'HelloActivityFunction' -Input 'Tokyo'
# Write-Output($Context.IsReplaying)
# Invoke-DurableActivityExternal -FunctionName 'HelloActivityFunction' -Input $Context.InstanceId
# Write-Output($Context.IsReplaying)
Write-Output("wasu")

"success"
