using namespace System.Net

param($Request, $TriggerMetadata)
$ErrorActionPreference = 'Stop'

$FunctionName = $Request.Params.FunctionName
$InstanceId = Start-DurableOrchestrationExternal -FunctionName $FunctionName
Write-Host "Started orchestration with ID = '$InstanceId'"

$Response = New-DurableOrchestrationCheckStatusResponseExternal -Request $Request -InstanceId $InstanceId
Push-OutputBinding -Name Response -Value $Response

$Status = Get-DurableStatusExternal -InstanceId $InstanceId
Write-Host "Orchestration $InstanceId status: $($Status | ConvertTo-Json)"
if ($Status.RuntimeStatus -notin 'Pending', 'Running', 'Failed', 'Completed') {
    throw "Unexpected orchestration $InstanceId runtime status: $($Status.RuntimeStatus)"
}

Write-Host "DurableAppHttpStartCompleted"