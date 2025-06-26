---
external help file: AzureFunctions.PowerShell.Durable.SDK-help.xml
Module Name: AzureFunctions.PowerShell.Durable.SDK
online version:
schema: 2.0.0
---

# Stop-DurableOrchestration

## SYNOPSIS

Stop (terminate) a running durable orchestration instance.

## SYNTAX

```
Stop-DurableOrchestration [-InstanceId] <String> [-Reason] <String> [<CommonParameters>]
```

## DESCRIPTION

Terminates a running durable orchestration instance with the specified instance ID and reason.
This is a permanent action that cannot be undone.

## EXAMPLES

### Example 1

```powershell
Stop-DurableOrchestration -InstanceId "example-instance-id" -Reason "User requested termination"
```

Terminates the orchestration instance with the provided reason.

## PARAMETERS

### -InstanceId

The ID of the orchestration instance to terminate.

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

### -Reason

The reason for terminating the orchestration instance.
This will be recorded in the instance history.

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

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String (InstanceId)

You can pipe instance ID strings to terminate multiple orchestration instances.

### System.String (Reason)

You can pipe reason strings to specify the termination reason for orchestration instances.

## OUTPUTS

### None

This cmdlet does not return any output. It terminates the specified orchestration instance.

## NOTES

- This cmdlet can only be used in client functions, not within orchestrator or activity functions.
- Termination is permanent and cannot be undone; the orchestration cannot be resumed after termination.
- The termination is asynchronous; the orchestration may take some time to actually stop.
- Terminated orchestrations will have a final status of "Terminated" when checked with Get-DurableStatus.
- Use descriptive termination reasons to help with debugging and monitoring.
- Consider using Suspend-DurableOrchestration if you need to temporarily halt an orchestration that can be resumed later.

## RELATED LINKS

[Durable Functions for PowerShell](https://github.com/Azure/azure-functions-durable-powershell)
