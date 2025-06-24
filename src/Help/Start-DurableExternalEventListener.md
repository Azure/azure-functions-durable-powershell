---
external help file: AzureFunctions.PowerShell.Durable.SDK.dll-Help.xml
Module Name: AzureFunctions.PowerShell.Durable.SDK
online version:
schema: 2.0.0
---

# Start-DurableExternalEventListener

## SYNOPSIS

Starts listening for an external event with a specified name and returns a task that will complete when the event is received.

## SYNTAX

```
Start-DurableExternalEventListener -EventName <String> [-NoWait] [<CommonParameters>]
```

## DESCRIPTION

Creates a task that waits for an external event with the specified name to be raised
for the current orchestration instance. By default, this cmdlet blocks until the
external event is received and returns the event data directly. Use the -NoWait switch
to return a task object immediately that can be awaited later. External events enable
building interactive workflows and human-in-the-loop scenarios.

## EXAMPLES

### Example 1 - Synchronous execution (default behavior)

```powershell
$approvalData = Start-DurableExternalEventListener -EventName "UserApproval"
if ($approvalData.Approved) { /* proceed */ } else { /* handle rejection */ }
```

This example shows the default behavior where the cmdlet blocks until the external event is received and returns the event data directly.

### Example 2 - Asynchronous execution with -NoWait

```powershell
$eventListener = Start-DurableExternalEventListener -EventName "UserApproval" -NoWait
# Continue with other orchestration logic...
$approvalData = Get-DurableTaskResult -Task $eventListener
if ($approvalData.Approved) { /* proceed */ } else { /* handle rejection */ }
```

This example starts listening for an external event asynchronously using -NoWait, allowing other orchestration logic to execute while waiting for the event.

## PARAMETERS

### -EventName

The name of the external event to listen for. This must match the event name used when sending the external event to the orchestration instance.

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

### -NoWait

A switch parameter that, when specified, starts the external event listener without waiting for the event. Returns a task that can be awaited later.

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

Returns a DurableTask object that represents the external event listener. This task can be used with Wait-DurableTask to wait for the external event to be received.

## NOTES

- This cmdlet can only be used within orchestrator functions, not in activity functions or client functions.
- External events are sent to orchestrations using the Send-DurableExternalEvent cmdlet from client functions.
- Event listeners are fault-tolerant and will survive orchestration replays and Azure Functions host restarts.
- Multiple listeners can wait for the same event name within a single orchestration.
- Use the -NoWait parameter when implementing timeout patterns or waiting for multiple different events.
- Event data is preserved across orchestration replays, ensuring exactly-once delivery semantics.
- The event name is case-sensitive and must match exactly between the listener and sender.

## RELATED LINKS

[Durable Functions for PowerShell](https://github.com/Azure/azure-functions-durable-powershell)
