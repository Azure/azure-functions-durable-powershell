# This build script is designed to work with the ALC-based PowerShell guidance for dependency isolation.
# https://docs.microsoft.com/en-us/powershell/scripting/dev-cross-plat/resolving-dependency-conflicts?view=powershell-7.2#loading-through-net-core-assembly-load-contexts
param(
    [ValidateSet('Debug', 'Release')]
    [string]
    $Configuration = 'Debug'
)

$shimPath = "$PSScriptRoot/AzShim"
$durableEnginePath = "$PSScriptRoot/DurableEngine"

$outputPath = "$PSScriptRoot/out/"
$sharedDependenciesPath = "$outputPath/Dependencies/"

$netCoreTFM = 'net6.0'
$publishPathSuffix = "bin/$Configuration/$netCoreTFM/publish"

# Remove previous build, if it exists
if (Test-Path $sharedDependenciesPath)
{
    Remove-Item -Path $sharedDependenciesPath -Recurse
}
# Create output folder and its inner dependencies directory
New-Item -Path $sharedDependenciesPath -ItemType Directory

# Build Durable Engine project
Push-Location $durableEnginePath
try
{
    dotnet publish
}
finally
{
    Pop-Location
}

# Build Durable SDK project
Push-Location $shimPath
try
{
    dotnet publish -f $netCoreTFM
}
finally
{
    Pop-Location
}

$commonFiles = [System.Collections.Generic.HashSet[string]]::new()

# Copy all assemblies from Durable Engine project into shared dependencies directory
Get-ChildItem -Path "$durableEnginePath/$publishPathSuffix" |
    Where-Object { $_.Extension -in '.dll','.pdb' } |
    ForEach-Object { [void]$commonFiles.Add($_.Name); Copy-Item -LiteralPath $_.FullName -Destination $sharedDependenciesPath }

# Copy all *unique* assemblies from Durable SDK into output directory
Get-ChildItem -Path "$shimPath/$publishPathSuffix" |
    Where-Object { $_.Extension -in '.dll','.pdb' -and -not $commonFiles.Contains($_.Name) } |
    ForEach-Object { Copy-Item -LiteralPath $_.FullName -Destination $outputPath }