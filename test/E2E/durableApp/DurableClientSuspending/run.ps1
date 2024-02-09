param($Request, $TriggerMetadata)
$ErrorActionPreference = 'Stop'

Write-Host "DurableClientSuspending started"

$FunctionName = $Request.Params.FunctionName
$InstanceId = Start-DurableOrchestration -FunctionName $FunctionName -InputObject 'Hello'
Write-Host "Started orchestration with ID = '$InstanceId'"

Suspend-DurableOrchestration -InstanceId $InstanceId -Reason 'Suspend orchestrator'

$SuspendResponse = New-DurableOrchestrationCheckStatusResponse -Request $Request -InstanceId $InstanceId
Push-OutputBinding -Name Response -Value $SuspendResponse

Start-Sleep -Seconds 10

Resume-DurableOrchestration -InstanceId $InstanceId -Reason 'Resume orchestrator'

$ResumeResponse = New-DurableOrchestrationCheckStatusResponse -Request $Request -InstanceId $InstanceId
Push-OutputBinding -Name Response -Value $ResumeResponse

Write-Host "DurableClientSuspending completed"