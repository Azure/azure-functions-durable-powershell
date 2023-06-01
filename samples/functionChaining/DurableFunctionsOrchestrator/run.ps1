param($Context)

$output = @()

# Function chaining
$output += Invoke-DurableActivity -FunctionName 'Hello' -Input 'Tokyo'
$output += Invoke-DurableActivity -FunctionName 'Hello' -Input 'Seattle'
$output += Invoke-DurableActivity -FunctionName 'Hello' -Input 'London'

# final expression is the return statement
$output
