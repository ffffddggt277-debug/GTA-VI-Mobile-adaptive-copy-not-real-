#!/usr/bin/env bash
# Local Blender for headless renders (no root needed).
set -euo pipefail
cd "$(dirname "$0")/.."

mkdir -p tools
if [ ! -d tools/blender-4.2.3-linux-x64 ]; then
  curl -fL -o tools/blender-4.2.3-linux-x64.tar.xz \
    https://download.blender.org/release/Blender4.2/blender-4.2.3-linux-x64.tar.xz
  tar -xf tools/blender-4.2.3-linux-x64.tar.xz -C tools/
fi

# runtime libs missing from the minimal container image
mkdir -p tools/sysroot
base=http://archive.ubuntu.com/ubuntu/pool/main
for pair in libx/libxrender:libxrender1 libx/libxfixes:libxfixes3 libx/libxi:libxi6 \
            libx/libxkbcommon:libxkbcommon0 libs/libsm:libsm6 libi/libice6:libice6; do
  d=${pair%%:*}; pat=${pair##*:}
  url=$(curl -s "$base/$d/" | grep -o 'href="[^"]*amd64\.deb"' | sed 's/href="//;s/"$//' \
        | grep "$pat" | grep -v dbgsym | sort -V | tail -1)
  [ -n "$url" ] || continue
  (cd tools && curl -sO "$base/$d/$url" && dpkg-deb -x "$url" sysroot && rm -f "$url")
done

sed -i 's|^tools/blender|# tools/blender|' .gitignore 2>/dev/null || true
echo "run renders with:"
echo "LD_LIBRARY_PATH=\$PWD/tools/sysroot/usr/lib/x86_64-linux-gnu:\$PWD/tools/blender-4.2.3-linux-x64/lib \\"
echo "  tools/blender-4.2.3-linux-x64/blender -b --python tools/render_blender.py -- renders/single"
