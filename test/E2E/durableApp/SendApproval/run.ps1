using namespace System.Net

# Input bindings are passed in via param block.
param($Request, $TriggerMetadata)

# Write to the Azure Functions log stream.
Write-Host "PowerShell HTTP trigger function processed a request."

# Interact with query parameters or the body of the request.
$instanceId = $Request.Query.InstanceId
if (-not $instanceId) {
    throw "Instance ID not passed successfully to the SendApproval function!"
}

Send-DurableExternalEventE -InstanceId $instanceId -EventName "ApprovalEvent1" -EventData "true"
Send-DurableExternalEventE -InstanceId $instanceId -EventName "ApprovalEvent2" -EventData "true"

$body = "This HTTP triggered function executed successfully."

# Associate values to output bindings by calling 'Push-OutputBinding'.
Push-OutputBinding -Name Response -Value ([HttpResponseContext]@{
    StatusCode = [HttpStatusCode]::OK
    Body = $body
})
