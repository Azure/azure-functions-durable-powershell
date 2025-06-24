---
external help file: AzureFunctions.PowerShell.Durable.SDK-help.xml
Module Name: AzureFunctions.PowerShell.Durable.SDK
online version:
schema: 2.0.0
---

# Send-DurableExternalEvent

## SYNOPSIS

Send an external event to an orchestration instance.

## SYNTAX

```
Send-DurableExternalEvent [-InstanceId] <String> [-EventName] <String> [[-EventData] <Object>] [-TaskHubName <String>] [-ConnectionName <String>] [<CommonParameters>]
```

## DESCRIPTION

Send an external event with the given event name and event data to an orchestration instance with the given instance ID.
The orchestration must be waiting for this event using Start-DurableExternalEventListener.

## EXAMPLES

### Example 1

```powershell
Send-DurableExternalEvent -InstanceId "example-instance-id" -EventName "ApprovalReceived" -EventData "approved"
```

Sends an external event to the orchestration instance.

## PARAMETERS

### -InstanceId

The ID of the orchestration instance that will handle the external event.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -EventName

The name of the external event.
This must match the event name the orchestration is waiting for.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: 2
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -EventData

The JSON-serializable data associated with the external event.

```yaml
Type: Object
Parameter Sets: (All)
Aliases:

Required: False
Position: 3
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -TaskHubName

The TaskHubName of the orchestration instance that will handle the external event.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -ConnectionName

The name of the connection string associated with TaskHubName.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String (InstanceId)

You can pipe instance ID strings to specify which orchestration instance should receive the external event.

### System.String (EventName)

You can pipe event name strings to specify the name of the external event to send.

### System.Object (EventData)

You can pipe objects containing event data to be sent with the external event.

## OUTPUTS

### None

This cmdlet does not return any output. It sends the specified external event to the target orchestration instance.

## NOTES

- This cmdlet is typically used in HTTP trigger functions or other client functions to send events to running orchestrations.
- The target orchestration must be actively listening for the event using Start-DurableExternalEventListener.
- Event names are case-sensitive and must match exactly between sender and listener.
- Events sent to orchestrations that are not listening will be queued and delivered when a listener is started.
- Large event data should be stored in external storage and referenced by URL to avoid serialization limits.
- External events provide a way to implement human-in-the-loop patterns and real-time orchestration control.

## RELATED LINKS

[Durable Functions for PowerShell](https://github.com/Azure/azure-functions-durable-powershell)
