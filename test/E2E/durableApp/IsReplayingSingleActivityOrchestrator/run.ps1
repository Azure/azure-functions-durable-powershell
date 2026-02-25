param($Context)

# Capture IsReplaying before and after a single activity call.
# Before the activity completes, IsReplaying should be True on replay.
# After the activity completes (final replay), IsReplaying should be False.
$output = @()

$output += $Context.IsReplaying
$output += Invoke-DurableActivity -FunctionName 'Hello' -Input 'ReplayTest'
$output += $Context.IsReplaying

$output
