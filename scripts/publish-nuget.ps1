<#
.SYNOPSIS
    ShadBlazor.LucideIcon NuGet Packaging and Publishing Script
.DESCRIPTION
    Automates clean, restore, build, testing, packaging, and publishing to NuGet.org.
.PARAMETER ApiKey
    NuGet.org API Key. Defaults to $env:NUGET_API_KEY if not specified.
.PARAMETER Version
    Specific version override (e.g. 1.33.0). Defaults to project version.
.PARAMETER SkipPush
    If set, only build, test, and pack; do not push to NuGet.
.PARAMETER Source
    NuGet package source URL, defaults to https://api.nuget.org/v3/index.json.
.EXAMPLE
    .\scripts\publish-nuget.ps1 -SkipPush
.EXAMPLE
    .\scripts\publish-nuget.ps1 -ApiKey "oy2..."
#>

[CmdletBinding()]
param (
    [string]$ApiKey = $env:NUGET_API_KEY,
    [string]$Version = "",
    [string]$Source = "https://api.nuget.org/v3/index.json",
    [string]$Configuration = "Release",
    [switch]$SkipPush
)

$ErrorActionPreference = "Stop"
$RootPath = Resolve-Path (Join-Path $PSScriptRoot "..")
$ArtifactsPath = Join-Path $RootPath "artifacts"
$ProjectFile = Join-Path $RootPath "src\ShadBlazor.LucideIcon\ShadBlazor.LucideIcon.csproj"
$TestProject = Join-Path $RootPath "tests\ShadBlazor.LucideIcon.Tests\ShadBlazor.LucideIcon.Tests.csproj"
$SolutionFile = Join-Path $RootPath "ShadBlazor.Lucide.sln"

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "  ShadBlazor.LucideIcon NuGet Release & Publish Tool      " -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

# 1. Prepare Artifacts
if (Test-Path $ArtifactsPath) {
    Write-Host "[1/5] Cleaning old artifacts: $ArtifactsPath" -ForegroundColor Yellow
    Remove-Item $ArtifactsPath -Recurse -Force
}
New-Item -ItemType Directory -Path $ArtifactsPath -Force | Out-Null

# 2. Build
Write-Host "[2/5] Building solution ($Configuration)..." -ForegroundColor Yellow
$BuildArgs = @("build", $SolutionFile, "-c", $Configuration)
if ($Version) {
    $BuildArgs += "/p:Version=$Version"
}
& dotnet @BuildArgs
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed. Aborting release."
    exit $LASTEXITCODE
}

# 3. Test
Write-Host "[3/5] Running multi-target unit tests..." -ForegroundColor Yellow
& dotnet test $TestProject -c $Configuration --no-build
if ($LASTEXITCODE -ne 0) {
    Write-Error "Unit tests failed. Aborting release."
    exit $LASTEXITCODE
}

# 4. Pack
Write-Host "[4/5] Packing NuGet (.nupkg)..." -ForegroundColor Yellow
$PackArgs = @("pack", $ProjectFile, "-c", $Configuration, "-o", $ArtifactsPath, "--no-build")
if ($Version) {
    $PackArgs += "/p:Version=$Version"
}
& dotnet @PackArgs
if ($LASTEXITCODE -ne 0) {
    Write-Error "Packaging failed."
    exit $LASTEXITCODE
}

$Packages = Get-ChildItem -Path $ArtifactsPath -Filter "*.nupkg"
if ($Packages.Count -eq 0) {
    Write-Error "No .nupkg found in $ArtifactsPath"
    exit 1
}

Write-Host ""
Write-Host "Successfully generated NuGet packages:" -ForegroundColor Green
foreach ($pkg in $Packages) {
    $sizeKb = [Math]::Round($pkg.Length / 1KB, 2)
    Write-Host "  - $($pkg.Name) ($sizeKb KB)" -ForegroundColor Green
}

# 5. Push
if ($SkipPush) {
    Write-Host ""
    Write-Host "[5/5] SkipPush enabled. Packages saved in $ArtifactsPath" -ForegroundColor Cyan
    exit 0
}

if (-not $ApiKey) {
    Write-Host ""
    Write-Host "[5/5] No API Key provided. Enter NuGet API Key (or press Enter to skip):" -ForegroundColor Yellow
    $ApiKey = Read-Host "NuGet API Key"
    if (-not $ApiKey) {
        Write-Warning "Skipping push. Artifacts available in $ArtifactsPath"
        exit 0
    }
}

Write-Host ""
Write-Host "[5/5] Publishing to $Source ..." -ForegroundColor Yellow
foreach ($pkg in $Packages) {
    Write-Host "Pushing $($pkg.FullName) ..." -ForegroundColor Cyan
    & dotnet nuget push $pkg.FullName --api-key $ApiKey --source $Source --skip-duplicate
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Failed to push $($pkg.Name)!"
        exit $LASTEXITCODE
    }
}

Write-Host ""
Write-Host "==========================================================" -ForegroundColor Green
Write-Host "  ShadBlazor.LucideIcon Published Successfully!" -ForegroundColor Green
Write-Host "==========================================================" -ForegroundColor Green
