"""Generate GTA-style houses: separate animated doors, transmissive glass windows.

Blender 4.2 headless:
  blender -b --python tools/make_houses.py -- [count]

Each house is exported to models/generated/houses/house_XX.glb and keeps:
  * Wall_*, Roof, Floor_* as static meshes
  * Door_* objects whose origin is the hinge and which carry an "Open" animation
  * Glass_* objects with a transmissive Principled material (alpha-blended in glTF)
"""
import math
import os
import random
import sys

import bpy
from mathutils import Vector

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
OUT = os.path.join(ROOT, "models", "generated", "houses")
os.makedirs(OUT, exist_ok=True)

argv = sys.argv[sys.argv.index("--") + 1:] if "--" in sys.argv else []
COUNT = int(argv[0]) if argv else 12
random.seed(20261122)

PALETTES = [
    ((0.78, 0.74, 0.66), (0.16, 0.30, 0.42)),   # sand walls, blue accents
    ((0.86, 0.82, 0.76), (0.35, 0.13, 0.13)),   # cream, dark red
    ((0.55, 0.60, 0.58), (0.12, 0.12, 0.13)),   # grey concrete, black
    ((0.90, 0.62, 0.52), (0.20, 0.35, 0.30)),   # miami pink, green
    ((0.72, 0.68, 0.55), (0.25, 0.22, 0.20)),   # stucco, brown
    ((0.62, 0.72, 0.80), (0.80, 0.80, 0.78)),   # pale blue, white
]


def clear():
    bpy.ops.wm.read_factory_settings(use_empty=True)


def material(name, color, roughness=0.6, metallic=0.0, transmission=0.0, alpha=1.0):
    mat = bpy.data.materials.new(name)
    mat.use_nodes = True
    bsdf = mat.node_tree.nodes["Principled BSDF"]
    bsdf.inputs["Base Color"].default_value = (*color, 1)
    bsdf.inputs["Roughness"].default_value = roughness
    bsdf.inputs["Metallic"].default_value = metallic
    if transmission:
        bsdf.inputs["Transmission Weight"].default_value = transmission
        bsdf.inputs["IOR"].default_value = 1.45
    if alpha < 1.0:
        bsdf.inputs["Alpha"].default_value = alpha
        mat.blend_method = "BLEND"
    return mat


def box(name, size, location, mat, rotation_z=0.0):
    bpy.ops.mesh.primitive_cube_add(size=1, location=location)
    ob = bpy.context.active_object
    ob.name = name
    ob.scale = Vector(size)
    ob.rotation_euler = (0, 0, rotation_z)
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    ob.data.materials.append(mat)
    return ob


def cut(ob, cutter):
    mod = ob.modifiers.new("cut", "BOOLEAN")
    mod.operation = "DIFFERENCE"
    mod.object = cutter
    bpy.context.view_layer.objects.active = ob
    bpy.ops.object.modifier_apply(modifier=mod.name)
    bpy.data.objects.remove(cutter, do_unlink=True)


def wall(name, start, end, height, thickness, mat, openings):
    """A wall segment with rectangular openings, built by boolean subtraction."""
    start = Vector(start)
    end = Vector(end)
    length = (end - start).length
    mid = (start + end) / 2
    angle = math.atan2(end.y - start.y, end.x - start.x)
    w = box(name, (length, thickness, height), (mid.x, mid.y, height / 2), mat, angle)
    for (offset, width, sill, top) in openings:
        px = start + (end - start).normalized() * offset
        cutter = box("cut", (width, thickness * 3, top - sill),
                     (px.x, px.y, sill + (top - sill) / 2), mat, angle)
        cut(w, cutter)
    return w


def door(name, location, angle, width, height, mat):
    """Door mesh whose origin sits on the hinge, ready to rotate open."""
    bpy.ops.mesh.primitive_cube_add(size=1)
    ob = bpy.context.active_object
    ob.name = name
    ob.scale = (width, 0.06, height)
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    # move the mesh so the local origin is the hinge edge
    for v in ob.data.vertices:
        v.co.x += width / 2
        v.co.z += height / 2
    ob.location = location
    ob.rotation_euler = (0, 0, angle)
    ob.data.materials.append(mat)
    ob["hinge"] = "left"
    # 0 deg closed -> 95 deg open, then back, so the clip loops
    ob.rotation_mode = "XYZ"
    ob.keyframe_insert("rotation_euler", frame=1)
    ob.rotation_euler.z = math.radians(95)
    ob.keyframe_insert("rotation_euler", frame=40)
    ob.rotation_euler.z = 0.0
    ob.keyframe_insert("rotation_euler", frame=80)
    return ob


def build_house(index):
    clear()
    scene = bpy.context.scene
    scene.frame_start, scene.frame_end = 1, 80

    walls_c, accent_c = random.choice(PALETTES)
    concrete = material("Concrete", walls_c, 0.75)
    accent = material("Accent", accent_c, 0.55)
    wood = material("Wood", (0.35, 0.24, 0.16), 0.55)
    roof_mat = material("Roof", (0.22, 0.22, 0.24), 0.8)
    glass = material("Glass", (0.85, 0.92, 0.95), 0.05, transmission=1.0, alpha=0.25)

    storeys = random.choice([1, 2, 2, 3])
    w = random.uniform(7.0, 11.0)
    d = random.uniform(6.0, 9.0)
    sh = 3.0
    h = storeys * sh
    t = 0.25

    # floor slabs
    for s in range(storeys):
        box(f"Floor_{s}", (w, d, 0.2), (0, 0, s * sh + 0.1), concrete)
    box("Roof", (w + 0.6, d + 0.6, 0.25), (0, 0, h + 0.12), roof_mat)
    box("RoofRail", (w + 0.6, 0.15, 0.7), (0, d / 2 + 0.22, h + 0.6), accent)

    # facade openings: big ground-floor door, windows above
    door_w, door_h = 1.6, 2.4
    door_off = w * 0.32
    front = [(door_off, door_w, 0.0, door_h)]
    for s in range(storeys):
        for k in range(2):
            x = w * (0.22 + 0.45 * k)
            front.append((x, 1.5, s * sh + 0.9, s * sh + 2.3))
    back = [(w * 0.3, 1.6, s * sh + 0.9, s * sh + 2.3) for s in range(storeys)]
    left = [(d * 0.5, 1.2, s * sh + 1.0, s * sh + 2.2) for s in range(storeys)]
    right = [(d * 0.5, 1.2, s * sh + 1.0, s * sh + 2.2) for s in range(storeys)]

    wall("Wall_Front", (-w / 2, -d / 2), (w / 2, -d / 2), h, t, concrete, front)
    wall("Wall_Back", (w / 2, d / 2), (-w / 2, d / 2), h, t, concrete, back)
    wall("Wall_Left", (-w / 2, d / 2), (-w / 2, -d / 2), h, t, concrete, left)
    wall("Wall_Right", (w / 2, -d / 2), (w / 2, d / 2), h, t, concrete, right)

    # glass panes in every window opening
    def pane(name, loc, rot, sx, sz):
        bpy.ops.mesh.primitive_plane_add(size=1, location=loc, rotation=(math.pi / 2, 0, rot))
        ob = bpy.context.active_object
        ob.name = name
        # the plane lies in its local XY, so Y is the vertical size after the 90 deg tilt
        ob.scale = (sx, sz, 1)
        bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
        ob.data.materials.append(glass)
        return ob

    pane("Glass_Front_0", (door_off + w / 2 - w * 0.32 - w * 0.0, -d / 2 - 0.002, door_h + 0.1), 0, 1.2, 0.8)
    for i, (off, width, sill, top) in enumerate(front[1:], 1):
        pane(f"Glass_Front_{i}", (off - w / 2 - 0.0, -d / 2 - 0.002, sill + (top - sill) / 2),
             0, width, top - sill)
    for i, (off, width, sill, top) in enumerate(back, 1):
        pane(f"Glass_Back_{i}", (w / 2 - off, d / 2 + 0.002, sill + (top - sill) / 2),
             0, width, top - sill)
    for i, (off, width, sill, top) in enumerate(left, 1):
        pane(f"Glass_Left_{i}", (-w / 2 - 0.002, d / 2 - off, sill + (top - sill) / 2),
             math.pi / 2, width, top - sill)
    for i, (off, width, sill, top) in enumerate(right, 1):
        pane(f"Glass_Right_{i}", (w / 2 + 0.002, off - d / 2, sill + (top - sill) / 2),
             math.pi / 2, width, top - sill)

    # hinged front door, origin on the hinge axis
    hinge_x = -w / 2 + door_off - door_w / 2
    door("Door_Front", (hinge_x, -d / 2, 0.0), 0.0, door_w, door_h, wood)
    box("DoorFrame", (door_w + 0.2, 0.12, 0.12), (-w / 2 + door_off, -d / 2, door_h + 0.06), accent)

    # ground-floor shop awning on a third of the houses
    if index % 3 == 0:
        box("Awning", (w * 0.55, 1.4, 0.12), (w * 0.05, -d / 2 - 0.65, door_h + 0.45), accent)
        for k in range(4):
            box(f"AwningPost_{k}", (0.08, 0.08, door_h + 0.4),
                (w * (0.05 + (k - 1.5) * 0.13), -d / 2 - 1.3, (door_h + 0.4) / 2), accent)

    # simple interior: back wall of the shop, counter, ceiling lamp
    box("Interior_Wall", (w * 0.8, 0.15, sh * 0.8), (0, d / 2 - 1.2, sh * 0.4), accent)
    box("Counter", (2.4, 0.7, 1.0), (0, d / 2 - 2.4, 0.5), wood)

    out = os.path.join(OUT, f"house_{index:02d}.glb")
    bpy.ops.export_scene.gltf(
        filepath=out,
        export_format="GLB",
        export_animations=True,
        export_apply=False,
        use_selection=False,
    )
    return out, storeys


def main():
    made = []
    for i in range(COUNT):
        path, storeys = build_house(i)
        made.append(path)
        print(f"HOUSE {os.path.basename(path)} storeys={storeys}", flush=True)
    print(f"DONE houses={len(made)}", flush=True)


if __name__ == "__main__":
    main()
