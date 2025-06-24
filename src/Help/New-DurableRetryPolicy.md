---
external help file: AzureFunctions.PowerShell.Durable.SDK.dll-Help.xml
Module Name: AzureFunctions.PowerShell.Durable.SDK
online version:
schema: 2.0.0
---

# New-DurableRetryPolicy

## SYNOPSIS

Creates a new retry policy for durable activity functions and sub-orchestrators.

## SYNTAX

```
New-DurableRetryPolicy -FirstRetryInterval <TimeSpan> -MaxNumberOfAttempts <Int32> [-BackoffCoefficient <Double>] [-MaxRetryInterval <TimeSpan>] [-RetryTimeout <TimeSpan>] [<CommonParameters>]
```

## DESCRIPTION

Creates a retry policy object that can be used with durable activity functions and
sub-orchestrators to handle transient failures. The retry policy defines how many
times an operation should be retried, the delay between retries, and optionally
a maximum retry interval and backoff coefficient. This helps make orchestrations
more resilient to temporary failures in downstream services or activities.

## EXAMPLES

### Example 1

```powershell
$retryPolicy = New-DurableRetryPolicy -MaxNumberOfAttempts 5 -FirstRetryInterval (New-TimeSpan -Seconds 30) -BackoffCoefficient 2.0
$task = Invoke-DurableActivity -FunctionName "UnreliableOperation" -Input $data -RetryOptions $retryPolicy -NoWait
$result = Get-DurableTaskResult -Task $task
```

This example creates a retry policy with 5 maximum attempts, starting with a 30-second interval and doubling the delay with each retry, then uses it with a durable activity invoked asynchronously.

## PARAMETERS

### -BackoffCoefficient

The coefficient used to calculate exponential backoff between retry attempts. Must be greater than 1.0. Default is 2.0, which doubles the delay between each retry attempt.

```yaml
Type: Double
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -FirstRetryInterval

The initial retry interval in seconds. This is the delay before the first retry attempt.

```yaml
Type: TimeSpan
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -MaxNumberOfAttempts

The maximum number of retry attempts. Must be a positive integer.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -MaxRetryInterval

The maximum retry interval in seconds. Retry intervals will not exceed this value regardless of the backoff coefficient.

```yaml
Type: TimeSpan
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -RetryTimeout

The total timeout for all retry attempts as a TimeSpan. If this timeout is exceeded, no further retries will be attempted.

```yaml
Type: TimeSpan
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

This cmdlet does not accept pipeline input. All parameters must be specified directly.

## OUTPUTS

### RetryPolicy

Returns a RetryPolicy object that can be used with Invoke-DurableActivity or Invoke-DurableSubOrchestrator to specify retry behavior for failed operations.

## NOTES

- Retry policies help make orchestrations more resilient to transient failures in downstream services.
- Retry policies work with both Invoke-DurableActivity and Invoke-DurableSubOrchestrator cmdlets.
- Failed attempts will be logged in the orchestration history for debugging purposes.

## RELATED LINKS

[Durable Functions for PowerShell](https://github.com/Azure/azure-functions-durable-powershell)
