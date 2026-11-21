"""Headless Cycles turntable renders for every downloaded model (Blender 4.2).

Usage:
  blender -b --python tools/render_blender.py -- <out_dir> [--samples N] [--contact] [--only glob]
"""
import glob
import math
import os
import sys
import time

import bpy
from mathutils import Vector

argv = sys.argv[sys.argv.index("--") + 1:] if "--" in sys.argv else []
OUT = argv[0] if argv else "renders/single"
SAMPLES = 8
for i, a in enumerate(argv):
    if a == "--samples":
        SAMPLES = int(argv[i + 1])

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
os.makedirs(os.path.join(ROOT, OUT), exist_ok=True)


def clear():
    bpy.ops.wm.read_factory_settings(use_empty=True)


def setup_world(size):
    scene = bpy.context.scene
    scene.render.engine = "CYCLES"
    scene.cycles.device = "CPU"
    scene.cycles.samples = SAMPLES
    scene.cycles.use_denoising = True
    scene.cycles.use_adaptive_sampling = True
    scene.cycles.max_bounces = 1
    scene.cycles.diffuse_bounces = 1
    scene.cycles.glossy_bounces = 1
    scene.cycles.transmission_bounces = 1
    scene.cycles.use_light_tree = True
    scene.render.resolution_x = 480
    scene.render.resolution_y = 360
    scene.render.resolution_percentage = 100
    scene.render.film_transparent = False
    scene.view_settings.look = "AgX - Medium High Contrast"

    world = bpy.data.worlds.new("W")
    scene.world = world
    world.use_nodes = True
    bg = world.node_tree.nodes["Background"]
    bg.inputs[0].default_value = (0.85, 0.87, 0.9, 1)
    bg.inputs[1].default_value = 1.1

    sun_data = bpy.data.lights.new("Sun", "SUN")
    sun_data.energy = 3.5
    sun_data.angle = math.radians(15)
    sun = bpy.data.objects.new("Sun", sun_data)
    scene.collection.objects.link(sun)
    sun.rotation_euler = (math.radians(52), 0, math.radians(35))

    # cheap fill: a second sun instead of an area light (much cheaper to sample)
    fill_data = bpy.data.lights.new("Fill", "SUN")
    fill_data.energy = 1.2
    fill_data.angle = math.radians(40)
    fill = bpy.data.objects.new("Fill", fill_data)
    scene.collection.objects.link(fill)
    fill.rotation_euler = (math.radians(65), 0, math.radians(-120))


def add_ground(zmin, extent):
    bpy.ops.mesh.primitive_plane_add(size=extent * 8, location=(0, 0, zmin - 0.001))
    plane = bpy.context.active_object
    mat = bpy.data.materials.new("Ground")
    mat.use_nodes = True
    bsdf = mat.node_tree.nodes["Principled BSDF"]
    bsdf.inputs["Base Color"].default_value = (0.62, 0.62, 0.63, 1)
    bsdf.inputs["Roughness"].default_value = 0.85
    plane.data.materials.append(mat)


def bounds():
    lo = Vector((1e9, 1e9, 1e9))
    hi = Vector((-1e9, -1e9, -1e9))
    found = False
    for ob in bpy.context.scene.objects:
        if ob.type != "MESH":
            continue
        found = True
        for c in ob.bound_box:
            w = ob.matrix_world @ Vector(c)
            lo = Vector((min(lo[i], w[i]) for i in range(3)))
            hi = Vector((max(hi[i], w[i]) for i in range(3)))
    if not found:
        return Vector((0, 0, 0)), Vector((1, 1, 1))
    return lo, hi


def render_one(gltf, out_png):
    clear()
    bpy.ops.import_scene.gltf(filepath=gltf)
    lo, hi = bounds()
    center = (lo + hi) / 2
    size = hi - lo
    extent = max(size.x, size.y, size.z, 0.2)
    setup_world(size)
    add_ground(lo.z, extent)

    cam_data = bpy.data.cameras.new("Cam")
    cam_data.lens = 55
    cam = bpy.data.objects.new("Cam", cam_data)
    bpy.context.scene.collection.objects.link(cam)
    dist = extent * 1.75
    cam.location = center + Vector((dist * 0.72, -dist * 0.78, dist * 0.52))
    direction = center - cam.location
    cam.rotation_euler = direction.to_track_quat("-Z", "Y").to_euler()
    bpy.context.scene.camera = cam

    bpy.context.scene.render.filepath = out_png
    bpy.context.scene.render.image_settings.file_format = "PNG"
    t0 = time.time()
    bpy.ops.render.render(write_still=True)
    return time.time() - t0


models = sorted(glob.glob(os.path.join(ROOT, "models", "**", "*.gltf"), recursive=True))
models += sorted(glob.glob(os.path.join(ROOT, "models", "**", "*.glb"), recursive=True))
models = sorted(set(models))
limits = None
for i, a in enumerate(argv):
    if a == "--limit":
        limits = int(argv[i + 1])
if limits:
    models = models[:limits]
print(f"MODELS={len(models)} samples={SAMPLES}", flush=True)

done = 0
for m in models:
    rel = os.path.splitext(os.path.relpath(m, os.path.join(ROOT, "assets")))[0].replace(os.sep, "__")
    out = os.path.join(ROOT, OUT, rel + ".png")
    if os.path.exists(out):
        continue
    try:
        dt = render_one(m, out)
        done += 1
        print(f"RENDER_OK {rel} {dt:.1f}s", flush=True)
    except Exception as e:
        print(f"RENDER_FAIL {rel} {e}", flush=True)
print(f"DONE rendered={done}", flush=True)
