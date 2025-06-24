---
Module Name: AzureFunctions.PowerShell.Durable.SDK
Module Guid: 841fad61-94f5-4330-89be-613d54165289
Download Help Link: https://github.com/Azure/azure-functions-durable-powershell
Help Version: 1.0.0.0
Locale: en-US
---

# AzureFunctions.PowerShell.Durable.SDK Module

## Description

The AzureFunctions.PowerShell.Durable.SDK module provides cmdlets for building durable, stateful functions in Azure Functions using PowerShell. This SDK enables you to create orchestrator functions, activity functions, and manage durable orchestration workflows with features like timers, external events, and sub-orchestrations.

## AzureFunctions.PowerShell.Durable.SDK Cmdlets

### [Get-DurableStatus](Get-DurableStatus.md)

Gets the status of a durable orchestration instance, including execution history and input data.

### [Get-DurableTaskResult](Get-DurableTaskResult.md)

Gets the result of a completed durable task, such as an activity function or sub-orchestrator.

### [Invoke-DurableActivity](Invoke-DurableActivity.md)

Invokes an activity function from within an orchestrator function.

### [Invoke-DurableSubOrchestrator](Invoke-DurableSubOrchestrator.md)

Invokes a sub-orchestrator function from within a parent orchestrator function.

### [New-DurableOrchestrationCheckStatusResponse](New-DurableOrchestrationCheckStatusResponse.md)

Creates an HTTP response for orchestration status check endpoints with status polling URLs.

### [New-DurableRetryPolicy](New-DurableRetryPolicy.md)

Creates a retry policy for durable activity functions and sub-orchestrators.

### [Resume-DurableOrchestration](Resume-DurableOrchestration.md)

Resumes a suspended durable orchestration instance.

### [Send-DurableExternalEvent](Send-DurableExternalEvent.md)

Sends an external event to a running durable orchestration instance.

### [Set-DurableCustomStatus](Set-DurableCustomStatus.md)

Sets custom status information for a durable orchestration instance.

### [Set-FunctionInvocationContext](Set-FunctionInvocationContext.md)

Sets the function invocation context for durable function operations.

### [Start-DurableExternalEventListener](Start-DurableExternalEventListener.md)

Starts listening for an external event within an orchestrator function.

### [Start-DurableOrchestration](Start-DurableOrchestration.md)

Starts a new durable orchestration instance with the specified function name and input.

### [Start-DurableTimer](Start-DurableTimer.md)

Starts a durable timer that fires after a specified delay or at a specific time.

### [Stop-DurableOrchestration](Stop-DurableOrchestration.md)

Terminates a running durable orchestration instance.

### [Stop-DurableTimerTask](Stop-DurableTimerTask.md)

Stops a running durable timer task.

### [Suspend-DurableOrchestration](Suspend-DurableOrchestration.md)

Suspends a running durable orchestration instance.

### [Wait-DurableTask](Wait-DurableTask.md)

Waits for the completion of one or more durable tasks within an orchestrator function.
