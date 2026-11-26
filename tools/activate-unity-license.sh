#!/usr/bin/env bash
# Активация Unity Personal на этой машине без Unity Hub и без браузера.
#
#   bash tools/activate-unity-license.sh
#
# Почему так: Unity 6 больше не поддерживает ручную активацию Personal через
# .alf/.ulf на license.unity3d.com. Единственный способ без графического Hub —
# CLI-клиент лицензий, который умеет --activate-ulf с логином и паролем Unity ID.
#
# Пароль вводится скрытым вводом (read -s): он не попадает в аргументы командной
# строки на диске, в историю shell и в файлы. Скрипт ничего не сохраняет.

set -euo pipefail

CLIENT="$HOME/tools/unity/Editor/Data/Resources/Licensing/Client/Unity.Licensing.Client"
LICENSE_DIR="$HOME/.local/share/unity3d/Unity"

if [ ! -x "$CLIENT" ]; then
  echo "Не найден Unity.Licensing.Client: $CLIENT" >&2
  echo "Сначала: bash tools/setup-unity-android.sh" >&2
  exit 1
fi

read -r -p "Unity email: " UNITY_EMAIL
read -r -s -p "Unity password: " UNITY_PASS
printf '\n'

if [ -z "${UNITY_EMAIL}" ] || [ -z "${UNITY_PASS}" ]; then
  echo "Нужны и email, и пароль." >&2
  exit 1
fi

export LD_LIBRARY_PATH="${LD_LIBRARY_PATH:-$HOME/libs/usr/lib/x86_64-linux-gnu}"

echo "==> Запрашиваю ULF (Personal) у licensing server..."
set +e
"$CLIENT" --activate-ulf --username "$UNITY_EMAIL" --password "$UNITY_PASS"
STATUS=$?
set -e
unset UNITY_PASS

echo
if [ "$STATUS" -ne 0 ]; then
  echo "Активация не удалась (код $STATUS)." >&2
  echo "Частые причины: неверный пароль, 2FA в аккаунте, нет интернета." >&2
  exit "$STATUS"
fi

echo "==> Файлы лицензии:"
ls -la "$LICENSE_DIR" 2>/dev/null || echo "  (каталог пуст: $LICENSE_DIR)"

if ls "$LICENSE_DIR"/*.ulf >/dev/null 2>&1; then
  echo "Готово. Дальше: bash tools/build-apk.sh"
else
  echo "ULF не появился — смотрите вывод выше." >&2
  exit 1
fi
