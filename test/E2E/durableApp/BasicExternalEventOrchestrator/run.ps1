param($Context)

$output = @()

Invoke-DurableActivityE -FunctionName "DurableActivity" -Input "Tokyo"
Invoke-DurableActivityE -FunctionName "DurableActivity" -Input "Seattle"
$output += Start-DurableExternalEventListenerE -EventName "TESTEVENTNAME" 
Invoke-DurableActivityE -FunctionName "DurableActivity" -Input "London"

$output
