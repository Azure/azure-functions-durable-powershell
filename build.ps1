# This build script is designed to work with the ALC-based PowerShell guidance for dependency isolation.
# https://docs.microsoft.com/en-us/powershell/scripting/dev-cross-plat/resolving-dependency-conflicts?view=powershell-7.2#loading-through-net-core-assembly-load-contexts
param(
    [ValidateSet('Debug', 'Release')]
    [string]
    $Configuration = 'Debug'
)

$shimPath = "$PSScriptRoot/src/DurableSDK"
$durableEnginePath = "$PSScriptRoot/src/DurableEngine"
$durableAppPath = "$PSScriptRoot/samples/durableApp/Modules/AzureFunctions.PowerShell.Durable.SDK"

$outputPath = "$PSScriptRoot/src/out/"
if ($Configuration -eq "Debug")
{
    # Publish directly to the sample function app for testing
    $outputPath = $durableAppPath
}
$sharedDependenciesPath = "$outputPath/Dependencies/"

$netCoreTFM = 'net6.0'
$publishPathSuffix = "bin/$Configuration/$netCoreTFM/publish"

function Write-Log
{
    param (
        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        [System.String]
        $Message,

        [Switch]
        $Throw
    )

    $Message = (Get-Date -Format G)  + " -- $Message"

    if ($Throw)
    {
        throw $Message
    }

    Write-Host $Message
}

Write-Log "Build started. Configuration '$Configuration' and output folder '$outputPath' with shared dependencies folder '$sharedDependenciesPath'..."

# Map from project names to the folder containing the corresponding .csproj
$projects = @{
    'Durable SDK' = $shimPath
    'Durable Engine' = $durableEnginePath
}

# Remove previous build if it exists
Write-Log "Removing previous build from $outputPath if it exists..."
if (Test-Path $outputPath)
{
    Remove-Item -Path $outputPath -Recurse
}
# Create output folder and its inner dependencies directory
Write-Log "Creating a new output and shared dependencies folder at $outputPath and $sharedDependenciesPath..."
New-Item -Path $sharedDependenciesPath -ItemType Directory

# Build the Durable SDK and Durable Engine project
foreach ($project in $projects.GetEnumerator()) {
    Write-Log "Building $($project.Name) project with target framework $netCoreTFM...."
    Push-Location $project.Value
    try
    {
        dotnet publish -f $netCoreTFM
    }
    finally
    {
        Pop-Location
    }
}

$commonFiles = [System.Collections.Generic.HashSet[string]]::new()

Write-Log "Copying assemblies from the Durable Engine project into $sharedDependenciesPath"
Get-ChildItem -Path "$durableEnginePath/$publishPathSuffix" |
    Where-Object { $_.Extension -in '.dll','.pdb' } |
    ForEach-Object { [void]$commonFiles.Add($_.Name); Copy-Item -LiteralPath $_.FullName -Destination $sharedDependenciesPath }

# Copy all *unique* assemblies from Durable SDK into output directory
Write-Log "Copying UNIQUE assemblies from the Durable SDK project into $outputPath"
Get-ChildItem -Path "$shimPath/$publishPathSuffix" |
    Where-Object { $_.Extension -in '.dll','.pdb' -and -not $commonFiles.Contains($_.Name) } |
    ForEach-Object { Copy-Item -LiteralPath $_.FullName -Destination $outputPath }