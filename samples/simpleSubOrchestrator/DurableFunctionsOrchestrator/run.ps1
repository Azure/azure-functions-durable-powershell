param($Context)

$output = @()

# inputs to process
$inputs = @('Tokyo', 'Seattle', 'London', 'Lima', 'Cairo', 'New York', 'Sydney', 'Berlin', 'Paris')

# batch inputs into sub-lists of 3 items
$subOrchestrators = @()
for ($i = 0; $i -lt $inputs.Length; $i += 3) {
    # create a sub-orchestration task for each batch
    # each sub-orchestration will then fan-out over its own batch of inputs
    $batchedInput = $inputs[$i..($i+2)]
    $subOrchestrators += Invoke-DurableSubOrchestrator -FunctionName 'SubOrchestrator' -Input $batchedInput -NoWait
}

# fan-out over all sub-orchestrator tasks and aggregate results
$output = Wait-DurableTask -Task $subOrchestrators

# final expression is the return statement
$output
