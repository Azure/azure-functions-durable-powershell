---
external help file: AzureFunctions.PowerShell.Durable.SDK-help.xml
Module Name: AzureFunctions.PowerShell.Durable.SDK
online version:
schema: 2.0.0
---

# Start-DurableOrchestration

## SYNOPSIS

Start a durable orchestration.

## SYNTAX

```
Start-DurableOrchestration [-FunctionName] <String> [[-InputObject] <Object>] [-DurableClient <Object>] [-InstanceId <String>] [-Version <String>] [<CommonParameters>]
```

## DESCRIPTION

Start a durable orchestration with the given function name and input value.
Returns the instance ID of the newly started orchestration.

## EXAMPLES

### Example 1

```
Start-DurableOrchestration -FunctionName "OrchestratorFunction" -InputObject "input value for the orchestration function"
Returns the instance ID of the new orchestration.
```

## PARAMETERS

### -FunctionName

The name of the orchestration function you want to start.

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

### -InputObject

The input value that will be passed to the orchestration function.

```yaml
Type: Object
Parameter Sets: (All)
Aliases:

Required: False
Position: 2
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

### -InstanceId

Optional custom instance ID for the orchestration.
If not provided, a new GUID will be generated.

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

### -Version

Optional orchestration version.
The provided value will be available as `$Context.Version` within the orchestrator function context.
If not specified, the default version specified by the `defaultVersion` property in the Function app's host.json will be used.

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

### System.String (FunctionName)

You can pipe strings to the -FunctionName parameter to specify the orchestrator function to start.

### System.Object (InputObject)

You can pipe objects to the -InputObject parameter to provide input data for the orchestration function.

### System.String (InstanceId)

You can pipe strings to the -InstanceId parameter to specify a custom instance ID for the orchestration.

### System.String (Version)

You can pipe strings to the -Version parameter to specify a version for the orchestration function.

## OUTPUTS

### System.String

Returns the instance ID of the started orchestration as a string. This ID can be used to check the status, send external events, or manage the orchestration instance.

## NOTES

- This cmdlet is typically used in HTTP trigger functions or other client functions to start new orchestrations.
- The returned instance ID can be used with other cmdlets like Get-DurableStatus, Send-DurableExternalEvent, or Stop-DurableOrchestration.
- If you don't specify an InstanceId, a new GUID will be automatically generated.
- Custom instance IDs should be unique to avoid conflicts with existing orchestrations.
- Large input data should be stored in external storage and referenced by URL to avoid serialization limits.
- The orchestration function name must match a function defined in your Azure Functions app with an orchestration trigger.

## RELATED LINKS

[Durable Functions for PowerShell](https://github.com/Azure/azure-functions-durable-powershell)
