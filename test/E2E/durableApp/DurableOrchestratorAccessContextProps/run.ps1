param($Context)

$output = @()

$output += $Context.IsReplaying
$output += Invoke-DurableActivity -FunctionName 'Hello' -Input $Context.InstanceId
$output += $Context.IsReplaying
$output += $Context.Version
$output
