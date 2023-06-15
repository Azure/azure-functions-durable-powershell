param($Context)

# iterate over inputs, create a "Hello" activity task per input
$tasks = @()
foreach ($input in $Context.Input)
{
    $tasks += Invoke-DurableActivity -FunctionName "Hello" -Input $input -NoWait
}

# fan-out over all activity tasks and aggregate results
$output = Wait-DurableTask -Task $tasks

$output
