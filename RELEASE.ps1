#!/usr/bin/env pwsh
# Grondo Release Script (PowerShell)
# This script commits, tags, and pushes to GitHub
# GitHub Actions will automatically build, test, and publish to NuGet
#
# Usage: .\RELEASE.ps1 <version>
# Example: .\RELEASE.ps1 1.1.0

param(
    [Parameter(Mandatory=$true)]
    [string]$Version
)

$ErrorActionPreference = "Stop"

# Validate version format
if ($Version -notmatch '^\d+\.\d+\.\d+$') {
    Write-Host "❌ Error: Invalid version format: $Version" -ForegroundColor Red
    Write-Host "Expected format: MAJOR.MINOR.PATCH (e.g., 1.1.0)" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Version format:" -ForegroundColor Cyan
    Write-Host "  - MAJOR: Breaking changes (e.g., 2.0.0)"
    Write-Host "  - MINOR: New features (e.g., 1.1.0)"
    Write-Host "  - PATCH: Bug fixes (e.g., 1.0.1)"
    exit 1
}

Write-Host "🚀 Grondo Release Script" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host "Version: v$Version" -ForegroundColor Green
Write-Host ""
Write-Host "🔍 Pre-release verification:" -ForegroundColor Yellow
Write-Host "   - Code formatting check"
Write-Host "   - Comprehensive code analysis"
Write-Host "   - All unit tests"
Write-Host "   - NuGet package creation"
Write-Host ""
Write-Host "✨ GitHub Actions will automatically:" -ForegroundColor Yellow
Write-Host "   - Build the solution"
Write-Host "   - Run all tests"
Write-Host "   - Create NuGet package"
Write-Host "   - Publish to NuGet.org"
Write-Host ""
Write-Host "This script commits and pushes the tag."
Write-Host ""

# Step 1: Verify everything is ready
Write-Host "Step 1: Running final verification..." -ForegroundColor Cyan
Write-Host ""

# Clean build artifacts
Write-Host "🧹 Cleaning build artifacts..." -ForegroundColor Yellow
dotnet clean
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Clean failed!" -ForegroundColor Red
    exit 1
}
Write-Host ""

# Check code formatting
Write-Host "📐 Checking code formatting..." -ForegroundColor Yellow
dotnet format Grondo.sln --verify-no-changes --verbosity quiet
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Code formatting issues detected!" -ForegroundColor Red
    Write-Host "Run 'dotnet format Grondo.sln' to fix formatting issues." -ForegroundColor Yellow
    exit 1
}
Write-Host "✅ Code formatting verified!" -ForegroundColor Green
Write-Host ""

# Run comprehensive analyzers
Write-Host "🔍 Running comprehensive code analyzers..." -ForegroundColor Yellow
dotnet build Grondo.sln -c Release "/p:AnalysisMode=All" "/p:TreatWarningsAsErrors=true"
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Code analysis failed!" -ForegroundColor Red
    Write-Host "Fix all analyzer warnings before releasing." -ForegroundColor Yellow
    exit 1
}
Write-Host "✅ Code analysis passed!" -ForegroundColor Green
Write-Host ""

# Run tests
Write-Host "🧪 Running tests..." -ForegroundColor Yellow
dotnet test -c Release --verbosity minimal --no-build
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Tests failed!" -ForegroundColor Red
    exit 1
}
Write-Host "✅ All tests passed!" -ForegroundColor Green
Write-Host ""

# Create package
Write-Host "📦 Creating NuGet package..." -ForegroundColor Yellow
dotnet pack -c Release --no-build
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Package creation failed!" -ForegroundColor Red
    exit 1
}
Write-Host "✅ Package created!" -ForegroundColor Green
Write-Host ""

Write-Host "✅ All verification checks passed!" -ForegroundColor Green
Write-Host ""

# Step 2: Show current status
Write-Host "Step 2: Current Git status..." -ForegroundColor Cyan
git status

Write-Host ""
$response = Read-Host "Ready to commit? (y/n)"
if ($response -ne 'y' -and $response -ne 'Y') {
    Write-Host "Aborted." -ForegroundColor Yellow
    exit 1
}

# Step 3: Commit changes
Write-Host "Step 3: Committing changes..." -ForegroundColor Cyan
git add .
git commit -m "Release v$Version

See CHANGELOG.md for full details."

Write-Host ""
Write-Host "✅ Changes committed!" -ForegroundColor Green
Write-Host ""

# Step 4: Create tag
Write-Host "Step 4: Creating Git tag..." -ForegroundColor Cyan
git tag -a "v$Version" -m "Release v$Version

See CHANGELOG.md for full details."

Write-Host ""
Write-Host "✅ Tag created!" -ForegroundColor Green
Write-Host ""

# Step 5: Show tag
Write-Host "Step 5: Verifying tag..." -ForegroundColor Cyan
git tag -l -n9 "v$Version"

Write-Host ""
$response = Read-Host "Ready to push? (y/n)"
if ($response -ne 'y' -and $response -ne 'Y') {
    Write-Host "Aborted. To delete tag: git tag -d v$Version" -ForegroundColor Yellow
    exit 1
}

# Step 6: Push to GitHub
Write-Host "Step 6: Pushing to GitHub..." -ForegroundColor Cyan
git push origin main
git push origin "v$Version"

Write-Host ""
Write-Host "✅ Pushed to GitHub!" -ForegroundColor Green
Write-Host ""

# Step 7: Verify version locally
Write-Host "Step 7: Verifying MinVer version locally..." -ForegroundColor Cyan
dotnet build -c Release 2>&1 | Select-String "MinVer"

Write-Host ""
Write-Host "✅ Version verified!" -ForegroundColor Green
Write-Host ""

Write-Host "================================" -ForegroundColor Cyan
Write-Host "🎉 Tag Pushed Successfully!" -ForegroundColor Green
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "✨ GitHub Actions is now running:" -ForegroundColor Yellow
Write-Host "   1. Building the solution"
Write-Host "   2. Running all tests"
Write-Host "   3. Creating NuGet package"
Write-Host "   4. Publishing to NuGet.org"
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "1. Monitor GitHub Actions: https://github.com/gt-downunder/Grondo/actions"
Write-Host "2. Wait for green checkmark ✅"
Write-Host "3. Verify on NuGet.org: https://www.nuget.org/packages/Grondo"
Write-Host "4. (Optional) Create GitHub release: https://github.com/gt-downunder/Grondo/releases/new"
Write-Host ""
Write-Host "See .github/RELEASE_GUIDE.md for complete instructions." -ForegroundColor Cyan
Write-Host ""

