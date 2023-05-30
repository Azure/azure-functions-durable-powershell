param($Context)

$output = @()

$task = Invoke-DurableActivity -FunctionName 'Hello' -Input "world" -NoWait
$firstTask = Wait-DurableTask -Task @($task) -Any
$output += Get-DurableTaskResult -Task $firstTask
$output
