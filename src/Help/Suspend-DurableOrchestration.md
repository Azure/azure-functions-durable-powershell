---
external help file: AzureFunctions.PowerShell.Durable.SDK-help.xml
Module Name: AzureFunctions.PowerShell.Durable.SDK
online version:
schema: 2.0.0
---

# Suspend-DurableOrchestration

## SYNOPSIS

Suspend a running durable orchestration instance.

## SYNTAX

```
Suspend-DurableOrchestration [-InstanceId] <String> [-Reason] <String> [<CommonParameters>]
```

## DESCRIPTION

Suspends a running durable orchestration instance with the specified instance ID and reason.
A suspended instance can be resumed later using Resume-DurableOrchestration.

## EXAMPLES

### Example 1

```powershell
Suspend-DurableOrchestration -InstanceId "example-instance-id" -Reason "Maintenance window"
```

Suspends the orchestration instance with the provided reason.

## PARAMETERS

### -InstanceId

The ID of the orchestration instance to suspend.

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

The reason for suspending the orchestration instance.
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

You can pipe instance ID strings to suspend multiple orchestration instances.

### System.String (Reason)

You can pipe reason strings to specify the suspension reason for orchestration instances.

## OUTPUTS

### None

This cmdlet does not return any output. It suspends the specified orchestration instance.

## NOTES

- This cmdlet can only be used in client functions, not within orchestrator or activity functions.
- Suspended orchestrations can be resumed later using Resume-DurableOrchestration with the same instance ID.
- The suspension is asynchronous; the orchestration may take some time to actually suspend.
- Suspended orchestrations will have a status of "Suspended" when checked with Get-DurableStatus.
- Use descriptive suspension reasons to help with monitoring and maintenance.
- Suspension is useful for implementing manual approval workflows or maintenance windows.

## RELATED LINKS

[Durable Functions for PowerShell](https://github.com/Azure/azure-functions-durable-powershell)
