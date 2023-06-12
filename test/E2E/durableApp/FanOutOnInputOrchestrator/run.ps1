using namespace System.Net

param($Context)

$ErrorActionPreference = 'Stop'

$output = @()
# call activity in every input
foreach ($input in $Context.Input)
{
    # invoke the activity function
    $output += Invoke-DurableActivity -FunctionName "Hello" -Input $input
}

return $output
