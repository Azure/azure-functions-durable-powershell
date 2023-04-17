using namespace System.Net

param($Context)

$ErrorActionPreference = 'Stop'

Write-Host "DurablePatternsOrchestrator: started. Input: $($Context.Input)"

Set-DurableCustomStatus -CustomStatus 'Custom status: started'

# Function chaining
$output = @()
$output += Invoke-DurableActivity -FunctionName "Hello" -Input "Tokyo"

# Fan-out/Fan-in
$tasks = @()
$tasks += Invoke-DurableActivity -FunctionName "Hello" -Input "Seattle" -NoWait
$tasks += Invoke-DurableActivity -FunctionName "Hello" -Input "London" -NoWait
$output += Wait-DurableTask -Task $tasks

# Retries
$retryOptions = New-DurableRetryOptions -FirstRetryInterval (New-Timespan -Seconds 2) -MaxNumberOfAttempts 5
$inputData = @{ Name = 'Toronto'; StartTime = $Context.CurrentUtcDateTime }
$output += Invoke-DurableActivity -FunctionName "FlakyFunction" -Input $inputData -RetryOptions $retryOptions

Set-DurableCustomStatus -CustomStatus 'Custom status: finished'

Write-Host "DurablePatternsOrchestrator: finished."
Write-Host $output
Write-Host "DurablePatternsOrchestrator: finished."

return $output
