#!/usr/bin/env python3
"""Генерирует карту города Марisol (концепт) в SVG.

Запуск:  python3 game/tools/make_map.py
Результат: game/map/city_map.svg (+ PNG рендерится отдельно через resvg)
"""

import math
import os
import random

W, H = 2400, 1700
MARGIN = 0
CITY = "MARISOL"
rnd = random.Random(7)

out = []


def add(s):
    out.append(s)


def poly(pts, fill, stroke="none", sw=0, extra=""):
    d = " ".join(f"{p[0]:.1f},{p[1]:.1f}" for p in pts)
    add(f'<polygon points="{d}" fill="{fill}" stroke="{stroke}" stroke-width="{sw}" {extra}/>')


def path(d, fill="none", stroke="none", sw=1, extra=""):
    attrs = []
    if "fill=" not in extra:
        attrs.append(f'fill="{fill}"')
    if "stroke=" not in extra:
        attrs.append(f'stroke="{stroke}"')
    if "stroke-width=" not in extra:
        attrs.append(f'stroke-width="{sw}"')
    add(f'<path d="{d}" ' + " ".join(attrs) + f' {extra}/>')


def rect(x, y, w, h, fill, stroke="none", sw=0, r=0, extra=""):
    add(f'<rect x="{x:.1f}" y="{y:.1f}" width="{w:.1f}" height="{h:.1f}" rx="{r}" '
        f'fill="{fill}" stroke="{stroke}" stroke-width="{sw}" {extra}/>')


def circle(cx, cy, r, fill, stroke="none", sw=0, extra=""):
    add(f'<circle cx="{cx:.1f}" cy="{cy:.1f}" r="{r:.1f}" fill="{fill}" '
        f'stroke="{stroke}" stroke-width="{sw}" {extra}/>')


def line(x1, y1, x2, y2, stroke, sw=1, extra=""):
    add(f'<line x1="{x1:.1f}" y1="{y1:.1f}" x2="{x2:.1f}" y2="{y2:.1f}" '
        f'stroke="{stroke}" stroke-width="{sw}" stroke-linecap="round" {extra}/>')


def text(x, y, s, size=14, fill="#e6edf3", anchor="middle", weight=400, spacing=0,
         opacity=1.0, extra=""):
    add(f'<text x="{x:.1f}" y="{y:.1f}" font-family="DejaVu Sans, Verdana, sans-serif" '
        f'font-size="{size}" font-weight="{weight}" fill="{fill}" fill-opacity="{opacity}" '
        f'text-anchor="{anchor}" letter-spacing="{spacing}" {extra}>{s}</text>')


# --------------------------------------------------------------------------- #
# палитра
# --------------------------------------------------------------------------- #

SEA = "#0d2a3b"
SEA_DEEP = "#0a2230"
LAND = "#1b2731"
LAND_EDGE = "#2f4250"
PARK = "#20402c"
PARK_DARK = "#1a3324"
SAND = "#b9a878"
BLOCK = "#243440"
BLOCK_HI = "#2c3f4d"
ROAD_MINOR = "#3a4b5b"
ROAD_MAIN = "#54697c"
ROAD_HW = "#e0b34d"
ROAD_HW_EDGE = "#3b3324"
RAIL = "#7b8b98"
WATER_IN = "#123a52"
TEXT = "#e8eef4"
TEXT_DIM = "#93a3b1"
POI = {
    "police": ("#3b82f6", "POL"),
    "hospital": ("#ef4444", "MED"),
    "shop": ("#22c55e", "SHOP"),
    "garage": ("#f59e0b", "GAR"),
    "safehouse": ("#a855f7", "HOME"),
    "food": ("#fb923c", "EAT"),
    "clothes": ("#ec4899", "CLTH"),
    "gun": ("#94a3b8", "GUN"),
    "port": ("#0ea5e9", "PORT"),
    "fuel": ("#eab308", "FUEL"),
    "bank": ("#14b8a6", "BANK"),
    "fun": ("#e879f9", "FUN"),
}

# --------------------------------------------------------------------------- #
# дома, кварталы, детали
# --------------------------------------------------------------------------- #

ROOFS = ["#7a4a3a", "#6b5546", "#4d5a63", "#8a6a4a", "#5f6b58",
         "#7d5f52", "#55606b", "#946e50", "#4a5560", "#6f5a4a"]
WALLS = ["#8f8878", "#a8a091", "#7d8478", "#9a8f7c", "#b0a795",
         "#867a68", "#9c9585", "#77705f"]
LAWN = ["#1f3d2a", "#24452f", "#1b3626", "#274a33"]
TOWER = ["#4a6375", "#566f80", "#5e6a74", "#46586a", "#6b5f58",
         "#3f5364", "#5a6876"]
TOWER_PAIRS = [("#41576a", "#5f7d93"), ("#4a5f6e", "#6b8798"),
               ("#5b6a74", "#7f919d"), ("#46586a", "#617e96"),
               ("#685d55", "#87776a"), ("#3f5364", "#5a7592"),
               ("#546675", "#748a9b"), ("#6a5a52", "#877669")]


def _rect_pts(cx, cy, w, h, ang=0.0):
    c, s = math.cos(ang), math.sin(ang)
    pts = []
    for dx, dy in ((-w / 2, -h / 2), (w / 2, -h / 2),
                   (w / 2, h / 2), (-w / 2, h / 2)):
        pts.append((cx + dx * c - dy * s, cy + dx * s + dy * c))
    return pts


def house(cx, cy, w, d, ang=0.0, wall=None, roof=None, chimney=False):
    wall = wall or rnd.choice(WALLS)
    roof = roof or rnd.choice(ROOFS)
    poly(_rect_pts(cx + 1.7, cy + 2.2, w, d, ang), "#050a0f", "none", 0,
         'opacity="0.42"')
    poly(_rect_pts(cx, cy, w, d, ang), wall, "#101a22", 0.45)
    poly(_rect_pts(cx, cy, w - 1.0, d - 1.0, ang), roof, "none", 0)
    c, s = math.cos(ang), math.sin(ang)
    if w >= d:
        dx = w / 2 - 1.2
        line(cx - dx * c, cy - dx * s, cx + dx * c, cy + dx * s,
             "#2b211c", 0.55, 'opacity="0.55"')
    else:
        dy = d / 2 - 1.2
        line(cx + dy * s, cy - dy * c, cx - dy * s, cy + dy * c,
             "#2b211c", 0.55, 'opacity="0.55"')
    if chimney:
        circle(cx + (w * 0.26) * c - (-d * 0.2) * s,
               cy + (w * 0.26) * s + (-d * 0.2) * c, 0.7, "#5b4b42")


def lawn(cx, cy, w, h, ang=0.0):
    poly(_rect_pts(cx, cy, w, h, ang), rnd.choice(LAWN), "none", 0,
         'opacity="0.85"')


def tree(cx, cy, r=2.4):
    add(f'<ellipse cx="{cx + 1.3:.1f}" cy="{cy + 1.7:.1f}" rx="{r:.1f}" ry="{r * 0.8:.1f}" '
        f'fill="#050a0f" opacity="0.35"/>')
    circle(cx, cy, r, "#2b5238")
    circle(cx - r * 0.22, cy - r * 0.26, r * 0.66, "#3c6e4a")


def palm(cx, cy, s=1.0):
    add(f'<ellipse cx="{cx + 1.6:.1f}" cy="{cy + 2.0:.1f}" rx="{3.2 * s:.1f}" '
        f'ry="{1.5 * s:.1f}" fill="#050a0f" opacity="0.3"/>')
    line(cx, cy + 1.2, cx + 0.7, cy, "#5c4632", 1.1)
    for k in range(6):
        a = math.pi * 2 * k / 6 + 0.35
        line(cx + 0.7, cy, cx + 0.7 + math.cos(a) * 3.4 * s,
             cy + math.sin(a) * 2.4 * s, "#2f6a45", 0.9, 'opacity="0.9"')


def car(cx, cy, ang=0.0, color="#c9d4dc"):
    poly(_rect_pts(cx + 0.7, cy + 0.9, 3.4, 1.6, ang), "#050a0f", "none", 0,
         'opacity="0.4"')
    poly(_rect_pts(cx, cy, 3.2, 1.5, ang), color, "#101a22", 0.3)
    poly(_rect_pts(cx, cy - 0.1, 1.6, 1.2, ang), "#23313c", "none", 0,
         'opacity="0.8"')


def tower(cx, cy, w, d, h, pair=None):
    body, roof = pair or rnd.choice(TOWER_PAIRS)
    off = 2.0 + min(6.5, h / 8.0)
    # тень
    poly(_rect_pts(cx + off * 0.55, cy + off * 0.85, w, d), "#050a0f", "none", 0,
         'opacity="0.55"')
    # стилобат (первый этаж шире башни)
    poly(_rect_pts(cx, cy, w + 1.8, d + 1.8), "#33434f", "#111c25", 0.4,
         'opacity="0.9"')
    # корпус и кровля с отступом
    poly(_rect_pts(cx, cy, w, d), body, "#0f1a23", 0.6)
    rw, rd = max(3.0, w - 2.6), max(3.0, d - 2.6)
    poly(_rect_pts(cx, cy, rw, rd), roof, "none", 0)
    # оконные полосы
    for k in range(1, 3):
        y = cy - d / 2 + k * d / 3
        line(cx - w / 2 + 0.8, y, cx + w / 2 - 0.8, y, "#1d2a35", 0.45,
             'opacity="0.35"')
    line(cx - w / 2 + 1, cy - d / 2 + 1, cx + w / 2 - 1, cy - d / 2 + 1,
         "#b6c9d6", 0.6, 'opacity="0.4"')
    # детали кровли — у каждого здания свои
    r = rnd.random()
    if h > 74 or r < 0.16:
        circle(cx, cy, min(rw, rd) * 0.27, "#273742", "#a8bccb", 0.5)
        line(cx - 1.2, cy, cx + 1.2, cy, "#a8bccb", 0.5, 'opacity="0.8"')
        line(cx, cy - 1.2, cx, cy + 1.2, "#a8bccb", 0.5, 'opacity="0.8"')
    elif r < 0.46:
        rect(cx - rw * 0.28, cy - rd * 0.22, rw * 0.24, rd * 0.28, "#8ea3b2",
             "#394b57", 0.3, r=0.3)
        rect(cx + rw * 0.06, cy + rd * 0.02, rw * 0.22, rd * 0.24, "#7f95a4",
             "#394b57", 0.3, r=0.3)
    elif r < 0.60:
        rect(cx - rw * 0.22, cy - rd * 0.18, rw * 0.5, rd * 0.38, "#2c7d92",
             "#b9d6de", 0.35, r=0.3)
        circle(cx + rw * 0.28, cy + rd * 0.2, 0.9, "#2f6a45")
    elif r < 0.76:
        rect(cx - rw * 0.3, cy - rd * 0.28, rw * 0.6, rd * 0.56, "#2f5a3c",
             "#79b98d", 0.35, r=0.5)
    else:
        rect(cx - rw * 0.18, cy - rd * 0.16, rw * 0.3, rd * 0.3, "#455866",
             "none", 0, r=0.3)
        line(cx + rw * 0.16, cy + rd * 0.12, cx + rw * 0.16, cy - rd * 0.36,
             "#9db3c4", 0.55, 'opacity="0.65"')
        circle(cx + rw * 0.16, cy - rd * 0.36, 0.7, "#d55c4a")


# --------------------------------------------------------------------------- #
# кварталы
# --------------------------------------------------------------------------- #

def build_downtown():
    for yi, y in enumerate(range(400, 800, 32)):
        for xi, x in enumerate(range(1010, 1420, 34)):
            cx, cy = x + 17, y + 16
            r = rnd.random()
            if r < 0.09:
                # парковка / сквер
                rect(x + 4, y + 4, 26, 24, "#1b2a35", "#2c4050", 0.5, r=1)
                for k in range(4):
                    line(x + 8 + k * 6, y + 7, x + 8 + k * 6, y + 24,
                         "#3c4f5e", 0.5, 'opacity="0.7"')
                if rnd.random() < 0.5:
                    tree(x + 8, y + 8, 1.8)
                continue
            bw = rnd.uniform(18, 27)
            bd = rnd.uniform(16, 25)
            h = rnd.choice([14, 20, 26, 34, 44, 58, 76, 96]) * rnd.uniform(0.9, 1.1)
            tower(cx, cy, bw, bd, h)


def build_barrio():
    for x in range(530, 928, 46):
        for y in range(575, 876, 42):
            if rnd.random() < 0.06:
                tree(x + 23, y + 21, 3.2)
                continue
            rect(x + 2, y + 2, 42, 38, "#1c2a34", "#2a3b47", 0.4, r=0.8)
            for i in range(3):
                for j in range(2):
                    if rnd.random() < 0.08:
                        continue
                    cx = x + 10 + i * 14 + rnd.uniform(-1.2, 1.2)
                    cy = y + 11 + j * 17 + rnd.uniform(-1.2, 1.2)
                    lawn(cx, cy, 11.5, 13.5)
                    house(cx, cy, rnd.uniform(6.0, 8.2), rnd.uniform(4.8, 6.2),
                          rnd.uniform(-0.1, 0.1), chimney=rnd.random() < 0.2)
                    if rnd.random() < 0.22:
                        tree(cx + rnd.choice([-5.2, 5.2]), cy + 4.8, 1.9)


def build_suburb():
    # отдельные участки вдоль петляющих улиц
    for x in range(304, 700, 20):
        for y in range(620, 884, 20):
            if rnd.random() > 0.52:
                continue
            cx = x + rnd.uniform(-2, 2)
            cy = y + rnd.uniform(-2, 2)
            # не застраиваем сами петли улиц
            on_loop = any(((cx - ex) / (rx + 4)) ** 2 + ((cy - ey) / (ry + 4)) ** 2 < 1.0
                          and ((cx - ex) / (rx - 5)) ** 2 + ((cy - ey) / (ry - 5)) ** 2 > 1.0
                          for ex, ey, rx, ry in [(410, 730, 90, 60), (600, 740, 80, 54),
                                                 (430, 840, 84, 46), (620, 840, 70, 40)])
            if on_loop:
                continue
            lawn(cx, cy, 16, 15, 0.0)
            house(cx, cy, rnd.uniform(6.4, 8.6), rnd.uniform(5.4, 7.2),
                  rnd.choice([0, math.pi / 2]) + rnd.uniform(-0.06, 0.06),
                  chimney=rnd.random() < 0.35)
            # дорожка от дома
            line(cx, cy + 3.2, cx, cy + 7.2, "#49535b", 0.7, 'opacity="0.6"')
            if rnd.random() < 0.3:
                tree(cx + rnd.choice([-7, 7]), cy + rnd.uniform(-5, 5), 2.1)
    # дома вдоль берега залива
    for x in range(300, 560, 22):
        for y in range(660, 900, 24):
            if rnd.random() < 0.4:
                continue
            lawn(x, y, 17, 17)
            house(x, y, 7.0, 5.8, 0.0, chimney=rnd.random() < 0.3)


def build_villas():
    for x in range(1470, 1820, 42):
        for y in range(130, 380, 42):
            if rnd.random() < 0.18:
                tree(x + 20, y + 20, 3.6)
                continue
            lawn(x + 22, y + 22, 34, 34)
            house(x + 22, y + 20, rnd.uniform(13, 18), rnd.uniform(9, 12),
                  rnd.uniform(-0.08, 0.08), wall="#d8d2c2",
                  roof=rnd.choice(["#8a7a64", "#6a6f6a", "#9a8064"]))
            if rnd.random() < 0.7:
                add(f'<rect x="{x + 9:.1f}" y="{y + 31:.1f}" width="9" height="4.4" rx="1" '
                    f'fill="#2f7f96" opacity="0.85"/>')
                add(f'<rect x="{x + 9:.1f}" y="{y + 31:.1f}" width="9" height="4.4" rx="1" '
                    f'fill="none" stroke="#c9d4dc" stroke-width="0.5" opacity="0.5"/>')
            for k in range(2):
                if rnd.random() < 0.6:
                    tree(x + rnd.uniform(4, 38), y + rnd.uniform(6, 18), 2.0)
            car(x + 32, y + 36, rnd.uniform(-0.4, 0.4),
                rnd.choice(["#c9d4dc", "#8a2f2f", "#2f4a6a"]))


def build_industrial():
    for x in range(1566, 1756, 34):
        for y in range(706, 846, 30):
            if rnd.random() < 0.2:
                continue
            w = rnd.uniform(18, 28)
            d = rnd.uniform(14, 22)
            poly(_rect_pts(x + 16 + 2.2, y + 14 + 2.6, w, d), "#050a0f", "none", 0,
                 'opacity="0.5"')
            poly(_rect_pts(x + 16, y + 14, w, d), "#3d4b55", "#16222c", 0.5)
            for k in range(3):
                line(x + 16 - w / 2 + 2, y + 10 + k * 4, x + 16 + w / 2 - 2,
                     y + 10 + k * 4, "#56656f", 0.45, 'opacity="0.55"')


def text_along(x1, y1, x2, y2, s, size=9, fill=TEXT_DIM, opacity=0.75):
    ang = math.degrees(math.atan2(y2 - y1, x2 - x1))
    if ang > 90 or ang < -90:
        ang += 180
    mx, my = (x1 + x2) / 2, (y1 + y2) / 2
    add(f'<text x="{mx:.1f}" y="{my:.1f}" transform="rotate({ang:.1f} {mx:.1f} {my:.1f})" '
        f'font-family="DejaVu Sans, Verdana, sans-serif" font-size="{size}" '
        f'font-weight="600" fill="{fill}" fill-opacity="{opacity}" text-anchor="middle" '
        f'letter-spacing="1.6" paint-order="stroke" stroke="#0d1117" stroke-width="2.6">{s}</text>')


def coast_y(x):
    pts = sorted(COAST, key=lambda p: p[0])
    if x <= pts[0][0]:
        return pts[0][1]
    if x >= pts[-1][0]:
        return pts[-1][1]
    for (x0, y0), (x1, y1) in zip(pts, pts[1:]):
        if x0 <= x <= x1:
            u = (x - x0) / max(1e-6, (x1 - x0))
            return y0 + (y1 - y0) * u
    return pts[-1][1]


def build_ocean_strip():
    # городская подложка набережной
    strip = ([(x, coast_y(x) - 165) for x in range(470, 1260, 16)] +
             [(x, coast_y(x) - 36) for x in reversed(range(470, 1260, 16))])
    poly(strip, "#1a2730", "none", 0, 'opacity="0.95"')
    for x in range(520, 1240, 55):
        line(x, coast_y(x) - 160, x, coast_y(x) - 44, "#24333e", 3.2,
             'opacity="0.9"')
    # три ряда застройки между шоссе и пляжем
    for x in range(510, 1210, 27):
        if rnd.random() < 0.1:
            continue
        cy = coast_y(x) - rnd.uniform(58, 68)
        w, d = rnd.uniform(17, 24), rnd.uniform(11, 15)
        lawn(x, cy, w + 5, d + 6)
        house(x, cy, w, d, 0.0,
              wall=rnd.choice(["#d8cfb8", "#cfc7b4", "#b7c2c4", "#e2d4b8"]),
              roof=rnd.choice(["#8a6a52", "#5d6a6a", "#9a7a5c", "#7d8a90"]))
        if rnd.random() < 0.62:
            rect(x - w / 2 - 0.5, cy + d / 2 + 1.8, 8.5, 4.2, "#2f7f96",
                 "#d8e6ea", 0.4, r=1.2)
            for k in range(3):
                rect(x + w / 2 + 1.5, cy - d / 2 + 1.5 + k * 2.2, 1.1, 1.6,
                     rnd.choice(["#e8eef4", "#d96f4a"]), "none", 0, r=0.2)
        if rnd.random() < 0.7:
            palm(x + w / 2 + 2.5, cy + d / 2 + 1.8, 0.9)
    # второй ряд: апартаменты с балконами
    for x in range(540, 1180, 25):
        if rnd.random() < 0.22:
            continue
        cy = coast_y(x) - rnd.uniform(98, 108)
        w, d = rnd.uniform(16, 21), rnd.uniform(10, 14)
        rect(x - w / 2 - 1.5, cy - d / 2 - 1.5, w + 3, d + 3, "#2a3c47", "none", 0, r=1)
        rect(x - w / 2, cy - d / 2, w, d, rnd.choice(["#7d8a90", "#8d8a80", "#6d7d86"]),
             "#101a22", 0.5)
        poly(_rect_pts(x, cy, w - 2.2, d - 2.2), rnd.choice(["#6a7b85", "#7c766b"]), "none", 0)
        for k in range(1, 3):
            line(x - w / 2 + 0.8, cy - d / 2 + k * d / 3, x + w / 2 - 0.8,
                 cy - d / 2 + k * d / 3, "#43535e", 0.6, 'opacity="0.7"')
        if rnd.random() < 0.5:
            rect(x + w / 2 - 4.2, cy - 1.6, 4.6, 3.2, "#2f7f96", "#d8e6ea", 0.35, r=0.8)
    # третий ряд: небольшие жилые дома и магазины
    for x in range(560, 1160, 30):
        if rnd.random() < 0.35:
            continue
        cy = coast_y(x) - rnd.uniform(140, 152)
        lawn(x, cy, 18, 15)
        house(x, cy, rnd.uniform(7, 9.5), rnd.uniform(5.5, 7.5), 0.0)
    # пляжная полоса: зонты с лежаками, пальмы, спасательные вышки
    for x in range(540, 1560, 9):
        cy = coast_y(x)
        if rnd.random() < 0.7:
            uy = cy - rnd.uniform(8, 22)
            col = rnd.choice(["#d96f4a", "#e8c46a", "#4fa3a8", "#c95a78", "#e0e6ea"])
            add(f'<ellipse cx="{x + 1.6:.1f}" cy="{uy + 2.4:.1f}" rx="3.4" ry="1.4" '
                f'fill="#050a0f" opacity="0.35"/>')
            circle(x, uy, 3.0, col, "#f0e6d0", 0.45)
            line(x, uy + 3.0, x, uy + 4.6, "#6b5a44", 0.55)
            if rnd.random() < 0.6:
                rect(x - 3.8, uy + 4.0, 3.0, 1.5, rnd.choice(["#e8eef4", "#d96f4a"]),
                     "none", 0, r=0.2)
        if x % 45 == 0:
            palm(x + 3, cy - rnd.uniform(26, 36), 1.0)
    # пляжный клуб и кафе у моря
    for (cx, cy, w, d) in [(758, 1112, 26, 16), (656, 1052, 13, 9), (900, 1136, 15, 10)]:
        lawn(cx, cy, w + 6, d + 8)
        house(cx, cy, w, d, 0.0, wall="#e0d2b4", roof="#a07a54")
        rect(cx - w / 2 - 2, cy + d / 2 + 2, w * 0.45, 7, "#2f7f96",
             "#d8e6ea", 0.5, r=1.5)
        for k in range(5):
            add(f'<ellipse cx="{cx - 9 + k * 4.4:.1f}" cy="{cy + d / 2 + 12.4:.1f}" '
                f'rx="2.4" ry="1.1" fill="#050a0f" opacity="0.3"/>')
            circle(cx - 9 + k * 4.4, cy + d / 2 + 11, 2.0,
                   ["#e8c46a", "#d96f4a", "#e0e6ea"][k % 3], "#f0e6d0", 0.4)
    # деревянный настил вдоль пляжа
    board = [(x, coast_y(x) - 32) for x in range(500, 1600, 20)]
    path("M" + " L".join(f"{x:.0f},{y:.0f}" for x, y in board), "none", "#7a6448", 3.2,
         'opacity="0.55" stroke-linecap="round"')
    # спасательные вышки
    for x in (700, 880, 1120, 1380):
        y = coast_y(x) - 14
        rect(x - 1.4, y - 2.4, 2.8, 2.8, "#d8c79a", "#7a6448", 0.4, r=0.3)
        line(x - 1.4, y, x + 1.4, y, "#7a6448", 0.45)


def build_marina():
    # яхты в заливе и у Marina Real
    for i in range(9):
        x = 1470 + i * 15 + rnd.uniform(-3, 3)
        y = 1020 + (i % 3) * 12 + rnd.uniform(-2, 2)
        ang = 0.15 + rnd.uniform(-0.1, 0.1)
        c, s = math.cos(ang), math.sin(ang)
        pts = [(x - 7 * c, y - 7 * s), (x + 7 * c, y + 7 * s),
               (x + 4 * c - 2.2 * s, y + 4 * s + 2.2 * c),
               (x - 4 * c - 2.2 * s, y - 4 * s + 2.2 * c)]
        poly(pts, "#e8eef4", "#4a6474", 0.5, 'opacity="0.95"')
        add(f'<rect x="{x - 3:.1f}" y="{y - 1.2:.1f}" width="5" height="2.2" rx="0.6" '
            f'fill="#3c5a6a" opacity="0.9"/>')
    # пирсы Marina Real
    for i in range(6):
        line(1576 + i * 15, 866, 1576 + i * 15, 892, "#8aa0ad", 1.4, 'opacity="0.7"')


def build_airport():
    # самолёты и перрон
    for (cx, cy, ang) in [(1860, 470, -0.55), (1990, 395, -0.55), (1740, 330, 0.1)]:
        c, s = math.cos(ang), math.sin(ang)
        body = [(cx - 6 * c, cy - 6 * s), (cx + 6 * c, cy + 6 * s),
                (cx + 4.5 * c - 1.1 * s, cy + 4.5 * s + 1.1 * c),
                (cx - 4.5 * c - 1.1 * s, cy - 4.5 * s + 1.1 * c)]
        poly(body, "#c9d4dc", "#4a5b68", 0.4)
        line(cx - 1.5 * s, cy + 1.5 * c, cx + 1.5 * s, cy - 1.5 * c,
             "#8aa0ad", 5, 'opacity="0.9"')
    for i in range(4):
        rect(1772 + i * 26, 552, 20, 12, "#2a3c48", "#4a5b68", 0.4, r=1)
    # рулёжные дорожки
    for (x1, y1, x2, y2) in [(1800, 500, 1780, 560), (2130, 340, 2140, 400)]:
        line(x1, y1, x2, y2, "#3a4b57", 5, 'opacity="0.8"')


def build_outskirts():
    """Небольшие жилые и коммерческие кварталы по краям карты."""
    # северо-восток: малоэтажные дома между виллами и аэропортом
    for x in range(1840, 2120, 24):
        for y in range(404, 606, 24):
            if rnd.random() < 0.42:
                continue
            lawn(x, y, 20, 19)
            house(x, y, rnd.uniform(6.5, 8.5), rnd.uniform(5.2, 6.6),
                  rnd.choice([0, math.pi / 2]), chimney=rnd.random() < 0.3)
            if rnd.random() < 0.3:
                tree(x - 7, y + 6, 1.9)
    # восток: склады и мастерские
    for x in range(1776, 2050, 38):
        for y in range(624, 852, 34):
            if rnd.random() < 0.3:
                continue
            w, d = rnd.uniform(18, 26), rnd.uniform(14, 21)
            poly(_rect_pts(x + 2.2, y + 2.6, w, d), "#050a0f", "none", 0,
                 'opacity="0.45"')
            poly(_rect_pts(x, y, w, d), rnd.choice(["#3d4b55", "#444f56", "#37444e"]),
                 "#16222c", 0.5)
            for k in range(3):
                line(x - w / 2 + 2, y - d / 2 + 3 + k * 4, x + w / 2 - 2,
                     y - d / 2 + 3 + k * 4, "#56656f", 0.45, 'opacity="0.5"')
    # юго-запад: пригород у воды
    for x in range(118, 300, 22):
        for y in range(640, 900, 22):
            if rnd.random() < 0.5:
                continue
            lawn(x, y, 18, 18)
            house(x, y, rnd.uniform(6.2, 7.8), rnd.uniform(5.0, 6.2),
                  rnd.choice([0, math.pi / 2]))
            if rnd.random() < 0.35:
                tree(x + 6, y - 6, 2.0)
    # полоса между шоссе и набережной
    for x in range(620, 780, 20):
        for y in range(908, 1000, 20):
            if rnd.random() < 0.55:
                continue
            lawn(x, y, 16, 16)
            house(x, y, 6.8, 5.6, 0.0)
    # редкие дома у железной дороги на западе
    for x in range(60, 260, 26):
        for y in range(620, 780, 26):
            if rnd.random() < 0.62:
                continue
            lawn(x, y, 20, 18)
            house(x, y, 7.0, 5.6, 0.0)


def build_park_details():
    for _ in range(26):
        x = rnd.uniform(140, 560)
        y = rnd.uniform(120, 430)
        if any((x - cx) ** 2 + (y - cy) ** 2 < 900 for cx, cy in []):
            continue
        tree(x, y, rnd.uniform(1.8, 3.4))
    for (x, y) in [(820, 260), (960, 340), (860, 380), (770, 300)]:
        tree(x, y, 2.6)
    # лунки и бункеры гольфа
    for (cx, cy, r) in [(860, 260, 34), (960, 340, 26), (820, 350, 20)]:
        circle(cx, cy + 3, r, "#234a30", "#3f7a52", 1.2)
    for (cx, cy) in [(890, 300), (930, 320), (800, 330)]:
        add(f'<ellipse cx="{cx}" cy="{cy}" rx="7" ry="4" fill="#c9b78a" opacity="0.9"/>')
    # кампус
    for (x, y, w, h) in [(915, 270, 24, 16), (960, 300, 18, 14), (930, 320, 30, 12)]:
        poly(_rect_pts(x + w / 2 + 2, y + h / 2 + 2.5, w, h), "#050a0f", "none", 0,
             'opacity="0.4"')
        rect(x, y, w, h, "#5e6a74", "#2a3b47", 0.5, r=1)
        line(x + 3, y + 4, x + w - 3, y + 4, "#93a3b1", 0.5, 'opacity="0.5"')
    # стадион: трибуны
    add('<ellipse cx="1290" cy="330" rx="58" ry="40" fill="#394c5a" stroke="#6f8595" stroke-width="2"/>')
    add('<ellipse cx="1290" cy="330" rx="46" ry="31" fill="#4a5f6e" stroke="none"/>')
    add('<ellipse cx="1290" cy="330" rx="40" ry="25" fill="#2f5a3c" stroke="#79b98d" stroke-width="1.5"/>')
    line(1250, 330, 1330, 330, "#8fc79f", 0.7, 'opacity="0.6"')
    circle(1290, 330, 4, "none", "#8fc79d", 0.8)


# --------------------------------------------------------------------------- #
# береговая линия
# --------------------------------------------------------------------------- #

COAST = [
    (2400, 980), (2270, 1005), (2120, 1012), (1980, 1030), (1830, 1062),
    (1690, 1074), (1600, 1105), (1500, 1122), (1400, 1160), (1300, 1198),
    (1210, 1246), (1120, 1298), (1030, 1332), (940, 1336), (850, 1312),
    (760, 1262), (668, 1206), (576, 1150), (486, 1104), (392, 1068),
    (286, 1030), (170, 992), (60, 934), (0, 900),
]
LAND_PTS = [(0, 0), (W, 0)] + COAST

# --------------------------------------------------------------------------- #
# фон
# --------------------------------------------------------------------------- #

add(f'<defs>')
add(f'<linearGradient id="sea" x1="0" y1="0" x2="0" y2="1">'
    f'<stop offset="0" stop-color="{SEA}"/><stop offset="1" stop-color="{SEA_DEEP}"/>'
    f'</linearGradient>')
add(f'<linearGradient id="land" x1="0" y1="0" x2="0" y2="1">'
    f'<stop offset="0" stop-color="#1d2a34"/><stop offset="1" stop-color="{LAND}"/>'
    f'</linearGradient>')
add(f'<filter id="glow" x="-30%" y="-30%" width="160%" height="160%">'
    f'<feGaussianBlur stdDeviation="6" result="b"/><feMerge>'
    f'<feMergeNode in="b"/><feMergeNode in="SourceGraphic"/></feMerge></filter>')
add('</defs>')

rect(0, 0, W, H, "url(#sea)")

# волны на воде
for i in range(70):
    x = rnd.uniform(0, W)
    y = rnd.uniform(940, H)
    if y < 1180 and x < 1250:
        continue
    w = rnd.uniform(18, 60)
    path(f"M{x:.0f},{y:.0f} q{w/2:.0f},-5 {w:.0f},0", "none", "#17415a", 1.4,
         'stroke-linecap="round" opacity="0.5"')

poly(LAND_PTS, "url(#land)")
poly(LAND_PTS, "none", LAND_EDGE, 3)

# --------------------------------------------------------------------------- #
# залив, река, каналы
# --------------------------------------------------------------------------- #

# залив
path("M1440,1120 C1360,1090 1310,1030 1312,960 C1315,880 1370,830 1450,826 "
     "C1540,822 1600,870 1606,950 C1612,1030 1560,1080 1480,1108 Z",
     WATER_IN, "#2b6a8a", 3)
# канал из залива в город
path("M1450,830 C1430,760 1400,700 1340,650", "none", WATER_IN, 34)
path("M1450,830 C1430,760 1400,700 1340,650", "none", "#2b6a8a", 3, 'opacity="0.8"')
# река
path("M1150,60 C1180,220 1230,340 1240,470 C1250,600 1290,700 1340,650",
     "none", WATER_IN, 46)
path("M1150,60 C1180,220 1230,340 1240,470 C1250,600 1290,700 1340,650",
     "none", "#2b6a8a", 2.5, 'opacity="0.7"')
# каналы даунтауна
for i, x0 in enumerate([1050, 1150, 1250]):
    path(f"M{x0},{430 + i*10} L{x0 + 60},{700}", "none", WATER_IN, 12)

# --------------------------------------------------------------------------- #
# острова / пляж
# --------------------------------------------------------------------------- #

for (cx, cy, rx, ry) in [(1010, 1462, 118, 34), (1600, 1490, 150, 38), (2090, 1420, 108, 30)]:
    add(f'<ellipse cx="{cx}" cy="{cy}" rx="{rx}" ry="{ry}" fill="{SAND}" opacity="0.85"/>')
    add(f'<ellipse cx="{cx}" cy="{cy}" rx="{rx}" ry="{ry}" fill="none" stroke="#d8c79a" stroke-width="2" opacity="0.6"/>')

# пляж вдоль берега
beach = []
for (x, y) in COAST[3:-1]:
    beach.append((x, y))
inner = [(x, y - 24) for (x, y) in reversed(beach)]
poly(beach + inner, SAND, "#d8c79a", 2)
foam = [(x, y + 7) for (x, y) in COAST[3:]]
path("M" + " L".join(f"{x},{y}" for x, y in foam), "none", "#e8f4f8", 2,
     'opacity="0.35" stroke-dasharray="10 8"')

# --------------------------------------------------------------------------- #
# парки, болото, поле для гольфа
# --------------------------------------------------------------------------- #

poly([(120, 120), (560, 90), (640, 300), (520, 470), (180, 430), (90, 260)],
     "#18302a", "#2b6a5a", 2)
# болото: пятна
for i in range(16):
    cx = rnd.uniform(160, 560)
    cy = rnd.uniform(140, 430)
    r = rnd.uniform(22, 60)
    add(f'<ellipse cx="{cx:.0f}" cy="{cy:.0f}" rx="{r:.0f}" ry="{r*0.62:.0f}" '
        f'fill="{WATER_IN}" opacity="0.55"/>')
for i in range(26):
    x = rnd.uniform(140, 560)
    y = rnd.uniform(130, 430)
    line(x, y, x + rnd.uniform(-14, 14), y + rnd.uniform(10, 26), "#2f6a52", 1.4,
         'opacity="0.5"')

# гольф
poly([(760, 150), (1010, 130), (1080, 330), (900, 420), (740, 330)],
     "#2a5a3c", "#3f8a5c", 2)
for (cx, cy, r) in [(860, 260, 34), (960, 340, 26), (820, 350, 20)]:
    circle(cx, cy, r, "#3f7a52", "#79b98d", 1.5)
circle(905, 300, 6, "#e5e7eb")

# кладбище / парк в центре
poly([(930, 560), (1010, 545), (1030, 660), (950, 676)], PARK, "#2f6a44", 2)

# --------------------------------------------------------------------------- #
# кварталы (массинг)
# --------------------------------------------------------------------------- #

def scatter_blocks(x0, y0, x1, y1, cell=26, fill=BLOCK, chance=0.82, inset=4):
    shades = ["#22313d", "#263744", "#2b3e4d", "#304656"]
    y = y0
    while y < y1 - cell * 0.6:
        x = x0
        while x < x1 - cell * 0.6:
            if rnd.random() < chance:
                w = cell - inset * 2
                h = cell - inset * 2
                c = rnd.choice(shades) if fill in ("#2b3e4d", BLOCK, BLOCK_HI) else fill
                rect(x + inset + 0.8, y + inset + 0.8, w, h, "#16202a", "none", 0, r=1.5,
                     extra='opacity="0.7"')
                rect(x + inset, y + inset, w, h, c, "#3a5064", 0.6, r=1.5)
            x += cell
        y += cell


# тонировка районов
DISTRICT_FILL = [
    ([(1000, 386), (1420, 386), (1420, 796), (1000, 796)], "#2e4b63", 0.28),
    ([(514, 552), (934, 552), (934, 886), (514, 886)], "#4a3f63", 0.22),
    ([(296, 616), (704, 616), (704, 886), (296, 886)], "#2f5a44", 0.18),
    ([(1556, 696), (1764, 696), (1764, 856), (1556, 856)], "#5a4a33", 0.2),
    ([(1456, 116), (1834, 116), (1834, 384), (1456, 384)], "#3b4f63", 0.2),
]
for pts, color, op in DISTRICT_FILL:
    poly(pts, color, "none", 0, f'opacity="{op}"')

# жилые и деловые кварталы
build_downtown()
build_barrio()
build_suburb()
build_villas()
build_industrial()
build_outskirts()
build_ocean_strip()
build_park_details()

# --------------------------------------------------------------------------- #
# дороги
# --------------------------------------------------------------------------- #

# шоссе (кольцо)
HW = [(300, 640), (520, 470), (860, 360), (1180, 300), (1520, 268),
      (1860, 340), (2100, 600), (2060, 900), (1780, 1040), (1420, 1092),
      (980, 1030), (600, 900), (300, 760), (300, 640)]
d_hw = "M" + " L".join(f"{x},{y}" for x, y in HW)
path(d_hw, "none", ROAD_HW_EDGE, 16, 'fill="none" stroke-linejoin="round"')
path(d_hw, "none", ROAD_HW, 9, 'fill="none" stroke-linejoin="round"')
path(d_hw, "none", "#f6e3a8", 2.4, 'fill="none" stroke-dasharray="18 14" opacity="0.8"')

# магистрали
MAIN = [
    [(700, 620), (980, 700), (1180, 760), (1420, 830), (1700, 900)],
    [(1000, 1080), (1000, 900), (1010, 700), (1180, 300)],
    [(1180, 300), (1240, 470), (1420, 830)],
    [(600, 900), (760, 760), (980, 700)],
    [(1830, 380), (1760, 560), (1700, 900)],
    [(980, 360), (760, 380), (520, 470)],
]
for m in MAIN:
    path("M" + " L".join(f"{x},{y}" for x, y in m), "none", "#2b3945", 9,
         'fill="none" stroke-linecap="round"')
    path("M" + " L".join(f"{x},{y}" for x, y in m), "none", ROAD_MAIN, 5,
         'fill="none" stroke-linecap="round"')

# прибрежные улицы Ocean Drive / Shore Ave / Boardwalk Lane
for off, wdt in ((46, 5), (88, 5), (128, 4.2)):
    pts = [(x, coast_y(x) - off) for x in range(480, 1640, 24)]
    d = "M" + " L".join(f"{x:.0f},{y:.0f}" for x, y in pts)
    path(d, "none", "#2b3945", wdt + 4, 'fill="none" stroke-linecap="round"')
    path(d, "none", ROAD_MAIN, wdt, 'fill="none" stroke-linecap="round"')
path("M" + " L".join(f"{x:.0f},{coast_y(x) - 46:.0f}" for x in range(480, 1640, 24)),
     "none", "#d9e2e8", 0.8, 'fill="none" stroke-dasharray="9 12" opacity="0.35"')
# подходы к пляжу от Coast Road
for x in range(760, 1600, 90):
    line(x, coast_y(x) - 44, x, coast_y(x) - 10, "#3a4b5b", 2.2, 'opacity="0.8"')

# сетка даунтауна: тротуары + проезжая часть + разметка
for x in range(1010, 1420, 34):
    line(x, 396, x, 790, "#33434f", 5.4, 'opacity="0.85"')
    line(x, 396, x, 790, ROAD_MINOR, 3)
for y in range(400, 800, 32):
    line(1006, y, 1416, y, "#33434f", 5.4, 'opacity="0.85"')
    line(1006, y, 1416, y, ROAD_MINOR, 3)
# машины в центре
for _ in range(48):
    x = rnd.choice([x for x in range(1010, 1420, 34)]) + rnd.uniform(-1.2, 1.2)
    y = rnd.uniform(402, 788)
    car(x, y, math.pi / 2 + rnd.uniform(-0.05, 0.05),
        rnd.choice(["#c9d4dc", "#8a2f2f", "#2f4a6a", "#d8b34a", "#2b2f34"]))
# новые кварталы на пустых пятнах
scatter_blocks(1764, 610, 2040, 840, cell=38, fill=BLOCK, chance=0.42, inset=6)
scatter_blocks(250, 900, 560, 1000, cell=34, fill=BLOCK_HI, chance=0.5, inset=6)

# сетка баррио
for x in range(530, 930, 46):
    line(x, 566, x + 18, 878, "#2a3742", 4.2, 'opacity="0.8"')
    line(x, 566, x + 18, 878, ROAD_MINOR, 2.4)
for y in range(575, 875, 42):
    line(526, y, 928, y, "#2a3742", 4.2, 'opacity="0.8"')
    line(526, y, 928, y, ROAD_MINOR, 2.4)
# пригород: извилистые улицы
for i, (cx, cy, rx, ry) in enumerate([(410, 730, 90, 60), (600, 740, 80, 54),
                                      (430, 840, 84, 46), (620, 840, 70, 40)]):
    add(f'<ellipse cx="{cx}" cy="{cy}" rx="{rx}" ry="{ry}" fill="none" '
        f'stroke="{ROAD_MINOR}" stroke-width="4"/>')
line(300, 800, 700, 800, ROAD_MINOR, 3)

# подъездные пути аэропорта
path("M1750,600 L1900,560 L2050,520", "none", ROAD_MAIN, 5)

# названия улиц
text_along(1520, 268, 1860, 340, "M-1  ·  MARISOL RING", 9, "#d8b34a", 0.7)
text_along(300, 640, 402, 600, "M-1", 9, "#d8b34a", 0.65)
text_along(1120, 396, 1120, 790, "NEON BLVD", 8.5, TEXT_DIM, 0.55)
text_along(1010, 600, 1416, 600, "UNION AVE", 8.5, TEXT_DIM, 0.5)
text_along(530, 700, 930, 700, "CALLE SOL", 8.5, TEXT_DIM, 0.6)
text_along(304, 800, 700, 800, "PALM AVENUE", 8.5, TEXT_DIM, 0.55)
text_along(620, 880, 900, 900, "OCEAN DRIVE", 8.5, "#c9d4dc", 0.55)
text_along(1760, 560, 2000, 525, "PORT ROAD", 8, TEXT_DIM, 0.45)

# --------------------------------------------------------------------------- #
# железная дорога + мосты
# --------------------------------------------------------------------------- #

RAIL_PTS = [(0, 800), (200, 790), (420, 760), (640, 742), (860, 726),
            (1060, 712), (1240, 690), (1420, 706), (1660, 742), (1900, 770),
            (2160, 742), (2400, 720)]
d_rail = "M" + " L".join(f"{x},{y}" for x, y in RAIL_PTS)
path(d_rail, "none", "#111a21", 7)
path(d_rail, "none", RAIL, 2, 'stroke-dasharray="14 10"')

# вокзал
rect(1180, 640, 140, 46, "#2f4553", "#5f7d8f", 2, r=4)
text(1250, 668, "UNION STATION", 12, TEXT, weight=600, spacing=1)

# мосты
for (x1, y1, x2, y2) in [(1330, 700, 1420, 706), (1180, 460, 1230, 470)]:
    line(x1, y1, x2, y2, "#8aa0ad", 8)
    line(x1, y1, x2, y2, "#1b2731", 2, 'stroke-dasharray="6 6"')

# --------------------------------------------------------------------------- #
# аэропорт
# --------------------------------------------------------------------------- #

rect(1760, 300, 420, 250, "#212c35", "#3c5464", 2, r=6)
text(1970, 288, "AEROPUERTO MARISOL", 15, TEXT, weight=700, spacing=2)
add('<g opacity="0.95">')
for (x1, y1, x2, y2) in [(1800, 500, 2130, 340), (1830, 360, 2090, 510)]:
    line(x1, y1, x2, y2, "#1a232b", 26)
    line(x1, y1, x2, y2, "#4a5b68", 20)
    line(x1, y1, x2, y2, "#c9d4dc", 1.6, 'stroke-dasharray="22 16"')
    for t in (0.15, 0.5, 0.85):
        cx = x1 + (x2 - x1) * t
        cy = y1 + (y2 - y1) * t
        line(cx - 6, cy - 6, cx + 6, cy + 6, "#c9d4dc", 1.4)
        line(cx - 6, cy + 6, cx + 6, cy - 6, "#c9d4dc", 1.4)
add("</g>")
rect(1900, 430, 90, 34, "#2c3d49", "#5f7d8f", 1.5, r=3)
text(1945, 452, "TERMINAL", 10, TEXT_DIM, weight=600)
build_airport()

# --------------------------------------------------------------------------- #
# порт
# --------------------------------------------------------------------------- #

# пирсы
for i in range(4):
    x = 1740 + i * 118
    rect(x, 1040, 62, 130, "#2b3945", "#3f5566", 2, r=2)
    for j in range(4):
        rect(x + 6, 1048 + j * 30, 50, 22, ["#3c6a86", "#8a5a3a", "#4d6b4f", "#6a5a86"][j],
             "none", 0, r=1.5)
text(1810, 1010, "PUERTO MARISOL", 15, TEXT, weight=700, spacing=2)
rect(1700, 960, 320, 60, "#243440", "#3f5566", 1.5, r=4)
text(1860, 998, "CONTAINER YARD", 11, TEXT_DIM, spacing=1)

# паромная линия
path("M1625,900 C1700,1080 1650,1300 1600,1450", "none", "#8fb6c8", 2,
     'stroke-dasharray="8 12" opacity="0.45"')
for (cx, cy) in [(1010, 1462), (1600, 1490), (2090, 1420)]:
    text(cx, cy + 5, "CAYO", 11, "#6b5a33", weight=700, spacing=3, opacity=0.75)

# маяк
circle(1640, 1120, 8, "#f6d365", "#8a6d1f", 2)
circle(1640, 1120, 18, "none", "#f6d365", 1, 'opacity="0.4"')

# --------------------------------------------------------------------------- #
# стадион, больница-кампус, университет
# --------------------------------------------------------------------------- #

text(1290, 396, "ESTADIO MARISOL", 12, TEXT_DIM, weight=600, spacing=1)
text(975, 366, "UNIVERSIDAD", 12, TEXT_DIM, weight=600, spacing=1)

# --------------------------------------------------------------------------- #
# набережная / марина
# --------------------------------------------------------------------------- #

rect(1560, 860, 130, 22, "#2b3945", "#3f5566", 1.5, r=2)
for i in range(7):
    line(1572 + i * 16, 868, 1572 + i * 16, 878, "#8aa0ad", 2)
text(1625, 902, "MARINA REAL", 11, TEXT_DIM, weight=600)

# --------------------------------------------------------------------------- #
# кварталы-подписи
# --------------------------------------------------------------------------- #

DISTRICTS = [
    ("DOWNTOWN", 1205, 762, 30),
    ("BARRIO SOL", 725, 700, 22),
    ("OCEAN STRIP", 620, 1058, 20),
    ("SUNSET PALMS", 500, 800, 18),
    ("COLINAS ALTAS", 1640, 250, 18),
    ("ZONA INDUSTRIAL", 1660, 780, 16),
    ("MARINA REAL", 1625, 940, 15),
    ("PORT MARISOL", 1885, 1120, 15),
]
for (name, x, y, size) in DISTRICTS:
    text(x, y, name, size, TEXT, weight=700, spacing=3, opacity=0.95,
         extra='paint-order="stroke" stroke="#0d1117" stroke-width="4"')

for (name, x, y) in [("PANTANO VERDE", 350, 470), ("COSTA VERDE GOLF", 902, 466),
                     ("AEROPUERTO MARISOL", 1970, 288)]:
    text(x, y, name, 14, "#9fd6b8", weight=600, spacing=2, opacity=0.9)

# океан
text(1750, 1330, "OCEANO", 34, "#2d6c8e", weight=700, spacing=14, opacity=0.75)
text(1750, 1372, "MARISOL BAY", 16, "#2d6c8e", weight=600, spacing=8, opacity=0.6)

# --------------------------------------------------------------------------- #
# POI
# --------------------------------------------------------------------------- #

POIS = [
    ("police", 1184, 690, "Центральный участок"),
    ("police", 620, 940, "Участок Sunset"),
    ("hospital", 838, 632, "Госпиталь Marisol"),
    ("hospital", 1610, 604, "Клиника Norte"),
    ("shop", 1046, 442, "Супермаркет"),
    ("shop", 690, 618, "Мини-маркет Barrio"),
    ("shop", 1470, 962, "ТЦ Marina"),
    ("garage", 1186, 872, "Автомастерская"),
    ("garage", 500, 686, "Гараж Sunset"),
    ("safehouse", 776, 1002, "Квартира (сейв)"),
    ("safehouse", 1560, 300, "Вилла Colinas"),
    ("food", 1166, 538, "Кафе"),
    ("food", 656, 1070, "Закусочная на пляже"),
    ("clothes", 1352, 470, "Одежда"),
    ("gun", 1520, 800, "Оружейный"),
    ("fuel", 884, 902, "АЗС"),
    ("fuel", 2054, 706, "АЗС (шоссе)"),
    ("bank", 1122, 606, "Банк"),
    ("fun", 758, 1128, "Пляжный клуб"),
    ("fun", 1290, 330, "Стадион"),
]


def poi(kind, x, y, label):
    color, short = POI[kind]
    circle(x, y, 15, "#0b1218", color, 2.4, 'opacity="0.96"')
    text(x, y + 2.6, short, 7.6, color, weight=700, spacing=0.2)
    text(x, y + 31, label, 10.2, TEXT_DIM, weight=500,
         extra='paint-order="stroke" stroke="#0d1117" stroke-width="3"')


for p in POIS:
    poi(*p)

# --------------------------------------------------------------------------- #
# сетка карты и компас
# --------------------------------------------------------------------------- #

for i in range(1, 8):
    x = i * 300
    line(x, 40, x, H - 40, "#ffffff", 0.7, 'opacity="0.07"')
for j in range(1, 5):
    y = j * 340
    line(40, y, W - 40, y, "#ffffff", 0.7, 'opacity="0.07"')
for i in range(8):
    text(i * 300 + 150, 34, "ABCDEFGH"[i], 13, TEXT_DIM, weight=700, opacity=0.5)
for j in range(5):
    text(24, j * 340 + 190, str(j + 1), 13, TEXT_DIM, weight=700, opacity=0.5)

# компас
add('<g transform="translate(2260,120)">')
circle(0, 0, 44, "#0e1720", "#3f5566", 2, 'opacity="0.9"')
add('<polygon points="0,-32 9,6 0,0 -9,6" fill="#e8eef4"/>')
add('<polygon points="0,32 9,-6 0,0 -9,-6" fill="#54697c"/>')
text(0, -50, "N", 15, TEXT, weight=700)
add("</g>")

# масштаб
add('<g transform="translate(1740,1600)">')
rect(0, 0, 300, 46, "#0e1720", "#3f5566", 1.5, r=6, extra='opacity="0.92"')
for i in range(4):
    rect(18 + i * 68, 18, 68, 8, "#e8eef4" if i % 2 == 0 else "#54697c")
text(18, 40, "0", 10, TEXT_DIM, anchor="start")
text(290, 40, "3 км", 10, TEXT_DIM, anchor="end")
add("</g>")

# заголовок
add('<g transform="translate(48,64)">')
text(0, 0, "MARISOL", 46, TEXT, anchor="start", weight=800, spacing=8)
text(2, 30, "LEONIDA · КАРТА ГОРОДА · КОНЦЕПТ", 13, TEXT_DIM, anchor="start",
     weight=600, spacing=4)
add("</g>")
rect(48, 108, 250, 3, ROAD_HW, r=2, extra='opacity="0.9"')

# легенда
LEGEND = [
    ("police", "Полиция"), ("hospital", "Больница"), ("shop", "Магазины"),
    ("garage", "Гараж / тюнинг"), ("safehouse", "Убежище"), ("food", "Еда"),
    ("clothes", "Одежда"), ("gun", "Оружие"), ("bank", "Банк"),
    ("fuel", "Заправка"), ("port", "Порт"), ("fun", "Развлечения"),
]
add('<g transform="translate(48,1470)">')
rect(0, 0, 470, 196, "#0e1720", "#3f5566", 1.5, r=8, extra='opacity="0.94"')
text(20, 28, "ЛЕГЕНДА", 13, TEXT, anchor="start", weight=700, spacing=3)
for i, (kind, label) in enumerate(LEGEND):
    cx = 30 + (i % 3) * 150
    cy = 62 + (i // 3) * 34
    color, short = POI[kind]
    circle(cx, cy - 4, 9, "#0e1720", color, 2)
    circle(cx, cy - 4, 4.5, color)
    text(cx + 18, cy, label, 11.5, TEXT_DIM, anchor="start")
add("</g>")

# рамка
rect(0, 0, W, H, "none", "#3f5566", 6)

svg = (f'<svg xmlns="http://www.w3.org/2000/svg" width="{W}" height="{H}" '
       f'viewBox="0 0 {W} {H}">\n' + "\n".join(out) + "\n</svg>\n")

here = os.path.dirname(os.path.abspath(__file__))
outdir = os.path.abspath(os.path.join(here, "..", "map"))
os.makedirs(outdir, exist_ok=True)
with open(os.path.join(outdir, "city_map.svg"), "w") as f:
    f.write(svg)
print("game/map/city_map.svg:", len(svg) // 1024, "KB")
