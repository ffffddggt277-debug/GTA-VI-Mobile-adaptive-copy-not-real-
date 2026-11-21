# Промпт для GitHub Copilot (coding agent)

Скопируй текст ниже в новый Issue и назначь его на Copilot (Assignees → Copilot).
Агент откроет PR в `main`.

---

## Задача

В репозитории лежит папка `models/` со 125 моделями в формате glTF/GLB + PBR-текстуры 1K.
Нужно отрендерить превью каждой модели и превью по категориям, затем закоммитить их.

1. Установи Blender 4.2 LTS (headless) — скрипт `tools/setup_blender.sh` качает его в `tools/`
   и подтягивает недостающие библиотеки без root.
2. Отрендерь все модели:
   ```bash
   LD_LIBRARY_PATH=$PWD/tools/sysroot/usr/lib/x86_64-linux-gnu:$PWD/tools/blender-4.2.3-linux-x64/lib \
   tools/blender-4.2.3-linux-x64/blender -b --python tools/render_blender.py -- renders/single
   ```
   Скрипт сам обходит `models/**/*.gltf` и `models/**/*.glb`, ставит камеру, свет,
   пол и рендерит в Cycles (8 сэмплов, 480×360). Имя файла: `<источник>__<модель>__<файл>.png`.
3. Собери контактные листы по категориям: `python3 tools/make_sheets.py`
   (результат — `renders/sheets/*.jpg`, по одному листу на категорию, с подписями и лицензиями).
4. Обнови каталог: `python3 tools/write_sources.py` → `models/SOURCES.md`.
5. Закоммить `renders/models/*.jpg`, `renders/sheets/*.jpg` и `models/SOURCES.md`.
   Сырые PNG из `renders/single/` не коммитить (они в `.gitignore`).

## Требования

- Python 3.12 + Pillow, Blender 4.2 LTS.
- Все пути — относительно корня репозитория; папка моделей — `models/`.
- Не менять и не удалять сами модели, лицензии и авторов в `models/SOURCES.md`.
- Сообщение коммита: `docs(renders): add model previews and category sheets`.

## Структура

```
models/
  polyhaven/<asset_id>/...     118 фотореалистичных моделей, glTF + 1K PBR, CC0
  khronos/<name>/<name>.glb    транспорт и персонажи с ригом, CC-BY 4.0
  generated/money_stack.glb    процедурные деньги
  manifest.json                каталог
  SOURCES.md                   лицензии и ссылки
tools/
  setup_blender.sh             установка Blender
  render_blender.py            рендер превью
  make_sheets.py               контактные листы по категориям
  write_sources.py             каталог лицензий
  fetch_polyhaven.py           повторная загрузка моделей с Poly Haven API
  make_money.py                генерация модели денег
```
