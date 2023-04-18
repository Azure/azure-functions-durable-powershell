using namespace System.Net

param($Request, $TriggerMetadata)
$ErrorActionPreference = 'Stop'

Write-Host "DurableClient started"

$FunctionName = $Request.Params.FunctionName
$InstanceId = Start-DurableOrchestration -FunctionName $FunctionName
Write-Host "Started orchestration with ID = '$InstanceId'"

$Response = New-DurableOrchestrationCheckStatusResponse -Request $Request -InstanceId $InstanceId
Push-OutputBinding -Name Response -Value $Response

$Status = Get-DurableStatus -InstanceId $InstanceId
#Write-Host "Orchestration $InstanceId status: $($Status | ConvertTo-Json)"
if ($Status.RuntimeStatus -notin 'Pending', 'Running', 'Failed', 'Completed') {
    throw "Unexpected orchestration $InstanceId runtime status: $($Status.RuntimeStatus)"
}

Write-Host "DurableClient completed"