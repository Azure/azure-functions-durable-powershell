using namespace System.Net

param($Context)

$ErrorActionPreference = 'Stop'

$myTask = Invoke-DurableActivityE -FunctionName "Hello" -Input "Tokyo" -NoWait

$tasks = @($myTask)
Wait-DurableTaskE -Task $tasks -Any
#$result = Get-DurableTaskResult -Task $myTask

#Write-Host "++++++++++++++++++"
#Write-Host $myTask

return 5
