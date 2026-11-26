#!/usr/bin/env bash
# Builds game/unity into an Android APK.
#
#   bash tools/build-apk.sh
#
# Unity 6 requires an activated licence before it will even import the project.
# If activation is missing the script stops early and explains how to do it.
set -uo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
PROJECT="$ROOT/game/unity"
LOG="${LOG:-/tmp/leonida-build.log}"
APK="$PROJECT/Builds/GTA6-Leonida.apk"

export UNITY_EDITOR="${UNITY_EDITOR:-$HOME/tools/unity/Editor/Unity}"
export JAVA_HOME="${JAVA_HOME:-$HOME/tools/jdk/amazon-corretto-17.0.21.1.2-linux-x64}"
export ANDROID_HOME="${ANDROID_HOME:-$HOME/android-sdk}"
export ANDROID_SDK_ROOT="$ANDROID_HOME"
export LD_LIBRARY_PATH="$HOME/libs/usr/lib/x86_64-linux-gnu${LD_LIBRARY_PATH:+:$LD_LIBRARY_PATH}"
export PATH="$JAVA_HOME/bin:$ANDROID_HOME/platform-tools:$PATH"

if [ ! -x "$UNITY_EDITOR" ]; then
  echo "build-apk: Unity is not installed. Run: bash tools/setup-unity-android.sh" >&2
  exit 2
fi

LIC="$HOME/.local/share/unity3d/Unity/Unity_lic.ulf"
if [ ! -f "$LIC" ]; then
  cat >&2 <<'MSG'
build-apk: Unity has no licence, so it cannot open the project or build.

Unity 6 no longer supports manual .alf/.ulf activation of Personal licences.
Activate once with the bundled command-line licensing client:

    bash tools/activate-unity-license.sh

It asks for your Unity ID email and password (hidden input, nothing is saved)
and writes ~/.local/share/unity3d/Unity/Unity_lic.ulf.

Then re-run: bash tools/build-apk.sh
MSG
  exit 3
fi

echo "build-apk: importing and building (first run takes several minutes)"
rm -f "$LOG"
"$UNITY_EDITOR" -batchmode -nographics -quit \
  -projectPath "$PROJECT" \
  -buildTarget Android \
  -disable-assembly-updater \
  -executeMethod GTA6.Build.BuildScript.BuildApk \
  -logFile "$LOG"
STATUS=$?

if [ $STATUS -ne 0 ]; then
  echo "build-apk: Unity exited with $STATUS" >&2
  echo "--- last 40 log lines ---" >&2
  tail -40 "$LOG" >&2
  exit $STATUS
fi

if [ ! -f "$APK" ]; then
  echo "build-apk: build reported success but $APK is missing" >&2
  exit 4
fi

AAPT="$ANDROID_HOME/build-tools/35.0.0/aapt2"
APKSIGNER="$ANDROID_HOME/build-tools/35.0.0/apksigner"
echo "apk: $(ls -lh "$APK" | awk '{print $5}')  $APK"
if [ -x "$AAPT" ]; then
  echo "--- manifest ---"
  "$AAPT" dump badging "$APK" 2>/dev/null | head -12
fi
if [ -x "$APKSIGNER" ]; then
  "$APKSIGNER" verify --print-certs "$APK" 2>&1 | head -6
fi
