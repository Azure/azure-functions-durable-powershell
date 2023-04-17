param($Context)

$output = @()

$output += $Context.IsReplaying
$output += Invoke-DurableActivityE -FunctionName 'Hello' -Input $Context.InstanceId
$output += $Context.IsReplaying
$output
