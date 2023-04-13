param($Context)

$output = @()

$task = Invoke-DurableActivityE -FunctionName 'Hello' -Input "world" -NoWait
$firstTask = Wait-DurableTaskE -Task @($task) -Any
$output += Get-DurableTaskResult -Task @($firstTask)
$output
