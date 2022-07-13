param(
    [ValidateSet('Debug', 'Release')]
    [string]
    $Configuration = 'Debug'
)

$netCoreTFM = 'net6.0'
$shimPath = "$PSScriptRoot/AzShim"
$durableEnginePath = "$PSScriptRoot/DurableEngine"
$outputPath = "$PSScriptRoot/out/Dependencies/"

if (Test-Path $outputPath)
{
    Remove-Item -Path $outputPath -Recurse
}
New-Item -Path $outputPath -ItemType Directory

Push-Location $durableEnginePath
try
{
    dotnet publish
}
finally
{
    Pop-Location
}

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

Get-ChildItem -Path "$PSScriptRoot/DurableEngine/bin/$Configuration/net6.0/publish" |
    Where-Object { $_.Extension -in '.dll','.pdb' } |
    ForEach-Object { [void]$commonFiles.Add($_.Name); Copy-Item -LiteralPath $_.FullName -Destination $outputPath }

Get-ChildItem -Path "$PSScriptRoot/AzShim/bin/$Configuration/$netCoreTFM/publish" |
    Where-Object { $_.Extension -in '.dll','.pdb' -and -not $commonFiles.Contains($_.Name) } |
    ForEach-Object { Copy-Item -LiteralPath $_.FullName -Destination "$outputPath/.." }