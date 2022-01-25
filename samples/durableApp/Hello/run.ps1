param($name)

# Import-Module myModule
# Import-Module .\Modules\MyModule2\bin\Debug\netstandard2.0\myModule2.dll
Hello($name)
"Success"
# $body = $(Get-Module -ListAvailable | Select-Object Name, Path)
# $body