#	
# Copyright (c) Microsoft. All rights reserved.	
# Licensed under the MIT license. See LICENSE file in the project root for full license information.	
#
param
(
    [Switch]
    $UseCoreToolsBuildFromIntegrationTests,
    [Switch]
    $SkipCoreToolsDownload
)

$ErrorActionPreference = 'Stop'

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

$taskHubName = NewTaskHubName -Length 45

$FUNC_RUNTIME_VERSION = '4'
$POWERSHELL_VERSION = '7.2'
$FUNC_CMDLET_NAME = "func"

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

Write-Host 'Deleting Functions Core Tools if exists...'
Remove-Item -Force "$FUNC_CLI_DIRECTORY.zip" -ErrorAction Ignore
Remove-Item -Recurse -Force $FUNC_CLI_DIRECTORY -ErrorAction Ignore

if (-not $SkipCoreToolsDownload.IsPresent)
{
    Write-Host "Downloading Core Tools because SkipCoreToolsDownload switch parameter is not present..."
    $coreToolsDownloadURL = $null
    if ($UseCoreToolsBuildFromIntegrationTests.IsPresent)
    {
        $coreToolsDownloadURL = "https://functionsintegclibuilds.blob.core.windows.net/builds/$FUNC_RUNTIME_VERSION/latest/Azure.Functions.Cli.$os-$arch.zip"
        $env:CORE_TOOLS_URL = "https://functionsintegclibuilds.blob.core.windows.net/builds/$FUNC_RUNTIME_VERSION/latest"
    }
    else
    {
        $coreToolsDownloadURL = "https://functionsclibuilds.blob.core.windows.net/builds/$FUNC_RUNTIME_VERSION/latest/Azure.Functions.Cli.$os-$arch.zip"
        if (-not $env:CORE_TOOLS_URL)
        {
            $env:CORE_TOOLS_URL = "https://functionsclibuilds.blob.core.windows.net/builds/$FUNC_RUNTIME_VERSION/latest"
        }
    }

    $version = Invoke-RestMethod -Uri "$env:CORE_TOOLS_URL/version.txt"
    Write-Host "Downloading Functions Core Tools (Version: $version)..."

    $output = "$FUNC_CLI_DIRECTORY.zip"
    Invoke-RestMethod -Uri $coreToolsDownloadURL -OutFile $output

    Write-Host 'Extracting Functions Core Tools...'
    Expand-Archive $output -DestinationPath $FUNC_CLI_DIRECTORY

    # Prepend installed Core Tools to the PATH
    $Env:Path = "$FUNC_CLI_DIRECTORY$([System.IO.Path]::PathSeparator)$Env:Path"
    $funcPath = Join-Path $FUNC_CLI_DIRECTORY $FUNC_EXE_NAME

    # TODO: Remove after the worker enables the external SDK
    # Deploy a version of the PowerShell worker that enables the external SDK
    $powerShellWorkerDirectory = "$PSScriptRoot/azure-functions-powershell-worker"
    Write-Host 'Deleting the PowerShell worker if exists...'
    Remove-Item -Force "$powerShellWorkerDirectory.zip" -ErrorAction Ignore
    $enableExternalSDKBranchName = "dajusto/enable-external-df-sdk"
    git clone "https://github.com/Azure/azure-functions-powershell-worker.git" $powerShellWorkerDirectory
    Push-Location $powerShellWorkerDirectory
    git checkout $enableExternalSDKBranchName
    & $powerShellWorkerDirectory/build.ps1 -Bootstrap -Deploy "$FUNC_CLI_DIRECTORY"
    Pop-Location
}
else
{
    $funcPath = (Get-Command $FUNC_CMDLET_NAME).Source
    $version = & $funcPath --version
    Write-Host "Using local Functions Core Tools (Version: $version)..."
}

# Set an environment variable containing the path to the func executable. This will be accessed from
# in FixtureHelper.cs for E2E testing
$env:FUNC_PATH = $funcPath
Write-Host "Set FUNC_PATH environment variable to $env:FUNC_PATH"

# For both integration build test runs and regular test runs, we copy binaries to DurableApp/Modules
Write-Host "Building the DurableSDK module and copying binaries to the DurableApp/Modules directory..."
$configuration = if ($env:CONFIGURATION) { $env:CONFIGURATION } else { 'Debug' }

Push-Location "$PSScriptRoot/../.."
& ./build.ps1 -Configuration 'Debug'
Pop-Location

Write-Host "Starting Core Tools..."
Push-Location "$PSScriptRoot\DurableApp"

$Env:TestTaskHubName = $taskHubName
$Env:FUNCTIONS_WORKER_RUNTIME = "powershell"
$Env:FUNCTIONS_WORKER_RUNTIME_VERSION = $POWERSHELL_VERSION
$Env:AZURE_FUNCTIONS_ENVIRONMENT = "Development"

Write-Host "Installing extensions..."

if (($IsMacOS -or $IsLinux) -and (-not $SkipCoreToolsDownload)) {
    chmod +x $funcPath
}

& $funcPath extensions install | ForEach-Object {    
  if ($_ -match 'OK')    
  { Write-Host $_ -f Green }    
  elseif ($_ -match 'FAIL|ERROR')   
  { Write-Host $_ -f Red }
  else    
  { Write-Host $_ }    
}

if ($LASTEXITCODE -ne 0) { throw "Installing extensions failed." }
Pop-Location

Write-Host "Running E2E integration tests..." -ForegroundColor Green
Write-Host "-----------------------------------------------------------------------------`n" -ForegroundColor Green

dotnet test "$PSScriptRoot/AzureFunctions.PowerShell.Durable.SDK.E2E/AzureFunctions.PowerShell.Durable.SDK.E2E.csproj" --logger:trx --results-directory "$PSScriptRoot/../../TestResults"
if ($LASTEXITCODE -ne 0) { throw "xunit tests failed." }

Write-Host "-----------------------------------------------------------------------------" -ForegroundColor Green