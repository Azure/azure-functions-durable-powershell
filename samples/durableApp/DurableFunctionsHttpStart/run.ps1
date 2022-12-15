using namespace System.Net

param($Request, $TriggerMetadata)

Get-Module -ListAvailable | Write-Host
$FunctionName = $Request.Params.FunctionName
$InstanceId = Start-DurableOrchestrationExternal -FunctionName $FunctionName
Write-Host "Started orchestration with ID = '$InstanceId'"

$url = $Request.Url
$suffix = "/api/orchestrators/DurableFunctionsOrchestrator"
$baseUrl = $url.Substring(0, $url.Length - $suffix.Length)
$sendApproval = Invoke-RestMethod -Uri "$baseUrl/api/SendApproval?InstanceId=$InstanceId"
$Response = New-DurableOrchestrationCheckStatusResponseExternal -Request $Request -InstanceId $InstanceId
Push-OutputBinding -Name Response -Value $Response
