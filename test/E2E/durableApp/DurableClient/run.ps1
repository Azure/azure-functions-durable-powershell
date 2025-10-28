using namespace System.Net

param($Request, $TriggerMetadata)
$ErrorActionPreference = 'Stop'

Write-Host "DurableClient started"

$FunctionName = $Request.Params.FunctionName

$Version = $Request.Query.Version
$SubOrchestratorVersion = $Request.Query.SubOrchestratorVersion

$InputObject = if ($SubOrchestratorVersion) {
    @{ SubOrchestratorVersion = $SubOrchestratorVersion }
} else {
    $null
}

if ($Version) {
    Write-Host "Starting orchestration '$FunctionName' with Version '$Version'"
    $InstanceId = Start-DurableOrchestration -FunctionName $FunctionName -Version $Version -InputObject $InputObject
} else {
    Write-Host "Starting orchestration '$FunctionName' with default version"
    $InstanceId = Start-DurableOrchestration -FunctionName $FunctionName -InputObject $InputObject
}

Write-Host "Started orchestration with ID = '$InstanceId'"

$Response = New-DurableOrchestrationCheckStatusResponse -Request $Request -InstanceId $InstanceId
Push-OutputBinding -Name Response -Value $Response

$Status = Get-DurableStatus -InstanceId $InstanceId
Write-Host "Orchestration $InstanceId status: $($Status | ConvertTo-Json)"
if ($Status.RuntimeStatus -notin 'Pending', 'Running', 'Failed', 'Completed') {
    throw "Unexpected orchestration $InstanceId runtime status: $($Status.RuntimeStatus)"
}

Write-Host "DurableClient completed"