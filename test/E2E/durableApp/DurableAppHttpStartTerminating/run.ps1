param($Request, $TriggerMetadata)
$ErrorActionPreference = 'Stop'

$FunctionName = $Request.Params.FunctionName ?? 'DurablePatternsOrchestrator'
$InstanceId = Start-DurableOrchestrationExternal -FunctionName $FunctionName -InputObject 'Hello'
Write-Host "Started orchestration with ID = '$InstanceId'"

Stop-DurableOrchestrationExternal -InstanceId $InstanceId -Reason 'Terminated intentionally'

$Response = New-DurableOrchestrationCheckStatusResponseExternal -Request $Request -InstanceId $InstanceId
Push-OutputBinding -Name Response -Value $Response