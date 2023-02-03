param($name)
if ((Get-Random -Maximum 2) -lt 1)
{
    "Hello $name"
}
else {
    Write-Host "FLAKYFUNCTION FAILED"
    throw "Error in FlakyFunction"
}