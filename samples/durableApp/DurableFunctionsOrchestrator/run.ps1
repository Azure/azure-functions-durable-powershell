param($Context)

$output = @()

# Write-Output($Context.IsReplaying)
#Invoke-DurableActivityExternal -FunctionName 'Hello' -Input 'Seattle'
#Invoke-DurableActivityExternal -FunctionName 'Hello' -Input 'Tokyo'
#Write-Output($Context.IsReplaying)

$t1 = @{a=1;b=2}
$t2 = @{t1=$t1;c=3}
$compareInput = @{
    msList = $t1
    dids = $t2
}

Invoke-DurableActivityExternal -FunctionName 'Hello' -Input $compareInput
#Write-Output($Context.IsReplaying)

"success"
