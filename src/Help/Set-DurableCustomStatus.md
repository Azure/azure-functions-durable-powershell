---
external help file: AzureFunctions.PowerShell.Durable.SDK.dll-Help.xml
Module Name: AzureFunctions.PowerShell.Durable.SDK
online version:
schema: 2.0.0
---

# Set-DurableCustomStatus

## SYNOPSIS

Sets a custom status value for the current durable orchestration instance.

## SYNTAX

```
Set-DurableCustomStatus [-CustomStatus] <Object> [<CommonParameters>]
```

## DESCRIPTION

Sets a custom status value for the current durable orchestration instance. This
status can be retrieved by external clients using the orchestration management APIs
to monitor the progress or state of the orchestration. The custom status is useful
for providing meaningful progress updates or state information that can be queried
from outside the orchestration function.

## EXAMPLES

### Example 1

```powershell
Set-DurableCustomStatus -CustomStatus @{ Phase = "Processing"; Progress = 45; ItemsProcessed = 90; TotalItems = 200 }
# Continue with orchestration logic...
Set-DurableCustomStatus -CustomStatus @{ Phase = "Finalizing"; Progress = 95; ItemsProcessed = 190; TotalItems = 200 }
```

This example shows how to set custom status information during an orchestration to track processing progress and phase.

## PARAMETERS

### -CustomStatus

A custom object containing status information that will be available when querying the orchestration instance status. This can include progress indicators, current phase, error messages, or any other relevant status data.

```yaml
Type: Object
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.Object

You can pipe objects directly to this cmdlet to set them as the custom status for the current orchestration instance.

## OUTPUTS

### None

This cmdlet does not return any output. It sets the custom status for the current orchestration instance.

## NOTES

- This cmdlet can only be used within orchestrator functions, not in activity functions or client functions.
- Custom status is visible when querying orchestration status with Get-DurableStatus.
- Status updates are useful for providing progress information to external monitoring systems.
- The status object will be JSON-serialized, so it should contain serializable data types.
- Custom status is preserved across orchestration replays and checkpoints.
- Keep status objects reasonably small to avoid performance impacts during serialization.

## RELATED LINKS

[Durable Functions for PowerShell](https://github.com/Azure/azure-functions-durable-powershell)
