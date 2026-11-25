#!/usr/bin/env bash
# Builds the pure rules layer plus its tests and runs them on Unity's bundled
# .NET 6 runtime. No Unity licence is needed.
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
DATA="${UNITY_DATA:-$HOME/tools/unity/Editor/Data}"
DOTNET="$DATA/NetCoreRuntime/dotnet"
CSC="$DATA/DotNetSdkRoslyn/csc.dll"
OUT_DIR="${TMPDIR:-/tmp}/gta6-core-tests"
DLL="$OUT_DIR/CoreTests.dll"

mkdir -p "$OUT_DIR"

REFS=()
while IFS= read -r f; do REFS+=("-r:$f"); done < <(
  find "$DATA/NetStandard" \( -name 'netstandard.dll' -o -name 'System*.dll' \) \
    -path '*2.1.0*' ! -path '*EditorExtensions*' | sort -u
)

SOURCES=()
while IFS= read -r f; do SOURCES+=("$f"); done < <(find "$ROOT/game/unity/Assets/Scripts/Core" -name '*.cs' | sort)
while IFS= read -r f; do SOURCES+=("$f"); done < <(find "$ROOT/game/unity/Tests" -name '*.cs' | sort)

echo "run-core-tests: compiling ${#SOURCES[@]} files"
"$DOTNET" "$CSC" -nologo -noconfig -nostdlib -target:exe -langversion:9.0 \
  -warn:4 -nowarn:CS0649,CS0169,CS0414,CS1591 \
  "${REFS[@]}" -out:"$DLL" "${SOURCES[@]}"

cat > "$OUT_DIR/CoreTests.runtimeconfig.json" <<'JSON'
{
  "runtimeOptions": {
    "tfm": "net6.0",
    "framework": { "name": "Microsoft.NETCore.App", "version": "6.0.0" }
  }
}
JSON

echo "run-core-tests: running"
set +e
"$DOTNET" "$DLL" "$@"
STATUS=$?
set -e
echo "run-core-tests: exit $STATUS"
exit $STATUS
