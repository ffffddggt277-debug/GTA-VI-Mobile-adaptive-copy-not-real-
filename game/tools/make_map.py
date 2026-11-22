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

# даунтаун
scatter_blocks(1004, 392, 1412, 792, cell=30, fill="#2b3e4d", chance=0.92, inset=3)
# баррио
scatter_blocks(520, 560, 930, 880, cell=32, fill=BLOCK_HI, chance=0.7, inset=5)
# пригород
scatter_blocks(300, 620, 700, 880, cell=36, fill=BLOCK, chance=0.45, inset=7)
# промзона
scatter_blocks(1560, 700, 1760, 850, cell=34, fill="#2a343c", chance=0.75, inset=4)
# виллы
scatter_blocks(1460, 120, 1830, 380, cell=42, fill=BLOCK_HI, chance=0.5, inset=9)

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

# сетка даунтауна
for x in range(1010, 1420, 34):
    line(x, 396, x, 790, ROAD_MINOR, 3)
for y in range(400, 800, 32):
    line(1006, y, 1416, y, ROAD_MINOR, 3)
# новые кварталы на пустых пятнах
scatter_blocks(1764, 610, 2040, 840, cell=38, fill=BLOCK, chance=0.42, inset=6)
scatter_blocks(250, 900, 560, 1000, cell=34, fill=BLOCK_HI, chance=0.5, inset=6)

# сетка баррио
for x in range(530, 930, 46):
    line(x, 566, x + 18, 878, ROAD_MINOR, 2.4)
for y in range(575, 875, 42):
    line(526, y, 928, y, ROAD_MINOR, 2.4)
# пригород: извилистые улицы
for i, (cx, cy, rx, ry) in enumerate([(410, 730, 90, 60), (600, 740, 80, 54),
                                      (430, 840, 84, 46), (620, 840, 70, 40)]):
    add(f'<ellipse cx="{cx}" cy="{cy}" rx="{rx}" ry="{ry}" fill="none" '
        f'stroke="{ROAD_MINOR}" stroke-width="4"/>')
line(300, 800, 700, 800, ROAD_MINOR, 3)

# подъездные пути аэропорта
path("M1750,600 L1900,560 L2050,520", "none", ROAD_MAIN, 5)

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

add(f'<ellipse cx="1290" cy="330" rx="58" ry="40" fill="#243440" stroke="#5f7d8f" stroke-width="2"/>')
add(f'<ellipse cx="1290" cy="330" rx="40" ry="25" fill="#2f5a3c" stroke="#79b98d" stroke-width="1.5"/>')
text(1290, 396, "ESTADIO MARISOL", 12, TEXT_DIM, weight=600, spacing=1)

rect(900, 250, 150, 90, "#243440", "#3f5566", 1.5, r=4)
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
