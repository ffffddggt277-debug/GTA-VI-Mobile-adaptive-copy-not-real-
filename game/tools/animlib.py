"""animlib — сборка скелетных анимаций прямо в существующие GLB-персонажи.

Идея: у всех людей в models/sketchfab один и тот же риг (mixamo, 66 костей),
поэтому клипы можно генерировать процедурно и печь в GLB как обычные glTF-анимации.

Позы задаются в мировых осях покоя персонажа (персонаж смотрит в +Z, верх +Y):
    pose[role] = (pitch, yaw, roll)  в градусах
    pitch > 0 — наклон/мах вперёд (+Z)
    yaw   > 0 — поворот влево (вокруг +Y)
    roll  > 0 — наклон вправо (вокруг +Z)

Мировая добавка к кости переводится в локальную так:
    L = P^-1 * W * P * L_base
где P — мировая ориентация покоя родителя, L_base — локальная ориентация базы
(T-поза + опущенные руки).
"""

import json
import math
import re
import struct

import numpy as np

# --------------------------------------------------------------------------- #
# кватернионы (x, y, z, w), numpy
# --------------------------------------------------------------------------- #


def qnorm(q):
    q = np.asarray(q, dtype=np.float64)
    n = np.linalg.norm(q)
    return q / n if n > 1e-12 else np.array([0.0, 0.0, 0.0, 1.0])


def qmul(a, b):
    ax, ay, az, aw = a
    bx, by, bz, bw = b
    return np.array([
        aw * bx + ax * bw + ay * bz - az * by,
        aw * by - ax * bz + ay * bw + az * bx,
        aw * bz + ax * by - ay * bx + az * bw,
        aw * bw - ax * bx - ay * by - az * bz,
    ])


def qaxis(axis, deg):
    if abs(deg) < 1e-12:
        return np.array([0.0, 0.0, 0.0, 1.0])
    h = math.radians(deg) * 0.5
    s = math.sin(h)
    return np.array([axis[0] * s, axis[1] * s, axis[2] * s, math.cos(h)])


X = (1.0, 0.0, 0.0)
Y = (0.0, 1.0, 0.0)
Z = (0.0, 0.0, 1.0)


def qfrom_pyr(pitch, yaw, roll):
    """Мировая добавка: сначала pitch, потом roll, потом yaw."""
    return qmul(qmul(qaxis(Y, yaw), qaxis(Z, roll)), qaxis(X, -pitch))


def qmat(q):
    x, y, z, w = q
    return np.array([
        [1 - 2 * (y * y + z * z), 2 * (x * y - z * w), 2 * (x * z + y * w)],
        [2 * (x * y + z * w), 1 - 2 * (x * x + z * z), 2 * (y * z - x * w)],
        [2 * (x * z - y * w), 2 * (y * z + x * w), 1 - 2 * (x * x + y * y)],
    ])


def mat2q(m):
    m = np.asarray(m, dtype=np.float64)
    t = np.trace(m)
    if t > 0:
        s = math.sqrt(t + 1.0) * 2
        w = 0.25 * s
        x = (m[2, 1] - m[1, 2]) / s
        y = (m[0, 2] - m[2, 0]) / s
        z = (m[1, 0] - m[0, 1]) / s
    elif m[0, 0] > m[1, 1] and m[0, 0] > m[2, 2]:
        s = math.sqrt(1.0 + m[0, 0] - m[1, 1] - m[2, 2]) * 2
        w = (m[2, 1] - m[1, 2]) / s
        x = 0.25 * s
        y = (m[0, 1] + m[1, 0]) / s
        z = (m[0, 2] + m[2, 0]) / s
    elif m[1, 1] > m[2, 2]:
        s = math.sqrt(1.0 + m[1, 1] - m[0, 0] - m[2, 2]) * 2
        w = (m[0, 2] - m[2, 0]) / s
        x = (m[0, 1] + m[1, 0]) / s
        y = 0.25 * s
        z = (m[1, 2] + m[2, 1]) / s
    else:
        s = math.sqrt(1.0 + m[2, 2] - m[0, 0] - m[1, 1]) * 2
        w = (m[1, 0] - m[0, 1]) / s
        x = (m[0, 2] + m[2, 0]) / s
        y = (m[1, 2] + m[2, 1]) / s
        z = 0.25 * s
    return qnorm(np.array([x, y, z, w]))


# --------------------------------------------------------------------------- #
# GLB чтение / запись
# --------------------------------------------------------------------------- #

JSON_CHUNK = 0x4E4F534A
BIN_CHUNK = 0x004E4942


def read_glb(path):
    data = open(path, "rb").read()
    magic, version, length = struct.unpack_from("<III", data, 0)
    if magic != 0x46546C67:
        raise ValueError(f"{path}: not a GLB")
    off, js, binc = 12, None, b""
    while off < length:
        ln, ty = struct.unpack_from("<II", data, off)
        off += 8
        chunk = data[off:off + ln]
        off += ln
        if ty == JSON_CHUNK:
            js = json.loads(chunk)
        elif ty == BIN_CHUNK:
            binc = chunk
    return js, bytearray(binc)


def write_glb(path, js, binc):
    jb = json.dumps(js, separators=(",", ":")).encode("utf-8")
    jb += b" " * ((4 - len(jb) % 4) % 4)
    bb = bytes(binc) + b"\0" * ((4 - len(binc) % 4) % 4)
    total = 12 + 8 + len(jb) + (8 + len(bb) if bb else 0)
    out = bytearray()
    out += struct.pack("<III", 0x46546C67, 2, total)
    out += struct.pack("<II", len(jb), JSON_CHUNK) + jb
    if bb:
        out += struct.pack("<II", len(bb), BIN_CHUNK) + bb
    with open(path, "wb") as f:
        f.write(out)


# --------------------------------------------------------------------------- #
# риг
# --------------------------------------------------------------------------- #

ROLES = [
    "Hips", "Spine", "Spine1", "Spine2", "Neck", "Head", "HeadTop_End",
    "LeftShoulder", "LeftArm", "LeftForeArm", "LeftHand",
    "RightShoulder", "RightArm", "RightForeArm", "RightHand",
    "LeftUpLeg", "LeftLeg", "LeftFoot", "LeftToeBase", "LeftToe_End",
    "RightUpLeg", "RightLeg", "RightFoot", "RightToeBase", "RightToe_End",
]


def bone_role(name):
    base = name.split(":")[-1]
    base = re.sub(r"_\d+$", "", base)
    return base if base in ROLES else None


class Rig:
    """Скелет персонажа + базовая поза + запись каналов анимации."""

    def __init__(self, path):
        self.path = path
        self.js, self.bin = read_glb(path)
        js = self.js
        self.nodes = js["nodes"]
        self.parent = {}
        for i, n in enumerate(self.nodes):
            for c in n.get("children", []):
                self.parent[c] = i

        self.skin = js["skins"][0]
        self.joints = list(self.skin["joints"])
        self.role_node = {}
        for i in self.joints:
            r = bone_role(self.nodes[i].get("name", ""))
            if r and r not in self.role_node:
                self.role_node[r] = i
        self.node_role = {v: k for k, v in self.role_node.items()}

        self.rest_local = {}
        self.rest_world = {}
        for i in range(len(self.nodes)):
            q = self.nodes[i].get("rotation", [0, 0, 0, 1])
            self.rest_local[i] = qnorm(q)
            p = qmul(self.rest_world[self.parent[i]], self.rest_local[i]) \
                if i in self.parent else self.rest_local[i]
            self.rest_world[i] = p

        hips = self.role_node.get("Hips")
        hw = qmat(self.rest_world[hips])
        self.hip_height = float(np.array(self.nodes[hips].get("translation", [0, 0, 0]))[1]) if hips is not None else 90.0
        self.unit = abs(self.hip_height) / 87.0 or 1.0  # модель в сантиметрах

        self.base_local = dict(self.rest_local)
        self.base_hips_t = np.array(self.nodes[hips].get("translation", [0, 0, 0]), dtype=np.float64)
        self._apply_rest_arms_down()

    # ---- базовая поза: T-поза -> руки опущены ---------------------------- #

    def _world_delta(self, role, pitch=0.0, yaw=0.0, roll=0.0):
        i = self.role_node.get(role)
        if i is None:
            return
        p = self.rest_world[self.parent[i]] if i in self.parent else np.array([0.0, 0.0, 0.0, 1.0])
        w = qfrom_pyr(pitch, yaw, roll)
        self.base_local[i] = qmul(qmul(qnorm(np.array([-p[0], -p[1], -p[2], p[3]])), w), qmul(p, self.base_local[i]))

    def _apply_rest_arms_down(self):
        self._world_delta("LeftShoulder", roll=-6)
        self._world_delta("RightShoulder", roll=6)
        self._world_delta("LeftArm", roll=-70, pitch=2)
        self._world_delta("RightArm", roll=70, pitch=2)
        self._world_delta("LeftForeArm", pitch=8)
        self._world_delta("RightForeArm", pitch=8)

    # ---- перевод позы в локальные кватернионы ---------------------------- #

    def solve(self, pose):
        """pose = {'bones': {role: (pitch, yaw, roll)}, 'hips': (dx, dy, dz)}"""
        out = dict(self.base_local)
        for role, (pitch, yaw, roll) in pose.get("bones", {}).items():
            i = self.role_node.get(role)
            if i is None:
                continue
            p = self.rest_world[self.parent[i]] if i in self.parent else np.array([0.0, 0.0, 0.0, 1.0])
            w = qfrom_pyr(pitch, yaw, roll)
            out[i] = qmul(qmul(qnorm(np.array([-p[0], -p[1], -p[2], p[3]])), w), qmul(p, self.base_local[i]))
        hips = self.role_node["Hips"]
        hp = self.rest_world[self.parent[hips]] if hips in self.parent else np.array([0.0, 0.0, 0.0, 1.0])
        d = np.array(pose.get("hips", (0.0, 0.0, 0.0)), dtype=np.float64) * self.unit
        inv = qnorm(np.array([-hp[0], -hp[1], -hp[2], hp[3]]))
        out_t = self.base_hips_t + qmat(inv) @ d
        return out, out_t

    # ---- запись анимации ------------------------------------------------- #

    def add_animation(self, name, fps, frames, loop, extras=None):
        """frames: список (pose_dict,) -> печём по кадрам."""
        n = len(frames)
        times = np.arange(n, dtype=np.float32) / np.float32(fps)
        quats = {i: np.zeros((n, 4), dtype=np.float32) for i in self.role_node.values()}
        hips_ts = np.zeros((n, 3), dtype=np.float32)
        for k, pose in enumerate(frames):
            loc, t = self.solve(pose)
            for i in quats:
                quats[i][k] = loc[i]
            hips_ts[k] = t

        channels, samplers = [], []
        bin_ = self.bin
        js = self.js

        def push(data, typ, count, acc_type, minmax=True):
            arr = np.ascontiguousarray(data)
            while len(bin_) % 4:
                bin_.append(0)
            off = len(bin_)
            bin_.extend(arr.tobytes())
            js["bufferViews"].append({"buffer": 0, "byteOffset": off, "byteLength": arr.nbytes})
            acc = {"bufferView": len(js["bufferViews"]) - 1, "componentType": typ,
                   "count": count, "type": acc_type}
            if minmax:
                vals = arr.reshape(count, -1)
                acc["min"] = [float(v) for v in vals.min(axis=0)]
                acc["max"] = [float(v) for v in vals.max(axis=0)]
            js["accessors"].append(acc)
            return len(js["accessors"]) - 1

        t_acc = push(times, 5126, n, "SCALAR")

        def add_ch(node, path, values, typ, count):
            a = push(values, 5126, count, typ)
            samplers.append({"input": t_acc, "output": a, "interpolation": "LINEAR"})
            channels.append({"sampler": len(samplers) - 1,
                             "target": {"node": node, "path": path}})

        for i, arr in quats.items():
            add_ch(i, "rotation", arr, "VEC4", n)
        add_ch(self.role_node["Hips"], "translation", hips_ts, "VEC3", n)

        anim = {"name": name, "channels": channels, "samplers": samplers}
        meta = {"fps": fps, "loop": bool(loop), "duration": round(n / fps, 4),
                "channels": len(channels)}
        if extras:
            meta.update(extras)
        anim["extras"] = meta
        js.setdefault("animations", []).append(anim)
        js["buffers"][0]["byteLength"] = len(bin_)
        return meta
