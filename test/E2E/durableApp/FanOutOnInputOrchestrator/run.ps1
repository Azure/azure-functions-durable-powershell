using namespace System.Net

param($Context)

$ErrorActionPreference = 'Stop'

$output = @()

# We call an activity for every element in the input list.
# This allows us to test that array-type inputs are correctly serialized.
# In the past, we've seen issues where arrays are received as JTokens instead of as PS lists.
# Since we cannot iterate over JTokens, an incorrectly serialized array will result in no activities being called. Our test will then error.
foreach ($input in $Context.Input)
{
    # invoke the activity function
    $output += Invoke-DurableActivity -FunctionName "Hello" -Input $input
}

return $output
