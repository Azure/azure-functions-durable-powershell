param($Context)

# Track IsReplaying before and after a sub-orchestrator invocation.
$output = @()

$output += $Context.IsReplaying                                                        # [0]
$null = Invoke-DurableSubOrchestrator -FunctionName 'SimpleOrchestrator'
$output += $Context.IsReplaying                                                        # [1]
$output += Invoke-DurableActivity -FunctionName 'Hello' -Input 'AfterSubOrchestrator'  # [2]
$output += $Context.IsReplaying                                                        # [3]

$output
