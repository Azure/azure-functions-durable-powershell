---
external help file: AzureFunctions.PowerShell.Durable.SDK.dll-Help.xml
Module Name: AzureFunctions.PowerShell.Durable.SDK
online version:
schema: 2.0.0
---

# Invoke-DurableActivity

## SYNOPSIS

Invokes a durable activity function.

## SYNTAX

```
Invoke-DurableActivity -FunctionName <String> [-Input <Object>] [-RetryOptions <RetryPolicy>] [-NoWait] [<CommonParameters>]
```

## DESCRIPTION

Schedules a durable activity function for execution. By default, this cmdlet blocks
until the activity completes and returns the result directly. Use the -NoWait switch
to return a task object immediately without waiting for completion, allowing the
orchestrator to schedule multiple activities concurrently and retrieve their results
later using Get-DurableTaskResult or Wait-DurableTask.

## EXAMPLES

### Example 1 - Synchronous execution (default behavior)

```powershell
$result = Invoke-DurableActivity -FunctionName "ProcessData" -Input @{ Data = "example"; ProcessType = "validation" }
Write-Host "Processing result: $result"
```

This example shows the default behavior where the cmdlet blocks until completion and returns the result directly.

### Example 2 - Asynchronous execution with -NoWait

```powershell
$task = Invoke-DurableActivity -FunctionName "ProcessData" -Input @{ Data = "example"; ProcessType = "validation" } -NoWait
$result = Get-DurableTaskResult -Task $task
Write-Host "Processing result: $result"
```

This example shows how to invoke a durable activity function asynchronously using -NoWait, which returns a task object that can be awaited later.

## PARAMETERS

### -FunctionName

The name of the activity function to invoke. This should match the name of an activity function defined in your Azure Functions app.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Input

The input data to pass to the activity function. This can be any object that will be serialized and passed as input to the activity function.

```yaml
Type: Object
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -NoWait

When specified, the cmdlet returns a task object immediately without waiting for completion. By default, the cmdlet blocks and waits for the activity to complete before returning the result.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -RetryOptions

A retry policy object created with New-DurableRetryPolicy that defines how the activity should be retried if it fails. If not specified, the activity will not be retried on failure.

```yaml
Type: RetryPolicy
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

### System.Object

Returns the result of the activity function execution by default. If -NoWait is specified, returns a DurableTask object that can be used with Get-DurableTaskResult or Wait-DurableTask to retrieve the result later.

## NOTES

- This cmdlet can only be used within orchestrator functions, not in activity functions or client functions.
- The activity function name must match a function defined in your Azure Functions app.
- Activity functions are automatically retried on transient failures when a RetryOptions policy is specified.
- Activity functions should be stateless and idempotent.
- Use the -NoWait parameter when you need to invoke multiple activities concurrently (fan-out pattern).

## RELATED LINKS

[Durable Functions for PowerShell](https://github.com/Azure/azure-functions-durable-powershell)
