#!/usr/bin/env python3
"""Write assets/SOURCES.md: every downloaded model, its category, license and source URL."""
import glob
import json
import os

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
ASSETS = os.path.join(ROOT, "models")

KHR = {
    "CarConcept": ("Транспорт", "CC-BY 4.0 (Darmstadt Graphics Group / Khronos)"),
    "CesiumMilkTruck": ("Транспорт", "CC-BY 4.0 (Cesium)"),
    "RiggedFigure": ("Персонаж с ригом", "CC-BY 4.0 (Cesium)"),
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
    sf_path = os.path.join(ASSETS, "sketchfab_manifest.json")
    sketchfab = json.load(open(sf_path)) if os.path.exists(sf_path) else {}
    houses = sorted(glob.glob(os.path.join(ASSETS, "generated", "houses", "*.glb")))
    total = len([1 for e in manifest.values() if not e.get("error")]) + len(KHR) + 1 + len(sketchfab) + len(houses)
    lines = [
        "# Каталог 3D-ассетов",
        "",
        "Все модели скачаны из открытых источников и лежат локально в `models/`.",
        "Формат: **glTF 2.0 / GLB + PBR-текстуры 1K** — импортируется в Blender, Unity, Unreal, Godot, three.js без конвертации.",
        "",
        f"Всего моделей: **{total}**",
        "",
        "## Источники и лицензии",
        "",
        "| Источник | Лицензия | Что покрывает |",
        "|---|---|---|",
        f"| [Poly Haven](https://polyhaven.com/models) | CC0 1.0 (public domain) | {len([1 for e in manifest.values() if not e.get('error')])} фотореалистичных моделей: мебель, интерьеры, техника, инструменты, еда, контейнеры, фасады, двери, оружие |",
        f"| [Sketchfab](https://sketchfab.com) | CC-BY 4.0 / CC-BY-SA 4.0 | {len(sketchfab)} моделей с ригом и техники; автор обязателен к указанию (см. таблицу ниже) |",
        f"| Сгенерировано `tools/make_houses.py` | — | {len(houses)} домов: отдельные двери с анимацией, прозрачные стёкла (KHR_materials_transmission) |",
        "| [Khronos glTF Sample Assets](https://github.com/KhronosGroup/glTF-Sample-Assets) | CC-BY 4.0 | CarConcept, CesiumMilkTruck, RiggedFigure. ToyCar / CesiumMan / Fox удалены как нереалистичные |",
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
        lines.append(f"| `models/khronos/{name}/{name}.glb` | {cat} | {lic} |")
    lines += [
        "| `models/generated/money_stack.glb` | Деньги (процедурные) | генерируется локально |",
        "",
        "RiggedFigure — со **скелетом и анимациями**; люди со Sketchfab — с ригом (см. таблицу).",
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
    if houses:
        lines += [
            "",
            "## Дома (сгенерированы локально)",
            "",
            "Каждый GLB содержит статичные стены/крышу, **отдельные объекты дверей с анимацией открывания** (`Door_FrontAction`) и **прозрачные стёкла** (`KHR_materials_transmission`, alpha BLEND).",
            "",
            "| Файл | Размер |",
            "|---|---|",
        ]
        for p in houses:
            lines.append(f"| `{os.path.relpath(p, ROOT)}` | {os.path.getsize(p) // 1024} КБ |")

    if sketchfab:
        lines += [
            "",
            "## Sketchfab (CC-BY / CC-BY-SA)",
            "",
            "Для этих моделей **обязательно указать автора** в титрах — лицензия CC-BY требует атрибуции.",
            "",
            "| Модель | Категория | Автор | Лицензия | Риг | Ссылка |",
            "|---|---|---|---|---|---|",
        ]
        for uid in sorted(sketchfab):
            e = sketchfab[uid]
            lines.append(
                f"| {e.get('name', uid)} | {', '.join(e.get('categories') or [])} | "
                f"{e.get('author') or '?'} | {e.get('license') or '?'} | "
                f"{'да' if e.get('rigged') else 'нет'} | [{uid[:8]}]({e.get('source')}) |"
            )

    lines += [
        "",
        "## Важные ограничения",
        "",
        "- В бесплатных библиотеках **нет фот��реалистичных людей, полицейских и целых зданий**. Персонажи и техника — модели с прямой загрузкой; фотореалистичных людей/машин/полиции нет.",
        "- Poly Haven даёт только **модульные куски архитектуры** (фасады, двери, ворота, заборы) — целые дома собраны процедурно в `tools/make_houses.py`.",
        "- Люди и техника со Sketchfab — **не фотосканы**: это авторские модели под CC-BY, стиль и качество у них разные, атрибуция обязательна.",
        "- Модели с лицензиями NC/ND со Sketchfab скрипт **пропускает**, чтобы проект можно было использовать коммерчески.",
        "- У домов двери — отдельные объекты с анимацией, стёкла прозрачные; у остальных моделей подвижных деталей (капот/дверцы машин) нет — их нужно разрезать в Blender (`P` → Separate) и анимировать.",
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
