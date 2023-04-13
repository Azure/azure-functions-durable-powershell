using namespace System.Net

param($Request, $TriggerMetadata)

$InstanceId = Start-DurableOrchestrationExternal -FunctionName "DurableOrchestratorAccessContextProps" -InstanceId "myInstanceId"
Write-Host "Started orchestration with ID = '$InstanceId'"

$Response = New-DurableOrchestrationCheckStatusResponseExternal -Request $Request -InstanceId $InstanceId
Push-OutputBinding -Name Response -Value $Response
