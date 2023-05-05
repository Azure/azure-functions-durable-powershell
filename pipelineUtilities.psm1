#
# Copyright (c) Microsoft. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.
#

using namespace System.Runtime.InteropServices

$DotnetSDKVersionRequirements = @{

    # .NET SDK 3.1 is required by the Microsoft.ManifestTool.dll tool
    '3.1' = @{
        MinimalPatch = '415'
        DefaultPatch = '415'
    }
}

function Write-Log
{
    param (
        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [System.String]
        $Message,

        [Switch]
        $Warning,

        [Switch]
        $Throw,

        [System.String]
        $Color
    )

    $Message = (Get-Date -Format G)  + " -- $Message"

    if ($Throw)
    {
        throw $Message
    }

    $foregroundColor = if ($Warning.IsPresent) { 'Yellow' } elseif ($Color) { $Color } else { 'Green' }
    Write-Host -ForegroundColor $foregroundColor $Message
}

function Install-SBOMUtil
{
    if ([string]::IsNullOrEmpty($env:SBOMUtilSASUrl))
    {
        throw "The `$SBOMUtilSASUrl environment variable cannot be null or empty when specifying the `$AddSBOM switch"
    }

    $MANIFESTOOLNAME = "ManifestTool"
    Write-Log "Installing $MANIFESTOOLNAME..."

    $MANIFESTOOL_DIRECTORY = Join-Path $PSScriptRoot $MANIFESTOOLNAME
    Remove-Item -Recurse -Force $MANIFESTOOL_DIRECTORY -ErrorAction Ignore

    Invoke-RestMethod -Uri $env:SBOMUtilSASUrl -OutFile "$MANIFESTOOL_DIRECTORY.zip"
    Expand-Archive "$MANIFESTOOL_DIRECTORY.zip" -DestinationPath $MANIFESTOOL_DIRECTORY

    $dllName = "Microsoft.ManifestTool.dll"
    $manifestToolPath = Join-Path "$MANIFESTOOL_DIRECTORY" "$dllName"

    if (-not (Test-Path $manifestToolPath))
    {
        throw "$MANIFESTOOL_DIRECTORY does not contain '$dllName'"
    }

    Write-Log 'Done.'

    return $manifestToolPath
}


function AddLocalDotnetDirPath {
    $LocalDotnetDirPath = if ($IsWindows) { "$env:ProgramFiles/dotnet" } else { "/usr/share/dotnet" }
    if (($env:PATH -split [IO.Path]::PathSeparator) -notcontains $LocalDotnetDirPath) {
        $env:PATH = $LocalDotnetDirPath + [IO.Path]::PathSeparator + $env:PATH
    }
}

function Find-Dotnet
{
    AddLocalDotnetDirPath
    $listSdksOutput = dotnet --list-sdks
    $installedDotnetSdks = $listSdksOutput | ForEach-Object { $_.Split(" ")[0] }
    Write-Host "Detected dotnet SDKs: $($installedDotnetSdks -join ', ')"
    foreach ($majorMinorVersion in $DotnetSDKVersionRequirements.Keys) {
        $minimalVersion = "$majorMinorVersion.$($DotnetSDKVersionRequirements[$majorMinorVersion].MinimalPatch)"
        $firstAcceptable = $installedDotnetSdks |
                                Where-Object { $_.StartsWith("$majorMinorVersion.") } |
                                Where-Object { [System.Management.Automation.SemanticVersion]::new($_) -ge [System.Management.Automation.SemanticVersion]::new($minimalVersion) } |
                                Select-Object -First 1
        if (-not $firstAcceptable) {
            throw "Cannot find the dotnet SDK for .NET Core $majorMinorVersion. Version $minimalVersion or higher is required. Please specify '-Bootstrap' to install build dependencies."
        }
    }
}

function Install-Dotnet {
    [CmdletBinding()]
    param(
        [string]$Channel = 'release'
    )
    try {
        Find-Dotnet
        return  # Simply return if we find dotnet SDk with the correct version
    } catch { }
    $obtainUrl = "https://raw.githubusercontent.com/dotnet/cli/master/scripts/obtain"
    try {
        $installScript = if ($IsWindows) { "dotnet-install.ps1" } else { "dotnet-install.sh" }
        Invoke-WebRequest -Uri $obtainUrl/$installScript -OutFile $installScript
        foreach ($majorMinorVersion in $DotnetSDKVersionRequirements.Keys) {
            $version = "$majorMinorVersion.$($DotnetSDKVersionRequirements[$majorMinorVersion].DefaultPatch)"
            Write-Host "Installing dotnet SDK version $version"
            if ($IsWindows) {
                & .\$installScript -InstallDir "$env:ProgramFiles/dotnet" -Channel $Channel -Version $Version
            } else {
                bash ./$installScript --install-dir "/usr/share/dotnet" -c $Channel -v $Version
            }
        }
        AddLocalDotnetDirPath
    }
    finally {
        Remove-Item $installScript -Force -ErrorAction SilentlyContinue
    }
}