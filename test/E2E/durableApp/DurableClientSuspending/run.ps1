param($Request, $TriggerMetadata)
$ErrorActionPreference = 'Stop'

Write-Host "DurableClientSuspending started"

$OrchestratorInputs = @{ FirstDuration = 5; SecondDuration = 60 }

$FunctionName = $Request.Params.FunctionName
$InstanceId = Start-DurableOrchestration -FunctionName $FunctionName -InputObject $OrchestratorInputs
Write-Host "Started orchestration with ID = '$InstanceId'"

Start-Sleep -Seconds 5
Suspend-DurableOrchestration -InstanceId $InstanceId -Reason 'Suspend orchestrator'

$SuspendResponse = New-DurableOrchestrationCheckStatusResponse -Request $Request -InstanceId $InstanceId
Push-OutputBinding -Name Response -Value $SuspendResponse

Start-Sleep -Seconds 10
Resume-DurableOrchestration -InstanceId $InstanceId -Reason 'Resume orchestrator'

Send-DurableExternalEvent -InstanceId $InstanceId -EventName "SecondExternalEvent"

Write-Host "DurableClientSuspending completed"
