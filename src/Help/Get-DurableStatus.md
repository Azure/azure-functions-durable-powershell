---
external help file: AzureFunctions.PowerShell.Durable.SDK-help.xml
Module Name: AzureFunctions.PowerShell.Durable.SDK
online version:
schema: 2.0.0
---

# Get-DurableStatus

## SYNOPSIS

Get the status of a durable orchestration instance.

## SYNTAX

```
Get-DurableStatus [-InstanceId] <String> [-DurableClient <Object>] [-ShowHistory] [-ShowHistoryOutput] [-ShowInput] [<CommonParameters>]
```

## DESCRIPTION

Get the status of a durable orchestration instance with the given instance ID.
Optionally includes execution history, history output, and input data.

## EXAMPLES

### Example 1

```powershell
Get-DurableStatus -InstanceId "example-instance-id"
```

Returns the basic status of the orchestration instance.

### Example 2

```powershell
Get-DurableStatus -InstanceId "example-instance-id" -ShowHistory -ShowHistoryOutput
```

Returns the status with detailed execution history and output.

## PARAMETERS

### -InstanceId

The ID of the orchestration instance to get the status for.

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

### -DurableClient

The durable client object.
If not provided, it will be retrieved from module private data.

```yaml
Type: Object
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -ShowHistory

When present, includes the execution history in the response.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -ShowHistoryOutput

When present, includes the output of each step in the execution history.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -ShowInput

When present, includes the input data that was provided to the orchestration instance.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String

You can pipe instance ID strings to this cmdlet to get the status of multiple orchestration instances.

## OUTPUTS

### System.Object

Returns a status object containing information about the durable orchestration instance, including:

- InstanceId: The unique identifier of the orchestration instance
- RuntimeStatus: The current runtime status (Running, Completed, Failed, etc.)
- Input: The input data provided to the orchestration (if -ShowInput is specified)
- Output: The output of the orchestration (if completed)
- CreatedTime: When the orchestration was created
- LastUpdatedTime: When the orchestration was last updated
- History: Execution history (if -ShowHistory is specified)

## NOTES

- This cmdlet is typically used in HTTP trigger functions or other client functions to check orchestration progress.
- The InstanceId must be from an existing orchestration; invalid IDs will result in a null response.
- Use -ShowHistory to get detailed execution steps, which is useful for debugging orchestration behavior.
- The -ShowHistoryOutput parameter can produce large responses; use carefully in production environments.
- Status information includes runtime state, input/output data, creation time, and last update time.
- Orchestration status is eventually consistent and may take a moment to reflect the latest state after operations.

## RELATED LINKS

[Durable Functions for PowerShell](https://github.com/Azure/azure-functions-durable-powershell)
