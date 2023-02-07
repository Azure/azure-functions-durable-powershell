using namespace System.Net

param($Context)

$ErrorActionPreference = 'Stop'

Write-Host "DurablePatternsOrchestrator: started. Input: $($Context.Input)"

Set-DurableCustomStatusE -CustomStatus 'Custom status: started'

# Function chaining
$output = @()
$output += Invoke-DurableActivityE -FunctionName "Hello" -Input "Tokyo"

# Fan-out/Fan-in
$tasks = @()
$tasks += Invoke-DurableActivityE -FunctionName "Hello" -Input "Seattle" -NoWait
$tasks += Invoke-DurableActivityE -FunctionName "Hello" -Input "London" -NoWait
$output += Wait-DurableTask -Task $tasks

# Retries
$retryOptions = New-DurableRetryOptionsE -FirstRetryInterval (New-Timespan -Seconds 2) -MaxNumberOfAttempts 5
$inputData = @{ Name = 'Toronto'; StartTime = $Context.CurrentUtcDateTime }
$output += Invoke-DurableActivityE -FunctionName "FlakyFunction" -Input $inputData -RetryOptions $retryOptions

Set-DurableCustomStatusExternal -CustomStatus 'Custom status: finished'

Write-Host "DurablePatternsOrchestrator: finished."

return $output
