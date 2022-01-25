param($Context)

$output = @()

Test-SampleCmdlet -FavoriteNumber 7 -FavoritePet Cat
$output += Invoke-DurableActivity -FunctionName 'Hello' -Input 'Tokyo'
# $output += Invoke-DurableActivity -FunctionName 'Hello' -Input 'Seattle'
# $output += Invoke-DurableActivity -FunctionName 'Hello' -Input 'London'

$output
