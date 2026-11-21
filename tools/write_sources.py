#!/usr/bin/env python3
"""Write assets/SOURCES.md: every downloaded model, its category, license and source URL."""
import json
import os

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
ASSETS = os.path.join(ROOT, "models")

KHR = {
    "CarConcept": ("Транспорт", "CC-BY 4.0 (Darmstadt Graphics Group / Khronos)"),
    "ToyCar": ("Транспорт", "CC-BY 4.0 (Khronos glTF Sample Assets)"),
    "CesiumMilkTruck": ("Транспорт", "CC-BY 4.0 (Cesium)"),
    "CesiumMan": ("Персонаж с ригом и анимацией", "CC-BY 4.0 (Cesium)"),
    "RiggedFigure": ("Персонаж с ригом", "CC-BY 4.0 (Cesium)"),
    "Fox": ("Существо с ригом и анимацией", "CC-BY 4.0 (tomkranis / AsoboStudio)"),
}

CAT_RU = {
    "buildings": "Здания / фасады / двери / ворота", "structures": "Сооружения и улица",
    "furniture": "Мебель", "office": "Офис", "appliances": "Бытовая техника",
    "electronics": "Электроника", "decorative": "Декор", "lighting": "Освещение",
    "containers": "Контейнеры", "tools": "Инструменты", "food": "Еда", "dishes": "Посуда",
    "props": "Реквизит", "firearms": "Огнестрельное оружие", "weapons": "Оружие",
    "rigged": "С ригом", "creature": "Существа",
}


def main():
    manifest = json.load(open(os.path.join(ASSETS, "manifest.json")))
    lines = [
        "# Каталог 3D-ассетов",
        "",
        "Все модели скачаны из открытых источников и лежат локально в `assets/`.",
        "Формат: **glTF 2.0 / GLB + PBR-текстуры 1K** — импортируется в Blender, Unity, Unreal, Godot, three.js без конвертации.",
        "",
        f"Всего моделей: **{len(manifest) + len(KHR) + 1}**",
        "",
        "## Источники и лицензии",
        "",
        "| Источник | Лицензия | Что покрывает |",
        "|---|---|---|",
        "| [Poly Haven](https://polyhaven.com/models) | CC0 1.0 (public domain) | 118 фотореалистичных моделей: мебель, интерьеры, техника, инструменты, еда, контейнеры, фасады, двери, оружие |",
        "| [Khronos glTF Sample Assets](https://github.com/KhronosGroup/glTF-Sample-Assets) | CC-BY 4.0 | автомобили (CarConcept, ToyCar, CesiumMilkTruck), риггованные персонажи (CesiumMan, RiggedFigure), лиса с анимацией |",
        "| Сгенерировано скриптом `tools/make_money.py` | — | пачки банкнот (деньги), без воспроизведения реального дизайна валюты |",
        "",
        "CC0 — можно использовать в коммерческой игре без указания автора (указание приветствуется).",
        "CC-BY 4.0 — требуется указать автора в титрах; текст лицензии лежит рядом с моделью в `LICENSE-README.md`.",
        "",
        "## Транспорт, персонажи, оружие",
        "",
        "| Модель | Категория | Лицензия |",
        "|---|---|---|",
    ]
    for name, (cat, lic) in KHR.items():
        lines.append(f"| `assets/khronos/{name}/{name}.glb` | {cat} | {lic} |")
    lines += [
        "| `assets/generated/money_stack.glb` | Деньги (процедурные) | генерируется локально |",
        "",
        "Все персонажи и лиса — со **скелетом и анимациями** (skinning + animation clips).",
        "",
        "## Poly Haven (CC0)",
        "",
        "| Модель | Категории | Размер | Файл |",
        "|---|---|---|---|",
    ]
    for aid in sorted(manifest):
        e = manifest[aid]
        if e.get("error"):
            continue
        cats = ", ".join(CAT_RU.get(c, c) for c in e["categories"])
        lines.append(f"| [{e['name']}]({e['source']}) | {cats} | {e.get('size_mb', '?')} МБ | `{e['file']}` |")
    lines += [
        "",
        "## Важные ограничения",
        "",
        "- В бесплатных библиотеках **нет фот��реалистичных людей, полицейских и целых зданий**. Персонажи выше — единственные корректно лицензированные риггованные модели с прямой загрузкой; фотореалистичных людей/машин/полиции нет.",
        "- Модели Poly Haven — это **модульные куски архитектуры** (фасады, двери, ворота, заборы), а не готовые дома: их нужно собирать в уровне.",
        "- Готовые фотореалистичные персонажи/машины/оружие с ригом обычно лежат на Sketchfab/Mixamo — там нужен аккаунт и ручное скачивание, автоматически их забрать нельзя.",
        "- Отдельных подвижных деталей (дверцы/капот машины, открывающиеся двери домов) в этих наборах нет: у Kenney/Quaternius детали разделены, но они low-poly. Для подвижных частей модель нужно разрезать в Blender (`P` → Separate) и анимировать по костям/объектам.",
        "",
        "## Как перекачать / перерендерить",
        "",
        "```bash",
        "python3 tools/fetch_polyhaven.py      # скачать модели (Poly Haven API)",
        "python3 tools/make_sheets.py          # пересобрать контактные листы",
        "python3 tools/write_sources.py        # обновить этот файл",
        "```",
        "",
    ]
    out = os.path.join(ASSETS, "SOURCES.md")
    open(out, "w").write("\n".join(lines))
    print("WROTE", out, len(lines), "lines")


if __name__ == "__main__":
    main()
