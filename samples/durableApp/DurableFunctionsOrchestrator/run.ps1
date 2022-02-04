param($Context)

$output = @()

Write-Output($Context.IsReplaying)
Invoke-DurableActivityExternal -FunctionName 'Hello' -Input 'Seattle'
Invoke-DurableActivityExternal -FunctionName 'Hello' -Input 'Tokyo'
Write-Output($Context.IsReplaying)
Invoke-DurableActivityExternal -FunctionName 'Hello' -Input $Context.InstanceId
Write-Output($Context.IsReplaying)
Write-Output("wasu")

"success"
