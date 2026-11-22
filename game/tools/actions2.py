"""Боевые, полицейские и бытовые действия (дополнение к actions.py)."""

from clips import merge, put
from actions import A, ACT, seated, reach, both_arms


def sided(side, p=0.0, y=0.0, r=0.0, fore=0.0):
    """Добавка к руке нужной стороны: у левой зеркалим yaw/roll."""
    s = "Left" if side == "left" else "Right"
    sign = -1.0 if side == "left" else 1.0
    return {s + "Arm": [p, y * sign, r * sign], s + "ForeArm": [fore, 0, 0]}


# --------------------------------------------------------------------------- #
# удары руками
# --------------------------------------------------------------------------- #

def _punch(kind, side):
    s = "Left" if side == "left" else "Right"
    o = "Right" if side == "left" else "Left"
    sign = -1.0 if side == "left" else 1.0
    windup = {
        f"{s}Arm": [30, 22 * sign, 0], f"{s}ForeArm": [66, 0, 0],
        f"{o}Arm": [26, -18 * sign, 0], f"{o}ForeArm": [70, 0, 0],
        "Spine": [2, -10 * sign, 0], "Hips": [0, -8 * sign, 0],
    }
    if kind == "jab":
        strike = {f"{s}Arm": [88, 24 * sign, 0], f"{s}ForeArm": [4, 0, 0],
                  f"{s}Hand": [-8, 0, 0], f"{o}Arm": [40, -20 * sign, 0],
                  f"{o}ForeArm": [72, 0, 0], "Spine": [3, 20 * sign, 0],
                  "Hips": [0, 12 * sign, 0], "Head": [2, -6 * sign, 0]}
        dur = 0.7
    elif kind == "cross":
        strike = {f"{s}Arm": [92, 18 * sign, 0], f"{s}ForeArm": [2, 0, 0],
                  f"{s}Hand": [-6, 0, 0], f"{o}Arm": [34, -24 * sign, 0],
                  f"{o}ForeArm": [96, 0, 0], "Spine": [4, 30 * sign, 0],
                  "Spine1": [2, 12 * sign, 0], "Hips": [0, 22 * sign, 0],
                  "Head": [3, -10 * sign, 0], "@hips": [0, -2, 3]}
        dur = 0.85
    elif kind == "hook":
        strike = {f"{s}Arm": [72, 62 * sign, 0], f"{s}ForeArm": [78, 0, 0],
                  f"{o}Arm": [40, -22 * sign, 0], f"{o}ForeArm": [84, 0, 0],
                  "Spine": [2, 32 * sign, 0], "Hips": [0, 24 * sign, 0],
                  "Head": [2, -8 * sign, 0]}
        dur = 0.9
    else:  # uppercut
        strike = {f"{s}Arm": [96, 12 * sign, 0], f"{s}ForeArm": [34, 0, 0],
                  f"{s}Hand": [-18, 0, 0], f"{o}Arm": [44, -18 * sign, 0],
                  f"{o}ForeArm": [80, 0, 0], "Spine": [-6, 18 * sign, 0],
                  "Hips": [0, 12 * sign, 0], "Head": [-8, -6 * sign, 0],
                  "@hips": [0, 2, 2]}
        dur = 0.9
    return A(dur, [
        (0.0, {}),
        (0.3, windup),
        (0.5, strike),
        (0.68, blend_back(strike, 0.5)),
        (1.0, {}),
    ], cat="melee", desc=f"Удар рукой ({kind}, {side})")


def blend_back(pose, k=0.5):
    return {key: [v * k for v in val] for key, val in pose.items()}


ACT["punch_jab"] = _punch("jab", "right")
ACT["punch_cross"] = _punch("cross", "right")
ACT["punch_jab_left"] = _punch("jab", "left")
ACT["punch_cross_left"] = _punch("cross", "left")
ACT["punch_hook_l"] = _punch("hook", "left")
ACT["punch_hook_r"] = _punch("hook", "right")
ACT["punch_uppercut"] = _punch("uppercut", "right")

_combo = {}
for i, (t, p) in enumerate([(0.0, {}), (0.22, _punch("jab", "left")["keys"][2][1]),
                            (0.5, _punch("cross", "right")["keys"][2][1]),
                            (0.8, _punch("hook", "left")["keys"][2][1]),
                            (1.0, {})]):
    _combo[t] = p
ACT["punch_combo_3"] = A(1.5, sorted(_combo.items()), cat="melee",
                         desc="Серия из трёх ударов")

ACT["punch_running"] = A(0.9, [
    (0.0, {"Spine": [12, 0, 0], "LeftUpLeg": [26, 0, 0], "RightUpLeg": [-18, 0, 0],
           "LeftArm": [20, 0, 0], "RightArm": [20, 0, 0]}),
    (0.25, {"RightArm": [40, 20, 0], "RightForeArm": [70, 0, 0], "Spine": [10, -12, 0],
            "LeftUpLeg": [40, 0, 0], "LeftLeg": [-40, 0, 0], "RightUpLeg": [-6, 0, 0]}),
    (0.45, {"RightArm": [96, 22, 0], "RightForeArm": [6, 0, 0], "Spine": [16, 22, 0],
            "LeftUpLeg": [52, 0, 0], "LeftLeg": [-30, 0, 0], "RightUpLeg": [12, 0, 0]},
     ),
    (0.7, {"RightArm": [60, 20, 0], "RightForeArm": [50, 0, 0], "Spine": [12, 8, 0],
           "LeftUpLeg": [24, 0, 0], "RightUpLeg": [-10, 0, 0]}),
    (1.0, {"Spine": [12, 0, 0], "LeftUpLeg": [26, 0, 0], "RightUpLeg": [-18, 0, 0],
           "LeftArm": [20, 0, 0], "RightArm": [20, 0, 0]}),
], cat="melee", desc="Бьёт на бегу")

ACT["punch_crouch"] = A(0.9, [
    (0.0, merge(seated(0.7), {})),
    (0.3, merge(seated(0.75), {"RightArm": [44, 22, 0], "RightForeArm": [70, 0, 0],
                               "Spine": [6, -10, 0]})),
    (0.5, merge(seated(0.7), {"RightArm": [92, 20, 0], "RightForeArm": [6, 0, 0],
                              "Spine": [10, 24, 0], "Hips": [0, 14, 0]})),
    (1.0, merge(seated(0.7), {})),
], cat="melee", desc="Бьёт из приседа", crouch=0.7)

ACT["elbow_strike"] = A(0.85, [
    (0.0, {}),
    (0.3, {"RightArm": [40, -40, 0], "RightForeArm": [112, 0, 0], "Spine": [2, -18, 0]}),
    (0.5, {"RightArm": [56, 66, 0], "RightForeArm": [116, 0, 0], "Spine": [4, 30, 0],
           "Hips": [0, 20, 0], "LeftArm": [40, -24, 0], "LeftForeArm": [80, 0, 0]}),
    (1.0, {}),
], cat="melee", desc="Удар локтем")

ACT["headbutt"] = A(0.9, [
    (0.0, {}),
    (0.3, {"Spine": [-10, 0, 0], "Head": [-14, 0, 0], "Neck": [-6, 0, 0]}),
    (0.5, {"Spine": [26, 0, 0], "Head": [22, 0, 0], "Neck": [8, 0, 0], "@hips": [0, -2, 4],
           "RightArm": [30, 18, 0], "LeftArm": [26, -18, 0]}),
    (1.0, {}),
], cat="melee", desc="Удар головой")

ACT["tackle"] = A(1.2, [
    (0.0, {"Spine": [16, 0, 0], "LeftUpLeg": [24, 0, 0], "RightUpLeg": [-16, 0, 0]}),
    (0.3, {"Spine": [30, 0, 0], "LeftUpLeg": [56, 0, 0], "LeftLeg": [-40, 0, 0],
           "RightUpLeg": [-6, 0, 0], "RightLeg": [-20, 0, 0],
           "RightArm": [70, 22, 0], "LeftArm": [66, -22, 0], "@hips": [0, -4, 6]}),
    (0.6, {"Spine": [36, 0, 0], "RightArm": [96, 40, 0], "LeftArm": [92, -40, 0],
           "LeftUpLeg": [40, 0, 0], "RightUpLeg": [20, 0, 0], "@hips": [0, -8, 10]}),
    (1.0, merge(seated(0.8), {"Spine": [24, 0, 0], "RightArm": [70, 30, 0],
                              "LeftArm": [66, -30, 0]})),
], cat="melee", desc="Захват и свалка")

ACT["grapple_throw"] = A(1.6, [
    (0.0, {}),
    (0.3, {"RightArm": [64, 24, 0], "LeftArm": [60, -24, 0],
           "RightForeArm": [50, 0, 0], "LeftForeArm": [54, 0, 0], "Spine": [14, 0, 0]}),
    (0.6, {"RightArm": [40, 40, 0], "LeftArm": [76, -46, 0],
           "RightForeArm": [70, 0, 0], "LeftForeArm": [30, 0, 0],
           "Spine": [10, 34, 0], "Hips": [0, 24, 0]}),
    (0.85, {"Spine": [18, 40, 0], "RightArm": [30, 50, 0], "LeftArm": [90, -50, 0],
            "LeftForeArm": [8, 0, 0]}),
    (1.0, {}),
], cat="melee", desc="Бросок через бедро")

ACT["kick_front"] = A(1.1, [
    (0.0, {}),
    (0.25, {"RightUpLeg": [30, 0, 0], "RightLeg": [-56, 0, 0], "Spine": [-4, 0, 0]}),
    (0.45, {"RightUpLeg": [92, 0, 0], "RightLeg": [-6, 0, 0], "RightFoot": [-18, 0, 0],
            "Spine": [-10, 0, 0], "LeftArm": [26, -20, 0], "RightArm": [20, 26, 0],
            "@hips": [0, 2, -2]}),
    (0.7, {"RightUpLeg": [40, 0, 0], "RightLeg": [-46, 0, 0], "Spine": [-2, 0, 0]}),
    (1.0, {}),
], cat="melee", desc="Прямой удар ногой")

ACT["kick_low"] = A(1.0, [
    (0.0, {}),
    (0.25, {"RightUpLeg": [22, 0, 0], "RightLeg": [-40, 0, 0], "Spine": [4, 0, 0]}),
    (0.45, {"RightUpLeg": [40, 12, 0], "RightLeg": [-14, 0, 0], "RightFoot": [-10, 0, 0],
            "Spine": [10, -8, 0], "RightArm": [26, 22, 0], "LeftArm": [22, -22, 0]}),
    (1.0, {}),
], cat="melee", desc="Лоу-кик по ноге")

ACT["kick_side"] = A(1.1, [
    (0.0, {}),
    (0.25, {"RightUpLeg": [16, -20, 12], "RightLeg": [-50, 0, 0], "Spine": [0, -12, -8]}),
    (0.5, {"RightUpLeg": [40, -46, 26], "RightLeg": [-10, 0, 0], "RightFoot": [-14, 0, 0],
           "Spine": [-4, -20, -14], "LeftArm": [30, -26, 0], "RightArm": [16, 30, 0],
           "@hips": [2, 0, 0]}),
    (1.0, {}),
], cat="melee", desc="Боковой удар ногой")

ACT["knee_strike"] = A(0.9, [
    (0.0, {}),
    (0.3, {"RightUpLeg": [20, 0, 0], "RightLeg": [-40, 0, 0], "Spine": [4, -6, 0],
           "RightArm": [50, 30, 0], "LeftArm": [46, -30, 0]}),
    (0.5, {"RightUpLeg": [80, 0, 0], "RightLeg": [-66, 0, 0], "Spine": [12, -10, 0],
           "RightArm": [56, 36, 0], "LeftArm": [52, -36, 0], "@hips": [0, 2, 2]}),
    (1.0, {}),
], cat="melee", desc="Удар коленом")

ACT["stomp"] = A(1.0, [
    (0.0, {}),
    (0.25, {"RightUpLeg": [50, 0, 0], "RightLeg": [-80, 0, 0], "Spine": [8, 0, 0]}),
    (0.45, {"RightUpLeg": [30, 0, 0], "RightLeg": [-20, 0, 0], "RightFoot": [16, 0, 0],
            "Spine": [18, 0, 0], "RightArm": [40, 24, 0], "LeftArm": [36, -24, 0]}),
    (1.0, {}),
], cat="melee", desc="Затаптывает лежащего")

ACT["block_hold"] = A(2.0, [
    (0.0, {"Spine": [8, 0, 0], "RightArm": [54, 24, 0], "RightForeArm": [92, 0, 0],
           "LeftArm": [50, -24, 0], "LeftForeArm": [94, 0, 0], "Head": [8, 0, 0],
           "LeftUpLeg": [10, 0, 0], "RightUpLeg": [-6, 0, 0]}),
    (0.5, {"Spine": [9, 0, 0], "RightArm": [55, 23, 0], "RightForeArm": [90, 0, 0],
           "LeftArm": [51, -23, 0], "LeftForeArm": [92, 0, 0], "Head": [9, 0, 0],
           "LeftUpLeg": [10, 0, 0], "RightUpLeg": [-6, 0, 0]}),
    (1.0, {"Spine": [8, 0, 0], "RightArm": [54, 24, 0], "RightForeArm": [92, 0, 0],
           "LeftArm": [50, -24, 0], "LeftForeArm": [94, 0, 0], "Head": [8, 0, 0],
           "LeftUpLeg": [10, 0, 0], "RightUpLeg": [-6, 0, 0]}),
], cat="melee", desc="Держит блок (цикл)", loop=True)

ACT["block_impact"] = A(0.8, [
    (0.0, {"Spine": [8, 0, 0], "RightArm": [54, 24, 0], "RightForeArm": [92, 0, 0],
           "LeftArm": [50, -24, 0], "LeftForeArm": [94, 0, 0]}),
    (0.25, {"Spine": [-4, 0, 0], "RightArm": [44, 30, 0], "RightForeArm": [104, 0, 0],
            "LeftArm": [40, -30, 0], "LeftForeArm": [106, 0, 0], "@hips": [0, -3, -4],
            "Head": [-6, 0, 0]}),
    (1.0, {"Spine": [8, 0, 0], "RightArm": [54, 24, 0], "RightForeArm": [92, 0, 0],
           "LeftArm": [50, -24, 0], "LeftForeArm": [94, 0, 0]}),
], cat="melee", desc="Держит удар в блок")

ACT["dodge_left"] = A(0.8, [
    (0.0, {}),
    (0.3, {"@hips": [8, -3, 0], "Spine": [0, 0, 10], "LeftUpLeg": [10, -10, 0],
           "RightUpLeg": [-4, -6, 0], "Head": [0, 0, 8]}),
    (0.7, {"@hips": [3, -1, 0], "Spine": [0, 0, 4]}),
    (1.0, {}),
], cat="melee", desc="Уклон влево")

ACT["dodge_right"] = A(0.8, [
    (0.0, {}),
    (0.3, {"@hips": [-8, -3, 0], "Spine": [0, 0, -10], "RightUpLeg": [10, 10, 0],
           "LeftUpLeg": [-4, 6, 0], "Head": [0, 0, -8]}),
    (0.7, {"@hips": [-3, -1, 0], "Spine": [0, 0, -4]}),
    (1.0, {}),
], cat="melee", desc="Уклон вправо")

ACT["dodge_back"] = A(0.8, [
    (0.0, {}),
    (0.3, {"@hips": [0, -4, -8], "Spine": [-10, 0, 0], "LeftUpLeg": [16, 0, 0],
           "RightUpLeg": [10, 0, 0], "LeftLeg": [-24, 0, 0], "RightLeg": [-18, 0, 0]}),
    (0.7, {"@hips": [0, -2, -3], "Spine": [-4, 0, 0]}),
    (1.0, {}),
], cat="melee", desc="Отскок назад")

ACT["bat_swing"] = A(1.1, [
    (0.0, {}),
    (0.3, {"RightArm": [30, -50, 0], "RightForeArm": [50, 0, 0], "Spine": [0, -24, 0],
           "Hips": [0, -14, 0], "LeftArm": [40, -20, 0], "LeftForeArm": [80, 0, 0]}),
    (0.55, {"RightArm": [76, 60, 0], "RightForeArm": [20, 0, 0], "Spine": [6, 30, 0],
            "Hips": [0, 22, 0], "LeftArm": [30, -30, 0], "LeftForeArm": [70, 0, 0]}),
    (1.0, {}),
], cat="melee", desc="Удар битой сбоку")

ACT["bat_overhead"] = A(1.2, [
    (0.0, {}),
    (0.3, {"RightArm": [110, -10, 0], "RightForeArm": [40, 0, 0], "LeftArm": [100, -16, 0],
           "LeftForeArm": [50, 0, 0], "Spine": [-12, 0, 0], "Head": [-10, 0, 0]}),
    (0.55, {"RightArm": [56, 6, 0], "RightForeArm": [10, 0, 0], "LeftArm": [48, -8, 0],
            "LeftForeArm": [16, 0, 0], "Spine": [26, 0, 0], "Head": [14, 0, 0],
            "@hips": [0, -4, 2]}),
    (1.0, {}),
], cat="melee", desc="Удар битой сверху")

ACT["crowbar_swing"] = A(1.0, [
    (0.0, {}),
    (0.3, {"RightArm": [40, -34, 0], "RightForeArm": [58, 0, 0], "Spine": [0, -16, 0]}),
    (0.55, {"RightArm": [84, 34, 0], "RightForeArm": [16, 0, 0], "Spine": [8, 22, 0],
            "Hips": [0, 16, 0]}),
    (1.0, {}),
], cat="melee", desc="Удар монтировкой")

ACT["knife_slash"] = A(0.8, [
    (0.0, {}),
    (0.25, {"RightArm": [30, 24, 0], "RightForeArm": [80, 0, 0], "Spine": [0, -12, 0]}),
    (0.45, {"RightArm": [76, -30, 0], "RightForeArm": [24, 0, 0], "Spine": [4, 18, 0]}),
    (1.0, {}),
], cat="melee", desc="Режущий удар ножом")

ACT["knife_stab"] = A(0.8, [
    (0.0, {}),
    (0.25, {"RightArm": [40, 16, 0], "RightForeArm": [96, 0, 0], "Spine": [4, -10, 0]}),
    (0.45, {"RightArm": [86, 14, 0], "RightForearm": [10, 0, 0], "Spine": [10, 14, 0],
            "@hips": [0, 0, 3]}),
    (1.0, {}),
], cat="melee", desc="Колющий удар ножом")

# --------------------------------------------------------------------------- #
# оружие: достать / убрать / перезарядка
# --------------------------------------------------------------------------- #

ACT["draw_pistol"] = A(1.1, [
    (0.0, {}),
    (0.2, {"RightArm": [6, 34, 0], "RightForeArm": [26, 0, 0], "Spine": [4, 8, 0]}),
    (0.45, {"RightArm": [46, 22, 0], "RightForeArm": [78, 0, 0], "Spine": [2, 6, 0]}),
    (0.75, {"RightArm": [80, 24, 0], "RightForeArm": [10, 0, 0], "Spine": [4, 8, 0],
            "LeftArm": [68, -32, 0], "LeftForeArm": [22, 0, 0]}),
    (1.0, {"RightArm": [78, 24, 0], "RightForeArm": [8, 0, 0], "Spine": [4, 8, 0],
           "LeftArm": [70, -34, 0], "LeftForeArm": [20, 0, 0], "Head": [-2, -4, 0]}),
], cat="firearm", desc="Достаёт пистолет")

ACT["holster_pistol"] = A(1.0, [
    (0.0, {"RightArm": [78, 24, 0], "RightForeArm": [8, 0, 0], "Spine": [4, 8, 0],
           "LeftArm": [70, -34, 0], "LeftForeArm": [20, 0, 0]}),
    (0.35, {"RightArm": [40, 26, 0], "RightForeArm": [72, 0, 0], "Spine": [2, 6, 0],
            "LeftArm": [30, -20, 0], "LeftForeArm": [50, 0, 0]}),
    (0.7, {"RightArm": [8, 34, 0], "RightForeArm": [28, 0, 0]}),
    (1.0, {}),
], cat="firearm", desc="Убирает пистолет в кобуру")

ACT["draw_rifle"] = A(1.5, [
    (0.0, {}),
    (0.2, {"RightArm": [24, 36, 0], "LeftArm": [24, -36, 0], "Spine": [6, 0, 0],
           "RightForeArm": [40, 0, 0], "LeftForeArm": [40, 0, 0]}),
    (0.5, {"RightArm": [58, 40, 0], "LeftArm": [86, -30, 0], "Spine": [8, 10, 0],
           "RightForeArm": [64, 0, 0], "LeftForeArm": [30, 0, 0]}),
    (0.8, {"RightArm": [80, 34, 0], "RightForeArm": [42, 0, 0],
           "LeftArm": [76, -40, 0], "LeftForeArm": [52, 0, 0], "Spine": [6, 14, 0]}),
    (1.0, {"RightArm": [80, 34, 0], "RightForeArm": [42, 0, 0],
           "LeftArm": [76, -40, 0], "LeftForeArm": [52, 0, 0], "Spine": [6, 14, 0],
           "Head": [2, -6, 0]}),
], cat="firearm", desc="Берёт винтовку на изготовку")

ACT["holster_rifle"] = A(1.4, [
    (0.0, {"RightArm": [80, 34, 0], "RightForeArm": [42, 0, 0],
           "LeftArm": [76, -40, 0], "LeftForeArm": [52, 0, 0], "Spine": [6, 14, 0]}),
    (0.4, {"RightArm": [50, 36, 0], "RightForeArm": [70, 0, 0],
           "LeftArm": [40, -34, 0], "LeftForeArm": [60, 0, 0], "Spine": [4, 8, 0]}),
    (0.75, {"RightArm": [20, 34, 0], "LeftArm": [20, -34, 0], "Spine": [2, 0, 0]}),
    (1.0, {}),
], cat="firearm", desc="Убирает винтовку за спину")

_mag = {"LeftArm": [66, -30, 0], "LeftForeArm": [96, 0, 0], "Spine": [10, 6, 0],
        "Head": [10, -6, 0]}

ACT["reload_pistol"] = A(2.0, [
    (0.0, {"RightArm": [74, 22, 0], "RightForeArm": [24, 0, 0], "Spine": [4, 8, 0]}),
    (0.25, {"RightArm": [46, 24, 0], "RightForeArm": [80, 0, 0],
            "LeftArm": [50, -26, 0], "LeftForeArm": [90, 0, 0], "Spine": [8, 6, 0],
            "Head": [12, -4, 0]}),
    (0.5, {"RightArm": [56, 20, 0], "RightForeArm": [60, 0, 0], "LeftArm": [46, -34, 0],
           "LeftForeArm": [84, 0, 0], "Spine": [4, 14, 0], "Head": [14, -8, 0]}),
    (0.75, {"RightArm": [72, 24, 0], "RightForeArm": [30, 0, 0], "LeftArm": [52, -30, 0],
            "LeftForeArm": [70, 0, 0], "Spine": [6, 10, 0], "Head": [8, -6, 0]}),
    (1.0, {"RightArm": [78, 24, 0], "RightForeArm": [8, 0, 0], "Spine": [4, 8, 0],
           "LeftArm": [70, -34, 0], "LeftForeArm": [20, 0, 0], "Head": [-2, -4, 0]}),
], cat="firearm", desc="Перезаряжает пистолет")

ACT["reload_rifle"] = A(2.6, [
    (0.0, {"RightArm": [80, 34, 0], "RightForeArm": [42, 0, 0],
           "LeftArm": [76, -40, 0], "LeftForeArm": [52, 0, 0], "Spine": [6, 14, 0]}),
    (0.25, {"RightArm": [70, 36, 0], "RightForeArm": [58, 0, 0], "LeftArm": [58, -20, 0],
            "LeftForeArm": [86, 0, 0], "Spine": [10, 12, 0], "Head": [14, -8, 0]}),
    (0.5, {"RightArm": [64, 38, 0], "RightForeArm": [70, 0, 0], "LeftArm": [40, -18, 0],
           "LeftForeArm": [74, 0, 0], "Spine": [8, 10, 0], "Head": [16, -10, 0]}),
    (0.75, {"RightArm": [76, 36, 0], "RightForeArm": [50, 0, 0], "LeftArm": [62, -28, 0],
            "LeftForeArm": [60, 0, 0], "Spine": [8, 12, 0], "Head": [10, -6, 0]}),
    (1.0, {"RightArm": [80, 34, 0], "RightForeArm": [42, 0, 0],
           "LeftArm": [76, -40, 0], "LeftForeArm": [52, 0, 0], "Spine": [6, 14, 0]}),
], cat="firearm", desc="Перезаряжает винтовку")

ACT["reload_shotgun"] = A(3.0, [
    (0.0, {"RightArm": [84, 30, 0], "RightForeArm": [30, 0, 0],
           "LeftArm": [80, -34, 0], "LeftForeArm": [26, 0, 0], "Spine": [8, 12, 0]}),
    (0.3, {"RightArm": [64, 28, 0], "RightForeArm": [60, 0, 0], "LeftArm": [66, -26, 0],
           "LeftForeArm": [60, 0, 0], "Spine": [10, 8, 0], "Head": [16, -6, 0]}),
    (0.55, {"RightArm": [72, 26, 0], "RightForeArm": [46, 0, 0], "LeftArm": [72, -30, 0],
            "LeftForeArm": [44, 0, 0], "Spine": [10, 10, 0], "Head": [12, -6, 0]}),
    (0.8, {"RightArm": [80, 28, 0], "RightForeArm": [36, 0, 0], "LeftArm": [78, -34, 0],
           "LeftForeArm": [34, 0, 0], "Spine": [8, 12, 0]}),
    (1.0, {"RightArm": [84, 30, 0], "RightForeArm": [30, 0, 0],
           "LeftArm": [80, -34, 0], "LeftForeArm": [26, 0, 0], "Spine": [8, 12, 0]}),
], cat="firearm", desc="Перезаряжает дробовик")

# --------------------------------------------------------------------------- #
# полиция
# --------------------------------------------------------------------------- #

ACT["radio_call"] = A(2.6, [
    (0.0, {"RightArm": [56, -18, 0], "RightForeArm": [104, 0, 0], "RightHand": [14, 8, 0],
           "Head": [2, 4, 0]}),
    (0.4, {"RightArm": [58, -20, 0], "RightForeArm": [108, 0, 0], "RightHand": [16, 8, 0],
           "Head": [3, 6, 0], "Spine": [1, 2, 0]}),
    (0.7, {"RightArm": [54, -14, 0], "RightForeArm": [96, 0, 0], "RightHand": [10, 6, 0],
           "Head": [0, 0, 0]}),
    (1.0, {"RightArm": [56, -18, 0], "RightForeArm": [104, 0, 0], "RightHand": [14, 8, 0],
           "Head": [2, 4, 0]}),
], cat="police", desc="Говорит по рации (цикл)", loop=True)

ACT["write_ticket"] = A(3.2, [
    (0.0, {"RightArm": [52, 20, 0], "RightForeArm": [96, 0, 0], "LeftArm": [50, -22, 0],
           "LeftForeArm": [92, 0, 0], "Head": [16, -6, 0], "Spine": [6, 0, 0]}),
    (0.35, {"RightArm": [50, 22, 0], "RightForeArm": [100, 0, 0], "LeftArm": [50, -22, 0],
            "LeftForeArm": [92, 0, 0], "Head": [16, -4, 0], "Spine": [6, 0, 0],
            "RightHand": [0, 6, 0]}),
    (0.7, {"RightArm": [54, 18, 0], "RightForeArm": [92, 0, 0], "LeftArm": [50, -22, 0],
           "LeftForeArm": [92, 0, 0], "Head": [14, -8, 0], "Spine": [6, 0, 0],
           "RightHand": [0, -6, 0]}),
    (1.0, {"RightArm": [52, 20, 0], "RightForeArm": [96, 0, 0], "LeftArm": [50, -22, 0],
           "LeftForeArm": [92, 0, 0], "Head": [16, -6, 0], "Spine": [6, 0, 0]}),
], cat="police", desc="Выписывает штраф")

ACT["pat_down"] = A(2.4, [
    (0.0, {**reach("Right", 60, 14, 50), "Spine": [8, -6, 0], "LeftArm": [30, -12, 0],
           "LeftForeArm": [60, 0, 0]}),
    (0.3, merge(seated(0.5), {**reach("Right", 56, 16, 40, 10), "Spine": [18, -8, 0],
                              "Head": [10, -6, 0]})),
    (0.55, merge(seated(0.5), {**reach("Right", 46, 20, 60, 8), **reach("Left", 52, 18, 66),
                               "Spine": [16, 6, 0], "Head": [12, 4, 0]})),
    (0.8, merge(seated(0.35), {**reach("Right", 40, 14, 70, 4), "Spine": [10, -4, 0]})),
    (1.0, {**reach("Right", 60, 14, 50), "Spine": [8, -6, 0], "LeftArm": [30, -12, 0],
           "LeftForeArm": [60, 0, 0]}),
], cat="police", desc="Обыскивает задержанного")

ACT["arrest_push"] = A(1.8, [
    (0.0, {}),
    (0.25, {"RightArm": [70, 20, 0], "RightForeArm": [30, 0, 0], "LeftArm": [66, -20, 0],
            "LeftForeArm": [36, 0, 0], "Spine": [8, 0, 0]}),
    (0.55, {"RightArm": [88, 12, 0], "RightForeArm": [6, 0, 0], "LeftArm": [84, -12, 0],
            "LeftForeArm": [10, 0, 0], "Spine": [16, 0, 0], "@hips": [0, -2, 5],
            "LeftUpLeg": [18, 0, 0], "RightUpLeg": [-8, 0, 0]}),
    (0.8, {"RightArm": [70, 16, 0], "RightForeArm": [40, 0, 0], "LeftArm": [66, -16, 0],
           "LeftForeArm": [44, 0, 0], "Spine": [8, 0, 0]}),
    (1.0, {}),
], cat="police", desc="Прижимает задержанного к машине")

ACT["taser_draw"] = A(1.0, [
    (0.0, {}),
    (0.3, {"RightArm": [30, 24, 0], "RightForeArm": [70, 0, 0], "Spine": [4, 6, 0]}),
    (0.7, {"RightArm": [84, 18, 0], "RightForeArm": [10, 0, 0], "Spine": [2, 10, 0],
           "LeftArm": [50, -24, 0], "LeftForeArm": [60, 0, 0]}),
    (1.0, {"RightArm": [86, 16, 0], "RightForeArm": [8, 0, 0], "Spine": [2, 10, 0],
           "LeftArm": [52, -26, 0], "LeftForeArm": [64, 0, 0], "Head": [0, -6, 0]}),
], cat="police", desc="Достаёт тазер")

ACT["taser_fire"] = A(1.0, [
    (0.0, {"RightArm": [86, 16, 0], "RightForeArm": [8, 0, 0], "Spine": [2, 10, 0],
           "LeftArm": [52, -26, 0], "LeftForeArm": [64, 0, 0]}),
    (0.2, {"RightArm": [90, 12, 0], "RightForearm": [0, 0, 0], "Spine": [8, 6, 0],
           "@hips": [0, 0, -2], "RightArm": [-6, 0, 0]}),
    (0.6, {"RightArm": [86, 16, 0], "RightForeArm": [8, 0, 0], "Spine": [4, 10, 0]}),
    (1.0, {"RightArm": [86, 16, 0], "RightForeArm": [8, 0, 0], "Spine": [2, 10, 0],
           "LeftArm": [52, -26, 0], "LeftForeArm": [64, 0, 0]}),
], cat="police", desc="Стреляет из тазера")

ACT["baton_draw"] = A(0.9, [
    (0.0, {}),
    (0.35, {"RightArm": [10, 36, 0], "RightForeArm": [40, 0, 0]}),
    (0.7, {"RightArm": [52, 26, 0], "RightForeArm": [64, 0, 0], "Spine": [4, 8, 0]}),
    (1.0, {"RightArm": [56, 24, 0], "RightForeArm": [78, 0, 0], "Spine": [6, 6, 0]}),
], cat="police", desc="Достаёт дубинку")

ACT["baton_swing"] = A(1.0, [
    (0.0, {"RightArm": [56, 24, 0], "RightForeArm": [78, 0, 0], "Spine": [6, 6, 0]}),
    (0.3, {"RightArm": [40, -40, 0], "RightForeArm": [70, 0, 0], "Spine": [2, -20, 0]}),
    (0.55, {"RightArm": [80, 48, 0], "RightForeArm": [20, 0, 0], "Spine": [8, 26, 0],
            "Hips": [0, 18, 0]}),
    (1.0, {"RightArm": [56, 24, 0], "RightForeArm": [78, 0, 0], "Spine": [6, 6, 0]}),
], cat="police", desc="Бьёт дубинкой")

ACT["shield_block"] = A(2.0, [
    (0.0, {"LeftArm": [78, 26, 0], "LeftForeArm": [64, 0, 0], "Spine": [10, 10, 0],
           "RightArm": [30, 20, 0], "RightForeArm": [70, 0, 0], "Head": [6, -4, 0],
           "LeftUpLeg": [14, 0, 0], "LeftLeg": [-24, 0, 0], "RightUpLeg": [-8, 0, 0],
           "RightLeg": [-16, 0, 0], "@hips": [0, -8, 0]}),
    (0.5, {"LeftArm": [80, 24, 0], "LeftForeArm": [62, 0, 0], "Spine": [11, 10, 0],
           "RightArm": [31, 20, 0], "RightForeArm": [69, 0, 0], "Head": [7, -4, 0],
           "LeftUpLeg": [14, 0, 0], "LeftLeg": [-24, 0, 0], "RightUpLeg": [-8, 0, 0],
           "RightLeg": [-16, 0, 0], "@hips": [0, -7, 0]}),
    (1.0, {"LeftArm": [78, 26, 0], "LeftForeArm": [64, 0, 0], "Spine": [10, 10, 0],
           "RightArm": [30, 20, 0], "RightForeArm": [70, 0, 0], "Head": [6, -4, 0],
           "LeftUpLeg": [14, 0, 0], "LeftLeg": [-24, 0, 0], "RightUpLeg": [-8, 0, 0],
           "RightLeg": [-16, 0, 0], "@hips": [0, -8, 0]}),
], cat="police", desc="Держит щит (цикл)", loop=True)

ACT["shield_bash"] = A(0.9, [
    (0.0, {"LeftArm": [78, 26, 0], "LeftForeArm": [64, 0, 0], "Spine": [10, 10, 0]}),
    (0.3, {"LeftArm": [60, 30, 0], "LeftForeArm": [76, 0, 0], "Spine": [4, 2, 0]}),
    (0.5, {"LeftArm": [92, 20, 0], "LeftForeArm": [40, 0, 0], "Spine": [16, 16, 0],
           "@hips": [0, -2, 6], "LeftUpLeg": [24, 0, 0], "LeftLeg": [-30, 0, 0]}),
    (1.0, {"LeftArm": [78, 26, 0], "LeftForeArm": [64, 0, 0], "Spine": [10, 10, 0]}),
], cat="police", desc="Толкает щитом")

ACT["flashlight_scan"] = A(4.0, [
    (0.0, {"RightArm": [74, 20, 0], "RightForeArm": [14, 0, 0], "LeftArm": [56, -22, 0],
           "LeftForeArm": [70, 0, 0], "Spine": [4, 8, 0], "Head": [-2, -6, 0]}),
    (0.35, {"RightArm": [74, 20, 0], "RightForeArm": [14, 0, 0], "LeftArm": [56, -22, 0],
            "LeftForeArm": [70, 0, 0], "Spine": [4, -4, 0], "Head": [-2, 6, 0]}),
    (0.7, {"RightArm": [78, 18, 0], "RightForeArm": [10, 0, 0], "LeftArm": [56, -22, 0],
           "LeftForeArm": [70, 0, 0], "Spine": [4, 12, 0], "Head": [-4, -10, 0]}),
    (1.0, {"RightArm": [74, 20, 0], "RightForeArm": [14, 0, 0], "LeftArm": [56, -22, 0],
           "LeftForeArm": [70, 0, 0], "Spine": [4, 8, 0], "Head": [-2, -6, 0]}),
], cat="police", desc="Светит фонарём (цикл)", loop=True)

ACT["direct_traffic"] = A(3.0, [
    (0.0, {"RightArm": [88, 26, 0], "RightForeArm": [4, 0, 0], "RightHand": [-8, 0, 0],
           "LeftArm": [24, -14, 0], "LeftForeArm": [40, 0, 0], "Spine": [2, 6, 0]}),
    (0.5, {"RightArm": [70, -30, 0], "RightForeArm": [10, 0, 0], "RightHand": [-8, 0, 0],
           "LeftArm": [24, -14, 0], "LeftForeArm": [40, 0, 0], "Spine": [2, -8, 0]}),
    (1.0, {"RightArm": [88, 26, 0], "RightForeArm": [4, 0, 0], "RightHand": [-8, 0, 0],
           "LeftArm": [24, -14, 0], "LeftForeArm": [40, 0, 0], "Spine": [2, 6, 0]}),
], cat="police", desc="Регулирует движение (цикл)", loop=True)

ACT["suspect_kneel"] = A(2.0, [
    (0.0, {}),
    (0.35, merge(seated(0.55), {"Spine": [8, 0, 0], "Head": [10, 0, 0]})),
    (0.7, merge(seated(1.0), {"Spine": [10, 0, 0], "Head": [12, 0, 0],
                              "RightArm": [20, 24, 0], "LeftArm": [18, -24, 0],
                              "RightForeArm": [40, 0, 0], "LeftForeArm": [40, 0, 0],
                              "LeftUpLeg": [30, 0, 0], "RightUpLeg": [30, 0, 0]})),
    (1.0, merge(seated(1.1), {"Spine": [10, 0, 0], "Head": [14, 0, 0],
                              "RightArm": [26, 30, 0], "LeftArm": [24, -30, 0],
                              "RightForeArm": [50, 0, 0], "LeftForeArm": [50, 0, 0],
                              "LeftUpLeg": [34, 0, 0], "RightUpLeg": [34, 0, 0],
                              "@hips": [0, -6, 0]})),
], cat="police", desc="Ставит задержанного на колени")

ACT["surrender"] = A(1.6, [
    (0.0, {}),
    (0.4, {**both_arms(104, 22, 6), "Spine": [-4, 0, 0], "Head": [-6, 0, 0],
           "LeftUpLeg": [10, 0, 0], "RightUpLeg": [8, 0, 0], "LeftLeg": [-16, 0, 0],
           "RightLeg": [-14, 0, 0], "@hips": [0, -4, 0]}),
    (1.0, {**both_arms(106, 24, 4), "Spine": [-4, 0, 0], "Head": [-4, 0, 0],
           "LeftUpLeg": [10, 0, 0], "RightUpLeg": [8, 0, 0], "LeftLeg": [-16, 0, 0],
           "RightLeg": [-14, 0, 0], "@hips": [0, -4, 0]}),
], cat="reaction", desc="Поднимает руки (сдаётся)")

# --------------------------------------------------------------------------- #
# гражданские
# --------------------------------------------------------------------------- #

ACT["groceries"] = A(2.4, [
    (0.0, {**both_arms(70, 18, 30), "Spine": [10, 0, 0], "LeftUpLeg": [20, 0, 0],
           "RightUpLeg": [-12, 0, 0]}),
    (0.3, {"LeftUpLeg": [40, 0, 0], "LeftLeg": [-50, 0, 0], "RightUpLeg": [-6, 0, 0],
           "RightLeg": [-10, 0, 0], "Spine": [16, 0, 0], "@hips": [0, 2, 3]}),
    (0.6, {"LeftUpLeg": [10, 0, 0], "RightUpLeg": [22, 0, 0], "RightLeg": [-40, 0, 0],
           "Spine": [12, 0, 0], "@hips": [0, 1, 1]}),
    (1.0, {**both_arms(70, 18, 30), "Spine": [10, 0, 0], "LeftUpLeg": [20, 0, 0],
           "RightUpLeg": [-12, 0, 0]}),
], cat="civilian", desc="Толкает тележку (цикл)", loop=True)

ACT["dance"] = A(2.0, [
    (0.0, {"RightArm": [30, 20, -40], "LeftArm": [30, -20, 40], "Spine": [0, 10, 8],
           "Hips": [0, -10, 0], "LeftUpLeg": [10, 0, 0], "RightUpLeg": [-6, 0, 0],
           "Head": [0, 8, 0]}),
    (0.25, {"RightArm": [80, 10, -20], "LeftArm": [20, -30, 30], "Spine": [4, -10, -6],
            "Hips": [0, 12, 0], "LeftUpLeg": [-6, 0, 0], "RightUpLeg": [12, 0, 0],
            "Head": [0, -8, 0], "@hips": [2, 0, 0]}),
    (0.5, {"RightArm": [30, 20, -40], "LeftArm": [30, -20, 40], "Spine": [0, 10, 8],
           "Hips": [0, -10, 0], "LeftUpLeg": [10, 0, 0], "RightUpLeg": [-6, 0, 0],
           "Head": [0, 8, 0]}),
    (0.75, {"RightArm": [20, 30, -30], "LeftArm": [80, -10, 20], "Spine": [4, 10, 6],
            "Hips": [0, -12, 0], "LeftUpLeg": [12, 0, 0], "RightUpLeg": [-6, 0, 0],
            "Head": [0, 8, 0], "@hips": [-2, 0, 0]}),
    (1.0, {"RightArm": [30, 20, -40], "LeftArm": [30, -20, 40], "Spine": [0, 10, 8],
           "Hips": [0, -10, 0], "LeftUpLeg": [10, 0, 0], "RightUpLeg": [-6, 0, 0],
           "Head": [0, 8, 0]}),
], cat="civilian", desc="Танцует (цикл)", loop=True)

ACT["chat"] = A(2.6, [
    (0.0, {"RightArm": [40, 10, 0], "RightForeArm": [70, 0, 0], "LeftArm": [30, -8, 0],
           "LeftForeArm": [60, 0, 0], "Spine": [2, -4, 0], "Head": [0, 6, 0]}),
    (0.35, {"RightArm": [50, 18, 0], "RightForeArm": [84, 0, 0], "LeftArm": [30, -8, 0],
            "LeftForeArm": [60, 0, 0], "Spine": [2, 4, 0], "Head": [2, -4, 0]}),
    (0.7, {"RightArm": [36, 6, 0], "RightForeArm": [62, 0, 0], "LeftArm": [34, -12, 0],
           "LeftForeArm": [66, 0, 0], "Spine": [2, -6, 0], "Head": [0, 8, 0]}),
    (1.0, {"RightArm": [40, 10, 0], "RightForeArm": [70, 0, 0], "LeftArm": [30, -8, 0],
           "LeftForeArm": [60, 0, 0], "Spine": [2, -4, 0], "Head": [0, 6, 0]}),
], cat="civilian", desc="Разговаривает жестами (цикл)", loop=True)

ACT["laugh"] = A(1.8, [
    (0.0, {}),
    (0.3, {"Spine": [-6, 0, 0], "Head": [-12, 0, 0], "RightArm": [24, 14, 0],
           "LeftArm": [22, -14, 0], "RightForeArm": [50, 0, 0], "LeftForeArm": [50, 0, 0]}),
    (0.5, {"Spine": [2, 0, 0], "Head": [2, 0, 0], "RightArm": [26, 16, 0],
           "LeftArm": [24, -16, 0], "RightForeArm": [56, 0, 0], "LeftForeArm": [56, 0, 0]}),
    (0.7, {"Spine": [-6, 0, 0], "Head": [-12, 0, 0], "RightArm": [24, 14, 0],
           "LeftArm": [22, -14, 0], "RightForeArm": [50, 0, 0], "LeftForeArm": [50, 0, 0]}),
    (1.0, {}),
], cat="civilian", desc="Смеётся")

ACT["argue"] = A(2.4, [
    (0.0, {"RightArm": [56, 12, 0], "RightForeArm": [40, 0, 0], "Spine": [4, 4, 0],
           "Head": [-2, 4, 0]}),
    (0.3, {"RightArm": [70, 8, 0], "RightForeArm": [20, 0, 0], "Spine": [8, -4, 0],
           "Head": [4, -6, 0], "@hips": [0, 0, 2]}),
    (0.6, {"RightArm": [46, 16, 0], "RightForeArm": [56, 0, 0], "Spine": [2, 8, 0],
           "Head": [-4, 6, 0]}),
    (1.0, {"RightArm": [56, 12, 0], "RightForeArm": [40, 0, 0], "Spine": [4, 4, 0],
           "Head": [-2, 4, 0]}),
], cat="civilian", desc="Спорит (цикл)", loop=True)

ACT["panic"] = A(1.4, [
    (0.0, {"RightArm": [60, 20, 0], "LeftArm": [56, -20, 0], "RightForeArm": [70, 0, 0],
           "LeftForeArm": [70, 0, 0], "Spine": [10, 0, 0], "Head": [-8, 0, 0]}),
    (0.5, {"RightArm": [70, 26, 0], "LeftArm": [66, -26, 0], "RightForeArm": [60, 0, 0],
           "LeftForeArm": [60, 0, 0], "Spine": [14, 0, 0], "Head": [-12, 0, 0],
           "@hips": [0, 1, 0]}),
    (1.0, {"RightArm": [60, 20, 0], "LeftArm": [56, -20, 0], "RightForeArm": [70, 0, 0],
           "LeftForeArm": [70, 0, 0], "Spine": [10, 0, 0], "Head": [-8, 0, 0]}),
], cat="reaction", desc="Паникует (цикл)", loop=True)

ACT["construction_work"] = A(2.0, [
    (0.0, {"RightArm": [100, 10, 0], "RightForeArm": [30, 0, 0], "LeftArm": [80, -14, 0],
           "LeftForeArm": [40, 0, 0], "Spine": [-6, 0, 0], "Head": [-8, 0, 0]}),
    (0.35, {"RightArm": [60, 12, 0], "RightForeArm": [50, 0, 0], "LeftArm": [70, -14, 0],
            "LeftForeArm": [44, 0, 0], "Spine": [14, 0, 0], "Head": [6, 0, 0],
            "@hips": [0, -2, 2]}),
    (0.6, {"RightArm": [90, 10, 0], "RightForeArm": [36, 0, 0], "LeftArm": [78, -14, 0],
           "LeftForeArm": [42, 0, 0], "Spine": [2, 0, 0], "Head": [0, 0, 0]}),
    (1.0, {"RightArm": [100, 10, 0], "RightForeArm": [30, 0, 0], "LeftArm": [80, -14, 0],
           "LeftForeArm": [40, 0, 0], "Spine": [-6, 0, 0], "Head": [-8, 0, 0]}),
], cat="civilian", desc="Работает инструментом (цикл)", loop=True)

ACT["cook"] = A(2.2, [
    (0.0, {"RightArm": [54, 16, 0], "RightForeArm": [80, 0, 0], "LeftArm": [50, -16, 0],
           "LeftForeArm": [86, 0, 0], "Spine": [8, 0, 0], "Head": [10, 0, 0]}),
    (0.4, {"RightArm": [58, 10, 0], "RightForeArm": [70, 0, 0], "LeftArm": [52, -18, 0],
           "LeftForeArm": [82, 0, 0], "Spine": [9, 4, 0], "Head": [11, 2, 0]}),
    (0.75, {"RightArm": [52, 20, 0], "RightForeArm": [86, 0, 0], "LeftArm": [48, -14, 0],
            "LeftForeArm": [90, 0, 0], "Spine": [8, -4, 0], "Head": [10, -2, 0]}),
    (1.0, {"RightArm": [54, 16, 0], "RightForeArm": [80, 0, 0], "LeftArm": [50, -16, 0],
           "LeftForeArm": [86, 0, 0], "Spine": [8, 0, 0], "Head": [10, 0, 0]}),
], cat="civilian", desc="Готовит еду (цикл)", loop=True)

ACT["clean_floor"] = A(2.2, [
    (0.0, {"RightArm": [70, 14, 0], "RightForeArm": [30, 0, 0], "LeftArm": [64, -14, 0],
           "LeftForeArm": [36, 0, 0], "Spine": [16, 0, 0], "Head": [12, 0, 0]}),
    (0.5, {"RightArm": [54, 22, 0], "RightForeArm": [50, 0, 0], "LeftArm": [48, -22, 0],
           "LeftForeArm": [56, 0, 0], "Spine": [22, 10, 0], "Head": [16, 6, 0],
           "@hips": [0, -2, 0]}),
    (1.0, {"RightArm": [70, 14, 0], "RightForeArm": [30, 0, 0], "LeftArm": [64, -14, 0],
           "LeftForeArm": [36, 0, 0], "Spine": [16, 0, 0], "Head": [12, 0, 0]}),
], cat="civilian", desc="Моет пол (цикл)", loop=True)

ACT["read_phone"] = A(3.0, [
    (0.0, {"RightArm": [40, -20, 0], "RightForeArm": [96, 0, 0], "RightHand": [20, 0, 0],
           "LeftArm": [24, -10, 0], "LeftForeArm": [50, 0, 0], "Head": [18, 0, 0]}),
    (0.5, {"RightArm": [42, -22, 0], "RightForeArm": [100, 0, 0], "RightHand": [22, 0, 0],
           "LeftArm": [24, -10, 0], "LeftForeArm": [50, 0, 0], "Head": [18, -4, 0],
           "RightHand": [22, 20, 0]}),
    (1.0, {"RightArm": [40, -20, 0], "RightForeArm": [96, 0, 0], "RightHand": [20, 0, 0],
           "LeftArm": [24, -10, 0], "LeftForeArm": [50, 0, 0], "Head": [18, 0, 0]}),
], cat="civilian", desc="Листает телефон (цикл)", loop=True)

ACT["selfie"] = A(1.8, [
    (0.0, {}),
    (0.3, {"RightArm": [80, -30, 0], "RightForeArm": [40, 0, 0], "Head": [-6, -8, 0],
           "Spine": [-2, -4, 0]}),
    (0.6, {"RightArm": [84, -32, 0], "RightForeArm": [36, 0, 0], "Head": [-8, -10, 0],
           "Spine": [-2, -6, 0]}),
    (1.0, {}),
], cat="civilian", desc="Делает селфи")

ACT["sit_ground"] = A(2.4, [
    (0.0, {}),
    (0.3, merge(seated(0.8), {"Spine": [10, 0, 0]})),
    (0.65, merge(seated(1.2), {"LeftUpLeg": [20, 0, 0], "RightUpLeg": [10, 0, 0],
                               "LeftLeg": [-40, 0, 0], "RightLeg": [-30, 0, 0],
                               "Spine": [-6, 0, 0], "RightArm": [30, 30, 0],
                               "LeftArm": [28, -30, 0]})),
    (1.0, merge(seated(1.25), {"LeftUpLeg": [22, 0, 0], "RightUpLeg": [12, 0, 0],
                               "LeftLeg": [-44, 0, 0], "RightLeg": [-32, 0, 0],
                               "Spine": [-8, 0, 0], "RightArm": [34, 34, 0],
                               "LeftArm": [32, -34, 0], "@hips": [0, -8, 0]})),
], cat="civilian", desc="Садится на землю")

ACT["sit_ground_idle"] = A(4.0, [
    (0.0, merge(seated(1.25), {"LeftUpLeg": [22, 0, 0], "RightUpLeg": [12, 0, 0],
                               "LeftLeg": [-44, 0, 0], "RightLeg": [-32, 0, 0],
                               "Spine": [-8, 0, 0], "RightArm": [34, 34, 0],
                               "LeftArm": [32, -34, 0], "@hips": [0, -8, 0]})),
    (0.5, merge(seated(1.25), {"LeftUpLeg": [22, 0, 0], "RightUpLeg": [12, 0, 0],
                               "LeftLeg": [-44, 0, 0], "RightLeg": [-32, 0, 0],
                               "Spine": [-6, 0, 0], "RightArm": [34, 34, 0],
                               "LeftArm": [32, -34, 0], "@hips": [0, -8, 0],
                               "Head": [0, 6, 0], "Spine1": [1.4, 0, 0]})),
    (1.0, merge(seated(1.25), {"LeftUpLeg": [22, 0, 0], "RightUpLeg": [12, 0, 0],
                               "LeftLeg": [-44, 0, 0], "RightLeg": [-32, 0, 0],
                               "Spine": [-8, 0, 0], "RightArm": [34, 34, 0],
                               "LeftArm": [32, -34, 0], "@hips": [0, -8, 0]})),
], cat="civilian", desc="Сидит на земле (цикл)", loop=True)

ACT["bus_wait"] = A(4.0, [
    (0.0, {"RightArm": [10, 12, 0], "RightForeArm": [30, 0, 0], "LeftArm": [8, -10, 0],
           "LeftForeArm": [26, 0, 0], "Head": [0, 0, 0]}),
    (0.5, {"RightArm": [10, 12, 0], "RightForeArm": [30, 0, 0], "LeftArm": [8, -10, 0],
           "LeftForeArm": [26, 0, 0], "Head": [0, 16, 0], "Spine": [0, 8, 0],
           "RightFoot": [-8, 0, 0]}),
    (1.0, {"RightArm": [10, 12, 0], "RightForeArm": [30, 0, 0], "LeftArm": [8, -10, 0],
           "LeftForeArm": [26, 0, 0], "Head": [0, 0, 0]}),
], cat="civilian", desc="Ждёт транспорт (цикл)", loop=True)

ACT["jog_stretch"] = A(3.0, [
    (0.0, {**both_arms(70, 10, 20), "Spine": [4, 0, 0]}),
    (0.3, {"LeftUpLeg": [40, 0, 0], "LeftLeg": [-50, 0, 0], "LeftFoot": [10, 0, 0],
           "Spine": [14, 0, 0], "@hips": [0, -3, 0]}),
    (0.6, {"RightUpLeg": [40, 0, 0], "RightLeg": [-50, 0, 0], "RightFoot": [10, 0, 0],
           "Spine": [14, 0, 0], "@hips": [0, -3, 0]}),
    (1.0, {**both_arms(70, 10, 20), "Spine": [4, 0, 0]}),
], cat="civilian", desc="Разминается перед пробежкой (цикл)", loop=True)

ACT["cold_shiver"] = A(2.0, [
    (0.0, {"RightArm": [26, 26, 0], "LeftArm": [24, -26, 0], "RightForeArm": [100, 0, 0],
           "LeftForeArm": [104, 0, 0], "Spine": [6, 0, 0], "Head": [6, 0, 0]}),
    (0.25, {"RightArm": [28, 30, 0], "LeftArm": [26, -30, 0], "RightForeArm": [104, 0, 0],
            "LeftForeArm": [108, 0, 0], "Spine": [8, 2, 0], "Head": [8, 2, 0]}),
    (0.5, {"RightArm": [26, 26, 0], "LeftArm": [24, -26, 0], "RightForeArm": [100, 0, 0],
           "LeftForeArm": [104, 0, 0], "Spine": [6, -2, 0], "Head": [6, -2, 0]}),
    (0.75, {"RightArm": [28, 30, 0], "LeftArm": [26, -30, 0], "RightForeArm": [104, 0, 0],
            "LeftForeArm": [108, 0, 0], "Spine": [8, 2, 0], "Head": [8, 2, 0]}),
    (1.0, {"RightArm": [26, 26, 0], "LeftArm": [24, -26, 0], "RightForeArm": [100, 0, 0],
           "LeftForeArm": [104, 0, 0], "Spine": [6, 0, 0], "Head": [6, 0, 0]}),
], cat="civilian", desc="Дрожит от холода (цикл)", loop=True)

ACT["squat_exercise"] = A(2.0, [
    (0.0, {}),
    (0.3, merge(seated(0.9), {**both_arms(80, 12, 20), "Spine": [14, 0, 0]})),
    (0.6, merge(seated(0.2), {**both_arms(84, 12, 14), "Spine": [2, 0, 0]})),
    (1.0, {}),
], cat="civilian", desc="Приседает (цикл)", loop=True)
