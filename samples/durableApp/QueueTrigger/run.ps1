# Input bindings are passed in via param block.
param([string] $InstanceId, $TriggerMetadata)

# Write out the queue message and insertion time to the information log.
Write-Host "PowerShell queue trigger function processed work item: $QueueItem"
Send-DurableExternalEventE -InstanceId $InstanceId -EventName "ApprovalEvent1" -EventData "true"
Send-DurableExternalEventE -InstanceId $InstanceId -EventName "ApprovalEvent2" -EventData "true"
