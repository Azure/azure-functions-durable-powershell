param($Context)

$output = @()

$retryOptions1 = New-DurableRetryOptions -FirstRetryInterval (New-TimeSpan -Seconds 1) -MaxNumberOfAttempts 4
$output +=  -FunctionName "FlakyFunction" -Input "Seattle" -RetryOptions $retryOptions1

$output