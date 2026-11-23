#!/usr/bin/env python3
"""Генерирует 24 обычных жилых дома с PBR-текстурами (glTF 2.0 / GLB, метры).

Запуск:  python3 game/tools/make_houses.py
Результат:
    models/generated/houses/house_00.glb … house_23.glb
    models/generated/houses/textures/*.png

Каждый дом — низкополигональная модель с UV-развёрткой и текстурами:
штукатурка / кирпич / сайдинг / дерево, черепица / металл, двери, стекло,
бетон, вода в бассейне. Начало координат — центр дома на уровне земли,
фасад смотрит в +Z. Дверь — отдельный узел `Door_Front` с пивотом на петле.
"""

import json
import math
import os
import struct

import numpy as np
from PIL import Image, ImageDraw

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.abspath(os.path.join(HERE, "..", ".."))
OUT = os.path.join(ROOT, "models", "generated", "houses")
TEX = os.path.join(OUT, "textures")
SIZE = 256


# --------------------------------------------------------------------------- #
# текстуры
# --------------------------------------------------------------------------- #

def _noise(rng, base, specs, grain=2.5):
    yy, xx = np.mgrid[0:SIZE, 0:SIZE]
    arr = np.full((SIZE, SIZE), float(base), dtype=np.float32)
    for fx, fy, amp, ph in specs:
        arr += amp * np.sin(2 * np.pi * (fx * xx + fy * yy) / SIZE + ph)
    arr += rng.normal(0, grain, (SIZE, SIZE))
    return arr


def _save(arr, name):
    img = Image.fromarray(np.clip(arr, 0, 255).astype(np.uint8))
    img.convert("RGB").save(os.path.join(TEX, name + ".png"), optimize=True)


def _pattern(rng, base, draw_fn, grain=3.0):
    img = Image.new("L", (SIZE, SIZE), base)
    draw_fn(ImageDraw.Draw(img))
    arr = np.asarray(img, dtype=np.float32) + rng.normal(0, grain, (SIZE, SIZE))
    return np.clip(arr, 0, 255)


def make_textures():
    os.makedirs(TEX, exist_ok=True)
    rng = np.random.default_rng(7)

    _save(_noise(rng, 226, [(1, 2, 5, 0.4), (3, 1, 3.5, 1.7),
                            (5, 4, 2.5, 0.2), (9, 7, 1.8, 2.4)], 2.0), "stucco")

    def brick(d):
        for row in range(0, SIZE, 16):
            d.line([(0, row), (SIZE, row)], fill=168, width=2)
            off = 0 if (row // 16) % 2 == 0 else 16
            for x in range(-32, SIZE + 32, 32):
                d.line([(x + off, row), (x + off, min(SIZE, row + 16))], fill=176, width=2)
    _save(_pattern(rng, 200, brick, 4.5), "brick")

    def siding(d):
        for y in range(0, SIZE, 14):
            d.line([(0, y), (SIZE, y)], fill=176, width=2)
            d.line([(0, y + 1), (SIZE, y + 1)], fill=236, width=1)
        for _ in range(5):
            x = int(rng.integers(0, SIZE))
            y = int(rng.integers(0, SIZE))
            d.line([(x, y), (x, y + 14)], fill=190, width=1)
    _save(_pattern(rng, 214, siding, 2.4), "siding")

    def shingle(d):
        for row in range(0, SIZE, 18):
            d.rectangle([0, row, SIZE, row + 17], fill=190 if (row // 18) % 2 else 198)
            d.line([(0, row + 16), (SIZE, row + 16)], fill=150, width=2)
            off = 0 if (row // 18) % 2 == 0 else 14
            for x in range(-28, SIZE + 28, 28):
                d.line([(x + off, row), (x + off, row + 17)], fill=168, width=1)
                d.arc([x + off - 12, row + 8, x + off + 12, row + 26], 0, 180, fill=140, width=2)
    _save(_pattern(rng, 195, shingle, 2.6), "shingle")

    def tiles(d):
        for row in range(0, SIZE, 22):
            d.line([(0, row + 20), (SIZE, row + 20)], fill=154, width=2)
            off = 0 if (row // 22) % 2 == 0 else 12
            for x in range(-24, SIZE + 24, 24):
                d.arc([x + off - 12, row - 2, x + off + 12, row + 22], 0, 180,
                      fill=170, width=2)
                d.line([(x + off, row + 2), (x + off, row + 19)], fill=184, width=1)
    _save(_pattern(rng, 206, tiles, 3.0), "tile")

    _save(_noise(rng, 213, [(1, 1, 4, 0.0), (2, 3, 3, 1.2), (7, 5, 2, 0.7)], 3.2),
          "concrete")

    def wood(d):
        for x in range(0, SIZE, 18):
            d.line([(x, 0), (x, SIZE)], fill=178, width=1)
            d.line([(x + 1, 0), (x + 1, SIZE)], fill=206, width=1)
        for _ in range(4):
            x = int(rng.integers(0, SIZE))
            y = int(rng.integers(0, SIZE))
            d.ellipse([x - 4, y - 7, x + 4, y + 7], outline=155, width=2)
    _save(_pattern(rng, 192, wood, 2.0), "wood")

    def door(d):
        for x in range(0, SIZE, 32):
            d.line([(x, 0), (x, SIZE)], fill=140, width=2)
        d.rectangle([18, 24, SIZE - 18, 96], outline=178, width=3)
        d.rectangle([18, 144, SIZE - 18, 216], outline=178, width=3)
    _save(_pattern(rng, 172, door, 2.2), "door")

    trim = _pattern(rng, 232, lambda d: [d.line([(0, y), (SIZE, y)], fill=214, width=1)
                                        for y in range(0, SIZE, 9)], 1.4)
    _save(trim, "trim")

    yy, xx = np.mgrid[0:SIZE, 0:SIZE]
    glass = 232 + 9 * np.sin(2 * np.pi * (xx + yy) / SIZE) + \
        5 * np.sin(2 * np.pi * (2 * xx - yy) / SIZE)
    for k in range(5):
        x0 = int(rng.integers(0, SIZE))
        glass += np.where((xx > x0) & (xx < x0 + 10), 12, 0)
    _save(glass + rng.normal(0, 1.4, (SIZE, SIZE)), "glass")

    water = 196 + 12 * np.sin(2 * np.pi * (xx * 3) / SIZE + xx / 18) + \
        8 * np.sin(2 * np.pi * (yy * 2) / SIZE)
    _save(water + rng.normal(0, 2.0, (SIZE, SIZE)), "water")

    def metal(d):
        for x in range(0, SIZE, 32):
            d.line([(x, 0), (x, SIZE)], fill=158, width=3)
            d.line([(x + 3, 0), (x + 3, SIZE)], fill=216, width=1)
        for y in range(0, SIZE, 64):
            d.line([(0, y), (SIZE, y)], fill=180, width=1)
    _save(_pattern(rng, 198, metal, 2.6), "metal")

    return sorted(f[:-4] for f in os.listdir(TEX) if f.endswith(".png"))


# --------------------------------------------------------------------------- #
# геометрия
# --------------------------------------------------------------------------- #

class Builder:
    def __init__(self):
        self.parts = []          # (name, verts, normals, uvs, indices, material, pivot)
        self.materials = []
        self.mat_index = {}
        self.mat_tile = []
        self.textures = []
        self.tex_index = {}

    def material(self, name, color, rough=0.8, metal=0.0, alpha=1.0,
                 tex=None, tile=2.0):
        key = (name, tuple(color), rough, metal, alpha, tex)
        if key in self.mat_index:
            return self.mat_index[key]
        m = {
            "name": name,
            "pbrMetallicRoughness": {
                "baseColorFactor": [color[0], color[1], color[2], alpha],
                "metallicFactor": metal,
                "roughnessFactor": rough,
            },
            "doubleSided": True,
        }
        if alpha < 1.0:
            m["alphaMode"] = "BLEND"
        if tex:
            if tex not in self.tex_index:
                self.tex_index[tex] = len(self.textures)
                self.textures.append(tex)
            m["pbrMetallicRoughness"]["baseColorTexture"] = {
                "index": self.tex_index[tex], "texCoord": 0
            }
        self.materials.append(m)
        i = len(self.materials) - 1
        self.mat_index[key] = i
        self.mat_tile.append(tile)
        return i

    def add(self, name, verts, normals, uvs, indices, mat, pivot=None):
        self.parts.append((name, verts, normals, uvs, indices, mat, pivot))

    def _emit(self, name, v, faces, mat, pivot=None):
        tile = self.mat_tile[mat]
        verts, normals, uvs, indices = [], [], [], []
        for quad, nn in faces:
            uniq = []
            for idx in quad:
                if idx not in uniq:
                    uniq.append(idx)
            qv = [v[i] for i in uniq]
            base = len(verts)
            for idx in uniq:
                verts.append(v[idx])
                normals.append(nn)
            du = max(1e-4, math.dist(qv[0], qv[1])) / tile
            dv = max(1e-4, math.dist(qv[0], qv[-1])) / tile
            if len(qv) == 4:
                uvs += [(0, 0), (du, 0), (du, dv), (0, dv)]
                indices += [base, base + 1, base + 2, base, base + 2, base + 3]
            else:
                uvs += [(0, 0), (du, 0), (du * 0.5, dv)]
                indices += [base, base + 1, base + 2]
        self.add(name, verts, normals, uvs, indices, mat, pivot)

    def box(self, name, cx, cy, cz, w, h, d, mat, rot=0.0, pivot=None):
        hw, hh, hd = w / 2.0, h / 2.0, d / 2.0
        c, s = math.cos(rot), math.sin(rot)

        def p(x, y, z):
            return (cx + x * c + z * s, cy + y, cz - x * s + z * c)

        v = [
            p(-hw, -hh, -hd), p(hw, -hh, -hd), p(hw, hh, -hd), p(-hw, hh, -hd),
            p(-hw, -hh, hd), p(hw, -hh, hd), p(hw, hh, hd), p(-hw, hh, hd),
        ]

        def n(x, y, z):
            return (x * c + z * s, y, -x * s + z * c)

        faces = [
            ((0, 1, 2, 3), n(0, 0, -1)), ((5, 4, 7, 6), n(0, 0, 1)),
            ((4, 0, 3, 7), n(-1, 0, 0)), ((1, 5, 6, 2), n(1, 0, 0)),
            ((3, 2, 6, 7), n(0, 1, 0)), ((4, 5, 1, 0), n(0, -1, 0)),
        ]
        self._emit(name, v, faces, mat, pivot)

    def prism(self, name, cx, cy, cz, w, d, h, mat, ridge_axis="x", rot=0.0):
        c, s = math.cos(rot), math.sin(rot)

        def p(x, y, z):
            return (cx + x * c + z * s, cy + y, cz - x * s + z * c)

        hw, hd = w / 2.0, d / 2.0
        if ridge_axis == "x":
            v = [p(-hw, 0, -hd), p(hw, 0, -hd), p(hw, h, 0), p(-hw, h, 0),
                 p(-hw, 0, hd), p(hw, 0, hd)]
            quads = [
                ((1, 0, 3, 2), (0, 0.447, -0.894)),
                ((4, 5, 2, 3), (0, 0.447, 0.894)),
                ((0, 4, 3, 3), (-1, 0, 0)),
                ((5, 1, 2, 2), (1, 0, 0)),
            ]
        else:
            v = [p(-hw, 0, -hd), p(-hw, 0, hd), p(0, h, hd), p(0, h, -hd),
                 p(hw, 0, -hd), p(hw, 0, hd)]
            quads = [
                ((5, 2, 3, 4), (0.894, 0.447, 0)),
                ((0, 3, 2, 1), (-0.894, 0.447, 0)),
                ((1, 2, 5, 5), (0, 0, 1)),
                ((4, 3, 0, 0), (0, 0, -1)),
            ]
        faces = [(q, (nn[0] * c + nn[2] * s, nn[1], -nn[0] * s + nn[2] * c))
                 for q, nn in quads]
        self._emit(name, v, faces, mat)

    def hip(self, name, cx, cy, cz, w, d, h, mat, rot=0.0, overhang=0.5):
        c, s = math.cos(rot), math.sin(rot)
        hw, hd = w / 2.0 + overhang, d / 2.0 + overhang
        rw = max(0.0, hw - hd * 0.55)
        rl = max(0.0, hd * 0.45)

        def p(x, y, z):
            return (cx + x * c + z * s, cy + y, cz - x * s + z * c)

        v = [p(-hw, 0, -hd), p(hw, 0, -hd), p(hw, 0, hd), p(-hw, 0, hd),
             p(-rw, h, -rl), p(rw, h, -rl), p(rw, h, rl), p(-rw, h, rl)]
        faces = [
            ((0, 1, 5, 4), (0, 0.55, -0.835)),
            ((2, 3, 7, 6), (0, 0.55, 0.835)),
            ((1, 2, 6, 5), (0.835, 0.55, 0)),
            ((3, 0, 4, 7), (-0.835, 0.55, 0)),
            ((4, 5, 6, 7), (0, 1, 0)),
        ]
        out = []
        for q, nn in faces:
            out.append((q, (nn[0] * c + nn[2] * s, nn[1], -nn[0] * s + nn[2] * c)))
        self._emit(name, v, out, mat)

    def flat_roof(self, name, cx, y, cz, w, d, mat, parapet=0.45, rot=0.0):
        self.box(name + "_slab", cx, y + 0.12, cz, w + 0.7, 0.25, d + 0.7, mat, rot)
        if parapet <= 0:
            return
        t = 0.18
        self.box(name + "_p1", cx, y + 0.12 + parapet / 2, cz - d / 2, w + 0.4, parapet, t, mat, rot)
        self.box(name + "_p2", cx, y + 0.12 + parapet / 2, cz + d / 2, w + 0.4, parapet, t, mat, rot)
        self.box(name + "_p3", cx - w / 2, y + 0.12 + parapet / 2, cz, t, parapet, d + 0.4, mat, rot)
        self.box(name + "_p4", cx + w / 2, y + 0.12 + parapet / 2, cz, t, parapet, d + 0.4, mat, rot)


# --------------------------------------------------------------------------- #
# дом
# --------------------------------------------------------------------------- #

def make_house(style):
    b = Builder()
    w, d, h = style["w"], style["d"], style["h"]
    wall_rgb = tuple(min(1.0, c * 1.04) for c in style["wall"])
    roof_rgb = tuple(min(1.0, c * 1.30 + 0.05) for c in style["roof"])
    door_rgb = tuple(min(1.0, c * 1.16 + 0.02) for c in style["door"])
    wall = b.material("Wall", wall_rgb, 0.88,
                      tex=style.get("wall_tex", "stucco"), tile=2.4)
    trim = b.material("Trim", style["trim"], 0.72, tex="trim", tile=2.0)
    roof = b.material("Roof", roof_rgb, 0.85,
                      tex=style.get("roof_tex", "shingle"), tile=2.0)
    glass = b.material("Glass", (0.62, 0.78, 0.86), 0.04, 0.0, 0.34,
                       tex="glass", tile=1.6)
    door_m = b.material("Door", door_rgb, 0.66, tex="door", tile=1.0)
    conc = b.material("Concrete", (0.72, 0.72, 0.70), 0.92, tex="concrete", tile=3.0)
    wood = b.material("Porch", style.get("porch_color", style["trim"]), 0.78,
                      tex="wood", tile=1.8)
    pool_m = b.material("PoolWater", (0.30, 0.62, 0.72), 0.05, 0.05, 0.72,
                        tex="water", tile=3.0)

    base_h = 0.28
    b.box("Foundation", 0, base_h / 2, 0, w + 0.5, base_h, d + 0.5, conc)

    body_y = base_h + h / 2
    b.box("Body", 0, body_y, 0, w, h, d, wall)
    b.box("BaseTrim", 0, base_h + 0.22, 0, w + 0.08, 0.36, d + 0.08, trim)

    wing = style.get("wing")
    if wing:
        ww, wd, wh, ox, oz = wing
        b.box("Wing", ox, base_h + wh / 2, oz, ww, wh, wd, wall)
        b.box("WingBaseTrim", ox, base_h + 0.22, oz, ww + 0.08, 0.36, wd + 0.08, trim)

    rt = style["roof_type"]
    top = base_h + h
    if rt == "gable":
        b.prism("Roof", 0, top, 0, w + 1.0, d + 1.0, style["roof_h"], roof,
                ridge_axis=style.get("ridge", "x"))
        if wing:
            ww, wd, wh, ox, oz = wing
            b.prism("WingRoof", ox, base_h + wh, oz, ww + 0.8, wd + 0.8,
                    style["roof_h"] * 0.8, roof, ridge_axis="z")
    elif rt == "hip":
        b.hip("Roof", 0, top, 0, w, d, style["roof_h"], roof)
        if wing:
            ww, wd, wh, ox, oz = wing
            b.hip("WingRoof", ox, base_h + wh, oz, ww, wd, style["roof_h"] * 0.85, roof)
    else:
        b.flat_roof("Roof", 0, top, 0, w, d, roof, style.get("parapet", 0.45))
        if wing:
            ww, wd, wh, ox, oz = wing
            b.flat_roof("WingRoof", ox, base_h + wh, oz, ww, wd, roof, 0.35)

    if style.get("chimney"):
        b.box("Chimney", w * 0.26, top + style["roof_h"] * 0.55, -d * 0.2,
              1.0, style["roof_h"] * 1.25, 0.9, trim)

    win_w, win_h = 1.25, 1.35
    door_x = style.get("door_x", 0.0)
    cols = style.get("windows", 2)

    def window(name, x, y, z, lateral=False):
        if lateral:
            b.box(name + "_frame", x, y, z, 0.14, win_h + 0.26, win_w + 0.26, trim)
            b.box(name + "_glass", x + (0.09 if x > 0 else -0.09), y, z,
                  0.08, win_h, win_w, glass)
        else:
            b.box(name + "_frame", x, y, z, win_w + 0.26, win_h + 0.26, 0.14, trim)
            b.box(name + "_glass", x, y, z + (0.09 if z > 0 else -0.09),
                  win_w, win_h, 0.08, glass)

    for i in range(cols):
        x = -w / 2 + (i + 1) * w / (cols + 1)
        if abs(x - door_x) < 1.55:
            continue
        for level in range(style.get("floors", 1)):
            y = base_h + 1.45 + level * (h / max(1, style.get("floors", 1)))
            if y + win_h / 2 > base_h + h - 0.25:
                continue
            window(f"Window_F{i}_{level}", x, y, d / 2 + 0.03)
            window(f"Window_B{i}_{level}", x, y, -d / 2 - 0.03)
    for i in range(max(1, cols - 1)):
        z = -d / 2 + (i + 1) * d / (max(1, cols - 1) + 1)
        for level in range(style.get("floors", 1)):
            y = base_h + 1.45 + level * (h / max(1, style.get("floors", 1)))
            if y + win_h / 2 > base_h + h - 0.25:
                continue
            window(f"Window_L{i}_{level}", -w / 2 - 0.03, y, z, lateral=True)
            window(f"Window_R{i}_{level}", w / 2 + 0.03, y, z, lateral=True)

    dw, dh = 1.05, 2.15
    dz = d / 2 + 0.06
    b.box("DoorFrame", door_x, base_h + dh / 2, dz - 0.02, dw + 0.28, dh + 0.14, 0.16, trim)
    hinge_x = door_x - dw / 2
    b.box("Door_Front", hinge_x + dw / 2, base_h + dh / 2, dz, dw, dh, 0.10, door_m,
          pivot={"x": hinge_x, "y": base_h, "z": dz})
    b.box("DoorKnob", door_x + dw * 0.30, base_h + 1.02, dz + 0.10, 0.10, 0.10, 0.12,
          b.material("Brass", (0.62, 0.48, 0.20), 0.35, 0.8, tex="metal", tile=0.5))

    if style.get("porch"):
        pw, pd = style.get("porch_w", min(w - 1.0, 5.0)), style.get("porch_d", 2.0)
        px = door_x if abs(door_x) < w / 2 - pw / 2 else 0
        b.box("PorchSlab", px, base_h - 0.10, d / 2 + pd / 2, pw, 0.28, pd, conc)
        b.box("Step", px, base_h - 0.24, d / 2 + pd + 0.35, pw * 0.55, 0.18, 0.7, conc)
        for sx in (-pw / 2 + 0.28, pw / 2 - 0.28):
            b.box("PorchPost", px + sx, base_h + 1.25, d / 2 + pd - 0.3, 0.16, 2.5, 0.16, wood)
        b.box("PorchRoof", px, base_h + 2.62, d / 2 + pd / 2 - 0.1, pw + 0.4, 0.22,
              pd + 0.5, roof)

    if style.get("garage"):
        gw, gd = 4.2, 6.0
        gx = style.get("garage_x", w / 2 - gw / 2 - 0.4)
        gz = d / 2 + gd / 2 - 0.6
        gh = 2.9
        b.box("Garage", gx, base_h + gh / 2, gz, gw, gh, gd, wall)
        b.box("GarageTrim", gx, base_h + gh + 0.08, gz, gw + 0.4, 0.18, gd + 0.4, trim)
        b.box("GarageDoor", gx, base_h + 1.35, gz + gd / 2 + 0.04, gw - 0.45, 2.35,
              0.12, door_m)
        for i in range(5):
            b.box(f"GarageLine{i}", gx, base_h + 0.35 + i * 0.45, gz + gd / 2 + 0.12,
                  gw - 0.5, 0.05, 0.03, trim)
        b.box("Driveway", gx, base_h + 0.02, gz + gd / 2 + 2.2, gw - 0.2, 0.08, 4.2, conc)

    if style.get("pool"):
        pw, pd = 7.0, 3.6
        px, pz = -w / 2 - 5.0, 1.5
        b.box("PoolDeck", px, 0.06, pz, pw + 3.0, 0.12, pd + 3.0, conc)
        b.box("PoolWater", px, 0.10, pz, pw, 0.10, pd, pool_m)
        b.box("PoolEdgeN", px, 0.16, pz - pd / 2 - 0.12, pw + 0.4, 0.20, 0.24, trim)
        b.box("PoolEdgeS", px, 0.16, pz + pd / 2 + 0.12, pw + 0.4, 0.20, 0.24, trim)
        b.box("PoolEdgeW", px - pw / 2 - 0.12, 0.16, pz, 0.24, 0.20, pd + 0.4, trim)
        b.box("PoolEdgeE", px + pw / 2 + 0.12, 0.16, pz, 0.24, 0.20, pd + 0.4, trim)

    if style.get("fence"):
        fw, fd = style["fence"]
        # Передний забор — за крыльцом, а не поперёк него.
        porch_front = style.get("porch_d", 2.0) if style.get("porch") else 0.0
        fz = d / 2 + porch_front + 2.2
        bz = -fd / 2
        side_len = fz - bz
        b.box("FenceFrontL", (-fw / 2 + door_x - 1.2) / 2, 0.45, fz,
              max(0.2, door_x - 1.2 + fw / 2), 0.9, 0.12, trim)
        b.box("FenceFrontR", (door_x + 1.4 + fw / 2) / 2, 0.45, fz,
              max(0.2, fw / 2 - door_x - 1.4), 0.9, 0.12, trim)
        b.box("FenceBack", 0, 0.45, bz, fw, 0.9, 0.12, trim)
        b.box("FenceLeft", -fw / 2, 0.45, (fz + bz) / 2, 0.12, 0.9, side_len, trim)
        b.box("FenceRight", fw / 2, 0.45, (fz + bz) / 2, 0.12, 0.9, side_len, trim)

    return b


# --------------------------------------------------------------------------- #
# стили
# --------------------------------------------------------------------------- #

STYLES = [
    dict(key="house_00", name="Bungalow", name_ru="Бунгало", w=9.0, d=8.0, h=3.0,
         roof_type="hip", roof_h=1.7, wall=(0.90, 0.86, 0.76), trim=(0.97, 0.96, 0.92),
         roof=(0.62, 0.33, 0.22), door=(0.42, 0.28, 0.18), windows=3, porch=True,
         chimney=True, fence=(13, 12), wall_tex="stucco", roof_tex="tile"),
    dict(key="house_01", name="Ranch", name_ru="Ранчо", w=14.0, d=9.0, h=3.2,
         roof_type="gable", roof_h=1.8, wall=(0.78, 0.84, 0.72), trim=(0.97, 0.97, 0.94),
         roof=(0.42, 0.44, 0.47), door=(0.32, 0.22, 0.14), windows=4, garage=True,
         ridge="x", fence=(18, 14), wall_tex="siding", roof_tex="shingle"),
    dict(key="house_02", name="Two-storey", name_ru="Двухэтажный", w=9.5, d=9.0, h=6.2,
         roof_type="hip", roof_h=1.9, wall=(0.92, 0.86, 0.70), trim=(0.98, 0.97, 0.93),
         roof=(0.48, 0.30, 0.20), door=(0.36, 0.24, 0.15), windows=3, floors=2,
         porch=True, chimney=True, wall_tex="stucco", roof_tex="tile"),
    dict(key="house_03", name="Townhouse", name_ru="Таунхаус", w=7.0, d=12.0, h=6.4,
         roof_type="flat", roof_h=0, wall=(0.76, 0.44, 0.38), trim=(0.94, 0.93, 0.91),
         roof=(0.38, 0.39, 0.41), door=(0.24, 0.30, 0.22), windows=2, floors=2,
         parapet=0.6, door_x=1.6, wall_tex="brick", roof_tex="metal"),
    dict(key="house_04", name="Cottage", name_ru="Коттедж", w=8.0, d=7.0, h=3.1,
         roof_type="gable", roof_h=2.4, wall=(0.94, 0.92, 0.86), trim=(0.99, 0.99, 0.97),
         roof=(0.38, 0.47, 0.54), door=(0.30, 0.40, 0.50), windows=2, porch=True,
         chimney=True, ridge="x", fence=(11, 10), wall_tex="stucco", roof_tex="shingle"),
    dict(key="house_05", name="Duplex", name_ru="Дуплекс", w=15.0, d=8.5, h=3.3,
         roof_type="gable", roof_h=1.7, wall=(0.92, 0.86, 0.68), trim=(0.98, 0.97, 0.93),
         roof=(0.50, 0.32, 0.22), door=(0.40, 0.27, 0.16), windows=4, door_x=4.2,
         ridge="x", wall_tex="siding", roof_tex="shingle"),
    dict(key="house_06", name="Shotgun", name_ru="Узкий дом", w=6.2, d=14.0, h=3.5,
         roof_type="gable", roof_h=1.9, wall=(0.80, 0.74, 0.84), trim=(0.97, 0.96, 0.95),
         roof=(0.36, 0.37, 0.41), door=(0.26, 0.20, 0.17), windows=2, porch=True,
         porch_w=5.0, ridge="z", wall_tex="siding", roof_tex="metal"),
    dict(key="house_07", name="Villa", name_ru="Вилла", w=16.0, d=12.0, h=3.8,
         roof_type="flat", roof_h=0, wall=(0.96, 0.95, 0.90), trim=(0.99, 0.99, 0.97),
         roof=(0.80, 0.79, 0.75), door=(0.28, 0.22, 0.16), windows=4, pool=True,
         parapet=0.5, wing=(7.0, 8.0, 3.4, 9.5, -1.0), wall_tex="stucco",
         roof_tex="concrete"),
    dict(key="house_08", name="Cabin", name_ru="Хижина", w=7.4, d=6.4, h=2.9,
         roof_type="gable", roof_h=2.0, wall=(0.58, 0.42, 0.30), trim=(0.44, 0.32, 0.22),
         roof=(0.28, 0.38, 0.30), door=(0.36, 0.25, 0.16), windows=2, porch=True,
         chimney=True, ridge="x", wall_tex="wood", roof_tex="shingle"),
    dict(key="house_09", name="Modern", name_ru="Модерн", w=12.0, d=9.0, h=5.8,
         roof_type="flat", roof_h=0, wall=(0.92, 0.93, 0.92), trim=(0.40, 0.41, 0.42),
         roof=(0.34, 0.35, 0.37), door=(0.20, 0.20, 0.21), windows=3, floors=2,
         parapet=0.35, garage=True, wall_tex="stucco", roof_tex="metal"),
    dict(key="house_10", name="Garage house", name_ru="Дом с гаражом", w=13.0,
         d=9.5, h=3.2, roof_type="gable", roof_h=1.8, wall=(0.86, 0.90, 0.93),
         trim=(0.98, 0.98, 0.97), roof=(0.45, 0.46, 0.49), door=(0.24, 0.30, 0.40),
         windows=3, garage=True, ridge="x", fence=(17, 13), wall_tex="siding",
         roof_tex="shingle"),
    dict(key="house_11", name="Corner house", name_ru="Угловой дом", w=10.0, d=10.0,
         h=6.0, roof_type="hip", roof_h=1.8, wall=(0.90, 0.76, 0.60),
         trim=(0.98, 0.97, 0.93), roof=(0.40, 0.27, 0.19), door=(0.34, 0.23, 0.15),
         windows=3, floors=2, wing=(5.0, 7.0, 3.0, -6.0, 3.0), porch=True,
         chimney=True, wall_tex="brick", roof_tex="tile"),
    dict(key="house_12", name="Garden cottage", name_ru="Садовый коттедж", w=8.4,
         d=7.2, h=3.0, roof_type="hip", roof_h=1.6, wall=(0.94, 0.82, 0.80),
         trim=(0.98, 0.97, 0.95), roof=(0.46, 0.36, 0.30), door=(0.30, 0.24, 0.18),
         windows=3, porch=True, fence=(12, 11), wall_tex="stucco", roof_tex="shingle"),
    dict(key="house_13", name="Brick rowhouse", name_ru="Кирпичный ряд", w=7.4,
         d=11.0, h=6.4, roof_type="flat", roof_h=0, wall=(0.72, 0.40, 0.34),
         trim=(0.94, 0.92, 0.89), roof=(0.36, 0.37, 0.39), door=(0.22, 0.28, 0.22),
         windows=2, floors=2, parapet=0.55, door_x=1.4, wall_tex="brick",
         roof_tex="metal"),
    dict(key="house_14", name="Beach bungalow", name_ru="Пляжное бунгало", w=10.0,
         d=8.0, h=2.9, roof_type="hip", roof_h=1.5, wall=(0.96, 0.90, 0.72),
         trim=(0.99, 0.98, 0.94), roof=(0.40, 0.56, 0.60), door=(0.28, 0.42, 0.48),
         windows=4, porch=True, fence=(14, 12), wall_tex="siding", roof_tex="shingle"),
    dict(key="house_15", name="Family house", name_ru="Семейный дом", w=13.0,
         d=10.0, h=3.3, roof_type="gable", roof_h=1.9, wall=(0.86, 0.88, 0.80),
         trim=(0.98, 0.98, 0.95), roof=(0.42, 0.33, 0.26), door=(0.34, 0.25, 0.17),
         windows=4, garage=True, ridge="x", wall_tex="siding", roof_tex="shingle"),
    dict(key="house_16", name="Narrow house", name_ru="Узкий двухэтажный", w=6.4,
         d=10.0, h=6.0, roof_type="flat", roof_h=0, wall=(0.82, 0.80, 0.76),
         trim=(0.96, 0.95, 0.92), roof=(0.40, 0.41, 0.43), door=(0.24, 0.24, 0.26),
         windows=2, floors=2, parapet=0.4, wall_tex="brick", roof_tex="metal"),
    dict(key="house_17", name="Wood cabin", name_ru="Деревянная хижина", w=8.0,
         d=7.0, h=3.0, roof_type="gable", roof_h=2.2, wall=(0.64, 0.48, 0.34),
         trim=(0.46, 0.34, 0.24), roof=(0.26, 0.36, 0.30), door=(0.38, 0.26, 0.16),
         windows=2, porch=True, chimney=True, ridge="x", wall_tex="wood",
         roof_tex="shingle"),
    dict(key="house_18", name="Modern villa", name_ru="Современная вилла", w=17.0,
         d=11.0, h=3.8, roof_type="flat", roof_h=0, wall=(0.94, 0.94, 0.92),
         trim=(0.34, 0.35, 0.36), roof=(0.76, 0.76, 0.74), door=(0.18, 0.19, 0.20),
         windows=5, pool=True, parapet=0.45, wing=(6.5, 8.0, 3.2, 10.5, -0.5),
         wall_tex="stucco", roof_tex="concrete"),
    dict(key="house_19", name="Small ranch", name_ru="Малый ранчо", w=12.0,
         d=8.0, h=3.1, roof_type="gable", roof_h=1.6, wall=(0.90, 0.84, 0.72),
         trim=(0.97, 0.96, 0.93), roof=(0.43, 0.40, 0.35), door=(0.32, 0.24, 0.16),
         windows=3, garage=True, ridge="x", wall_tex="stucco", roof_tex="shingle"),
    dict(key="house_20", name="Lake cottage", name_ru="Домик у озера", w=8.2,
         d=6.4, h=2.9, roof_type="gable", roof_h=2.1, wall=(0.78, 0.80, 0.72),
         trim=(0.92, 0.90, 0.84), roof=(0.32, 0.40, 0.44), door=(0.30, 0.26, 0.20),
         windows=2, porch=True, ridge="z", wall_tex="wood", roof_tex="shingle"),
    dict(key="house_21", name="Twin duplex", name_ru="Двойной дуплекс", w=15.0,
         d=8.4, h=3.4, roof_type="gable", roof_h=1.7, wall=(0.88, 0.80, 0.86),
         trim=(0.97, 0.96, 0.95), roof=(0.38, 0.34, 0.38), door=(0.34, 0.24, 0.30),
         windows=4, door_x=3.8, ridge="x", wall_tex="siding", roof_tex="shingle"),
    dict(key="house_22", name="Sun house", name_ru="Солнечный дом", w=9.0,
         d=8.4, h=3.0, roof_type="hip", roof_h=1.7, wall=(0.94, 0.82, 0.52),
         trim=(0.99, 0.98, 0.94), roof=(0.60, 0.38, 0.24), door=(0.36, 0.26, 0.16),
         windows=3, porch=True, chimney=True, wall_tex="stucco", roof_tex="tile"),
    dict(key="house_23", name="Corner two-storey", name_ru="Угловой двухэтажный",
         w=10.0, d=10.0, h=6.0, roof_type="hip", roof_h=1.8, wall=(0.88, 0.74, 0.58),
         trim=(0.98, 0.97, 0.94), roof=(0.38, 0.26, 0.18), door=(0.34, 0.23, 0.15),
         windows=3, floors=2, wing=(5.0, 7.0, 3.0, -6.0, 3.0), porch=True,
         chimney=True, wall_tex="brick", roof_tex="tile"),
]


# --------------------------------------------------------------------------- #
# запись GLB
# --------------------------------------------------------------------------- #

def write_glb(path, builder, meta):
    blob = bytearray()
    views, accessors = [], []
    nodes, meshes = [], []

    def add_view(data, target=None):
        nonlocal blob
        while len(blob) % 4:
            blob.append(0)
        off = len(blob)
        blob += data
        view = {"buffer": 0, "byteOffset": off, "byteLength": len(data)}
        if target:
            view["target"] = target
        views.append(view)
        return len(views) - 1

    for name, verts, normals, uvs, indices, mat, pivot in builder.parts:
        pos = b"".join(struct.pack("<3f", *v) for v in verts)
        nor = b"".join(struct.pack("<3f", *n) for n in normals)
        uv = b"".join(struct.pack("<2f", *t) for t in uvs)
        idx = b"".join(struct.pack("<H", i) for i in indices)

        pv = add_view(pos, 34962)
        nv = add_view(nor, 34962)
        tv = add_view(uv, 34962)
        iv = add_view(idx, 34963)

        floats = struct.unpack("<%df" % (len(verts) * 3), pos)
        mn = [min(floats[i::3]) for i in range(3)]
        mx = [max(floats[i::3]) for i in range(3)]
        accessors.append({"bufferView": pv, "componentType": 5126, "count": len(verts),
                          "type": "VEC3", "min": mn, "max": mx})
        pa = len(accessors) - 1
        accessors.append({"bufferView": nv, "componentType": 5126,
                          "count": len(normals), "type": "VEC3"})
        na = len(accessors) - 1
        accessors.append({"bufferView": tv, "componentType": 5126,
                          "count": len(uvs), "type": "VEC2"})
        ta = len(accessors) - 1
        accessors.append({"bufferView": iv, "componentType": 5123,
                          "count": len(indices), "type": "SCALAR"})
        ia = len(accessors) - 1
        meshes.append({"name": name, "primitives": [{
            "attributes": {"POSITION": pa, "NORMAL": na, "TEXCOORD_0": ta},
            "indices": ia, "material": mat}]})
        node = {"name": name, "mesh": len(meshes) - 1}
        if pivot:
            node["translation"] = [pivot["x"], pivot["y"], pivot["z"]]
            node["extras"] = {"pivot": "hinge", "axis": "y"}
        nodes.append(node)

    # Текстуры общие для всех домов и лежат рядом: 12 файлов вместо копий
    # в каждом GLB. Так браузер грузит и закачивает в GPU только 12 текстур.
    images = [{"uri": "textures/" + name + ".png"} for name in builder.textures]
    js = {
        "asset": {"version": "2.0", "generator": "make_houses.py (GTA VI Mobile concept)"},
        "scene": 0,
        "scenes": [{"name": "Scene", "nodes": list(range(len(nodes)))}],
        "nodes": nodes,
        "meshes": meshes,
        "materials": builder.materials,
        "accessors": accessors,
        "bufferViews": views,
        "buffers": [{"byteLength": len(blob)}],
        "extras": meta,
    }
    if images:
        js["images"] = images
        js["textures"] = [{"source": i} for i in range(len(images))]
        js["samplers"] = [{"magFilter": 9729, "minFilter": 9987,
                           "wrapS": 10497, "wrapT": 10497}]
        for i in range(len(images)):
            js["textures"][i]["sampler"] = 0

    jb = json.dumps(js, separators=(",", ":")).encode()
    while len(jb) % 4:
        jb += b" "
    while len(blob) % 4:
        blob.append(0)
    total = 12 + 8 + len(jb) + 8 + len(blob)
    with open(path, "wb") as f:
        f.write(struct.pack("<III", 0x46546C67, 2, total))
        f.write(struct.pack("<II", len(jb), 0x4E4F534A))
        f.write(jb)
        f.write(struct.pack("<II", len(blob), 0x004E4942))
        f.write(blob)


def main():
    os.makedirs(OUT, exist_ok=True)
    textures = make_textures()
    print(f"текстуры: {len(textures)} PNG в models/generated/houses/textures/")
    total = 0
    for style in STYLES:
        b = make_house(style)
        fixed = []
        for name, verts, normals, uvs, indices, mat, pivot in b.parts:
            if pivot and name == "Door_Front":
                px, py, pz = pivot["x"], pivot["y"], pivot["z"]
                verts = [(v[0] - px, v[1] - py, v[2] - pz) for v in verts]
            fixed.append((name, verts, normals, uvs, indices, mat, pivot))
        b.parts = fixed
        path = os.path.join(OUT, style["key"] + ".glb")
        write_glb(path, b, {
            "name": style["name"], "name_ru": style["name_ru"],
            "category": "ordinary-house", "units": "m",
            "origin": "ground centre", "front": "+Z",
            "style": style["name"], "parts": len(fixed),
            "textured": True,
        })
        kb = os.path.getsize(path) / 1024
        total += kb
        print(f"{style['key']}.glb  {style['name_ru']:<22} {kb:6.1f} KB  "
              f"{len(fixed)} деталей  {len(b.textures)} текстур")
    print(f"итого: {len(STYLES)} домов, {total / 1024:.2f} МБ")


if __name__ == "__main__":
    main()
