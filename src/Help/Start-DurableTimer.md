---
external help file: AzureFunctions.PowerShell.Durable.SDK.dll-Help.xml
Module Name: AzureFunctions.PowerShell.Durable.SDK
online version:
schema: 2.0.0
---

# Start-DurableTimer

## SYNOPSIS

Starts a durable timer that will complete after the specified duration.

## SYNTAX

```
Start-DurableTimer -Duration <TimeSpan> [-NoWait] [<CommonParameters>]
```

## DESCRIPTION

Creates a durable timer that will complete after the specified duration. By default,
this cmdlet blocks until the timer fires and returns completion status. Use the
-NoWait switch to return a task object immediately that can be awaited later. Durable
timers are fault-tolerant and will survive orchestration replays and restarts, making
them the recommended way to implement delays in durable orchestrations.

## EXAMPLES

### Example 1 - Synchronous execution (default behavior)

```powershell
$duration = New-TimeSpan -Minutes 30
Start-DurableTimer -Duration $duration
Write-Host "Timer completed, continuing with scheduled operation"
```

This example shows the default behavior where the cmdlet blocks until the timer fires.

### Example 2 - Asynchronous execution with -NoWait

```powershell
$duration = New-TimeSpan -Minutes 30
$timerTask = Start-DurableTimer -Duration $duration -NoWait
# Continue with other orchestration logic...
Wait-DurableTask -Task $timerTask
Write-Host "Timer completed, continuing with scheduled operation"
```

This example creates a timer that will fire 30 minutes from now using -NoWait, allowing other logic to execute while the timer runs.

## PARAMETERS

### -Duration

The duration to wait before the timer fires, specified as a TimeSpan object. Alternative to using FireAt parameter.

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

### -NoWait

A switch parameter that, when specified, starts the timer without waiting for it to complete. Returns a task that can be awaited or cancelled later.

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

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

This cmdlet does not accept pipeline input. All parameters must be specified directly.

## OUTPUTS

### DurableTask

Returns a DurableTask object representing the timer. This task can be used with Wait-DurableTask to wait for the timer to complete or with Stop-DurableTimerTask to cancel it.

## NOTES

- This cmdlet can only be used within orchestrator functions, not in activity functions or client functions.
- Durable timers are fault-tolerant and will survive orchestration replays and Azure Functions host restarts.
- Use durable timers instead of Start-Sleep or similar delay mechanisms in orchestrator functions to maintain deterministic replay behavior.
- Timer tasks can be cancelled using Stop-DurableTimerTask if created with the -NoWait parameter.
- The maximum timer duration is limited by the orchestration timeout configuration of your Azure Functions app.
- Timers created with -NoWait return immediately and can be used in timeout patterns with Wait-DurableTask -Any.

## RELATED LINKS

[Durable Functions for PowerShell](https://github.com/Azure/azure-functions-durable-powershell)
