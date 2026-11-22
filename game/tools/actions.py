"""Библиотека действий: ключевые кадры для клипов.

Все позы — добавки (дельты) к базовой стойке `clips.stance(...)`,
поэтому один и тот же набор годится и стоя, и в приседе, и с оружием.
"""

from clips import merge, put


def A(dur, keys, cat="interaction", desc="", crouch=0.0, arms="free",
      loop=False, ease="smooth"):
    return dict(dur=dur, keys=keys, cat=cat, desc=desc, crouch=crouch,
                arms=arms, loop=loop, ease=ease)


def seated(depth=1.0):
    d = {}
    put(d, "LeftUpLeg", p=88 * depth)
    put(d, "RightUpLeg", p=88 * depth)
    put(d, "LeftLeg", p=-94 * depth)
    put(d, "RightLeg", p=-94 * depth)
    put(d, "LeftFoot", p=18 * depth)
    put(d, "RightFoot", p=18 * depth)
    put(d, "@hips", y=-46 * depth)
    put(d, "Spine", p=8 * depth)
    return d


def reach(role="Right", p=78, y=10, fore=12, hand=-6):
    s = "Left" if role == "Left" else "Right"
    return {s + "Arm": [p, y if role == "Right" else -y, 0],
            s + "ForeArm": [fore, 0, 0],
            s + "Hand": [hand, 0, 0]}


def both_arms(p=70, y=16, fore=10, hand=-6):
    return {**reach("Right", p, y, fore, hand), **reach("Left", p, y, fore, hand)}


ACT = {}

# --------------------------------------------------------------------------- #
# двери, окна, подъёмы
# --------------------------------------------------------------------------- #

ACT["door_open"] = A(1.1, [
    (0.0, {}),
    (0.28, {**reach("Right", 58, 8, 36), "Spine": [6, 0, 0]}),
    (0.55, {**reach("Right", 72, 4, 8), "Spine": [14, 0, 0], "@hips": [0, 0, 4]}),
    (0.80, {**reach("Right", 80, 2, 4), "Spine": [18, 0, 0], "@hips": [0, 0, 8]}),
    (1.0, {}),
], desc="Открывает дверь рукой от себя")

ACT["door_open_slow"] = A(2.2, [
    (0.0, {}),
    (0.3, {**reach("Right", 55, 10, 42), "Spine": [5, 0, 0]}),
    (0.6, {**reach("Right", 68, 6, 14), "Spine": [10, 0, 0], "@hips": [0, 0, 3]}),
    (0.85, {**reach("Right", 76, 4, 6), "Spine": [13, 0, 0], "@hips": [0, 0, 6]}),
    (1.0, {}),
], desc="Медленно открывает дверь и заглядывает внутрь")

ACT["door_close"] = A(1.0, [
    (0.0, {**reach("Right", 76, 4, 10), "Spine": [12, 0, 0]}),
    (0.4, {**reach("Right", 52, 8, 30), "Spine": [4, 0, 0]}),
    (0.7, {**reach("Right", 24, 14, 56)}),
    (1.0, {}),
], desc="Закрывает дверь")

ACT["door_knock"] = A(1.3, [
    (0.0, {}),
    (0.2, {**reach("Right", 62, 6, 60), "Spine": [4, 0, 0]}),
    (0.34, {**reach("Right", 70, 4, 40), "Spine": [8, 0, 0]}),
    (0.46, {**reach("Right", 62, 6, 60), "Spine": [4, 0, 0]}),
    (0.58, {**reach("Right", 70, 4, 40), "Spine": [8, 0, 0]}),
    (1.0, {}),
], desc="Стучит в дверь")

ACT["door_kick"] = A(1.4, [
    (0.0, {}),
    (0.22, {"RightUpLeg": [34, 0, 0], "RightLeg": [-62, 0, 0], "Spine": [-6, 0, 0], "@hips": [0, -3, -4]}),
    (0.42, {"RightUpLeg": [86, 0, 0], "RightLeg": [-8, 0, 0], "RightFoot": [-16, 0, 0],
            "Spine": [10, 0, 0], "LeftUpLeg": [-6, 0, 0], "@hips": [0, 2, 6]}),
    (0.62, {"RightUpLeg": [56, 0, 0], "RightLeg": [-46, 0, 0], "Spine": [2, 0, 0]}),
    (1.0, {}),
], desc="Выбивает дверь ударом ноги")

ACT["door_lockpick"] = A(2.6, [
    (0.0, merge(seated(0.45), {})),
    (0.3, {**reach("Right", 62, 12, 58), **reach("Left", 58, 12, 62), "Spine": [16, 0, 0]}),
    (0.6, {**reach("Right", 64, 14, 66), **reach("Left", 60, 14, 70), "Spine": [18, -4, 0],
            "RightHand": [0, 18, 0], "LeftHand": [0, -18, 0]}),
    (0.85, {**reach("Right", 60, 10, 54), **reach("Left", 56, 10, 58), "Spine": [14, 4, 0]}),
    (1.0, merge(seated(0.45), {})),
], cat="interaction", desc="Вскрывает замок отмычкой", crouch=0.45)

ACT["window_open"] = A(1.6, [
    (0.0, {}),
    (0.3, {**both_arms(96, 10, 24), "Spine": [-4, 0, 0]}),
    (0.6, {**both_arms(104, 6, 12), "Spine": [2, 0, 0], "@hips": [0, 2, 2]}),
    (0.85, {**both_arms(88, 8, 40), "Spine": [6, 0, 0]}),
    (1.0, {}),
], desc="Открывает окно")

ACT["window_climb"] = A(2.0, [
    (0.0, {}),
    (0.2, {**both_arms(100, 10, 20), "@hips": [0, -6, 0], "Spine": [6, 0, 0]}),
    (0.45, {**both_arms(110, 8, 8), "@hips": [0, 6, 6], "LeftUpLeg": [50, 0, 0],
            "LeftLeg": [-70, 0, 0], "RightUpLeg": [20, 0, 0], "RightLeg": [-40, 0, 0]}),
    (0.7, {"LeftUpLeg": [86, 0, 0], "LeftLeg": [-30, 0, 0], "@hips": [0, 14, 10],
           "Spine": [22, 0, 0], "RightArm": [40, 10, 0]}),
    (0.88, {"LeftUpLeg": [40, 0, 0], "LeftLeg": [-60, 0, 0], "@hips": [0, 4, 6], "Spine": [10, 0, 0]}),
    (1.0, {}),
], cat="parkour", desc="Залезает в окно")

# --------------------------------------------------------------------------- #
# предметы, обыск, касса
# --------------------------------------------------------------------------- #

ACT["pick_up"] = A(1.8, [
    (0.0, {}),
    (0.25, merge(seated(0.5), reach("Right", 56, 10, 26, 8))),
    (0.55, merge(seated(0.9), reach("Right", 62, 8, 12, 14), {"Spine": [10, 0, 0]})),
    (0.75, merge(seated(0.45), reach("Right", 50, 10, 60, -4))),
    (1.0, {}),
], desc="Поднимает предмет с земли")

ACT["pick_up_ground"] = A(1.9, [
    (0.0, merge(seated(0.6), {})),
    (0.35, merge(seated(0.95), reach("Right", 60, 8, 16, 10), {"Spine": [14, 0, 0]})),
    (0.65, merge(seated(0.85), reach("Right", 56, 8, 20, 6), {"Spine": [10, 0, 0]})),
    (1.0, merge(seated(0.6), {})),
], cat="interaction", desc="Подбирает что-то с земли из приседа", crouch=0.5)

ACT["loot_body"] = A(2.0, [
    (0.0, merge(seated(0.7), {})),
    (0.3, merge(seated(0.95), reach("Right", 66, 14, 30, 10), {"Spine": [16, -4, 0]})),
    (0.6, merge(seated(0.95), reach("Right", 62, 18, 44, 16), {"Spine": [18, 4, 0]})),
    (0.8, merge(seated(0.6), reach("Right", 46, 12, 78, -8))),
    (1.0, merge(seated(0.7), {})),
], desc="Обыскивает лежащее тело")

ACT["search_pockets"] = A(1.6, [
    (0.0, merge(seated(0.5), {})),
    (0.3, merge(seated(0.75), reach("Right", 68, 12, 40, 10), reach("Left", 60, 10, 50))),
    (0.65, merge(seated(0.75), reach("Right", 64, 16, 52, 14), reach("Left", 58, 12, 58), {"Spine": [12, 0, 0]})),
    (1.0, merge(seated(0.5), {})),
], desc="Обыскивает карманы")

ACT["register_take"] = A(2.4, [
    (0.0, {}),
    (0.22, {**reach("Right", 58, 10, 34), "Spine": [8, -6, 0]}),
    (0.45, {**reach("Right", 72, 6, 14), "RightHand": [-14, 0, 0], "Spine": [16, -10, 0],
            "@hips": [0, 0, 4]}),
    (0.65, {**reach("Right", 66, 10, 40), "RightHand": [10, 0, 0], "Spine": [10, -4, 0]}),
    (0.85, {**reach("Right", 40, 14, 82), "RightHand": [14, 0, 0], "Spine": [4, 0, 0]}),
    (1.0, {}),
], desc="Достаёт деньги из кассы")

ACT["drawer_open"] = A(1.4, [
    (0.0, {}),
    (0.3, {**reach("Right", 62, 8, 46), "Spine": [6, 0, 0]}),
    (0.6, {**reach("Right", 54, 8, 62), "Spine": [2, 0, 0], "@hips": [0, 0, -3]}),
    (1.0, {}),
], desc="Открывает ящик")

ACT["atm_use"] = A(2.2, [
    (0.0, {}),
    (0.2, {**reach("Right", 74, 4, 20), "Spine": [4, 0, 0], "Head": [6, 0, 0]}),
    (0.45, {**reach("Right", 78, 2, 8), "RightHand": [-10, 0, 0], "Spine": [6, 0, 0]}),
    (0.6, {**reach("Right", 72, 6, 26), "Spine": [3, 0, 0], "Head": [8, 0, 0]}),
    (0.75, {**reach("Right", 78, 2, 8), "RightHand": [-12, 0, 0], "Spine": [6, 0, 0]}),
    (1.0, {}),
], desc="Нажимает кнопки банкомата")

ACT["vending_use"] = A(2.0, [
    (0.0, {}),
    (0.25, {**reach("Right", 60, 6, 34), "Spine": [6, 0, 0], "Head": [8, 0, 0]}),
    (0.5, {**reach("Right", 66, 4, 18), "RightHand": [-8, 0, 0]}),
    (0.75, merge(seated(0.35), reach("Right", 40, 6, 40, -6))),
    (1.0, {}),
], desc="Покупает в автомате и забирает товар")

ACT["shop_buy"] = A(2.0, [
    (0.0, {}),
    (0.3, {**reach("Right", 62, 6, 58), "Spine": [4, 0, 0]}),
    (0.55, {**reach("Right", 70, 4, 30), "Spine": [6, 0, 0], "RightHand": [-8, 0, 0]}),
    (0.8, {**reach("Right", 66, 6, 20), "Spine": [4, 0, 0]}),
    (1.0, {}),
], desc="Расплачивается на кассе")

ACT["eat"] = A(2.4, [
    (0.0, {}),
    (0.2, {**reach("Right", 46, 10, 92), "Head": [6, 0, 0]}),
    (0.35, {**reach("Right", 44, 6, 108), "Head": [10, 0, 0], "RightHand": [10, 0, 0]}),
    (0.5, {**reach("Right", 46, 10, 92), "Head": [4, 0, 0]}),
    (0.65, {**reach("Right", 44, 6, 108), "Head": [10, 0, 0], "RightHand": [10, 0, 0]}),
    (1.0, {}),
], cat="civilian", desc="Ест на ходу")

ACT["drink"] = A(2.2, [
    (0.0, {}),
    (0.3, {**reach("Right", 50, 8, 96), "Head": [-6, 0, 0]}),
    (0.5, {**reach("Right", 56, 4, 116), "Head": [-18, 0, 0], "RightHand": [20, 0, 0]}),
    (0.7, {**reach("Right", 50, 8, 96), "Head": [-4, 0, 0]}),
    (1.0, {}),
], cat="civilian", desc="Пьёт из бутылки")

ACT["smoke_drag"] = A(3.0, [
    (0.0, {}),
    (0.25, {**reach("Right", 44, -18, 96), "RightHand": [14, 10, 0], "Head": [2, 6, 0]}),
    (0.45, {**reach("Right", 48, -12, 110), "RightHand": [22, 10, 0], "Head": [-4, 8, 0]}),
    (0.7, {**reach("Right", 40, -14, 70), "Head": [4, 4, 0]}),
    (1.0, {}),
], cat="civilian", desc="Затягивается сигаретой")

# --------------------------------------------------------------------------- #
# сидеть
# --------------------------------------------------------------------------- #

ACT["sit_down"] = A(1.6, [
    (0.0, {}),
    (0.3, merge(seated(0.35), {"Spine": [8, 0, 0]})),
    (0.65, merge(seated(0.85), {"Spine": [4, 0, 0]})),
    (1.0, seated(1.0)),
], cat="interaction", desc="Садится")

ACT["sit_idle"] = A(4.0, [
    (0.0, seated(1.0)),
    (0.5, merge(seated(1.0), {"Spine1": [1.6, 0, 0], "Head": [-2, 4, 0]})),
    (1.0, seated(1.0)),
], cat="idle", desc="Сидит (цикл)", loop=True)

ACT["stand_up"] = A(1.5, [
    (0.0, seated(1.0)),
    (0.35, merge(seated(0.8), {"Spine": [14, 0, 0]})),
    (0.7, merge(seated(0.35), {"Spine": [8, 0, 0]})),
    (1.0, {}),
], cat="interaction", desc="Встаёт")

# --------------------------------------------------------------------------- #
# жесты
# --------------------------------------------------------------------------- #

ACT["wave"] = A(1.8, [
    (0.0, {}),
    (0.2, {"RightArm": [50, 8, -78], "RightForeArm": [20, 0, -10], "Spine": [0, 0, -4]}),
    (0.4, {"RightArm": [52, 8, -92], "RightForeArm": [18, 0, -22], "RightHand": [0, 20, 0]}),
    (0.6, {"RightArm": [52, 8, -74], "RightForeArm": [18, 0, 6], "RightHand": [0, -20, 0]}),
    (0.8, {"RightArm": [52, 8, -92], "RightForeArm": [18, 0, -22], "RightHand": [0, 20, 0]}),
    (1.0, {}),
], cat="gesture", desc="Машет рукой")

ACT["point"] = A(1.2, [
    (0.0, {}),
    (0.3, {"RightArm": [72, 18, -8], "RightForeArm": [0, 0, 0], "RightHand": [-4, 0, 0]}),
    (0.8, {"RightArm": [74, 18, -8], "RightForeArm": [0, 0, 0], "RightHand": [-4, 0, 0]}),
    (1.0, {}),
], cat="gesture", desc="Показывает рукой")

ACT["salute"] = A(1.2, [
    (0.0, {}),
    (0.35, {"RightArm": [58, -14, -54], "RightForeArm": [96, 0, 0], "RightHand": [-10, 0, 0],
            "Spine": [0, 0, 0]}),
    (0.8, {"RightArm": [58, -14, -54], "RightForeArm": [96, 0, 0], "RightHand": [-10, 0, 0]}),
    (1.0, {}),
], cat="gesture", desc="Отдаёт честь")

ACT["thumbs_up"] = A(1.2, [
    (0.0, {}),
    (0.3, {"RightArm": [56, 8, 0], "RightForeArm": [64, 0, 0], "RightHand": [-16, 0, 0]}),
    (0.8, {"RightArm": [56, 8, 0], "RightForeArm": [64, 0, 0], "RightHand": [-16, 0, 0]}),
    (1.0, {}),
], cat="gesture", desc="Показывает большой палец")

ACT["beckon"] = A(1.6, [
    (0.0, {}),
    (0.25, {"RightArm": [58, 10, 0], "RightForeArm": [46, 0, 0], "RightHand": [-10, 0, 0]}),
    (0.45, {"RightArm": [58, 10, 0], "RightForeArm": [66, 0, 0], "RightHand": [16, 0, 0]}),
    (0.65, {"RightArm": [58, 10, 0], "RightForeArm": [46, 0, 0], "RightHand": [-10, 0, 0]}),
    (0.85, {"RightArm": [58, 10, 0], "RightForeArm": [66, 0, 0], "RightHand": [16, 0, 0]}),
    (1.0, {}),
], cat="gesture", desc="Подзывает к себе")

ACT["shrug"] = A(1.4, [
    (0.0, {}),
    (0.3, {"RightShoulder": [-12, 0, 8], "LeftShoulder": [-12, 0, -8],
           "RightArm": [14, 0, 12], "LeftArm": [14, 0, -12],
           "RightForeArm": [54, 0, 0], "LeftForeArm": [54, 0, 0],
           "Spine": [-3, 0, 0], "Head": [-4, 0, 0]}),
    (0.7, {"RightShoulder": [-12, 0, 8], "LeftShoulder": [-12, 0, -8],
           "RightArm": [14, 0, 12], "LeftArm": [14, 0, -12],
           "RightForeArm": [54, 0, 0], "LeftForeArm": [54, 0, 0],
           "Spine": [-3, 0, 0], "Head": [-4, 0, 0]}),
    (1.0, {}),
], cat="gesture", desc="Пожимает плечами")

ACT["facepalm"] = A(1.6, [
    (0.0, {}),
    (0.3, {"RightArm": [62, -10, 0], "RightForeArm": [104, 0, 0], "RightHand": [16, -10, 0],
           "Head": [14, 0, 0], "Spine": [6, 0, 0]}),
    (0.75, {"RightArm": [62, -10, 0], "RightForeArm": [104, 0, 0], "RightHand": [16, -10, 0],
            "Head": [14, 0, 0], "Spine": [6, 0, 0]}),
    (1.0, {}),
], cat="gesture", desc="Бьёт себя по лицу (фейспалм)")

ACT["clap"] = A(1.0, [
    (0.0, {"RightArm": [52, 22, 0], "LeftArm": [52, -22, 0],
           "RightForeArm": [72, 0, 0], "LeftForeArm": [72, 0, 0]}),
    (0.5, {"RightArm": [52, 12, 0], "LeftArm": [52, -12, 0],
           "RightForeArm": [92, 0, 0], "LeftForeArm": [92, 0, 0]}),
    (1.0, {"RightArm": [52, 22, 0], "LeftArm": [52, -22, 0],
           "RightForeArm": [72, 0, 0], "LeftForeArm": [72, 0, 0]}),
], cat="gesture", desc="Хлопает (цикл)", loop=True)

ACT["stretch"] = A(2.4, [
    (0.0, {}),
    (0.35, {**both_arms(104, 6, 8), "Spine": [-8, 0, 0], "Head": [-10, 0, 0]}),
    (0.65, {**both_arms(110, 4, 4), "Spine": [-10, 0, 0], "Head": [-12, 0, 0]}),
    (1.0, {}),
], cat="gesture", desc="Потягивается")

ACT["crack_neck"] = A(1.8, [
    (0.0, {}),
    (0.3, {"LeftArm": [58, -6, 0], "LeftForeArm": [110, 0, 0], "Head": [0, 0, 10], "Spine": [0, 0, 3]}),
    (0.65, {"LeftArm": [58, -6, 0], "LeftForeArm": [110, 0, 0], "Head": [0, 0, -12], "Spine": [0, 0, -3]}),
    (1.0, {}),
], cat="gesture", desc="Разминает шею")

ACT["wipe_brow"] = A(2.0, [
    (0.0, {}),
    (0.3, {"RightArm": [64, -6, 0], "RightForeArm": [104, 0, 0], "RightHand": [10, -12, 0],
           "Head": [8, 0, 0]}),
    (0.6, {"RightArm": [52, -14, 0], "RightForeArm": [92, 0, 0], "RightHand": [6, -18, 0],
           "Head": [6, 0, 0]}),
    (1.0, {}),
], cat="gesture", desc="Вытирает пот со лба")

ACT["check_watch"] = A(2.0, [
    (0.0, {}),
    (0.3, {"LeftArm": [30, 30, 0], "LeftForeArm": [104, 0, 0], "LeftHand": [-10, 0, 0],
           "Head": [12, 10, 0]}),
    (0.8, {"LeftArm": [30, 30, 0], "LeftForeArm": [104, 0, 0], "LeftHand": [-10, 0, 0],
           "Head": [12, 10, 0]}),
    (1.0, {}),
], cat="gesture", desc="Смотрит на часы")

ACT["take_photo"] = A(1.8, [
    (0.0, {}),
    (0.3, {"RightArm": [78, -16, 0], "RightForeArm": [56, 0, 0], "RightHand": [-6, 0, 0],
           "LeftArm": [76, -24, 0], "LeftForeArm": [64, 0, 0], "LeftHand": [-6, 0, 0],
           "Head": [-4, 0, 0]}),
    (0.55, {"RightArm": [80, -16, 0], "RightForeArm": [52, 0, 0],
            "LeftArm": [78, -24, 0], "LeftForeArm": [60, 0, 0], "Head": [-2, 0, 0]}),
    (1.0, {}),
], cat="civilian", desc="Делает фото на телефон")

ACT["phone_call"] = A(2.0, [
    (0.0, {"RightArm": [42, -26, 4], "RightForeArm": [104, 0, 0], "RightHand": [12, 10, 0]}),
    (0.5, {"RightArm": [44, -28, 6], "RightForeArm": [100, 0, 0], "RightHand": [12, 10, 0],
           "Head": [2, 6, 0], "Spine": [1, 3, 0]}),
    (1.0, {"RightArm": [42, -26, 4], "RightForeArm": [104, 0, 0], "RightHand": [12, 10, 0]}),
], cat="civilian", desc="Говорит по телефону (цикл)", loop=True)

ACT["throw_object"] = A(1.5, [
    (0.0, {}),
    (0.25, {"RightArm": [36, 4, 0], "RightForeArm": [70, 0, 0], "Spine": [-8, -14, 0], "Head": [0, -8, 0]}),
    (0.45, {"RightArm": [96, 12, 0], "RightForeArm": [6, 0, 0], "Spine": [10, 14, 0], "Head": [2, 8, 0]}),
    (0.6, {"RightArm": [70, 10, 0], "RightForeArm": [30, 0, 0], "Spine": [4, 6, 0]}),
    (1.0, {}),
], desc="Бросает предмет")

ACT["throw_grenade"] = A(2.2, [
    (0.0, {}),
    (0.25, {"RightArm": [46, -20, 0], "RightForeArm": [96, 0, 0], "LeftArm": [56, -24, 0],
            "LeftForeArm": [80, 0, 0], "Head": [6, 0, 0], "Spine": [4, 0, 0]}),
    (0.45, {"RightArm": [58, 6, 0], "RightForeArm": [66, 0, 0], "LeftArm": [40, -30, 0],
            "LeftForeArm": [70, 0, 0], "Spine": [-6, -10, 0]}),
    (0.65, {"RightArm": [102, 14, 0], "RightForeArm": [6, 0, 0], "Spine": [12, 16, 0]}),
    (0.85, {"RightArm": [76, 10, 0], "RightForeArm": [24, 0, 0], "Spine": [4, 8, 0]}),
    (1.0, {}),
], cat="combat", desc="Достаёт чеку и бросает гранату")

ACT["aim_grenade"] = A(2.0, [
    (0.0, {"RightArm": [70, 10, 0], "RightForeArm": [50, 0, 0], "LeftArm": [66, -22, 0],
           "LeftForeArm": [60, 0, 0], "Spine": [2, 6, 0]}),
    (0.5, {"RightArm": [74, 12, 0], "RightForeArm": [46, 0, 0], "LeftArm": [68, -24, 0],
           "LeftForeArm": [64, 0, 0], "Spine": [3, 7, 0], "Head": [-2, -3, 0]}),
    (1.0, {"RightArm": [70, 10, 0], "RightForeArm": [50, 0, 0], "LeftArm": [66, -22, 0],
           "LeftForeArm": [60, 0, 0], "Spine": [2, 6, 0]}),
], cat="combat", desc="Готовится бросить гранату (цикл)", loop=True)

ACT["push_object"] = A(2.4, [
    (0.0, {**both_arms(72, 14, 26), "Spine": [10, 0, 0], "LeftUpLeg": [14, 0, 0],
           "RightUpLeg": [-6, 0, 0]}),
    (0.5, {**both_arms(78, 12, 16), "Spine": [16, 0, 0], "LeftUpLeg": [22, 0, 0],
           "LeftLeg": [-24, 0, 0], "RightUpLeg": [-10, 0, 0], "@hips": [0, -2, 3]}),
    (1.0, {**both_arms(72, 14, 26), "Spine": [10, 0, 0], "LeftUpLeg": [14, 0, 0],
           "RightUpLeg": [-6, 0, 0]}),
], desc="Толкает тяжёлый объект (цикл)", loop=True)

ACT["plant_bomb"] = A(2.6, [
    (0.0, merge(seated(0.6), {})),
    (0.3, merge(seated(0.95), {**reach("Right", 64, 10, 20, 12), "Spine": [18, 0, 0]})),
    (0.6, merge(seated(0.95), {**reach("Right", 60, 8, 30, 16), "Spine": [20, 0, 0],
                               "Head": [16, 0, 0]})),
    (0.85, merge(seated(0.4), {**reach("Right", 52, 10, 70, 6)})),
    (1.0, merge(seated(0.6), {})),
], desc="Ставит заряд")

ACT["detonate"] = A(2.0, [
    (0.0, {}),
    (0.3, {"RightArm": [56, -18, 0], "RightForeArm": [92, 0, 0], "Head": [10, -6, 0]}),
    (0.55, {"RightArm": [58, -18, 0], "RightForeArm": [96, 0, 0], "RightHand": [-10, 0, 0],
            "Head": [12, -6, 0]}),
    (0.8, {"RightArm": [58, -22, 0], "RightForeArm": [70, 0, 0], "Head": [-6, -8, 0]}),
    (1.0, {}),
], desc="Нажимает кнопку детонатора")

ACT["jack_car"] = A(2.2, [
    (0.0, merge(seated(0.35), {})),
    (0.25, merge(seated(0.55), {"RightArm": [62, 12, 0], "RightForeArm": [70, 0, 0],
                                "LeftArm": [58, -12, 0], "LeftForeArm": [76, 0, 0]}),
     ),
    (0.45, merge(seated(0.6), {"RightArm": [58, 14, 0], "RightForeArm": [54, 0, 0],
                               "LeftArm": [62, -10, 0], "LeftForeArm": [42, 0, 0],
                               "Spine": [14, -8, 0]})),
    (0.7, merge(seated(0.6), {"RightArm": [66, 8, 0], "RightForeArm": [86, 0, 0],
                              "LeftArm": [56, -14, 0], "LeftForeArm": [72, 0, 0],
                              "Spine": [8, 6, 0]})),
    (1.0, merge(seated(0.35), {})),
], desc="Вскрывает дверь машины монтировкой")

ACT["fuel_car"] = A(3.0, [
    (0.0, {**reach("Right", 46, 10, 70), "Spine": [8, -6, 0]}),
    (0.4, {**reach("Right", 54, 8, 40), "Spine": [10, -8, 0], "RightHand": [-8, 0, 0]}),
    (0.8, {**reach("Right", 50, 8, 50), "Spine": [9, -6, 0]}),
    (1.0, {**reach("Right", 46, 10, 70), "Spine": [8, -6, 0]}),
], desc="Заправляет машину (цикл)", loop=True)

ACT["car_repair"] = A(2.0, [
    (0.0, merge(seated(0.4), {**both_arms(78, 16, 24)})),
    (0.35, merge(seated(0.4), {**both_arms(74, 14, 40), "Spine": [12, 0, 0]})),
    (0.6, merge(seated(0.4), {**both_arms(80, 16, 18), "Spine": [6, 0, 0], "Head": [10, 0, 0]})),
    (1.0, merge(seated(0.4), {**both_arms(78, 16, 24)})),
], desc="Копается под капотом (цикл)", loop=True)

ACT["car_trunk"] = A(1.8, [
    (0.0, {}),
    (0.3, {**both_arms(88, 12, 24), "Spine": [2, 0, 0]}),
    (0.55, {**both_arms(96, 10, 10), "Spine": [-2, 0, 0], "@hips": [0, 2, 0]}),
    (0.8, {**both_arms(74, 14, 40), "Spine": [6, 0, 0]}),
    (1.0, {}),
], desc="Открывает багажник")

ACT["car_open_door"] = A(1.4, [
    (0.0, {}),
    (0.3, {**reach("Right", 62, 22, 40), "Spine": [6, -8, 0]}),
    (0.6, {**reach("Right", 68, 30, 14), "Spine": [8, -12, 0], "@hips": [0, 0, -3]}),
    (0.85, {**reach("Right", 60, 36, 30), "Spine": [6, -14, 0]}),
    (1.0, {}),
], desc="Открывает дверь машины")

ACT["car_enter"] = A(2.2, [
    (0.0, {}),
    (0.2, {**reach("Right", 66, 26, 26), "Spine": [8, -10, 0]}),
    (0.45, merge(seated(0.7), {"Spine": [10, -16, 0], "RightArm": [40, 20, 0]})),
    (0.7, merge(seated(1.0), {"Spine": [4, -6, 0], "LeftUpLeg": [12, 0, 0],
                              "RightUpLeg": [6, 0, 0], "LeftLeg": [-10, 0, 0]})),
    (1.0, merge(seated(1.0), {"Spine": [2, 0, 0], "LeftForeArm": [30, 0, 0],
                              "RightForeArm": [24, 0, 0], "LeftArm": [30, -20, 0],
                              "RightArm": [28, 20, 0]})),
], desc="Садится в машину")

ACT["car_drive_idle"] = A(4.0, [
    (0.0, merge(seated(1.0), {"LeftArm": [58, -30, 0], "LeftForeArm": [48, 0, 0],
                              "RightArm": [58, 30, 0], "RightForeArm": [48, 0, 0]})),
    (0.5, merge(seated(1.0), {"LeftArm": [60, -32, 0], "LeftForeArm": [46, 0, 0],
                              "RightArm": [60, 32, 0], "RightForeArm": [46, 0, 0],
                              "Spine1": [1.2, 0, 0], "Head": [-1, 3, 0]})),
    (1.0, merge(seated(1.0), {"LeftArm": [58, -30, 0], "LeftForeArm": [48, 0, 0],
                              "RightArm": [58, 30, 0], "RightForeArm": [48, 0, 0]})),
], cat="vehicle", desc="Сидит за рулём (цикл)", loop=True)

ACT["car_exit"] = A(1.8, [
    (0.0, merge(seated(1.0), {"RightArm": [40, 20, 0], "LeftArm": [36, -22, 0]})),
    (0.3, merge(seated(0.8), {"Spine": [8, -14, 0], "RightArm": [56, 22, 0],
                              "LeftUpLeg": [8, 0, 0]})),
    (0.6, merge(seated(0.25), {"Spine": [10, -6, 0], "RightArm": [30, 14, 0]})),
    (1.0, {}),
], cat="vehicle", desc="Выходит из машины")

ACT["hotwire"] = A(2.6, [
    (0.0, merge(seated(0.9), {**both_arms(60, 20, 60, 10)})),
    (0.35, merge(seated(0.9), {**both_arms(56, 16, 76, 18), "Spine": [10, 0, 0],
                               "Head": [12, 0, 0]})),
    (0.6, merge(seated(0.9), {**both_arms(58, 18, 60, 6), "Spine": [6, 0, 0],
                              "Head": [8, 0, 0]})),
    (0.85, merge(seated(0.9), {**both_arms(60, 20, 68, 12), "Spine": [8, 0, 0],
                               "Head": [10, 0, 0]})),
    (1.0, merge(seated(0.9), {**both_arms(60, 20, 60, 10)})),
], cat="vehicle", desc="Замыкает провода (цикл)", loop=True)

ACT["handcuff"] = A(2.0, [
    (0.0, {}),
    (0.25, {"RightArm": [58, 16, 0], "RightForeArm": [72, 0, 0],
            "LeftArm": [54, -16, 0], "LeftForeArm": [78, 0, 0], "Spine": [8, 0, 0]}),
    (0.5, {"RightArm": [66, 10, 0], "RightForeArm": [56, 0, 0],
           "LeftArm": [62, -10, 0], "LeftForeArm": [62, 0, 0], "Spine": [10, 0, 0],
           "Head": [6, 0, 0]}),
    (0.75, {"RightArm": [52, 18, 0], "RightForeArm": [80, 0, 0],
            "LeftArm": [48, -18, 0], "LeftForeArm": [86, 0, 0], "Spine": [6, 0, 0]}),
    (1.0, {}),
], cat="police", desc="Надевает наручники")

# --------------------------------------------------------------------------- #
# паркур и прыжки
# --------------------------------------------------------------------------- #

ACT["jump_standing"] = A(1.2, [
    (0.0, {}),
    (0.18, merge(seated(0.6), {"Spine": [14, 0, 0]}),
     ),
    (0.35, {"LeftUpLeg": [-14, 0, 0], "RightUpLeg": [-10, 0, 0], "LeftLeg": [-6, 0, 0],
            "RightLeg": [-6, 0, 0], "LeftFoot": [-24, 0, 0], "RightFoot": [-24, 0, 0],
            "RightArm": [-24, 0, 0], "LeftArm": [-24, 0, 0], "@hips": [0, 6, 0], "Spine": [-4, 0, 0]}),
    (0.55, {"LeftUpLeg": [40, 0, 0], "RightUpLeg": [30, 0, 0], "LeftLeg": [-56, 0, 0],
            "RightLeg": [-44, 0, 0], "LeftFoot": [-10, 0, 0], "RightFoot": [-10, 0, 0],
            "RightArm": [30, 10, 0], "LeftArm": [26, -10, 0], "Spine": [10, 0, 0]}),
    (0.75, merge(seated(0.5), {"Spine": [12, 0, 0]})),
    (1.0, {}),
], cat="parkour", desc="Прыжок с места")

ACT["jump_run"] = A(1.0, [
    (0.0, {"Spine": [14, 0, 0], "LeftUpLeg": [22, 0, 0], "RightUpLeg": [-18, 0, 0]}),
    (0.25, {"Spine": [18, 0, 0], "LeftUpLeg": [-10, 0, 0], "RightUpLeg": [30, 0, 0],
            "LeftLeg": [-16, 0, 0], "RightLeg": [-50, 0, 0], "@hips": [0, 4, 0]}),
    (0.5, {"Spine": [10, 0, 0], "LeftUpLeg": [44, 0, 0], "RightUpLeg": [16, 0, 0],
           "LeftLeg": [-58, 0, 0], "RightLeg": [-34, 0, 0], "RightArm": [34, 14, 0],
           "LeftArm": [28, -14, 0], "@hips": [0, 8, 0]}),
    (0.8, merge(seated(0.45), {"Spine": [12, 0, 0]})),
    (1.0, {"Spine": [12, 0, 0], "LeftUpLeg": [18, 0, 0], "RightUpLeg": [-12, 0, 0]}),
], cat="parkour", desc="Прыжок с разбега")

ACT["jump_forward"] = A(1.3, [
    (0.0, {}),
    (0.16, merge(seated(0.55), {"Spine": [16, 0, 0]})),
    (0.32, {"LeftUpLeg": [-8, 0, 0], "RightUpLeg": [-4, 0, 0], "LeftFoot": [-22, 0, 0],
            "RightFoot": [-22, 0, 0], "RightArm": [-30, 6, 0], "LeftArm": [-26, -6, 0],
            "@hips": [0, 7, 2], "Spine": [6, 0, 0], "Head": [-8, 0, 0]}),
    (0.55, {"LeftUpLeg": [46, 0, 0], "RightUpLeg": [26, 0, 0], "LeftLeg": [-52, 0, 0],
            "RightLeg": [-30, 0, 0], "RightArm": [44, 12, 0], "LeftArm": [40, -12, 0],
            "Spine": [8, 0, 0]}),
    (0.8, merge(seated(0.55), {"Spine": [14, 0, 0]})),
    (1.0, {}),
], cat="parkour", desc="Прыжок вперёд")

ACT["land_soft"] = A(0.9, [
    (0.0, merge(seated(0.45), {"LeftUpLeg": [-16, 0, 0], "RightUpLeg": [-16, 0, 0],
                               "LeftLeg": [-18, 0, 0], "RightLeg": [-18, 0, 0],
                               "Spine": [8, 0, 0]})),
    (0.35, merge(seated(0.55), {"Spine": [12, 0, 0]})),
    (0.7, merge(seated(0.2), {"Spine": [4, 0, 0]})),
    (1.0, {}),
], cat="parkour", desc="Мягкое приземление")

ACT["land_hard"] = A(1.3, [
    (0.0, merge(seated(0.55), {"LeftUpLeg": [-20, 0, 0], "RightUpLeg": [-20, 0, 0],
                               "LeftLeg": [-14, 0, 0], "RightLeg": [-14, 0, 0],
                               "Spine": [12, 0, 0], "RightArm": [30, 20, 0], "LeftArm": [26, -20, 0]})),
    (0.22, merge(seated(1.0), {"Spine": [22, 0, 0], "RightArm": [54, 26, 0],
                               "LeftArm": [50, -26, 0], "Head": [14, 0, 0]})),
    (0.6, merge(seated(0.65), {"Spine": [14, 0, 0]})),
    (1.0, {}),
], cat="parkour", desc="Жёсткое приземление")

ACT["roll_land"] = A(1.6, [
    (0.0, merge(seated(0.6), {"Spine": [18, 0, 0], "LeftUpLeg": [-14, 0, 0],
                              "RightUpLeg": [-14, 0, 0]})),
    (0.3, merge(seated(1.0), {"Spine": [34, 0, 0], "Head": [20, 0, 0],
                              "RightArm": [70, 10, 0], "LeftArm": [66, -10, 0]})),
    (0.6, merge(seated(0.7), {"Spine": [20, 0, 0], "RightArm": [40, 12, 0],
                              "LeftArm": [36, -12, 0]})),
    (1.0, {}),
], cat="parkour", desc="Приземление с кувырком")

ACT["vault_low"] = A(1.4, [
    (0.0, {"Spine": [12, 0, 0], "LeftUpLeg": [18, 0, 0], "RightUpLeg": [-10, 0, 0]}),
    (0.3, {"Spine": [26, 0, 0], "RightArm": [86, 14, 0], "RightForeArm": [6, 0, 0],
           "LeftUpLeg": [40, 0, 0], "LeftLeg": [-60, 0, 0], "@hips": [0, 10, 4]}),
    (0.55, {"Spine": [30, 0, 0], "RightArm": [92, 10, 0], "LeftArm": [30, -12, 0],
            "LeftUpLeg": [78, 0, 0], "LeftLeg": [-38, 0, 0], "RightUpLeg": [24, 0, 0],
            "RightLeg": [-46, 0, 0], "@hips": [0, 16, 6], "Head": [-16, 0, 0]}),
    (0.8, merge(seated(0.5), {"Spine": [16, 0, 0]})),
    (1.0, {}),
], cat="parkour", desc="Перепрыгивает низкое препятствие")

ACT["vault_high"] = A(1.8, [
    (0.0, {"Spine": [14, 0, 0]}),
    (0.25, {"Spine": [26, 0, 0], "RightArm": [96, 12, 0], "LeftArm": [92, -12, 0],
            "LeftUpLeg": [30, 0, 0], "LeftLeg": [-40, 0, 0]}),
    (0.5, {"Spine": [34, 0, 0], "RightArm": [104, 8, 0], "LeftArm": [100, -8, 0],
           "LeftUpLeg": [96, 0, 0], "LeftLeg": [-20, 0, 0], "RightUpLeg": [46, 0, 0],
           "RightLeg": [-60, 0, 0], "@hips": [0, 20, 8], "Head": [-20, 0, 0]}),
    (0.75, merge(seated(0.6), {"Spine": [18, 0, 0]})),
    (1.0, {}),
], cat="parkour", desc="Перепрыгивает высокое препятствие")

ACT["mantle"] = A(2.2, [
    (0.0, {**both_arms(100, 10, 16), "Spine": [8, 0, 0]}),
    (0.25, {**both_arms(110, 8, 6), "@hips": [0, 6, 4], "LeftUpLeg": [40, 0, 0],
            "LeftLeg": [-70, 0, 0], "Spine": [14, 0, 0]}),
    (0.5, {**both_arms(96, 12, 34), "@hips": [0, 20, 8], "LeftUpLeg": [96, 0, 0],
           "LeftLeg": [-30, 0, 0], "RightUpLeg": [30, 0, 0], "RightLeg": [-60, 0, 0],
           "Spine": [26, 0, 0], "Head": [-14, 0, 0]}),
    (0.75, {**both_arms(60, 14, 60), "@hips": [0, 16, 6], "LeftUpLeg": [70, 0, 0],
            "LeftLeg": [-56, 0, 0], "Spine": [18, 0, 0]}),
    (1.0, {}),
], cat="parkour", desc="Забирается на уступ")

ACT["climb_wall"] = A(2.4, [
    (0.0, {**both_arms(104, 10, 12), "LeftUpLeg": [40, 0, 0], "LeftLeg": [-80, 0, 0],
           "RightUpLeg": [10, 0, 0], "RightLeg": [-40, 0, 0]}),
    (0.5, {**both_arms(94, 12, 30), "RightUpLeg": [46, 0, 0], "RightLeg": [-86, 0, 0],
           "LeftUpLeg": [12, 0, 0], "LeftLeg": [-40, 0, 0], "@hips": [0, 3, 2]}),
    (1.0, {**both_arms(104, 10, 12), "LeftUpLeg": [40, 0, 0], "LeftLeg": [-80, 0, 0],
           "RightUpLeg": [10, 0, 0], "RightLeg": [-40, 0, 0]}),
], cat="parkour", desc="Лезет по стене (цикл)", loop=True)

ACT["climb_ladder"] = A(1.8, [
    (0.0, {**both_arms(112, 8, 6), "LeftUpLeg": [46, 0, 0], "LeftLeg": [-84, 0, 0],
           "RightUpLeg": [14, 0, 0], "RightLeg": [-46, 0, 0], "Spine": [-4, 0, 0]}),
    (0.5, {**both_arms(96, 10, 26), "RightUpLeg": [46, 0, 0], "RightLeg": [-84, 0, 0],
           "LeftUpLeg": [14, 0, 0], "LeftLeg": [-46, 0, 0], "Spine": [-2, 0, 0]}),
    (1.0, {**both_arms(112, 8, 6), "LeftUpLeg": [46, 0, 0], "LeftLeg": [-84, 0, 0],
           "RightUpLeg": [14, 0, 0], "RightLeg": [-46, 0, 0], "Spine": [-4, 0, 0]}),
], cat="parkour", desc="Лезет по лестнице (цикл)", loop=True)

ACT["ledge_hang"] = A(2.0, [
    (0.0, {**both_arms(120, 8, 0), "LeftUpLeg": [10, 0, 0], "LeftLeg": [-24, 0, 0],
           "RightUpLeg": [4, 0, 0], "RightLeg": [-16, 0, 0], "Spine": [-2, 0, 0]}),
    (0.5, {**both_arms(118, 8, 2), "LeftUpLeg": [12, 0, 0], "LeftLeg": [-28, 0, 0],
           "RightUpLeg": [6, 0, 0], "RightLeg": [-20, 0, 0], "Spine": [0, 0, 2]}),
    (1.0, {**both_arms(120, 8, 0), "LeftUpLeg": [10, 0, 0], "LeftLeg": [-24, 0, 0],
           "RightUpLeg": [4, 0, 0], "RightLeg": [-16, 0, 0], "Spine": [-2, 0, 0]}),
], cat="parkour", desc="Висит на уступе (цикл)", loop=True)

ACT["shimmy"] = A(2.0, [
    (0.0, {**both_arms(118, 8, 4), "LeftUpLeg": [14, 0, 0], "LeftLeg": [-30, 0, 0],
           "RightUpLeg": [6, 0, 0], "RightLeg": [-18, 0, 0], "Spine": [0, 0, 4]}),
    (0.5, {**both_arms(116, 6, 8), "LeftUpLeg": [8, 0, 0], "LeftLeg": [-22, 0, 0],
           "RightUpLeg": [12, 0, 0], "RightLeg": [-26, 0, 0], "Spine": [0, 0, -4]}),
    (1.0, {**both_arms(118, 8, 4), "LeftUpLeg": [14, 0, 0], "LeftLeg": [-30, 0, 0],
           "RightUpLeg": [6, 0, 0], "RightLeg": [-18, 0, 0], "Spine": [0, 0, 4]}),
], cat="parkour", desc="Двигается вдоль уступа (цикл)", loop=True)

ACT["climb_fence"] = A(2.0, [
    (0.0, {}),
    (0.25, {**both_arms(100, 10, 20), "Spine": [12, 0, 0], "LeftUpLeg": [30, 0, 0],
            "LeftLeg": [-60, 0, 0]}),
    (0.5, {**both_arms(90, 12, 40), "@hips": [0, 22, 4], "LeftUpLeg": [92, 0, 0],
           "LeftLeg": [-30, 0, 0], "RightUpLeg": [40, 0, 0], "RightLeg": [-70, 0, 0],
           "Spine": [22, 0, 0]}),
    (0.75, {**both_arms(50, 14, 60), "@hips": [0, 10, 0], "Spine": [12, 0, 0],
            "LeftUpLeg": [40, 0, 0], "LeftLeg": [-50, 0, 0]}),
    (1.0, {}),
], cat="parkour", desc="Перелезает через забор")

ACT["drop_down"] = A(0.9, [
    (0.0, merge(seated(0.3), {"Spine": [-4, 0, 0]})),
    (0.4, {"LeftUpLeg": [-10, 0, 0], "RightUpLeg": [-10, 0, 0], "LeftFoot": [-20, 0, 0],
           "RightFoot": [-20, 0, 0], "@hips": [0, 4, 0], "Spine": [-2, 0, 0]}),
    (0.7, merge(seated(0.7), {"Spine": [14, 0, 0]})),
    (1.0, {}),
], cat="parkour", desc="Спрыгивает вниз")

# --------------------------------------------------------------------------- #
# реакции, смерть
# --------------------------------------------------------------------------- #

ACT["hit_light"] = A(0.7, [
    (0.0, {}),
    (0.2, {"Spine": [-10, -8, 0], "Head": [-12, -10, 0], "LeftArm": [-12, -6, 0],
           "RightArm": [-8, 6, 0], "@hips": [0, -1, -2]}),
    (0.5, {"Spine": [-4, -3, 0], "Head": [-4, -4, 0]}),
    (1.0, {}),
], cat="reaction", desc="Получает лёгкий удар")

ACT["hit_heavy"] = A(1.1, [
    (0.0, {}),
    (0.18, {"Spine": [-22, -14, 0], "Spine1": [-10, -8, 0], "Head": [-20, -12, 0],
            "LeftArm": [-24, -14, 0], "RightArm": [-18, 10, 0], "@hips": [0, -4, -6],
            "LeftUpLeg": [12, 0, 0], "RightUpLeg": [-6, 0, 0]}),
    (0.55, {"Spine": [-8, -4, 0], "Head": [-6, -4, 0], "@hips": [0, -2, -2]}),
    (1.0, {}),
], cat="reaction", desc="Получает тяжёлый удар")

ACT["hit_head"] = A(0.9, [
    (0.0, {}),
    (0.15, {"Head": [-26, -6, 0], "Neck": [-12, 0, 0], "Spine": [-12, -6, 0],
            "RightArm": [-10, 10, 0], "LeftArm": [-14, -10, 0]}),
    (0.5, {"Head": [-8, -2, 0], "Spine": [-4, -2, 0]}),
    (1.0, {}),
], cat="reaction", desc="Удар в голову")

ACT["hit_back"] = A(1.0, [
    (0.0, {}),
    (0.2, {"Spine": [16, 0, 0], "Spine1": [8, 0, 0], "Head": [14, 0, 0],
           "RightArm": [40, 16, 0], "LeftArm": [36, -16, 0], "@hips": [0, -2, 4]}),
    (0.6, {"Spine": [6, 0, 0], "Head": [4, 0, 0]}),
    (1.0, {}),
], cat="reaction", desc="Удар в спину")

ACT["knockdown"] = A(1.4, [
    (0.0, {}),
    (0.2, {"Spine": [-16, 0, 0], "Head": [-16, 0, 0], "RightArm": [-30, 14, 0],
           "LeftArm": [-26, -14, 0], "@hips": [0, -4, -4]}),
    (0.5, merge(seated(1.1), {"Spine": [-10, 0, 0], "LeftUpLeg": [70, 0, 0],
                              "RightUpLeg": [40, 0, 0], "LeftLeg": [-80, 0, 0],
                              "RightLeg": [-60, 0, 0], "RightArm": [70, 30, 0],
                              "LeftArm": [64, -30, 0], "@hips": [0, -40, -6]})),
    (1.0, merge(seated(1.0), {"Spine": [-4, 0, 0], "LeftUpLeg": [74, 0, 0],
                              "RightUpLeg": [44, 0, 0], "LeftLeg": [-86, 0, 0],
                              "RightLeg": [-64, 0, 0], "RightArm": [60, 26, 0],
                              "LeftArm": [56, -26, 0]})),
], cat="reaction", desc="Падает на землю")

ACT["getup"] = A(1.8, [
    (0.0, merge(seated(1.0), {"LeftUpLeg": [74, 0, 0], "RightUpLeg": [44, 0, 0],
                              "LeftLeg": [-86, 0, 0], "RightLeg": [-64, 0, 0]})),
    (0.35, merge(seated(1.1), {"Spine": [24, 0, 0], "RightArm": [80, 20, 0],
                               "RightForeArm": [30, 0, 0]}),
     ),
    (0.65, merge(seated(0.7), {"Spine": [20, 0, 0]})),
    (1.0, {}),
], cat="reaction", desc="Встаёт с земли")

ACT["death_forward"] = A(1.8, [
    (0.0, {}),
    (0.15, {"Spine": [-14, 0, 0], "Head": [-16, 0, 0], "RightArm": [-24, 12, 0],
            "LeftArm": [-20, -12, 0]}),
    (0.45, merge(seated(0.6), {"Spine": [30, 0, 0], "Head": [20, 0, 0],
                               "RightArm": [50, 24, 0], "LeftArm": [46, -24, 0]})),
    (0.7, {"Hips": [62, 0, 0], "Spine": [18, 0, 0], "Head": [16, 0, 0],
           "LeftUpLeg": [-18, 0, 0], "RightUpLeg": [-12, 0, 0],
           "LeftLeg": [-26, 0, 0], "RightLeg": [-20, 0, 0],
           "RightArm": [70, 40, 0], "LeftArm": [66, -40, 0],
           "@hips": [0, -52, 10]}),
    (0.88, {"Hips": [86, 0, 0], "Spine": [6, 0, 0], "Neck": [-4, 0, 0],
            "Head": [8, 0, 0], "LeftUpLeg": [-8, 0, 0], "RightUpLeg": [-4, 0, 0],
            "LeftLeg": [-14, 0, 0], "RightLeg": [-10, 0, 0],
            "RightArm": [58, 46, 0], "LeftArm": [54, -46, 0],
            "@hips": [0, -70, 8]}),
    (1.0, {"Hips": [88, 0, 0], "Spine": [4, 0, 0], "Neck": [-2, 0, 0],
           "Head": [10, 0, 0], "LeftUpLeg": [-6, 0, 0], "RightUpLeg": [-2, 0, 0],
           "LeftLeg": [-10, 0, 0], "RightLeg": [-6, 0, 0],
           "RightArm": [56, 48, 0], "LeftArm": [52, -48, 0],
           "@hips": [0, -72, 6]}),
], cat="reaction", desc="Падает лицом вниз")

ACT["death_back"] = A(1.8, [
    (0.0, {}),
    (0.18, {"Spine": [-18, 0, 0], "Head": [-22, 0, 0], "RightArm": [-34, 18, 0],
            "LeftArm": [-30, -18, 0], "@hips": [0, -2, -4]}),
    (0.45, {"Hips": [-42, 0, 0], "Spine": [-12, 0, 0], "Head": [-6, 0, 0],
            "LeftUpLeg": [-26, 0, 0], "RightUpLeg": [-18, 0, 0],
            "LeftLeg": [-30, 0, 0], "RightLeg": [-22, 0, 0],
            "@hips": [0, -34, -10], "RightArm": [-40, 26, 0], "LeftArm": [-36, -26, 0]}),
    (1.0, {"Hips": [-86, 0, 0], "Spine": [-6, 0, 0], "Neck": [4, 0, 0],
           "Head": [-8, 0, 0], "LeftUpLeg": [8, 0, 0], "RightUpLeg": [4, 0, 0],
           "LeftLeg": [-12, 0, 0], "RightLeg": [-8, 0, 0],
           "@hips": [0, -72, -6], "RightArm": [-56, 48, 0], "LeftArm": [-52, -48, 0]}),
], cat="reaction", desc="Падает на спину")

ACT["death_headshot"] = A(1.2, [
    (0.0, {}),
    (0.15, {"Head": [-30, 0, 0], "Neck": [-14, 0, 0], "Spine": [-12, 0, 0],
            "RightArm": [-16, 14, 0], "LeftArm": [-14, -14, 0]}),
    (0.5, merge(seated(0.8), {"Spine": [-20, 0, 0], "Head": [-20, 0, 0],
                              "LeftUpLeg": [50, 0, 0], "RightUpLeg": [30, 0, 0],
                              "LeftLeg": [-70, 0, 0], "RightLeg": [-50, 0, 0],
                              "@hips": [0, -36, -8]})),
    (1.0, merge(seated(1.1), {"Spine": [-26, 0, 0], "Head": [-16, 0, 0],
                              "LeftUpLeg": [56, 0, 0], "RightUpLeg": [34, 0, 0],
                              "LeftLeg": [-74, 0, 0], "RightLeg": [-54, 0, 0],
                              "@hips": [0, -46, -14], "RightArm": [-30, 30, 0],
                              "LeftArm": [-26, -30, 0]})),
], cat="reaction", desc="Смерть от выстрела в голову")

ACT["death_crouch"] = A(1.6, [
    (0.0, merge(seated(0.8), {})),
    (0.35, merge(seated(1.1), {"Spine": [40, 0, 0], "Head": [24, 0, 0],
                               "RightArm": [60, 20, 0], "LeftArm": [56, -20, 0]})),
    (1.0, merge(seated(1.2), {"Spine": [56, 0, 0], "Head": [20, 0, 0],
                              "LeftUpLeg": [60, 0, 0], "RightUpLeg": [40, 0, 0],
                              "LeftLeg": [-100, 0, 0], "RightLeg": [-80, 0, 0],
                              "RightArm": [80, 24, 0], "LeftArm": [76, -24, 0],
                              "@hips": [0, -50, 8]})),
], cat="reaction", desc="Падает из приседа")

ACT["stumble"] = A(1.2, [
    (0.0, {}),
    (0.25, {"Spine": [-10, 8, 0], "LeftUpLeg": [24, 0, 0], "LeftLeg": [-40, 0, 0],
            "RightArm": [-30, 14, 0], "LeftArm": [-24, -14, 0], "@hips": [0, -2, -2]}),
    (0.5, {"Spine": [10, -8, 0], "RightUpLeg": [20, 0, 0], "RightLeg": [-34, 0, 0],
           "RightArm": [30, 18, 0], "LeftArm": [26, -18, 0]}),
    (1.0, {}),
], cat="reaction", desc="Спотыкается")

ACT["fall_loop"] = A(1.0, [
    (0.0, {**both_arms(96, 26, 20), "LeftUpLeg": [30, 0, 0], "RightUpLeg": [-10, 0, 0],
           "LeftLeg": [-40, 0, 0], "RightLeg": [-20, 0, 0], "Spine": [-6, 0, 0]}),
    (0.5, {**both_arms(90, 32, 28), "LeftUpLeg": [16, 0, 0], "RightUpLeg": [6, 0, 0],
           "LeftLeg": [-26, 0, 0], "RightLeg": [-34, 0, 0], "Spine": [4, 0, 4]}),
    (1.0, {**both_arms(96, 26, 20), "LeftUpLeg": [30, 0, 0], "RightUpLeg": [-10, 0, 0],
           "LeftLeg": [-40, 0, 0], "RightLeg": [-20, 0, 0], "Spine": [-6, 0, 0]}),
], cat="reaction", desc="Падение (цикл)", loop=True)
