#!/usr/bin/env bash
set -euo pipefail

# ==========================================================
#   ShadBlazor.LucideIcon NuGet Release & Publish Script
# ==========================================================

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "${SCRIPT_DIR}/.." && pwd)"
ARTIFACTS_DIR="${ROOT_DIR}/artifacts"
PROJECT_FILE="${ROOT_DIR}/src/ShadBlazor.LucideIcon/ShadBlazor.LucideIcon.csproj"
TEST_PROJECT="${ROOT_DIR}/tests/ShadBlazor.LucideIcon.Tests/ShadBlazor.LucideIcon.Tests.csproj"
SOLUTION_FILE="${ROOT_DIR}/ShadBlazor.Lucide.sln"

API_KEY="${NUGET_API_KEY:-}"
SOURCE="https://api.nuget.org/v3/index.json"
CONFIG="Release"
SKIP_PUSH=false
VERSION=""

while [[ $# -gt 0 ]]; do
  case $1 in
    -k|--api-key)
      API_KEY="$2"
      shift 2
      ;;
    -v|--version)
      VERSION="$2"
      shift 2
      ;;
    -s|--source)
      SOURCE="$2"
      shift 2
      ;;
    --skip-push)
      SKIP_PUSH=true
      shift
      ;;
    *)
      echo "Unknown option: $1"
      exit 1
      ;;
  esac
done

echo "=========================================================="
echo "  ShadBlazor.LucideIcon NuGet Release & Publish Tool"
echo "=========================================================="

# 1. Clean artifacts
echo "[1/5] Cleaning artifacts directory: ${ARTIFACTS_DIR}"
rm -rf "${ARTIFACTS_DIR}"
mkdir -p "${ARTIFACTS_DIR}"

# 2. Build
echo "[2/5] Building solution (${CONFIG})..."
if [ -n "${VERSION}" ]; then
  dotnet build "${SOLUTION_FILE}" -c "${CONFIG}" /p:Version="${VERSION}"
else
  dotnet build "${SOLUTION_FILE}" -c "${CONFIG}"
fi

# 3. Test
echo "[3/5] Running tests across target frameworks..."
dotnet test "${TEST_PROJECT}" -c "${CONFIG}" --no-build

# 4. Pack
echo "[4/5] Packing NuGet (.nupkg)..."
if [ -n "${VERSION}" ]; then
  dotnet pack "${PROJECT_FILE}" -c "${CONFIG}" -o "${ARTIFACTS_DIR}" --no-build /p:Version="${VERSION}"
else
  dotnet pack "${PROJECT_FILE}" -c "${CONFIG}" -o "${ARTIFACTS_DIR}" --no-build
fi

echo ""
echo "Successfully generated packages:"
ls -la "${ARTIFACTS_DIR}"/*.nupkg

# 5. Push
if [ "${SKIP_PUSH}" = true ]; then
  echo ""
  echo "[5/5] SkipPush enabled, finished without publishing."
  exit 0
fi

if [ -z "${API_KEY}" ]; then
  read -r -p "Enter NuGet API Key (or leave empty to skip): " API_KEY
  if [ -z "${API_KEY}" ]; then
    echo "No API Key provided. Artifacts saved in ${ARTIFACTS_DIR}."
    exit 0
  fi
fi

echo ""
echo "[5/5] Publishing to ${SOURCE}..."
for pkg in "${ARTIFACTS_DIR}"/*.nupkg; do
  echo "Pushing ${pkg}..."
  dotnet nuget push "${pkg}" --api-key "${API_KEY}" --source "${SOURCE}" --skip-duplicate
done

echo ""
echo "=========================================================="
echo "  🎉 ShadBlazor.LucideIcon Published Successfully!"
echo "=========================================================="
