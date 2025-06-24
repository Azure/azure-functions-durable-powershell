---
external help file: AzureFunctions.PowerShell.Durable.SDK-help.xml
Module Name: AzureFunctions.PowerShell.Durable.SDK
online version:
schema: 2.0.0
---

# Resume-DurableOrchestration

## SYNOPSIS

Resume a suspended durable orchestration instance.

## SYNTAX

```
Resume-DurableOrchestration [-InstanceId] <String> [-Reason] <String> [<CommonParameters>]
```

## DESCRIPTION

Resumes a previously suspended durable orchestration instance with the specified instance ID and reason.
This will continue execution from where the orchestration was suspended.

## EXAMPLES

### Example 1

```powershell
Resume-DurableOrchestration -InstanceId "example-instance-id" -Reason "Maintenance complete"
```

Resumes the orchestration instance with the provided reason.

## PARAMETERS

### -InstanceId

The ID of the orchestration instance to resume.

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

The reason for resuming the orchestration instance.
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

You can pipe instance ID strings to resume multiple orchestration instances.

### System.String (Reason)

You can pipe reason strings to specify the resumption reason for orchestration instances.

## OUTPUTS

### None

This cmdlet does not return any output. It resumes the specified suspended orchestration instance.

## NOTES

- This cmdlet can only be used in client functions, not within orchestrator or activity functions.
- Only orchestrations in a "Suspended" state can be resumed.
- The resume operation is asynchronous; the orchestration will continue from where it was suspended.
- Use Get-DurableStatus to verify the orchestration is in a "Suspended" state before attempting to resume.
- The resumption reason is helpful for tracking why an orchestration was resumed.
- Resumed orchestrations will continue with their original input and context.

## RELATED LINKS

[Durable Functions for PowerShell](https://github.com/Azure/azure-functions-durable-powershell)
