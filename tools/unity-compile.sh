#!/usr/bin/env bash
# Compiles the Unity game scripts with Unity's own Roslyn against Unity's own
# managed assemblies. No Unity license is needed for this: it is a plain C#
# typecheck, so script errors are caught before a build is attempted.
#
#   bash tools/unity-compile.sh                 # typecheck game/unity/Assets/Scripts
#   bash tools/unity-compile.sh out.dll         # typecheck and keep the assembly
set -uo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
DATA="${UNITY_DATA:-$HOME/tools/unity/Editor/Data}"
DOTNET="$DATA/NetCoreRuntime/dotnet"
CSC="$DATA/DotNetSdkRoslyn/csc.dll"
PROJ="$ROOT/game/unity"
OUT="/tmp/gta6-scripts.dll"
EXTRA=()
DEFS=()
EDITOR=0
while [ $# -gt 0 ]; do
  case "$1" in
    --extra) EXTRA+=("$2"); shift 2 ;;
    --define) DEFS+=("/define:$2"); shift 2 ;;
    --editor) EDITOR=1; shift ;;
    -out) OUT="$2"; shift 2 ;;
    -*) echo "unity-compile: unknown argument $1" >&2; exit 2 ;;
    *) OUT="$1"; shift ;;
  esac
done

if [ ! -x "$DOTNET" ] || [ ! -f "$CSC" ]; then
  echo "unity-compile: Unity editor not found in $DATA (run tools/setup-unity-android.sh)" >&2
  exit 2
fi

REFS=()
PATTERN='UnityEngine*.dll'
[ "$EDITOR" = "1" ] && PATTERN='Unity*.dll'
while IFS= read -r f; do REFS+=("-r:$f"); done < <(
  find "$DATA/Managed/UnityEngine" -maxdepth 1 -name "$PATTERN" | sort
)
while IFS= read -r f; do REFS+=("-r:$f"); done < <(
  find "$DATA/NetStandard" \( -name 'netstandard.dll' -o -name 'mscorlib.dll' -o -name 'System*.dll' \) \
    -path '*2.1.0*' ! -path '*EditorExtensions*' | sort -u
)

SOURCES=()
while IFS= read -r f; do SOURCES+=("$f"); done < <(
  find "$PROJ/Assets/Scripts" -name '*.cs' | sort
)
if [ "$EDITOR" = "1" ]; then
  while IFS= read -r f; do SOURCES+=("$f"); done < <(find "$PROJ/Assets/Editor" -name '*.cs' | sort)
fi
# Optional extra sources (tests, tools) passed as --extra <path>...
for f in "${EXTRA[@]:-}"; do
  [ -n "$f" ] || continue
  while IFS= read -r g; do SOURCES+=("$g"); done < <(find "$f" -name '*.cs' 2>/dev/null | sort)
done

if [ ${#SOURCES[@]} -eq 0 ]; then
  echo "unity-compile: no sources found under $PROJ/Assets/Scripts" >&2
  exit 2
fi

CMD=("$DOTNET" "$CSC" -nologo -noconfig -nostdlib -target:library -langversion:9.0
     -warn:4 -nowarn:CS0649,CS0169,CS0414,CS1591)
[ ${#DEFS[@]} -gt 0 ] && CMD+=("${DEFS[@]}")
CMD+=("${REFS[@]}" -out:"$OUT" "${SOURCES[@]}")

echo "unity-compile: ${#SOURCES[@]} files"
"${CMD[@]}"
STATUS=$?
if [ $STATUS -ne 0 ]; then
  echo "unity-compile: FAILED ($STATUS errors above)" >&2
  exit 1
fi
echo "unity-compile: OK -> $OUT"
