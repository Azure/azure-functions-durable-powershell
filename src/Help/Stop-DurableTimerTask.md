---
external help file: AzureFunctions.PowerShell.Durable.SDK.dll-Help.xml
Module Name: AzureFunctions.PowerShell.Durable.SDK
online version:
schema: 2.0.0
---

# Stop-DurableTimerTask

## SYNOPSIS

Stops (cancels) a running durable timer task.

## SYNTAX

```
Stop-DurableTimerTask -Task <DurableTimerTask> [<CommonParameters>]
```

## DESCRIPTION

Cancels a running durable timer task before it completes. This is useful for
implementing timeout patterns or canceling scheduled delays when certain conditions
are met. Once a timer task is stopped, it will not fire and any orchestration
code waiting on the timer will need to handle the cancellation appropriately.
This cmdlet is typically used in conjunction with Wait-DurableTask to implement
race conditions between timers and other operations.

## EXAMPLES

### Example 1

```powershell
$timerTask = Start-DurableTimer -Duration (New-TimeSpan -Hours 1) -NoWait
# Later, if condition is met, cancel the timer
if ($conditionMet) {
    Stop-DurableTimerTask -Task $timerTask
    Write-Host "Timer cancelled due to early completion"
}
```

This example starts a timer for 1 hour using -NoWait to get a task object, then cancels it early if a certain condition is met, preventing unnecessary waiting.

## PARAMETERS

### -Task

The timer task object returned from Start-DurableTimer that should be cancelled. The task must be in a running state to be successfully cancelled.

```yaml
Type: DurableTimerTask
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

### None

This cmdlet does not return any output. It cancels the specified durable timer task.

## NOTES

- This cmdlet is used to implement timeout patterns and early cancellation scenarios in orchestrations.
- Once a timer task is stopped, any code waiting on it should handle the cancellation appropriately.
- Timer cancellation is commonly used in race conditions between timers and other operations.
- Only running timer tasks can be successfully cancelled; completed timers cannot be stopped.
- Use this cmdlet with Wait-DurableTask to implement sophisticated timeout and cancellation patterns.

## RELATED LINKS

[Durable Functions for PowerShell](https://github.com/Azure/azure-functions-durable-powershell)
