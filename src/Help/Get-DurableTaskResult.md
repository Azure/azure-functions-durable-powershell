---
external help file: AzureFunctions.PowerShell.Durable.SDK.dll-Help.xml
Module Name: AzureFunctions.PowerShell.Durable.SDK
online version:
schema: 2.0.0
---

# Get-DurableTaskResult

## SYNOPSIS

Gets the result value from a completed durable task.

## SYNTAX

```
Get-DurableTaskResult -Task <DurableTask> [<CommonParameters>]
```

## DESCRIPTION

Retrieves the result value from a completed durable task. This cmdlet blocks execution
until the specified task completes and then returns the task's result value. The type
and content of the returned result depend on what the underlying activity function or
sub-orchestrator returned. This is typically used within orchestrator functions to
obtain the output of previously scheduled durable tasks.

## EXAMPLES

### Example 1

```powershell
$task = Invoke-DurableActivity -FunctionName "GetUserData" -Input @{ UserId = 123 } -NoWait
$result = Get-DurableTaskResult -Task $task
Write-Host "User data: $result"
```

This example demonstrates invoking a durable activity function asynchronously with -NoWait to get a task, then using Get-DurableTaskResult to retrieve the result from the task.

## PARAMETERS

### -Task

The durable task object whose result you want to retrieve. This should be a task returned from cmdlets like Invoke-DurableActivity, Invoke-DurableSubOrchestrator, Start-DurableTimer, or Start-DurableExternalEventListener.

```yaml
Type: DurableTask
Parameter Sets: (All)
Aliases:

Required: True
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

Returns the result value from the completed durable task. The type and content depend on what the underlying activity function or sub-orchestrator returned.

## NOTES

- This cmdlet is typically used within orchestrator functions to retrieve results from tasks created with the -NoWait parameter.
- The cmdlet blocks execution until the specified task completes, which maintains the deterministic nature of orchestrations.
- If a task fails, this cmdlet will throw an exception with details about the failure.
- Tasks should be created within the same orchestration context where this cmdlet is called.
- Use Wait-DurableTask when you need to wait for multiple tasks with timeout or any/all semantics.

## RELATED LINKS

[Durable Functions for PowerShell](https://github.com/Azure/azure-functions-durable-powershell)
