param(
    [ValidateSet('Debug', 'Release')]
    [string]
    $Configuration = 'Debug'
)

$netCore = 'net6.0'

$outPath = "$PSScriptRoot/out/dependencies/"
if (Test-Path $outPath)
{
    Remove-Item -Path $outPath -Recurse
}
New-Item -Path $outPath -ItemType Directory

Push-Location "$PSScriptRoot/DurableEngine"
try
{
    dotnet publish
}
finally
{
    Pop-Location
}

Push-Location "$PSScriptRoot/AzShim"
try
{
    dotnet publish -f $netCore
}
finally
{
    Pop-Location
}

$commonFiles = [System.Collections.Generic.HashSet[string]]::new()
#Copy-Item -Path "$PSScriptRoot/DurableSDK.psd1" -Destination "$outPath/"
#Copy-Item -Path "$PSScriptRoot/DurableSDK.psm1" -Destination "$outPath/"

Get-ChildItem -Path "$PSScriptRoot/DurableEngine/bin/$Configuration/net6.0/publish" |
    Where-Object { $_.Extension -in '.dll','.pdb' } |
    ForEach-Object { [void]$commonFiles.Add($_.Name); Copy-Item -LiteralPath $_.FullName -Destination $outPath }

Get-ChildItem -Path "$PSScriptRoot/AzShim/bin/$Configuration/$netCore/publish" |
    Where-Object { $_.Extension -in '.dll','.pdb' -and -not $commonFiles.Contains($_.Name) } |
    ForEach-Object { Copy-Item -LiteralPath $_.FullName -Destination "$outPath/.." }