param($Request, $TriggerMetadata)
$ErrorActionPreference = 'Stop'

Write-Host "DurableClientSendExternalEvent started"

$OrchestratorInputs = @{ FirstDuration = 5; SecondDuration = 30 }

$InstanceId = Start-DurableOrchestration -FunctionName "SendDurableExternalEventOrchestrator" -InputObject $OrchestratorInputs
Write-Host "Started orchestration with ID = '$InstanceId'"

$Response = New-DurableOrchestrationCheckStatusResponse -Request $Request -InstanceId $InstanceId
Push-OutputBinding -Name Response -Value $Response

Start-Sleep -Seconds 10
Send-DurableExternalEvent -InstanceId $InstanceId -EventName "SecondExternalEvent"

Write-Host "DurableClientSendExternalEvent completed"