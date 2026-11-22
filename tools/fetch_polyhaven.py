#!/usr/bin/env python3
"""Download realistic CC0 3D models from Poly Haven (official API).

Selection is per-category with caps so the result is a balanced game asset set.
Everything is deglitched glTF + 1K PBR textures (portable, engine-agnostic).
"""
import json
import os
import shutil
import sys
import time
import urllib.request
import urllib.error

API = "https://api.polyhaven.com"
UA = "GTAVI-Mobile-Concept/1.0 (asset fetch script)"
ROOT = os.path.join(os.path.dirname(os.path.dirname(os.path.abspath(__file__))), "models", "polyhaven")
MANIFEST = os.path.join(os.path.dirname(ROOT), "manifest.json")

# category -> how many models to take (ordered by popularity)
CAPS = {
    "buildings": 13,
    "structures": 14,
    "firearms": 2,
    "weapons": 3,
    "rigged": 15,
    "creature": 1,
    "furniture": 18,
    "office": 8,
    "appliances": 8,
    "electronics": 8,
    "food": 8,
    "dishes": 5,
    "containers": 10,
    "tools": 10,
    "lighting": 6,
    "decorative": 10,
    "props": 12,
}


def get_json(url, tries=4):
    for i in range(tries):
        try:
            req = urllib.request.Request(url, headers={"User-Agent": UA})
            with urllib.request.urlopen(req, timeout=60) as r:
                return json.loads(r.read().decode())
        except Exception as e:  # network hiccup
            if i == tries - 1:
                raise
            time.sleep(1.5 * (i + 1))


def download(url, dest, size_hint=None):
    if os.path.exists(dest) and size_hint and os.path.getsize(dest) == size_hint:
        return False
    os.makedirs(os.path.dirname(dest), exist_ok=True)
    tmp = dest + ".part"
    for i in range(4):
        try:
            req = urllib.request.Request(url, headers={"User-Agent": UA})
            with urllib.request.urlopen(req, timeout=180) as r, open(tmp, "wb") as f:
                while True:
                    chunk = r.read(1 << 20)
                    if not chunk:
                        break
                    f.write(chunk)
            os.replace(tmp, dest)
            return True
        except Exception:
            if i == 3:
                raise
            time.sleep(1.5 * (i + 1))
    return False


def manifest_ids():
    if not os.path.exists(MANIFEST):
        return []
    return list(json.load(open(MANIFEST)))


def select(assets):
    by_cat = {}
    for aid, meta in assets.items():
        for c in meta.get("categories", []):
            by_cat.setdefault(c, []).append(aid)
    chosen = {}
    for cat, cap in CAPS.items():
        ids = sorted(by_cat.get(cat, []), key=lambda a: assets[a].get("download_count", 0), reverse=True)
        for aid in ids[:cap]:
            chosen.setdefault(aid, set()).add(cat)
    return {aid: sorted(cats) for aid, cats in chosen.items()}


# Pure environment scans (trees, shrubs, rocks, ground) are huge and mostly useless
# for a city block: excluded from the full download and pruned from earlier runs.
NATURE_CATS = {
    "nature", "plants", "trees", "rocks", "ground cover", "flowers", "grass",
    "succulent", "potted plants", "collection: pine_forest", "collection: namaqualand",
    "collection: moon", "collection: verdant_trail",
}


def main():
    args = sys.argv[1:]
    only = [a for a in args if not a.startswith("--")] or None
    assets = get_json(f"{API}/assets?t=models")
    if "--all" in args:
        sel = {aid: sorted(set(meta.get("categories", []))) for aid, meta in assets.items()}
        sel = {a: c for a, c in sel.items() if not set(c) <= NATURE_CATS}
    else:
        sel = select(assets)
    if "--prune" in args:
        keep = set(sel)
        old = json.load(open(MANIFEST)) if os.path.exists(MANIFEST) else {}
        removed = 0
        for aid in list(old):
            if aid not in keep:
                shutil.rmtree(os.path.join(ROOT, aid), ignore_errors=True)
                old.pop(aid)
                removed += 1
        with open(MANIFEST, "w") as f:
            json.dump(old, f, indent=1, ensure_ascii=False)
        print(f"pruned {removed} models outside the wanted set")
    if only:
        sel = {a: sel[a] for a in only if a in sel}
    print(f"selected {len(sel)} models from {len(assets)} available")
    manifest = {}
    if os.path.exists(MANIFEST):
        manifest = json.load(open(MANIFEST))
    for n, (aid, cats) in enumerate(sorted(sel.items()), 1):
        meta = assets[aid]
        out_dir = os.path.join(ROOT, aid)
        entry = {
            "id": aid,
            "name": meta.get("name", aid),
            "categories": cats,
            "license": "CC0 1.0 (Poly Haven)",
            "author": list({a for a in meta.get("authors", {})}),
            "format": "glTF 2.0 + 1K PBR textures",
            "source": f"https://polyhaven.com/a/{aid}",
        }
        try:
            files = get_json(f"{API}/files/{aid}")
            node = files["gltf"]["1k"]["gltf"]
            base = out_dir
            main_name = os.path.basename(urllib.request.unquote(node["url"]))
            n_bytes = 0
            if download(node["url"], os.path.join(base, main_name), node.get("size")):
                n_bytes += node.get("size") or 0
            for rel, inc in (node.get("include") or {}).items():
                rel = urllib.request.unquote(rel)
                dest = os.path.join(base, rel)
                if download(inc["url"], dest, inc.get("size")):
                    n_bytes += inc.get("size") or 0
            entry["file"] = os.path.relpath(os.path.join(base, main_name), os.path.dirname(ROOT))
            entry["size_mb"] = round(sum(
                os.path.getsize(os.path.join(dp, f))
                for dp, _, fs in os.walk(out_dir) for f in fs
            ) / 1e6, 2)
            manifest[aid] = entry
            print(f"[{n}/{len(sel)}] {aid} ({', '.join(cats)}) {entry['size_mb']} MB")
        except Exception as e:
            print(f"[{n}/{len(sel)}] FAILED {aid}: {e}")
            entry["error"] = str(e)
            manifest[aid] = entry
        with open(MANIFEST, "w") as f:
            json.dump(manifest, f, indent=1, ensure_ascii=False)
    print("done, manifest:", MANIFEST)


if __name__ == "__main__":
    main()
