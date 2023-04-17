using namespace System.Net

param($Context)

Invoke-DurableActivityE -FunctionName 'DurableActivityWritesToQueue' -Input 'QueueData'