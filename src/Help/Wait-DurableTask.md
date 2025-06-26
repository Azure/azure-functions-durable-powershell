---
external help file: AzureFunctions.PowerShell.Durable.SDK.dll-Help.xml
Module Name: AzureFunctions.PowerShell.Durable.SDK
online version:
schema: 2.0.0
---

# Wait-DurableTask

## SYNOPSIS

Waits for one or more durable tasks to complete and returns their results.

## SYNTAX

```
Wait-DurableTask [-Any] [-NoWait] -Task <DurableTask[]> [<CommonParameters>]
```

## DESCRIPTION

Waits for one or more durable tasks to complete before continuing orchestration execution.
By default, waits for all specified tasks to complete. When the -Any switch is used,
waits for any one of the tasks to complete. This cmdlet is used within orchestrator
functions to coordinate multiple asynchronous operations and retrieve their results.

## EXAMPLES

### Example 1

```powershell
$task1 = Invoke-DurableActivity -FunctionName "Step1" -Input "data1" -NoWait
$task2 = Invoke-DurableActivity -FunctionName "Step2" -Input "data2" -NoWait
$completedTasks = Wait-DurableTask -Task @($task1, $task2)

# Get the actual results from the completed tasks
$results = @()
foreach ($task in $completedTasks) {
    $results += Get-DurableTaskResult -Task $task
}
Write-Host "Both tasks completed with results: $results"
```

This example demonstrates waiting for multiple durable activity tasks to complete. Note that Wait-DurableTask returns the task objects themselves, not the results. To get the actual activity results, you need to use Get-DurableTaskResult on each completed task.

### Example 2

```powershell
$task1 = Invoke-DurableActivity -FunctionName "FastOperation" -Input $data1 -NoWait
$task2 = Invoke-DurableActivity -FunctionName "SlowOperation" -Input $data2 -NoWait
$firstCompletedTask = Wait-DurableTask -Task @($task1, $task2) -Any

# Determine which task completed first and get its result
if ($firstCompletedTask -eq $task1) {
    $activityResult = Get-DurableTaskResult -Task $firstCompletedTask
    Write-Host "FastOperation completed first with result: $activityResult"
} elseif ($firstCompletedTask -eq $task2) {
    $activityResult = Get-DurableTaskResult -Task $firstCompletedTask
    Write-Host "SlowOperation completed first with result: $activityResult"
} else {
    # This block should never be hit
    Write-Host "Unexpected task completion"
}
```

This example demonstrates waiting for any one of the tasks to complete using the -Any parameter. The cmdlet returns the first completed task object (not the result), which you can then compare to determine which task finished first and retrieve its actual result using Get-DurableTaskResult.

## PARAMETERS

### -Any

When specified, the cmdlet waits for any one of the provided tasks to complete. By default, the cmdlet waits for all tasks to complete before returning.

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

### -NoWait

When specified, the cmdlet returns immediately without waiting for the tasks to complete. This is useful for scheduling the wait operation itself as a task.

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

### -Task

An array of durable task objects to wait for. These should be tasks returned from cmdlets like Invoke-DurableActivity, Invoke-DurableSubOrchestrator, Start-DurableTimer, or Start-DurableExternalEventListener.

```yaml
Type: DurableTask[]
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

Returns the completed task object(s), not the task results. If waiting for a single task, returns the task object directly. If waiting for multiple tasks, returns an array of task objects in the same order as the input tasks. To get the actual results from the tasks, use Get-DurableTaskResult on the returned task objects.

## NOTES

- This cmdlet can only be used within orchestrator functions, not in activity functions or client functions.
- When using the -Any parameter, only the first completed task object is returned. Other tasks continue running in the background.
- Tasks passed to this cmdlet must be created with the -NoWait parameter from other durable cmdlets.
- The cmdlet is fault-tolerant and will survive orchestration replays and restarts.
- When waiting for multiple tasks without -Any, task objects are returned in the same order as the input tasks, regardless of completion order.
- This cmdlet returns task objects, not task results. Use Get-DurableTaskResult to retrieve the actual results from completed tasks.
- Use this cmdlet to implement common patterns like fan-out/fan-in, timeouts, and race conditions in orchestrations.

## RELATED LINKS

[Durable Functions for PowerShell](https://github.com/Azure/azure-functions-durable-powershell)
