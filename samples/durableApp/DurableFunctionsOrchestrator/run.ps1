param($Context)

Import-Module .\Modules\testModule\bin\Debug\net6.0\testModule.dll
$output = @()

# Test-SampleCmdlet -FavoriteNumber 7 -FavoritePet Cat
$output += Invoke-DurableActivity2 -FunctionName 'Hello' -Input 'Tokyo'
# $output += Invoke-DurableActivity -FunctionName 'Hello' -Input 'Seattle'
# $output += Invoke-DurableActivity -FunctionName 'Hello' -Input 'London'

$output
