"""Procedural banknote stacks (placeholder money prop) -> models/generated/money_stack.glb

No real currency design is reproduced: the bill print is generated noise/bands.
Run: blender -b --python tools/make_money.py
"""
import os
import random

import bpy

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
OUT_DIR = os.path.join(ROOT, "models", "generated")
os.makedirs(OUT_DIR, exist_ok=True)

bpy.ops.wm.read_factory_settings(use_empty=True)
random.seed(7)

W, H, T = 0.156, 0.0663, 0.00018  # note size (m), like a euro note


def make_print_texture():
    img = bpy.data.images.new("banknote_print", width=512, height=224)
    px = [0.0] * (512 * 224 * 4)
    for y in range(224):
        for x in range(512):
            i = (y * 512 + x) * 4
            # base paper: green-blue tinted banknote stock
            r, g, b = 0.58, 0.68, 0.62
            # colored security bands
            if (x // 64) % 3 == 1:
                r, g, b = 0.24, 0.42, 0.55
            # central oval
            dx, dy = (x - 256) / 150.0, (y - 112) / 80.0
            d = (dx * dx + dy * dy) ** 0.5
            if d < 1.0:
                r, g, b = 0.34 + 0.18 * d, 0.36 + 0.16 * d, 0.24
            # faux portrait ring
            if 0.55 < d < 0.75:
                r, g, b = 0.16, 0.20, 0.17
            # print noise
            n = random.random() * 0.06
            px[i:i + 4] = (max(0.0, r - n), max(0.0, g - n), max(0.0, b - n), 1.0)
    img.pixels.foreach_set(px)
    img.pack()
    return img


tex = make_print_texture()
mat = bpy.data.materials.new("Banknote")
mat.use_nodes = True
nt = mat.node_tree
bsdf = nt.nodes["Principled BSDF"]
timg = nt.nodes.new("ShaderNodeTexImage")
timg.image = tex
nt.links.new(timg.outputs["Color"], bsdf.inputs["Base Color"])
bsdf.inputs["Roughness"].default_value = 0.55

band_mat = bpy.data.materials.new("Band")
band_mat.use_nodes = True
band_mat.node_tree.nodes["Principled BSDF"].inputs["Base Color"].default_value = (0.05, 0.05, 0.06, 1)
band_mat.node_tree.nodes["Principled BSDF"].inputs["Roughness"].default_value = 0.75


def note_mesh(name):
    bpy.ops.mesh.primitive_cube_add(size=1)
    ob = bpy.context.active_object
    ob.name = name
    ob.scale = (W, H, T)
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    ob.data.materials.append(mat)
    return ob


def stack(x, y, count, rot):
    for k in range(count):
        ob = note_mesh(f"note_{x:.2f}_{y:.2f}_{k}")
        ob.location = (x + random.uniform(-0.002, 0.002),
                       y + random.uniform(-0.002, 0.002),
                       T / 2 + k * T * 1.02 + random.uniform(0, 0.0002))
        ob.rotation_euler = (0, 0, rot + random.uniform(-0.02, 0.02))
    # rubber band
    bpy.ops.mesh.primitive_cube_add(size=1)
    band = bpy.context.active_object
    band.scale = (W * 0.25, H * 1.08, T * count * 1.15)
    bpy.ops.object.transform_apply(scale=True)
    band.location = (x, y, T * count * 0.6)
    band.rotation_euler = (0, 0, rot)
    band.data.materials.append(band_mat)


stack(-0.10, 0.0, 55, 0.05)
stack(0.09, 0.02, 42, -0.12)
stack(0.0, 0.12, 30, 0.35)
for i in range(7):
    ob = note_mesh(f"loose_{i}")
    ob.location = (random.uniform(-0.18, 0.18), random.uniform(-0.16, 0.20), T / 2)
    ob.rotation_euler = (0, 0, random.uniform(0, 3.14))

bpy.ops.object.select_all(action="SELECT")
bpy.context.view_layer.objects.active = bpy.context.selected_objects[0]
bpy.ops.export_scene.gltf(
    filepath=os.path.join(OUT_DIR, "money_stack.glb"),
    export_format="GLB",
    export_image_format="AUTO",
)
print("WROTE", os.path.join(OUT_DIR, "money_stack.glb"), flush=True)
