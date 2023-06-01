using namespace System.Net

param($Request, $TriggerMetadata)

$FunctionName = $Request.Params.FunctionName
$InstanceId = Start-DurableOrchestration -FunctionName $FunctionName -Input "I wrote this in DurableFunctionsHttpStart"
Write-Host "Started orchestration with ID = '$InstanceId'"

$Response = New-DurableOrchestrationCheckStatusResponse -Request $Request -InstanceId $InstanceId
Push-OutputBinding -Name Response -Value $Response
