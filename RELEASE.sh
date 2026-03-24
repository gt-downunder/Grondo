#!/bin/bash
# Grondo Release Script
# This script commits, tags, and pushes to GitHub
# GitHub Actions will automatically build, test, and publish to NuGet
#
# Usage: ./RELEASE.sh <version>
# Example: ./RELEASE.sh 1.1.0

set -e  # Exit on error

# Check if version argument is provided
if [ -z "$1" ]; then
    echo "❌ Error: Version number required"
    echo ""
    echo "Usage: ./RELEASE.sh <version>"
    echo "Example: ./RELEASE.sh 1.1.0"
    echo ""
    echo "Version format: MAJOR.MINOR.PATCH"
    echo "  - MAJOR: Breaking changes (e.g., 2.0.0)"
    echo "  - MINOR: New features (e.g., 1.1.0)"
    echo "  - PATCH: Bug fixes (e.g., 1.0.1)"
    echo ""
    exit 1
fi

VERSION="$1"

# Validate version format (basic check)
if ! [[ $VERSION =~ ^[0-9]+\.[0-9]+\.[0-9]+$ ]]; then
    echo "❌ Error: Invalid version format: $VERSION"
    echo "Expected format: MAJOR.MINOR.PATCH (e.g., 1.1.0)"
    exit 1
fi

echo "🚀 Grondo Release Script"
echo "================================"
echo "Version: v$VERSION"
echo ""
echo "🔍 Pre-release verification:"
echo "   - Code formatting check"
echo "   - Comprehensive code analysis"
echo "   - All unit tests"
echo "   - NuGet package creation"
echo ""
echo "✨ GitHub Actions will automatically:"
echo "   - Build the solution"
echo "   - Run all tests"
echo "   - Create NuGet package"
echo "   - Publish to NuGet.org"
echo ""
echo "This script commits and pushes the tag."
echo ""

# Step 1: Verify everything is ready
echo "Step 1: Running final verification..."
echo ""

# Clean build artifacts
echo "🧹 Cleaning build artifacts..."
dotnet clean
echo ""

# Check code formatting
echo "📐 Checking code formatting..."
if ! dotnet format Grondo.sln --verify-no-changes --verbosity quiet; then
    echo "❌ Code formatting issues detected!"
    echo "Run 'dotnet format Grondo.sln' to fix formatting issues."
    exit 1
fi
echo "✅ Code formatting verified!"
echo ""

# Run comprehensive analyzers
echo "🔍 Running comprehensive code analyzers..."
if ! dotnet build Grondo.sln -c Release "/p:AnalysisMode=All" "/p:TreatWarningsAsErrors=true"; then
    echo "❌ Code analysis failed!"
    echo "Fix all analyzer warnings before releasing."
    exit 1
fi
echo "✅ Code analysis passed!"
echo ""

# Run tests
echo "🧪 Running tests..."
if ! dotnet test -c Release --verbosity minimal --no-build; then
    echo "❌ Tests failed!"
    exit 1
fi
echo "✅ All tests passed!"
echo ""

# Create package
echo "📦 Creating NuGet package..."
if ! dotnet pack -c Release --no-build; then
    echo "❌ Package creation failed!"
    exit 1
fi
echo "✅ Package created!"
echo ""

echo "✅ All verification checks passed!"
echo ""

# Step 2: Show current status
echo "Step 2: Current Git status..."
git status

echo ""
read -p "Ready to commit? (y/n) " -n 1 -r
echo ""
if [[ ! $REPLY =~ ^[Yy]$ ]]
then
    echo "Aborted."
    exit 1
fi

# Step 3: Commit changes
echo "Step 3: Committing changes..."
git add .
git commit -m "Release v$VERSION

See CHANGELOG.md for full details."

echo ""
echo "✅ Changes committed!"
echo ""

# Step 4: Create tag
echo "Step 4: Creating Git tag..."
git tag -a v$VERSION -m "Release v$VERSION

See CHANGELOG.md for full details."

echo ""
echo "✅ Tag created!"
echo ""

# Step 5: Show tag
echo "Step 5: Verifying tag..."
git tag -l -n9 v$VERSION

echo ""
read -p "Ready to push? (y/n) " -n 1 -r
echo ""
if [[ ! $REPLY =~ ^[Yy]$ ]]
then
    echo "Aborted. To delete tag: git tag -d v$VERSION"
    exit 1
fi

# Step 6: Push to GitHub
echo "Step 6: Pushing to GitHub..."
git push origin main
git push origin v$VERSION

echo ""
echo "✅ Pushed to GitHub!"
echo ""

# Step 7: Verify version locally
echo "Step 7: Verifying MinVer version locally..."
dotnet build -c Release 2>&1 | grep MinVer

echo ""
echo "✅ Version verified!"
echo ""

echo "================================"
echo "🎉 Tag Pushed Successfully!"
echo "================================"
echo ""
echo "✨ GitHub Actions is now running:"
echo "   1. Building the solution"
echo "   2. Running all tests"
echo "   3. Creating NuGet package"
echo "   4. Publishing to NuGet.org"
echo ""
echo "Next steps:"
echo "1. Monitor GitHub Actions: https://github.com/gt-downunder/Grondo/actions"
echo "2. Wait for green checkmark ✅"
echo "3. Verify on NuGet.org: https://www.nuget.org/packages/Grondo"
echo "4. (Optional) Create GitHub release: https://github.com/gt-downunder/Grondo/releases/new"
echo ""
echo "See .github/RELEASE_GUIDE.md for complete instructions."
echo ""

