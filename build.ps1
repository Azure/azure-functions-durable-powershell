# This build script is designed to work with the ALC-based PowerShell guidance for dependency isolation.
# https://docs.microsoft.com/en-us/powershell/scripting/dev-cross-plat/resolving-dependency-conflicts?view=powershell-7.2#loading-through-net-core-assembly-load-contexts
param(
    [ValidateSet('Debug', 'Release')]
    [string]
    $Configuration = 'Debug',
    [switch]
    $AddSBOM,
    [switch]
    $SkipExternalHelp
)

Import-Module "$PSScriptRoot\pipelineUtilities.psm1" -Force

$packageName = "AzureFunctions.PowerShell.Durable.SDK"
$shimPath = "$PSScriptRoot/src/DurableSDK"
$durableEnginePath = "$PSScriptRoot/src/DurableEngine"
$durableAppPath = "$PSScriptRoot/test/E2E/durableApp/Modules/$packageName"
$powerShellModulePath = "$PSScriptRoot/src/$packageName.psm1"
$manifestPath = "$PSScriptRoot/src/$packageName.psd1"
$cfsRepositoryName = 'upstream-public'
$cfsFeedUri = 'https://pkgs.dev.azure.com/azfunc/public/_packaging/upstream-public/nuget/v2'

# Publish directly to the test durable app for testing
$outputPath = $durableAppPath

$sharedDependenciesPath = "$outputPath/Dependencies/"

$netCoreTFM = 'net8.0'
$publishPathSuffix = "bin/$Configuration/$netCoreTFM/publish"

#region BUILD ARTIFACTS ===========================================================================
Write-Log "Build started..."
Write-Log "Configuration: '$Configuration'`nOutput folder '$outputPath'`nShared dependencies folder: '$sharedDependenciesPath'" "Gray"

# Map from project names to the folder containing the corresponding .csproj
$projects = @{
    'Durable SDK' = $shimPath
    'Durable Engine' = $durableEnginePath
}

# Remove previous build if it exists
Write-Log "Removing previous build from $outputPath if it exists..." "Cyan"
if (Test-Path $outputPath)
{
    Remove-Item -Path $outputPath -Recurse -Force -ErrorAction Ignore
}
# Create output folder and its inner dependencies directory
Write-Log "Creating a new output and shared dependencies folder at $outputPath and $sharedDependenciesPath..." "Cyan"
[void](New-Item -Path $sharedDependenciesPath -ItemType Directory)

# Build the Durable SDK and Durable Engine project
foreach ($project in $projects.GetEnumerator()) {
    Write-Log "Building $($project.Name) project with target framework $netCoreTFM...."
    Push-Location $project.Value
    try
    {
        dotnet publish -f $netCoreTFM -c $Configuration
    }
    finally
    {
        Pop-Location
    }
}

$commonFiles = [System.Collections.Generic.HashSet[string]]::new()

Write-Log "Copying assemblies from the Durable Engine project into $sharedDependenciesPath" "Gray"
Get-ChildItem -Path (Join-Path "$durableEnginePath" "$publishPathSuffix") |
    Where-Object { $_.Extension -in '.dll','.pdb' } |
    ForEach-Object { [void]$commonFiles.Add($_.Name); Copy-Item -LiteralPath $_.FullName -Destination $sharedDependenciesPath }

# Copy all *unique* assemblies from Durable SDK into output directory
Write-Log "Copying unique assemblies from the Durable SDK project into $outputPath" "Gray"
Get-ChildItem -Path (Join-Path "$shimPath" "$publishPathSuffix") |
    Where-Object { $_.Extension -in '.dll','.pdb' -and -not $commonFiles.Contains($_.Name) } |
    ForEach-Object { Copy-Item -LiteralPath $_.FullName -Destination $outputPath }

# Move Durable SDK manifest into the output directory
Write-Log "Copying PowerShell module and manifest from the Durable SDK source code into $outputPath" "Gray"
Copy-Item -Path $powerShellModulePath -Destination $outputPath
Copy-Item -Path $manifestPath -Destination $outputPath

#region GENERATE EXTERNAL HELP ===================================================================
if ($SkipExternalHelp) {
    Write-Log "Skipping external help generation." "Gray"
}
else {
    $repositoryParameters = @{
        Name = $cfsRepositoryName
        SourceLocation = $cfsFeedUri
        InstallationPolicy = 'Trusted'
    }

    if ($env:SYSTEM_ACCESSTOKEN) {
        $secureToken = ConvertTo-SecureString $env:SYSTEM_ACCESSTOKEN -AsPlainText -Force
        $repositoryParameters.Credential = [System.Management.Automation.PSCredential]::new(
            'AzurePipelines',
            $secureToken)
    }

    if (Get-PSRepository -Name $cfsRepositoryName -ErrorAction SilentlyContinue) {
        Set-PSRepository @repositoryParameters
    }
    else {
        Register-PSRepository @repositoryParameters
    }

    Write-Log "Installing PlatyPS from Central Feed Service..." "Cyan"
    Install-Module -Name PlatyPS -Repository $cfsRepositoryName -Scope CurrentUser -Force
    Import-Module PlatyPS -Force

    $docsPath = "$PSScriptRoot/src/Help"
    $helpPath = "$outputPath/en-US"

    if (-not (Test-Path $helpPath)) {
        Write-Log "Creating help directory at $helpPath" "Cyan"
        New-Item -Path $helpPath -ItemType Directory -Force
    }

    Write-Log "Converting markdown files to MAML help files..." "Gray"
    New-ExternalHelp -Path $docsPath -OutputPath $helpPath -Force
    Write-Log "External help files generated successfully in $helpPath" "Green"
}
#endregion

Write-Log "Build succeeded!"
#endregion

#region ADD SBOM ==================================================================================
if ($AddSBOM) {
    # Install manifest tool
    $manifestToolPath = Install-SBOMUtil
    Write-Log "Manifest tool path: $manifestToolPath"

    # Generate manifest
    $telemetryFilePath = Join-Path $PSScriptRoot ((New-Guid).Guid + ".json")
    $packageName = "AzureFunctions.PowerShell.Durable.SDK"

    Write-Log "Running: dotnet $manifestToolPath generate -BuildDropPath $outputPath -BuildComponentPath $outputPath -Verbosity Information -t $telemetryFilePath -PackageName $packageName"
    dotnet $manifestToolPath generate -BuildDropPath $outputPath -BuildComponentPath $outputPath -Verbosity Information -t $telemetryFilePath -PackageName $packageName

    # Discard telemetry generated
    Remove-Item -Path $telemetryFilePath -ErrorAction Ignore
}
#endregion