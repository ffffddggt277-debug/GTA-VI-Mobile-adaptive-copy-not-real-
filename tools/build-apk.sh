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

Activate the free Personal licence once (two minutes, needs a browser):

  1. Generate the activation request:
       source ~/unity-android-env.sh 2>/dev/null
       "$HOME/tools/unity/Editor/Unity" -batchmode -nographics -createManualActivationFile -logFile -
     -> ~/Unity_v6000.3.29f1.alf   (a copy is in licenses/)

  2. Open https://license.unity3d.com/manual, sign in, upload the .alf,
     pick "Unity Personal", download the .ulf file.

  3. Put the file here:
       mkdir -p ~/.local/share/unity3d/Unity
       cp Unity_v6000.3.29f1.ulf ~/.local/share/unity3d/Unity/Unity_lic.ulf

  4. Re-run: bash tools/build-apk.sh
MSG
  exit 3
fi

echo "build-apk: importing and building (first run takes several minutes)"
rm -f "$LOG"
"$UNITY_EDITOR" -batchmode -nographics -quit \
  -projectPath "$PROJECT" \
  -buildTarget Android \
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
