#!/usr/bin/env python3
"""Compose per-category contact sheets from renders/single/*.png."""
import json
import os
import re

from PIL import Image, ImageDraw, ImageFont

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SINGLE = os.path.join(ROOT, "renders", "single")
SHEETS = os.path.join(ROOT, "renders", "sheets")
os.makedirs(SHEETS, exist_ok=True)

RU = {
    "buildings": "Здания: фасады, двери, ворота", "structures": "Сооружения и улица",
    "furniture": "Мебель", "office": "Офис", "appliances": "Бытовая техника",
    "electronics": "Электроника", "decorative": "Декор", "lighting": "Освещение",
    "containers": "Контейнеры и ящики", "tools": "Инструменты", "food": "Еда",
    "dishes": "Посуда", "props": "Реквизит", "firearms": "Огнестрельное оружие",
    "weapons": "Оружие", "rigged": "Модели с ригом (кости)", "creature": "Существа",
    "vehicles": "Транспорт", "characters": "Персонажи", "money": "Деньги",
}
ORDER = list(RU)

KHRONOS = {
    "CarConcept": "vehicles", "ToyCar": "vehicles", "CesiumMilkTruck": "vehicles",
    "CesiumMan": "characters", "RiggedFigure": "characters", "Fox": "creature",
}
LICENSE_TAG = {"polyhaven": "CC0 · Poly Haven", "khronos": "CC-BY 4.0 · Khronos", "generated": "procedural"}
NICE = {
    "CarConcept": "CarConcept (авто)", "ToyCar": "ToyCar", "CesiumMilkTruck": "CesiumMilkTruck",
    "CesiumMan": "CesiumMan (риг+анимация)", "RiggedFigure": "RiggedFigure (риг)",
    "Fox": "Fox (риг+анимация)", "money_stack": "Money stack (процедурные)",
}


def font(size):
    for p in (os.path.join(ROOT, "tools", "fonts", "ui.ttf"),
              "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf"):
        if os.path.exists(p):
            return ImageFont.truetype(p, size)
    return ImageFont.load_default()


def parse(name):
    stem = name[:-4]
    parts = stem.split("__")
    src = parts[0]
    if src == "polyhaven":
        aid = parts[1]
        return src, aid, KHRONOS.get(aid)
    if src == "khronos":
        aid = parts[1]
        return src, aid, KHRONOS.get(aid, "characters")
    if "money" in stem:
        return src, parts[-1], "money"
    return src, parts[-1], None


def main():
    manifest = {}
    mp = os.path.join(ROOT, "models", "manifest.json")
    if os.path.exists(mp):
        manifest = json.load(open(mp))

    groups = {}
    for f in sorted(os.listdir(SINGLE)):
        if not f.endswith(".png"):
            continue
        src, aid, cat = parse(f)
        cats = manifest.get(aid, {}).get("categories") or []
        if not cat:
            cat = next((c for c in ORDER if c in cats), cats[0] if cats else "props")
        if cat not in ORDER:
            cat = "props"
        groups.setdefault(cat, []).append((aid, src, os.path.join(SINGLE, f)))

    TW, TH = 400, 300
    PAD, HEAD, LABEL, FOOT = 18, 76, 34, 40
    COLS = 4
    made = []
    for cat in ORDER:
        if cat not in groups:
            continue
        items = groups[cat]
        rows = (len(items) + COLS - 1) // COLS
        W = PAD * 2 + COLS * TW + (COLS - 1) * PAD
        H = HEAD + rows * (TH + LABEL + PAD) + PAD + FOOT
        sheet = Image.new("RGB", (W, H), (247, 247, 249))
        d = ImageDraw.Draw(sheet)
        d.rectangle([0, 0, W, HEAD - 22], fill=(24, 24, 28))
        d.text((PAD, 16), f"{RU[cat]}", font=font(34), fill=(255, 255, 255))
        d.text((W - PAD - 210, 26), f"{len(items)} моделей", font=font(22), fill=(190, 190, 200))
        for i, (aid, src, path) in enumerate(items):
            r, c = divmod(i, COLS)
            x = PAD + c * (TW + PAD)
            y = HEAD + r * (TH + LABEL + PAD)
            d.rectangle([x - 1, y - 1, x + TW + 1, y + TH + 1], fill=(255, 255, 255), outline=(214, 214, 220))
            try:
                im = Image.open(path).convert("RGB").resize((TW, TH), Image.LANCZOS)
                sheet.paste(im, (x, y))
            except Exception as e:
                d.text((x + 10, y + 10), f"err {e}", font=font(14), fill=(200, 0, 0))
            label = NICE.get(aid, aid)
            if len(label) > 34:
                label = label[:33] + "…"
            d.text((x + 2, y + TH + 7), label, font=font(17), fill=(30, 30, 34))
            d.text((x + 2, y + TH + 22), LICENSE_TAG.get(src, src), font=font(13), fill=(120, 120, 130))
        d.text((PAD, H - FOOT + 8),
               "Все модели скачаны из открытых источников (Poly Haven CC0, Khronos glTF CC-BY 4.0). "
               "Рендер: Blender Cycles, 480×360.",
               font=font(15), fill=(110, 110, 120))
        out = os.path.join(SHEETS, f"{ORDER.index(cat):02d}_{cat}.jpg")
        sheet.save(out, quality=88)
        made.append(out)
        print("SHEET", out, len(items))
    print("TOTAL", len(made))


if __name__ == "__main__":
    main()
