param($Context)

$output = @()

$retryOptions1 = New-DurableRetryOptions -FirstRetryInterval (New-TimeSpan -Seconds 1) -MaxNumberOfAttempts 4
$output += Invoke-DurableActivity -FunctionName "FlakyFunction" -Input "Seattle" -RetryOptions $retryOptions1

$output