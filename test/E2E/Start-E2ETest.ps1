#	
# Copyright (c) Microsoft. All rights reserved.	
# Licensed under the MIT license. See LICENSE file in the project root for full license information.	
#
param
(
    [Switch]
    $NoBuild,
    [Switch]
    $SkipCoreToolsDownload
)

$ErrorActionPreference = 'STOP'

function NewTaskHubName
{
    param(
        [int]$Length = 45
    )

    <#
    Task hubs are identified by a name that conforms to these rules:
      - Contains only alphanumeric characters
      - Starts with a letter
      - Has a minimum length of 3 characters, maximum length of 45 characters
    doc: According to the documentation here https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-task-hubs?tabs=csharp
    #>

    $min = 20
    $max = 45

    if ($length -lt $min -or $Length -gt $max)
    {
        throw "Length must be between $min and $max characters. Provided value: $length"
    }

    $letters = 'a'..'z' + 'A'..'Z'
    $numbers = 0..9
    $alphanumeric = $letters + $numbers

    # First value is a letter
    $sb = [System.Text.StringBuilder]::new()
    $value = $letters | Get-Random
    $sb.Append($value) | Out-Null

    # Add the date and time as part of the name. This way, we can delete older versions.
    # Example: 202104251929 is for 2021-04-25:1929 (this value is 12 characters long)
    $value = Get-Date -Format "yyyyMMddHHmm"
    $sb.Append($value) | Out-Null

    # The remaining of the characters are random alphanumeric values
    for ($index = 13; $index -lt $length; $index++)
    {
        $value = $alphanumeric | Get-Random
        $sb.Append($value) | Out-Null
    }

    $sb.ToString()
}

$FUNC_RUNTIME_VERSION = '4'
$POWERSHELL_VERSION = '7.4'
$FUNC_CMDLET_NAME = "func"
$CORE_TOOLS_VERSION = '4.0.7317'

# Set the appropriate environment variables
$taskHubName = NewTaskHubName -Length 45

# In non-ADO environments, host.json values are not populated with environment variables. Instead,
# We override the host.json taskHubName with an app setting/environment variable: see
# https://learn.microsoft.com/en-us/azure/azure-functions/functions-app-settings for reference.
$Env:AzureFunctionsJobHost__extensions__durableTask__hubName = $taskHubName

$Env:FUNCTIONS_WORKER_RUNTIME = "powershell"
$Env:FUNCTIONS_WORKER_RUNTIME_VERSION = $POWERSHELL_VERSION
$Env:AZURE_FUNCTIONS_ENVIRONMENT = "Development"

# TODO: Remove this once the external SDK is no longer behind a feature flag
# Enable the feature flag for the external durable SDK
$env:ExternalDurablePowerShellSDK = $true
Write-Host "Set ExternalDurablePowerShellSDK environment variable to $env:ExternalDurablePowerShellSDK"

if (-not $env:AzureWebJobsStorage) {
    $env:AzureWebJobsStorage = "UseDevelopmentStorage=true"
    Write-Host "Set AzureWebJobsStorage environment variable to $env:AzureWebJobsStorage"
}

$arch = [System.Runtime.InteropServices.RuntimeInformation]::OSArchitecture.ToString().ToLowerInvariant()
if ($IsWindows) {
    $FUNC_EXE_NAME = "$FUNC_CMDLET_NAME.exe"
    $os = "win"
} else {
    $FUNC_EXE_NAME = $FUNC_CMDLET_NAME
    if ($IsMacOS) {
        $os = "osx"
    } else {
        $os = "linux"
    }
}

$FUNC_CLI_DIRECTORY = Join-Path $PSScriptRoot 'Azure.Functions.Cli'

Write-Host "Deleting $FUNC_CLI_DIRECTORY if it exists..."
Remove-Item -Force "$FUNC_CLI_DIRECTORY.zip" -ErrorAction Ignore
Remove-Item -Recurse -Force $FUNC_CLI_DIRECTORY -ErrorAction Ignore

if (-not $SkipCoreToolsDownload)
{
    Write-Host "Downloading Core Tools because SkipCoreToolsDownload switch parameter is not present..."

    $coreToolsDownloadURL = "https://github.com/Azure/azure-functions-core-tools/releases/download/$CORE_TOOLS_VERSION/Azure.Functions.Cli.$os-$arch.$CORE_TOOLS_VERSION.zip"
    Write-Host "Downloading Functions Core Tools (Version: $CORE_TOOLS_VERSION)..."

    $output = "$FUNC_CLI_DIRECTORY.zip"
    Invoke-RestMethod -Uri $coreToolsDownloadURL -OutFile $output

    Write-Host 'Extracting Functions Core Tools...'
    Expand-Archive $output -DestinationPath $FUNC_CLI_DIRECTORY

    # Prepend installed Core Tools to the PATH
    $Env:Path = "$FUNC_CLI_DIRECTORY$([System.IO.Path]::PathSeparator)$Env:Path"
    $funcPath = Join-Path $FUNC_CLI_DIRECTORY $FUNC_EXE_NAME
} else {
    $funcPath = (Get-Command $FUNC_CMDLET_NAME).Source
    $funcDir = [System.IO.Path]::GetDirectoryName($funcPath)
    if ($funcDir.Contains("nodejs"))
    {
        $funcPath = Join-Path $funcDir "node_modules" "azure-functions-core-tools" "bin" "$FUNC_CMDLET_NAME.exe"
    }
    if (-not(Test-Path $funcPath))
    {
        throw "Invalid path for the Core Tools executable: $funcPath"
    }
    $version = & $funcPath --version
    Write-Host "Using local Functions Core Tools (Version: $version) from $funcPath..."
}

# Set an environment variable containing the path to the func executable. This will be accessed from
# in FixtureHelper.cs for E2E testing
$env:FUNC_PATH = $funcPath
Write-Host "Set FUNC_PATH environment variable to $env:FUNC_PATH"

if (-not $NoBuild) {
    # For both integration build test runs and regular test runs, we copy binaries to durableApp/Modules
    Write-Host "Building the DurableSDK module and copying binaries to the durableApp/Modules directory..."
    
    Push-Location "$PSScriptRoot/../.."
    try {
        & ./build.ps1 -Configuration 'Debug'
    }
    finally {
        Pop-Location
    }
}

Write-Host "Starting Core Tools..."
Push-Location "$PSScriptRoot\durableApp"
try {
    if (($IsMacOS -or $IsLinux) -and (-not $SkipCoreToolsDownload)) {
        chmod +x $funcPath
    }

    Write-Host "Initializing function app project..."
    & $funcPath init . --powershell

    if ($LASTEXITCODE -ne 0) { throw "Installing extensions failed." }

    Write-Host "Installing extensions..."
    & $funcPath extensions install | ForEach-Object {    
    if ($_ -match 'OK')    
    { Write-Host $_ -f Green }    
    elseif ($_ -match 'FAIL|ERROR')   
    { Write-Host $_ -f Red }
    else    
    { Write-Host $_ }    
    }

    if ($LASTEXITCODE -ne 0) { throw "Installing extensions failed." }
}
finally {
    Pop-Location
}

Write-Host "Running E2E integration tests..." -ForegroundColor Green
Write-Host "-----------------------------------------------------------------------------`n" -ForegroundColor Green

dotnet test "$PSScriptRoot/AzureFunctions.PowerShell.Durable.SDK.E2E/AzureFunctions.PowerShell.Durable.SDK.E2E.csproj" --logger:trx --results-directory "$PSScriptRoot/../../TestResults"
if ($LASTEXITCODE -ne 0) { throw "xunit tests failed." }

Write-Host "-----------------------------------------------------------------------------" -ForegroundColor Green