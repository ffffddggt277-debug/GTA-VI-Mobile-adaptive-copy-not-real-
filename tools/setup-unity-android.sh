#!/usr/bin/env bash
# Установка Unity 6 LTS + Android SDK без root в этом облачном окружении.
#
#   bash tools/setup-unity-android.sh
#
# Ставит всё в $HOME (вне git): ~/tools/unity, ~/tools/jdk, ~/android-sdk, ~/libs.
# Скрипт идемпотентен: уже скачанное и распакованное пропускается.
# Лицензию Unity этот скрипт не активирует — см. tools/UNITY-ANDROID.md.

set -euo pipefail

UNITY_VERSION="6000.3.29f1"
UNITY_CHANGESET="96d2fe17529c"
JDK_PACKAGE="amazon-corretto-17"
SDK_PLATFORM="android-35"
BUILD_TOOLS="35.0.0"

HOME_DIR="${HOME:?}"
DL="$HOME_DIR/dl"
TOOLS="$HOME_DIR/tools"
UNITY_DIR="$TOOLS/unity"
JDK_DIR="$TOOLS/jdk"
SDK_DIR="$HOME_DIR/android-sdk"
LIBS="$HOME_DIR/libs"
mkdir -p "$DL" "$TOOLS" "$JDK_DIR" "$LIBS"

say() { printf '\n==> %s\n' "$*"; }
have() { [ -e "$1" ]; }

# ── 1. Unity Editor ─────────────────────────────────────────────────────────
if have "$UNITY_DIR/Editor/Unity"; then
  say "Unity уже распакован: $UNITY_DIR"
else
  say "Скачиваю Unity $UNITY_VERSION (~4.6 ГБ)"
  curl -fL --retry 3 -o "$DL/unity.tar.xz" \
    "https://download.unity3d.com/download_unity/$UNITY_CHANGESET/LinuxEditorInstaller/Unity-$UNITY_VERSION.tar.xz"
  say "Распаковываю Unity"
  mkdir -p "$UNITY_DIR"
  tar -xJf "$DL/unity.tar.xz" -C "$UNITY_DIR"
  rm -f "$DL/unity.tar.xz"
fi

# ── 2. JDK 17 ───────────────────────────────────────────────────────────────
if have "$JDK_DIR"/*/bin/java; then
  say "JDK уже есть: $JDK_DIR"
else
  say "Скачиваю OpenJDK 17 (Corretto)"
  curl -fL --retry 3 -o "$DL/jdk.tar.gz" \
    "https://corretto.aws/downloads/latest/${JDK_PACKAGE}-x64-linux-jdk.tar.gz"
  tar -xzf "$DL/jdk.tar.gz" -C "$JDK_DIR"
  rm -f "$DL/jdk.tar.gz"
fi

export JAVA_HOME
JAVA_HOME="$(echo "$JDK_DIR"/*/ | head -1)"
JAVA_HOME="${JAVA_HOME%/}"
export PATH="$JAVA_HOME/bin:$PATH"

# ── 3. Android SDK ──────────────────────────────────────────────────────────
SDKMANAGER="$SDK_DIR/cmdline-tools/latest/bin/sdkmanager"
if have "$SDKMANAGER"; then
  say "Android cmdline-tools уже есть"
else
  say "Скачиваю Android cmdline-tools"
  curl -fL --retry 3 -o "$DL/cmdline-tools.zip" \
    "https://dl.google.com/android/repository/commandlinetools-linux-11076708_latest.zip"
  mkdir -p "$SDK_DIR/cmdline-tools"
  unzip -q -o "$DL/cmdline-tools.zip" -d "$SDK_DIR/cmdline-tools"
  rm -rf "$SDK_DIR/cmdline-tools/latest"
  mv "$SDK_DIR/cmdline-tools/cmdline-tools" "$SDK_DIR/cmdline-tools/latest"
  rm -f "$DL/cmdline-tools.zip"
fi

say "Принимаю лицензии и ставлю platform-tools, $SDK_PLATFORM, build-tools $BUILD_TOOLS"
yes | "$SDKMANAGER" --sdk_root="$SDK_DIR" --licenses >/dev/null
"$SDKMANAGER" --sdk_root="$SDK_DIR" "platform-tools" "platforms;$SDK_PLATFORM" "build-tools;$BUILD_TOOLS"

# ── 4. GUI-библиотеки для Unity (без root, через apt-get download) ───────────
if have "$LIBS/usr/lib/x86_64-linux-gnu/libgtk-3.so.0"; then
  say "Библиотеки GTK уже распакованы"
else
  say "Достаю .deb с GTK/X11 и распаковываю в $LIBS"
  W="$HOME_DIR/aptwork"
  mkdir -p "$W/lists/partial" "$W/archives/partial" "$W/cache" "$DL/debs"
  echo "deb http://archive.ubuntu.com/ubuntu noble main universe" > "$W/sources.list"
  : > "$W/status"
  APT=(apt-get
    -o "Dir::Etc::sourcelist=$W/sources.list"
    -o "Dir::Etc::sourceparts=/dev/null"
    -o "Dir::State=$W"
    -o "Dir::State::status=$W/status"
    -o "Dir::Cache=$W/cache"
    -o "APT::Sandbox::User=$USER")
  "${APT[@]}" update -qq
  ( cd "$DL/debs" && "${APT[@]}" download \
      libgtk-3-0t64 libgtk-3-common libxcursor1 libxrandr2 libharfbuzz0b \
      libpango-1.0-0 libpangocairo-1.0-0 libpangoft2-1.0-0 libcairo2 libcairo-gobject2 \
      libgdk-pixbuf-2.0-0 libfontconfig1 fontconfig-config libatk1.0-0t64 \
      libatk-bridge2.0-0t64 libatspi2.0-0t64 libepoxy0 libfribidi0 libthai0 libdatrie1 \
      libgraphite2-3 libpng16-16t64 libpixman-1-0 libxfixes3 libxi6 libxrender1 \
      libxinerama1 libxkbcommon0 libxcomposite1 libxdamage1 libfreetype6 libjpeg-turbo8 \
      libwayland-client0 libwayland-cursor0 libwayland-egl1 libxcb-render0 libxcb-shm0 \
      libxcb-glx0 libxcb-util1 libxcb-image0 libxcb-keysyms1 libxcb-randr0 \
      libxcb-render-util0 libxcb-shape0 libxcb-sync1 libxcb-xfixes0 libxcb-xkb1 \
      libxcb-xinerama0 libxcb-icccm4 libxtst6 libsm6 libice6 libxmu6 libxpm4 )
  for deb in "$DL/debs"/*.deb; do dpkg-deb -x "$deb" "$LIBS"; done
  rm -rf "$DL/debs"
fi

# ── 5. Android Build Support для редактора ──────────────────────────────────
ANDROID_PLAYER="$UNITY_DIR/Editor/Data/PlaybackEngines/AndroidPlayer"
if have "$ANDROID_PLAYER/Tools/gradle"; then
  say "Android Build Support уже на месте"
else
  say "Скачиваю модуль Android Build Support (~2 ГБ)"
  curl -fL --retry 3 -o "$DL/android.pkg" \
    "https://download.unity3d.com/download_unity/$UNITY_CHANGESET/MacEditorTargetInstaller/UnitySetup-Android-Support-for-Editor-$UNITY_VERSION.pkg"
  say "Распаковываю xar → cpio → PlaybackEngines/AndroidPlayer"
  mkdir -p "$ANDROID_PLAYER"
  python3 - "$DL/android.pkg" "$ANDROID_PLAYER" <<'PY'
import gzip, os, shutil, struct, sys, tempfile

pkg, dst = sys.argv[1], sys.argv[2]
with open(pkg, 'rb') as f:
    magic, hsize, _ver, toc_c, _toc_u, _alg = struct.unpack('>4sHHQQI', f.read(28))
    assert magic == b'xar!', 'не xar-пакет'
    toc = gzip.decompress(f.read(toc_c)).decode('utf8', 'replace')

# Payload — единственный большой файл в TOC
import re
block = None
for m in re.finditer(r'<file id="\d+">(.*?)</file>', toc, re.S):
    body = m.group(1)
    if '<name>Payload</name>' in body:
        block = body
        break
assert block, 'Payload не найден в TOC'
offset = int(re.search(r'<offset>(\d+)</offset>', block).group(1))
length = int(re.search(r'<length>(\d+)</length>', block).group(1))

tmp = tempfile.NamedTemporaryFile(suffix='.cpio.gz', delete=False)
with open(pkg, 'rb') as f:
    f.seek(28 + toc_c + offset)
    left = length
    while left:
        chunk = f.read(min(1 << 22, left))
        if not chunk:
            break
        tmp.write(chunk)
        left -= len(chunk)
tmp.close()

f = gzip.open(tmp.name, 'rb')
count = 0
while True:
    hdr = f.read(76)
    if len(hdr) < 76:
        break
    if hdr[:6] != b'070707':
        # однобайтовое выравнивание в odc-архиве: ищем следующий заголовок
        pos = f.tell()
        found = False
        for back in range(1, 5):
            f.seek(pos - 76 - back)
            chunk = f.read(76 + back)
            idx = chunk.find(b'070707')
            if idx >= 0:
                f.seek(pos - 76 - back + idx + 6)
                hdr = b'070707' + f.read(70)
                found = True
                break
        if not found:
            break
    namesize = int(hdr[59:65], 8)
    filesize = int(hdr[65:76], 8)
    mode = int(hdr[18:24], 8)
    name = f.read(namesize).split(b'\0')[0].decode('utf8', 'replace')
    if (76 + namesize) % 2:
        f.read(1)
    if name == 'TRAILER!!!':
        break
    path = os.path.join(dst, name.lstrip('./'))
    kind = mode & 0o170000
    if kind == 0o040000:
        os.makedirs(path, exist_ok=True)
    elif kind == 0o120000:
        target = f.read(filesize)
        os.makedirs(os.path.dirname(path), exist_ok=True)
        if not os.path.lexists(path):
            os.symlink(target.decode(), path)
    else:
        os.makedirs(os.path.dirname(path), exist_ok=True)
        with open(path, 'wb') as out:
            left = filesize
            while left:
                chunk = f.read(min(1 << 20, left))
                if not chunk:
                    break
                out.write(chunk)
                left -= len(chunk)
        os.chmod(path, mode & 0o777)
    if filesize % 2:
        f.read(1)
    count += 1
os.unlink(tmp.name)
print('распаковано файлов:', count)
PY
  rm -f "$DL/android.pkg"
fi

# ── 5b. Gradle ──────────────────────────────────────────────────────────────
# В macOS-пакете модуля Android нет исполняемого bin/gradle (только lib/), поэтому
# доставляем официальный дистрибутив той же версии, что и библиотеки Unity.
GRADLE_VERSION="9.3.1"
if [ ! -x "$ANDROID_PLAYER/Tools/gradle/bin/gradle" ]; then
  say "Ставлю Gradle $GRADLE_VERSION"
  TMP_GRADLE="$(mktemp -d)"
  if curl -fsSL -o "$TMP_GRADLE/gradle.zip" \
      "https://services.gradle.org/distributions/gradle-${GRADLE_VERSION}-bin.zip"; then
    unzip -q -o "$TMP_GRADLE/gradle.zip" -d "$TMP_GRADLE"
    [ -d "$ANDROID_PLAYER/Tools/gradle" ] && mv "$ANDROID_PLAYER/Tools/gradle" "$ANDROID_PLAYER/Tools/gradle.libonly"
    cp -r "$TMP_GRADLE/gradle-${GRADLE_VERSION}" "$ANDROID_PLAYER/Tools/gradle"
    chmod +x "$ANDROID_PLAYER/Tools/gradle/bin/gradle"
    rm -rf "$TMP_GRADLE"
    have "$ANDROID_PLAYER/Tools/gradle/bin/gradle" && say "Gradle на месте"
  else
    say "Не скачался Gradle — APK не соберётся, запустите скрипт ещё раз"
  fi
fi

# ── 6. Окружение ────────────────────────────────────────────────────────────
ENV_FILE="$HOME_DIR/unity-android-env.sh"
cat > "$ENV_FILE" <<EOF
# Подключается через: source ~/unity-android-env.sh
export JAVA_HOME="$JAVA_HOME"
export ANDROID_HOME="$SDK_DIR"
export ANDROID_SDK_ROOT="$SDK_DIR"
export UNITY_EDITOR="$UNITY_DIR/Editor/Unity"
export PATH="\$JAVA_HOME/bin:\$ANDROID_HOME/platform-tools:\$ANDROID_HOME/cmdline-tools/latest/bin:\$PATH"
# Unity линкуется с GTK/X11, которых нет в системе
export LD_LIBRARY_PATH="$LIBS/usr/lib/x86_64-linux-gnu\${LD_LIBRARY_PATH:+:\$LD_LIBRARY_PATH}"
EOF

say "Готово"
echo "  Unity:       $UNITY_DIR/Editor/Unity  ($("$UNITY_DIR/Editor/Unity" -version 2>/dev/null || echo 'лицензия не активирована'))"
echo "  JDK:         $JAVA_HOME"
echo "  Android SDK: $SDK_DIR"
echo "  Окружение:   source $ENV_FILE"
echo
echo "Дальше нужна лицензия Unity — инструкция в tools/UNITY-ANDROID.md."
