param($Context)

$output = @()

$retryOptions1 = New-DurableRetryOptionsE -FirstRetryInterval (New-TimeSpan -Seconds 1) -MaxNumberOfAttempts 4
$output += Invoke-DurableActivityE -FunctionName "FlakyFunction" -Input "Seattle" -RetryOptions $retryOptions1

$output