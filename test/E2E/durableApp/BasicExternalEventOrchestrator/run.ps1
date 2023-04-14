param($Context)

$output = @()

Invoke-DurableActivityE -FunctionName "Hello" -Input "Tokyo"
Invoke-DurableActivityE -FunctionName "Hello" -Input "Seattle"
$output += Start-DurableExternalEventListenerE -EventName "TESTEVENTNAME" 
Invoke-DurableActivityE -FunctionName "Hello" -Input "London"

$output
