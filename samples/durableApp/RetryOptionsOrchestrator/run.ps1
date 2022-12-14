param($Context)

$output = @()

$output += New-DurableRetryOptionsExternal -FirstRetryInterval (New-TimeSpan -Seconds 1) -MaxNumberOfAttempts 4

$output