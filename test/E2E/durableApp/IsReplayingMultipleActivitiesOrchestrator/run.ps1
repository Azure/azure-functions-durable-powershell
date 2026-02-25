param($Context)

# Track IsReplaying across multiple sequential activity calls.
# On the final replay, all activities are replayed so IsReplaying is True
# before each previously-completed activity, and False only after the last
# scheduled task completes.
$output = @()

$output += $Context.IsReplaying                                          # [0]
$output += Invoke-DurableActivity -FunctionName 'Hello' -Input 'First'   # [1]
$output += $Context.IsReplaying                                          # [2]
$output += Invoke-DurableActivity -FunctionName 'Hello' -Input 'Second'  # [3]
$output += $Context.IsReplaying                                          # [4]
$output += Invoke-DurableActivity -FunctionName 'Hello' -Input 'Third'   # [5]
$output += $Context.IsReplaying                                          # [6]

$output
