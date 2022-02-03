param($Context)

$output = @()

$Context.IsReplaying
Invoke-DurableActivityExternal -FunctionName 'Hello' -Input 'Seattle'
$Context.CurrentUtcDateTime
Invoke-DurableActivityExternal -FunctionName 'Hello' -Input 'Tokyo'
Invoke-DurableActivityExternal -FunctionName 'Hello' -Input $Context.InstanceId

"success"
