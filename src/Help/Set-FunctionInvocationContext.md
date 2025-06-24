---
external help file: AzureFunctions.PowerShell.Durable.SDK.dll-Help.xml
Module Name: AzureFunctions.PowerShell.Durable.SDK
online version:
schema: 2.0.0
---

# Set-FunctionInvocationContext

## SYNOPSIS

**INTERNAL USE ONLY** - Sets the function invocation context for the current PowerShell function execution.

## SYNTAX

### OrchestrationContext

```
Set-FunctionInvocationContext -OrchestrationContext <String> [<CommonParameters>]
```

### DurableClient

```
Set-FunctionInvocationContext -DurableClient <Object> [<CommonParameters>]
```

### Clear

```
Set-FunctionInvocationContext [-Clear] [<CommonParameters>]
```

## DESCRIPTION

**WARNING: This cmdlet is for internal use by the Durable Functions runtime only.**
**Do not call this cmdlet directly in your orchestrator or activity functions.**

This cmdlet is used internally by the Durable Functions runtime to establish the
execution context that enables durable orchestration capabilities. It provides the
necessary context for orchestrator functions to interact with the Durable Functions
framework, including task scheduling and state management.

Calling this cmdlet directly in user code may interfere with the runtime's operation
and could lead to unpredictable behavior or runtime errors.

## EXAMPLES

### Example 1

```powershell
# Internal use only - example for SDK development
Set-FunctionInvocationContext -OrchestrationContext $durableOrchestrationContext
```

This cmdlet is for internal SDK use only and should not be called directly in user orchestration functions.

## PARAMETERS

### -Clear

A switch parameter that, when specified, clears the current function invocation context. For internal SDK use only.

```yaml
Type: SwitchParameter
Parameter Sets: Clear
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -DurableClient

The durable client context object used for client operations. For internal SDK use only.

```yaml
Type: Object
Parameter Sets: DurableClient
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -OrchestrationContext

The orchestration context object containing the current orchestration state and capabilities. For internal SDK use only.

```yaml
Type: String
Parameter Sets: OrchestrationContext
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

This cmdlet does not return any output. It sets the function invocation context for use with durable function operations.

## NOTES

## RELATED LINKS

[Durable Functions for PowerShell](https://github.com/Azure/azure-functions-durable-powershell)
