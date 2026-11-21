# Задание для GitHub Copilot (coding agent)

Этот файл описывает работу, которую должен выполнить Copilot-агент в этом репозитории.
Чтобы агент её взял, нужно: **Settings → Copilot → Coding agent** включить, затем создать
Issue с текстом ниже и назначить его (Assignees → Copilot). Агент откроет PR в `main`.

## Что нужно сделать

1. Перерендерить превью всех моделей из папки `models/` и обновить контактные листы.
   - Скрипт: `tools/render_blender.py` (Blender 4.2, Cycles CPU).
   - Результат: `renders/models/<model>.jpg` (по одному превью на модель) и
     `renders/sheets/<nn>_<category>.jpg` (листы по категориям), сборка — `tools/make_sheets.py`.
2. Обновить каталог: `python3 tools/write_sources.py` → `models/SOURCES.md`.
3. Ничего не удалять из `models/`, лицензии и авторы должны остаться указанными.

## Требования

- Python 3.12, Pillow; Blender 4.2 LTS (headless): `blender -b --python tools/render_blender.py -- renders/single`.
- Считать пути относительно корня репозитория, папка моделей — `models/` (было `assets/`).
- Коммитить только `renders/*.jpg` и `models/SOURCES.md`: сырые PNG (`renders/single/`) в `.gitignore`.
- Сообщение коммита: `docs(renders): refresh model previews and category sheets`.

## Структура репозитория

```
models/
  polyhaven/<asset_id>/...     118 фотореалистичных моделей, glTF + 1K PBR, CC0
  khronos/<name>/<name>.glb    транспорт и персонажи с ригом, CC-BY 4.0
  generated/money_stack.glb    процедурные деньги
  manifest.json                полный каталог
  SOURCES.md                   лицензии и ссылки
renders/
  models/*.jpg                 превью каждой модели
  sheets/*.jpg                 листы по категориям
tools/                         скрипты загрузки, рендера, сборки листов
```
