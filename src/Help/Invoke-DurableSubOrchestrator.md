---
external help file: AzureFunctions.PowerShell.Durable.SDK.dll-Help.xml
Module Name: AzureFunctions.PowerShell.Durable.SDK
online version:
schema: 2.0.0
---

# Invoke-DurableSubOrchestrator

## SYNOPSIS

Invokes a sub-orchestrator function.

## SYNTAX

```
Invoke-DurableSubOrchestrator -FunctionName <String> [-InstanceId <String>] [-Input <Object>] [-RetryOptions <RetryPolicy>] [-Version <String>] [-NoWait] [<CommonParameters>]
```

## DESCRIPTION

Invokes a sub-orchestrator function. By default, this cmdlet blocks until the
sub-orchestrator completes and returns the result directly. Use the -NoWait switch
to return a task object immediately without waiting for completion, allowing you to
orchestrate multiple sub-orchestrators concurrently. Sub-orchestrators enable
composition of orchestrations and help manage complexity in large workflow scenarios.

## EXAMPLES

### Example 1 - Synchronous execution (default behavior)

```powershell
$batchResult = Invoke-DurableSubOrchestrator -FunctionName "ChildOrchestrator" -Input @{ BatchId = "batch123"; Items = @("item1", "item2", "item3") }
Write-Host "Sub-orchestrator completed with result: $batchResult"
```

This example shows the default behavior where the cmdlet blocks until completion and returns the result directly.

### Example 2 - Asynchronous execution with -NoWait

```powershell
$subOrchestratorTask = Invoke-DurableSubOrchestrator -FunctionName "ChildOrchestrator" -Input @{ BatchId = "batch123"; Items = @("item1", "item2", "item3") } -NoWait
$batchResult = Get-DurableTaskResult -Task $subOrchestratorTask
Write-Host "Sub-orchestrator completed with result: $batchResult"
```

This example shows how to invoke a sub-orchestrator function asynchronously using -NoWait, which returns a task object that can be awaited later.

### Example 3 - Specifying a version

```powershell
$result = Invoke-DurableSubOrchestrator -FunctionName "ChildOrchestrator" -Version "2.0" -Input @{ ProcessId = "proc456" }
Write-Host "Sub-orchestrator (version 2.0) completed with result: $result"
```

This example shows how to invoke a sub-orchestrator with a specific version, overriding the default version configured in host.json.

## PARAMETERS

### -FunctionName

The name of the orchestrator function to invoke as a sub-orchestrator. This should match the name of an orchestrator function defined in your Azure Functions app.

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

### -Input

The input data to pass to the sub-orchestrator function. This can be any object that will be serialized and passed as input to the sub-orchestrator.

```yaml
Type: Object
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -InstanceId

An optional instance ID for the sub-orchestrator. If not specified, a unique instance ID will be automatically generated for the sub-orchestrator.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Version

An optional version string for the sub-orchestrator function. When specified, this version overrides the default version configured in host.json for this specific sub-orchestrator invocation. This allows you to invoke specific versions of orchestrator functions.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -NoWait

When specified, the cmdlet returns a task object immediately without waiting for completion. By default, the cmdlet blocks and waits for the sub-orchestrator to complete before returning the result.

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

### -RetryOptions

A retry policy object created with New-DurableRetryPolicy that defines how the sub-orchestrator should be retried if it fails. If not specified, the sub-orchestrator will not be retried on failure.

```yaml
Type: RetryPolicy
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

### System.Object

Returns the result of the sub-orchestrator execution by default. If -NoWait is specified, returns a DurableTask object that can be used with Get-DurableTaskResult or Wait-DurableTask to retrieve the result later.

## NOTES

- This cmdlet can only be used within orchestrator functions, not in activity functions or client functions.
- Sub-orchestrators enable composition and modularity in complex workflow scenarios.
- Each sub-orchestrator runs as an independent orchestration instance with its own instance ID.
- Use the -NoWait parameter when you need to invoke multiple sub-orchestrators concurrently.
- Sub-orchestrators inherit the fault-tolerance and replay characteristics of the parent orchestration.
- The sub-orchestrator function name must match a function defined in your Azure Functions app with an orchestration trigger.
- The -Version parameter allows you to invoke specific versions of orchestrator functions, overriding the default version from host.json.
- Consider using sub-orchestrators to break down complex workflows into manageable, reusable components.

## RELATED LINKS

[Durable Functions for PowerShell](https://github.com/Azure/azure-functions-durable-powershell)
