#!/usr/bin/env python3
"""Нарезает 4К-карту на тайлы и делает облегчённые версии для быстрой загрузки.

Запуск:  python3 game/tools/make_tiles.py   (после render_map.cjs)
Результат: game/map/4k/{overview.webp, city_map_4k.webp, tiles/*.webp, manifest.json}

Смысл: вьюер сначала показывает overview (1600 px), а 4К-тайлы 1024×1024
подгружает только те, что попали в экран. Поэтому карта 4К не лагает
и не тянет мегабайты на телефоне.
"""

import json
import math
import os

from PIL import Image

ROOT = os.path.abspath(os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", ".."))
SRC = os.path.join(ROOT, "game", "map", "city_map.png")
OUT = os.path.join(ROOT, "game", "map", "4k")
TILE = 1024


def main():
    os.makedirs(os.path.join(OUT, "tiles"), exist_ok=True)
    im = Image.open(SRC).convert("RGB")
    w, h = im.size

    # полная 4К-версия одним файлом (для скачивания и GitHub)
    im.save(os.path.join(OUT, "city_map_4k.webp"), "WEBP", quality=88, method=6)

    # overview: хватает для общего вида, грузится мгновенно
    ov_w = 1600
    ov_h = round(h * ov_w / w)
    im.resize((ov_w, ov_h), Image.LANCZOS).save(
        os.path.join(OUT, "overview.webp"), "WEBP", quality=82, method=6)

    sea = im.getpixel((4, 4))
    cols, rows = math.ceil(w / TILE), math.ceil(h / TILE)
    tiles = []
    for r in range(rows):
        for c in range(cols):
            box = (c * TILE, r * TILE, min(w, (c + 1) * TILE), min(h, (r + 1) * TILE))
            crop = im.crop(box)
            if crop.size != (TILE, TILE):
                padded = Image.new("RGB", (TILE, TILE), sea)
                padded.paste(crop, (0, 0))
                crop = padded
            name = f"tiles/r{r}_c{c}.webp"
            crop.save(os.path.join(OUT, name), "WEBP", quality=88, method=6)
            tiles.append({"r": r, "c": c, "src": name})

    manifest = {
        "map": "Marisol",
        "width": w,
        "height": h,
        "tileSize": TILE,
        "cols": cols,
        "rows": rows,
        "overview": "overview.webp",
        "overviewWidth": ov_w,
        "full": "city_map_4k.webp",
        "tiles": tiles,
        "lod": {"overviewBelowScale": 0.5, "tilesAboveScale": 0.5},
        "source": "../city_map.svg",
        "world": {
            "baseWidth": 2400,
            "baseHeight": 1700,
            "metersPerPixel": 5,
            "worldX": "(px - 1200) * 5",
            "worldZ": "(py - 850) * 5",
        },
    }
    with open(os.path.join(OUT, "manifest.json"), "w") as f:
        json.dump(manifest, f, ensure_ascii=False, indent=1)

    total = sum(os.path.getsize(os.path.join(OUT, t["src"])) for t in tiles)
    print(f"4К: {w}×{h}, тайлов {len(tiles)} ({total / 1048576:.1f} МБ), "
          f"overview {ov_w}px, {os.path.getsize(os.path.join(OUT, 'overview.webp')) // 1024} КБ")


if __name__ == "__main__":
    main()
