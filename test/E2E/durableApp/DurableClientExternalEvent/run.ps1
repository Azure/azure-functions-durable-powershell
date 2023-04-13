param($Request, $TriggerMetadata)
$ErrorActionPreference = 'Stop'

Write-Host "ExternalEventClient started"

$OrchestratorInputs = @{ FirstDuration = 5; SecondDuration = 60 }

$InstanceId = Start-DurableOrchestrationExternal -FunctionName "ComplexExternalEventOrchestrator" -InputObject $OrchestratorInputs
Write-Host "Started orchestration with ID = '$InstanceId'"

$Response = New-DurableOrchestrationCheckStatusResponseExternal -Request $Request -InstanceId $InstanceId
Push-OutputBinding -Name Response -Value $Response

Start-Sleep -Seconds 15
Send-DurableExternalEventE -InstanceId $InstanceId -EventName "SecondExternalEvent"

Write-Host "DurableClientExternalEvent completed"