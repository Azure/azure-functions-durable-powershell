param($Context)

$output = @()

$output += Start-DurableExternalEventListenerE -EventName "TESTEVENTNAME" 

$output
