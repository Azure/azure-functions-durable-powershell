param($Request, $TriggerMetadata)
$ErrorActionPreference = 'Stop'

Write-Host "DurableClientSuspending started"

$FunctionName = $Request.Params.FunctionName
$InstanceId = Start-DurableOrchestration -FunctionName $FunctionName -InputObject 'Hello'
Write-Host "Started orchestration with ID = '$InstanceId'"

Suspend-DurableOrchestration -InstanceId $InstanceId -Reason 'Suspended intentionally'

$Response = New-DurableOrchestrationCheckStatusResponse -Request $Request -InstanceId $InstanceId
Push-OutputBinding -Name Response -Value $Response

Write-Host "DurableClientSuspending completed"