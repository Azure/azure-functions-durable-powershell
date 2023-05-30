param($Request, $TriggerMetadata)
$ErrorActionPreference = 'Stop'

Write-Host "DurableClientTerminating started"

$FunctionName = $Request.Params.FunctionName
$InstanceId = Start-DurableOrchestration -FunctionName $FunctionName -InputObject 'Hello'
Write-Host "Started orchestration with ID = '$InstanceId'"

Stop-DurableOrchestration -InstanceId $InstanceId -Reason 'Terminated intentionally'

$Response = New-DurableOrchestrationCheckStatusResponse -Request $Request -InstanceId $InstanceId
Push-OutputBinding -Name Response -Value $Response

Write-Host "DurableClientTerminating completed"