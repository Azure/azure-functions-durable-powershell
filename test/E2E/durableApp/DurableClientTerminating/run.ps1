param($Request, $TriggerMetadata)
$ErrorActionPreference = 'Stop'

Write-Host "DurableClientTerminating started"

$FunctionName = $Request.Params.FunctionName
$InstanceId = Start-DurableOrchestrationExternal -FunctionName $FunctionName -InputObject 'Hello'
Write-Host "Started orchestration with ID = '$InstanceId'"

Stop-DurableOrchestrationE -InstanceId $InstanceId -Reason 'Terminated intentionally'

$Response = New-DurableOrchestrationCheckStatusResponseExternal -Request $Request -InstanceId $InstanceId
Push-OutputBinding -Name Response -Value $Response

Write-Host "DurableClientTerminating completed"