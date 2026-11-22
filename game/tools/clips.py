"""Клипы анимаций: генераторы поз.

Поза — плоский словарь:
    'LeftUpLeg': [pitch, yaw, roll]   (градусы, мировые оси покоя)
    '@hips':     [dx, dy, dz]         (смещение таза в сантиметрах модели)
Позы из разных источников складываются (см. `merge`).
"""

import math

TAU = math.pi * 2.0


# --------------------------------------------------------------------------- #
# позы
# --------------------------------------------------------------------------- #

def merge(*poses):
    out = {}
    for d in poses:
        if not d:
            continue
        for k, v in d.items():
            cur = out.get(k)
            if cur is None:
                out[k] = [float(v[0]), float(v[1]), float(v[2])]
            else:
                cur[0] += v[0]
                cur[1] += v[1]
                cur[2] += v[2]
    return out


def put(d, role, p=0.0, y=0.0, r=0.0, x=None, z=None):
    """Добавить вектор к позе кости (p,y,r) или смещение таза (x,y,z)."""
    if x is not None:
        p = x
    if z is not None:
        r = z
    cur = d.get(role)
    if cur is None:
        d[role] = [p, y, r]
    else:
        cur[0] += p
        cur[1] += y
        cur[2] += r
    return d


def to_pose(flat):
    bones = {k: tuple(v) for k, v in flat.items() if k != "@hips"}
    return {"bones": bones, "hips": tuple(flat.get("@hips", (0.0, 0.0, 0.0)))}


def lerp(a, b, u):
    return a + (b - a) * u


def smooth(u):
    u = max(0.0, min(1.0, u))
    return u * u * (3.0 - 2.0 * u)


def ease_out(u):
    u = max(0.0, min(1.0, u))
    return 1.0 - (1.0 - u) ** 2


def ease_in(u):
    u = max(0.0, min(1.0, u))
    return u * u


def blend_flat(a, b, u):
    keys = set(a) | set(b)
    out = {}
    for k in keys:
        av = a.get(k, (0.0, 0.0, 0.0))
        bv = b.get(k, (0.0, 0.0, 0.0))
        out[k] = [lerp(av[i], bv[i], u) for i in range(3)]
    return out


def keyframes(keys, ease="smooth"):
    """keys: [(t, flat_pose), ...] с t в [0,1]. Возвращает f(p) -> flat_pose."""
    keys = sorted(keys, key=lambda k: k[0])
    fn = {"smooth": smooth, "out": ease_out, "in": ease_in,
          "linear": lambda u: max(0.0, min(1.0, u))}[ease]

    def f(p):
        p = max(0.0, min(1.0, p))
        if p <= keys[0][0]:
            return dict(keys[0][1])
        for (t0, a), (t1, b) in zip(keys, keys[1:]):
            if p <= t1:
                span = t1 - t0
                u = 0.0 if span <= 0 else (p - t0) / span
                return blend_flat(a, b, fn(u))
        return dict(keys[-1][1])

    return f


# --------------------------------------------------------------------------- #
# базовые стойки и положения рук
# --------------------------------------------------------------------------- #

def _arm_free(d, swing=0.0, bend=8.0):
    put(d, "LeftArm", p=swing, r=-1.5)
    put(d, "RightArm", p=-swing, r=1.5)
    put(d, "LeftForeArm", p=bend)
    put(d, "RightForeArm", p=bend)


def arms_preset(name):
    """Добавка к позе для разных положений рук (поверх опущенных рук базы)."""
    d = {}
    if name == "pistol" or name == "pistol_low":
        low = 18.0 if name == "pistol_low" else 0.0
        put(d, "Spine", p=4 - low * 0.1, y=8)
        put(d, "Spine2", p=2, y=4)
        put(d, "RightShoulder", p=-4, r=6)
        put(d, "RightArm", p=78 - low * 0.8, y=24, r=-4)
        put(d, "RightForeArm", p=6 + low * 0.5, y=-6)
        put(d, "RightHand", p=-8, r=-6)
        put(d, "LeftShoulder", p=-4, r=-6)
        put(d, "LeftArm", p=70 - low * 0.8, y=-34, r=6)
        put(d, "LeftForeArm", p=20 + low * 0.5, y=10)
        put(d, "LeftHand", p=-4, r=6)
        put(d, "Head", p=-3, y=-4)
    elif name == "rifle":
        put(d, "Spine", p=6, y=14)
        put(d, "Spine2", p=3, y=6)
        put(d, "RightArm", p=80, y=34, r=-8)
        put(d, "RightForeArm", p=42, y=-16)
        put(d, "RightHand", p=-10, r=-10)
        put(d, "LeftArm", p=76, y=-40, r=8)
        put(d, "LeftForeArm", p=52, y=18)
        put(d, "LeftHand", p=-6, r=10)
        put(d, "Head", p=2, y=-6)
    elif name == "shotgun":
        put(d, "Spine", p=8, y=12)
        put(d, "RightArm", p=86, y=30, r=-6)
        put(d, "RightForeArm", p=30, y=-12)
        put(d, "LeftArm", p=82, y=-34, r=6)
        put(d, "LeftForeArm", p=26, y=14)
        put(d, "Head", p=3, y=-4)
    elif name == "guard":
        put(d, "Spine", p=6, y=6)
        put(d, "RightArm", p=34, y=16, r=-6)
        put(d, "RightForeArm", p=66)
        put(d, "LeftArm", p=30, y=-16, r=6)
        put(d, "LeftForeArm", p=72)
        put(d, "Head", p=-4)
    elif name == "block":
        put(d, "Spine", p=8)
        put(d, "RightArm", p=52, y=22)
        put(d, "RightForeArm", p=88, y=-10)
        put(d, "LeftArm", p=50, y=-22)
        put(d, "LeftForeArm", p=90, y=10)
        put(d, "Head", p=6)
    elif name == "phone":
        put(d, "RightArm", p=42, y=-26, r=4)
        put(d, "RightForeArm", p=104)
        put(d, "RightHand", p=12, y=10)
        put(d, "Head", p=4, y=8)
    elif name == "smoke":
        put(d, "RightArm", p=46, y=-22)
        put(d, "RightForeArm", p=96)
        put(d, "RightHand", p=16, y=12)
        put(d, "Head", p=2, y=6)
    elif name == "box":
        put(d, "Spine", p=10)
        put(d, "RightArm", p=20, y=26)
        put(d, "RightForeArm", p=76, y=-14)
        put(d, "LeftArm", p=20, y=-26)
        put(d, "LeftForeArm", p=76, y=14)
    elif name == "body":
        put(d, "Spine", p=14, y=10)
        put(d, "RightArm", p=52, y=18)
        put(d, "RightForeArm", p=54)
        put(d, "LeftArm", p=44, y=-18)
        put(d, "LeftForeArm", p=62)
        put(d, "Head", p=6)
    elif name == "crossed":
        put(d, "Spine", p=4)
        put(d, "RightArm", p=30, y=34)
        put(d, "RightForeArm", p=112, y=-16)
        put(d, "LeftArm", p=28, y=-34)
        put(d, "LeftForeArm", p=118, y=16)
    elif name == "pockets":
        put(d, "RightArm", p=8, y=10)
        put(d, "RightForeArm", p=30)
        put(d, "LeftArm", p=8, y=-10)
        put(d, "LeftForeArm", p=30)
    elif name == "reach":  # правой рукой вперёд к объекту
        put(d, "Spine", p=8, y=-6)
        put(d, "RightArm", p=74, y=14)
        put(d, "RightForeArm", p=10)
        put(d, "RightHand", p=-6)
        put(d, "LeftArm", p=10, r=-4)
    elif name == "gloves":
        put(d, "RightArm", p=26, y=10)
        put(d, "RightForeArm", p=76)
        put(d, "LeftArm", p=26, y=-10)
        put(d, "LeftForeArm", p=76)
    return d


def stance(crouch=0.0, lean=0.0, arms="free", hip_y=0.0, sway=0.0, knee_out=0.0):
    d = {}
    if crouch:
        put(d, "LeftUpLeg", p=52 * crouch)
        put(d, "RightUpLeg", p=52 * crouch)
        put(d, "LeftLeg", p=-104 * crouch)
        put(d, "RightLeg", p=-104 * crouch)
        put(d, "LeftFoot", p=48 * crouch)
        put(d, "RightFoot", p=48 * crouch)
        put(d, "Spine", p=16 * crouch)
        put(d, "Spine1", p=6 * crouch)
        put(d, "Head", p=-14 * crouch)
        put(d, "@hips", y=-26.0 * crouch)
    if lean:
        put(d, "Hips", p=2.0 * lean)
        put(d, "Spine", p=3.0 * lean)
        put(d, "Spine1", p=2.0 * lean)
        put(d, "@hips", 0.0, 0.0, 1.5 * lean)
    if knee_out:
        put(d, "LeftUpLeg", r=8 * knee_out)
        put(d, "RightUpLeg", r=-8 * knee_out)
    if hip_y:
        put(d, "@hips", y=hip_y)
    if sway:
        put(d, "Hips", r=2.0 * sway)
        put(d, "Spine", r=-1.2 * sway)
    if arms != "none":
        d = merge(d, arms_preset(arms))
    return d


# --------------------------------------------------------------------------- #
# локомоция
# --------------------------------------------------------------------------- #

def locomotion(cfg):
    """cfg: stride, knee, arm, bob, lean, crouch, arms, limp, side, back,
    sway, speed(cycle duration), hip_drop, look, phone_walk, foot_lift."""
    stride = cfg.get("stride", 26.0)
    knee = cfg.get("knee", 46.0)
    arm_swing = cfg.get("arm", 22.0)
    bob = cfg.get("bob", 1.6)
    lean = cfg.get("lean", 6.0)
    crouch = cfg.get("crouch", 0.0)
    arms = cfg.get("arms", "free")
    limp = cfg.get("limp", 0.0)
    side = cfg.get("side", 0.0)
    back = cfg.get("back", 0.0)
    sway = cfg.get("sway", 1.0)
    hip_y = cfg.get("hip_drop", 0.0)
    elbow = cfg.get("elbow", 10.0)
    stride_l = stride * (1.0 - 0.45 * limp)
    stride_r = stride * (1.0 + 0.15 * limp)
    knee_l = knee * (1.0 + 0.4 * limp)
    knee_r = knee

    def f(p):
        th = TAU * p
        s, c = math.sin(th), math.cos(th)
        d = stance(crouch=crouch, lean=lean * 0.1, arms="none", hip_y=hip_y)
        # ноги
        swing_l = s * stride_l
        swing_r = -s * stride_r
        put(d, "LeftUpLeg", p=swing_l, r=side * 6.0)
        put(d, "RightUpLeg", p=swing_r, r=side * 6.0)
        knee_l_ang = -knee_l * (0.5 - 0.5 * math.sin(th + 0.7)) - 4.0 * limp
        knee_r_ang = -knee_r * (0.5 - 0.5 * math.sin(th + math.pi + 0.7))
        put(d, "LeftLeg", p=knee_l_ang)
        put(d, "RightLeg", p=knee_r_ang)
        put(d, "LeftFoot", p=-(swing_l + knee_l_ang) * 0.55)
        put(d, "RightFoot", p=-(swing_r + knee_r_ang) * 0.55)
        put(d, "LeftToeBase", p=-8.0 - 6.0 * max(0.0, -s))
        put(d, "RightToeBase", p=-8.0 - 6.0 * max(0.0, s))
        # таз
        put(d, "Hips", p=1.5, r=sway * 2.4 * s, y=-back * 6.0 * s + side * 4.0)
        put(d, "@hips", x=sway * 1.8 * s, y=bob * (0.5 - 0.5 * math.cos(2 * th)))
        put(d, "Spine", r=-1.6 * sway * s, y=(back * 6.0 * s) + (side * 4.0 * c), p=lean * 0.35)
        put(d, "Spine1", p=lean * 0.25, y=side * 2.0 * c)
        put(d, "Spine2", p=lean * 0.15)
        put(d, "Neck", p=-lean * 0.35)
        put(d, "Head", p=-lean * 0.25, y=-(back * 4.0 * s) - side * 3.0 * c)
        # руки
        if arms != "none":
            d = merge(d, arms_preset(arms))
            sw = arm_swing * (1.0 if arms in ("free", "pockets") else 0.25)
            put(d, "LeftArm", p=-s * sw)
            put(d, "RightArm", p=s * sw)
            put(d, "LeftForeArm", p=elbow + 8.0 * max(0.0, -s))
            put(d, "RightForeArm", p=elbow + 8.0 * max(0.0, s))
        else:
            put(d, "LeftArm", p=-s * arm_swing)
            put(d, "RightArm", p=s * arm_swing)
            put(d, "LeftForeArm", p=elbow + 8.0 * max(0.0, -s))
            put(d, "RightForeArm", p=elbow + 8.0 * max(0.0, s))
        return d

    return f


def idle(cfg):
    """Дыхание, перенос веса, мелкие движения."""
    period = cfg.get("period", 4.0)
    breath = cfg.get("breath", 1.6)
    sway = cfg.get("sway", 1.0)
    crouch = cfg.get("crouch", 0.0)
    arms = cfg.get("arms", "free")
    shiver = cfg.get("shiver", 0.0)
    tired = cfg.get("tired", 0.0)
    look = cfg.get("look", 1.0)
    impatient = cfg.get("impatient", 0.0)
    drunk = cfg.get("drunk", 0.0)

    def f(p):
        th = TAU * p
        base = stance(crouch=crouch, arms=arms, lean=cfg.get("lean", 0.0),
                      sway=0.4 * math.sin(th))
        b = math.sin(th * max(1.0, period / 2.0))
        put(base, "Spine1", p=breath * b)
        put(base, "Spine2", p=breath * 0.6 * b)
        put(base, "Neck", p=-breath * 0.5 * b)
        put(base, "@hips", y=0.5 * b - 6.0 * tired)
        put(base, "Hips", r=sway * 1.2 * math.sin(th * 0.5))
        put(base, "Spine", r=-sway * 0.8 * math.sin(th * 0.5), p=6.0 * tired)
        put(base, "Head", y=look * 4.0 * math.sin(th * 0.35), p=-4.0 * tired)
        if look:
            put(base, "Head", y=look * 3.0 * math.sin(th * 0.23 + 1.0))
        if shiver:
            s2 = math.sin(TAU * p * 7.0)
            put(base, "LeftArm", r=shiver * 1.5 * s2)
            put(base, "RightArm", r=-shiver * 1.5 * s2)
            put(base, "Spine", p=shiver * 3.0, y=shiver * 2.0 * s2)
        if tired:
            put(base, "LeftUpLeg", p=14.0 * tired)
            put(base, "RightUpLeg", p=14.0 * tired)
            put(base, "LeftLeg", p=-22.0 * tired)
            put(base, "RightLeg", p=-22.0 * tired)
            put(base, "@hips", y=-10.0 * tired)
            put(base, "LeftArm", p=20.0 * tired, y=-8.0 * tired)
            put(base, "RightArm", p=20.0 * tired, y=8.0 * tired)
            put(base, "LeftForeArm", p=40.0 * tired)
            put(base, "RightForeArm", p=40.0 * tired)
        if impatient:
            tap = max(0.0, math.sin(TAU * p * 6.0))
            put(base, "RightFoot", p=-18.0 * impatient * tap)
            put(base, "RightToeBase", p=-10.0 * impatient * tap)
            put(base, "Hips", r=2.0 * impatient)
        if drunk:
            put(base, "Hips", r=5.0 * drunk * math.sin(th * 0.5))
            put(base, "Spine", r=-4.0 * drunk * math.sin(th * 0.5 + 0.4))
            put(base, "@hips", x=3.0 * drunk * math.sin(th * 0.5))
            put(base, "Head", p=8.0 * drunk, r=3.0 * drunk * math.sin(th * 0.7))
        return base

    return f


# --------------------------------------------------------------------------- #
# действия из ключевых кадров
# --------------------------------------------------------------------------- #

def action(cfg):
    """cfg: crouch, lean, arms, keys=[(t, overrides)]"""
    crouch = cfg.get("crouch", 0.0)
    lean = cfg.get("lean", 0.0)
    arms = cfg.get("arms", "free")
    base = stance(crouch=crouch, lean=lean, arms=arms)
    keys = [(t, merge(base, o)) for t, o in cfg["keys"]]
    return keyframes(keys, cfg.get("ease", "smooth"))


def recoil_curve(p, decay=9.0):
    return math.exp(-decay * p) * math.cos(TAU * 1.6 * p)


# --------------------------------------------------------------------------- #
# огнестрел
# --------------------------------------------------------------------------- #

def firearm(cfg):
    """cfg: aim ('pistol','rifle','shotgun','pistol_low'), shots, burst, reload,
    crouch, cover ('left'/'right'), recoil."""
    aim = cfg.get("aim", "pistol")
    shots = cfg.get("shots", 1)
    burst = cfg.get("burst", 3)
    crouch = cfg.get("crouch", 0.0)
    cover = cfg.get("cover", None)
    rec = cfg.get("recoil", 7.0)
    aim_pose = stance(crouch=crouch, arms=aim)
    if cover == "right":
        base = merge(aim_pose, {
            "Spine": [2, -18, 8], "Hips": [0, -10, 0],
            "RightArm": [6, -14, 0], "LeftArm": [-6, 12, 0],
        })
    elif cover == "left":
        base = merge(aim_pose, {
            "Spine": [2, 18, -8], "Hips": [0, 10, 0],
            "RightArm": [8, 10, 0], "LeftArm": [-4, -10, 0],
        })
    else:
        base = aim_pose

    def f(p):
        d = {k: list(v) for k, v in base.items()}  # глубокая копия, иначе дельты копятся
        if cfg.get("sway"):
            sw = cfg["sway"]
            put(d, "Spine1", p=sw * math.sin(TAU * p * 2.0))
            put(d, "Spine", y=sw * 0.6 * math.sin(TAU * p * 1.0))
            put(d, "Head", y=sw * 0.8 * math.sin(TAU * p * 0.5))
            put(d, "@hips", y=sw * 0.4 * math.sin(TAU * p * 2.0))
        kick = 0.0
        step_t = 1.0 / max(1, shots)
        for i in range(shots):
            t0 = i * step_t + 0.15 * step_t
            if p >= t0:
                kick = max(kick, math.exp(-(p - t0) * 26.0) * math.cos(TAU * 3.0 * (p - t0)))
        if cfg.get("burst"):
            # длинная очередь с частотой
            kick = 0.65 * math.exp(-((p * burst) % 1.0) * 6.0) * (1.0 if p < 0.85 else 0.0)
        if kick:
            put(d, "Spine", p=-rec * 0.5 * kick, y=2.0 * kick)
            put(d, "Spine1", p=-rec * 0.35 * kick)
            put(d, "RightArm", p=-rec * 1.2 * kick, r=1.5 * kick)
            put(d, "LeftArm", p=-rec * 0.9 * kick)
            put(d, "RightForeArm", p=-rec * 0.4 * kick)
            put(d, "Head", p=-2.5 * kick)
            put(d, "@hips", 0.0, -0.8 * kick, -0.8 * kick)
        return d

    return f
