# Unity 6 и Android SDK в этой VM

Установлено без root (sudo в окружении запрещён) в домашний каталог. Всё ставится
заново одной командой:

```bash
bash tools/setup-unity-android.sh
source ~/unity-android-env.sh
```

## Что стоит и где

| Компонент | Версия | Путь | Проверка |
| --- | --- | --- | --- |
| Unity Editor | **6000.3.29f1** (Unity 6.3 LTS) | `~/tools/unity/Editor/Unity` | `Unity -version` → `6000.3.29f1` |
| Android Build Support | 6000.3.29f1 | `~/tools/unity/Editor/Data/PlaybackEngines/AndroidPlayer` | каталог с `Tools/gradle`, `UnityEditor.Android.Extensions.dll` |
| OpenJDK | 17.0.21.1 (Corretto) | `~/tools/jdk/amazon-corretto-17.0.21.1.2-linux-x64` | `java -version` |
| Android SDK Platform | 35 | `~/android-sdk/platforms/android-35` | `sdkmanager --list_installed` |
| Android Build-Tools | 35.0.0 | `~/android-sdk/build-tools/35.0.0` | `aapt2 version` → `2.19-11948202` |
| Platform-Tools (adb) | 37.0.3 | `~/android-sdk/platform-tools` | `adb version` |
| GTK/X11 для Unity | Ubuntu noble | `~/libs` | `ldd ~/tools/unity/Editor/Unity` — 0 `not found` |

Переменные окружения (`JAVA_HOME`, `ANDROID_HOME`, `UNITY_EDITOR`, `LD_LIBRARY_PATH`)
лежат в `~/unity-android-env.sh`.

## Главный блокер: лицензия Unity

Unity запускается и печатает версию, но в batch-режиме останавливается:

```
No valid Unity Editor license found. Please activate your license.
[Licensing::Client] Error: Code 404 ... Found 0 entitlement groups and 0 free entitlements
```

Активация требует аккаунта Unity — без него редактор не собирает проекты. Есть два пути.

### Вариант A. Ручная активация (Personal, бесплатно)

1. Сгенерировать файл активации:

   ```bash
   source ~/unity-android-env.sh
   cd ~ && "$UNITY_EDITOR" -batchmode -nographics -createManualActivationFile -logFile -
   # появится Unity_v6000.3.29f1.alf
   ```

2. Загрузить `.alf` на <https://license.unity3d.com/manual>, выбрать **Personal**,
   скачать `Unity_v6000.x.ulf`.
3. Положить файл в `~/.local/share/unity3d/Unity/Unity_lic.ulf`.
4. Проверить:

   ```bash
   "$UNITY_EDITOR" -batchmode -quit -nographics -logFile - -createProject /tmp/unity-check
   ```

### Вариант B. Unity Hub

Hub требует графический вход в аккаунт, в этой headless-VM это невозможно.
Активировать можно на своей машине и перенести `Unity_lic.ulf` сюда.

## Ограничения этого окружения

- **Ничего из установленного не переживает пересборку VM**: `/workspace` — рабочая
  папка с git, `$HOME` — временный. После восстановления запустите
  `bash tools/setup-unity-android.sh` ещё раз (~10 минут, ~20 ГБ).
- Unity-редактор здесь headless: окно не открыть, только `-batchmode`.
- Модуль Android Build Support взят из macOS-пакета Unity: для этой версии Unity
  больше не выкладывает отдельный Linux-архив модуля по прежним адресам. Внутри —
  управляемые сборки, Gradle и шаблоны, они не зависят от ОС, но сборку APK
  end-to-end проверить нельзя, пока не активирована лицензия.
- NDK не установлен. Для Android-сборки на IL2CPP Unity попросит NDK — поставьте
  версию, которую назовёт редактор:
  `~/android-sdk/cmdline-tools/latest/bin/sdkmanager "ndk;<версия>"`.
- Эмулятора Android нет: APK можно только собрать, не запустить здесь.

## Полезные команды

```bash
source ~/unity-android-env.sh

# сборка Unity-проекта в APK (после активации лицензии)
"$UNITY_EDITOR" -batchmode -quit -nographics -logFile - \
  -projectPath game/unity -buildTarget Android \
  -executeMethod BuildScript.BuildApk

# проверить APK
~/android-sdk/build-tools/35.0.0/aapt2 dump badging build/app.apk | head
~/android-sdk/build-tools/35.0.0/apksigner verify --print-certs build/app.apk
```
