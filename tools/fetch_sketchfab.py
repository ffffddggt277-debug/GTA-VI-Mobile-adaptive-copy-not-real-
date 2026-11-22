#!/usr/bin/env python3
"""Download realistic, rigged CC models (people, police, weapons) from Sketchfab.

Sketchfab requires an account + API token for downloads, so unlike Poly Haven this
script cannot run anonymously. The token is read from the SKETCHFAB_API_TOKEN
environment variable (project secret) and is never written to disk.

Usage:
  SKETCHFAB_API_TOKEN=... python3 tools/fetch_sketchfab.py [--limit N] [--dry-run]

Only models that Sketchfab marks downloadable and that carry a CC license are kept.
Output: models/sketchfab/<slug>__<uid>.glb (+ .gltf/.bin/textures when only glTF is offered)
        models/sketchfab_manifest.json
"""
import concurrent.futures
import json
import os
import re
import sys
import time
import urllib.error
import urllib.parse
import urllib.request

API = "https://api.sketchfab.com/v3"
TOKEN = os.environ.get("SKETCHFAB_API_TOKEN", "").strip()
ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
OUT_DIR = os.path.join(ROOT, "models", "sketchfab")
MANIFEST = os.path.join(ROOT, "models", "sketchfab_manifest.json")
UA = "GTAVI-Mobile-Concept/1.0 (asset fetch script)"

# CC licenses we accept: CC0, CC-BY, CC-BY-SA (attribution kept in the manifest).
LICENSES = {"cc0", "cc-by", "cc-by-sa"}

# What the concept needs: varied realistic people with rigs, police, and firearms.
QUERIES = [
    ("characters", "policeman"),
    ("characters", "police officer"),
    ("characters", "swat"),
    ("characters", "rigged character"),
    ("characters", "gangster"),
    ("characters", "man character"),
    ("characters", "woman character"),
    ("characters", "civilian"),
    ("firearms", "pistol"),
    ("firearms", "rifle"),
    ("firearms", "shotgun"),
    ("firearms", "smg"),
    ("vehicles", "car"),
    ("vehicles", "police car"),
    ("buildings", "house"),
    ("buildings", "building"),
]
PER_QUERY = 24


def api(path, params=None, tries=5):
    url = API + path
    if params:
        url += "?" + urllib.parse.urlencode(params)
    for attempt in range(tries):
        req = urllib.request.Request(url, headers={"User-Agent": UA})
        if TOKEN:
            req.add_header("Authorization", "Token " + TOKEN)
        try:
            with urllib.request.urlopen(req, timeout=60) as r:
                return json.loads(r.read().decode())
        except urllib.error.HTTPError as e:
            if e.code == 429 and attempt < tries - 1:
                wait = int(e.headers.get("Retry-After") or 0) or 6 * (attempt + 1)
                time.sleep(min(wait, 60))
                continue
            raise
    raise RuntimeError("unreachable")


def slugify(text, limit=60):
    s = re.sub(r"[^a-z0-9]+", "-", text.lower()).strip("-")
    return (s or "model")[:limit]


def license_ok(model):
    """Search results omit the license; the model detail endpoint carries it.

    Sketchfab slugs: zero = CC0, by = CC-BY, by-sa = CC-BY-SA. Non-commercial and
    no-derivatives variants are rejected because the concept may be shared/monetized.
    """
    slug = (((model.get("license") or {}).get("slug")) or "").lower()
    return slug in {"zero", "by", "by-sa"}



def search(query, category):
    """Sketchfab can filter by license server-side, so no per-model detail calls."""
    out = []
    for lic in ("by", "by-sa"):
        try:
            data = api("/search", {
                "type": "models",
                "q": query,
                "downloadable": "true",
                "license": lic,
                "sort_by": "-likeCount",
                "count": PER_QUERY,
            })
        except Exception as e:
            print(f"SEARCH_FAIL {query} ({lic}): {e}")
            continue
        for m in data.get("results", []):
            if not m.get("isDownloadable"):
                continue
            m["license"] = {"slug": lic}
            m["_category"] = category
            # people must actually be rigged, otherwise they are useless for a game
            if category == "characters" and not m.get("animationCount"):
                continue
            out.append(m)
        time.sleep(1.5)
    return out


MAX_FILE_MB = 45          # skip a single oversized model
MAX_TOTAL_GB = 1.2        # stop once the Sketchfab set reaches this size


def url_size_mb(url):
    req = urllib.request.Request(url, method="HEAD", headers={"User-Agent": UA})
    try:
        with urllib.request.urlopen(req, timeout=60) as r:
            n = int(r.headers.get("Content-Length") or 0)
            return n / 1e6
    except Exception:
        return 0.0


def download(url, dest):
    os.makedirs(os.path.dirname(dest), exist_ok=True)
    tmp = dest + ".part"
    req = urllib.request.Request(url, headers={"User-Agent": UA})
    with urllib.request.urlopen(req, timeout=300) as r, open(tmp, "wb") as f:
        while True:
            chunk = r.read(1 << 20)
            if not chunk:
                break
            f.write(chunk)
    os.replace(tmp, dest)


def fetch_model(model):
    model = model  # already enriched by search()
    uid = model["uid"]
    name = model.get("name") or uid
    stem = f"{slugify(name)}__{uid}"
    files = api(f"/models/{uid}/download")
    glb = (files.get("glb") or {}).get("url")
    gltf = (files.get("gltf") or {}).get("url")
    if glb:
        size = url_size_mb(glb)
        if size > MAX_FILE_MB:
            raise RuntimeError(f"too large: {size:.0f} MB")
        dest = os.path.join(OUT_DIR, stem + ".glb")
        download(glb, dest)
        file_rel = os.path.relpath(dest, os.path.join(ROOT, "models"))
    elif gltf:
        dest = os.path.join(OUT_DIR, stem + ".gltf")
        download(gltf, dest)
        file_rel = os.path.relpath(dest, os.path.join(ROOT, "models"))
    else:
        raise RuntimeError("no glb/gltf download offered")
    return {
        "id": uid,
        "name": name,
        "categories": [model["_category"]],
        "license": (model.get("license") or {}).get("label") or (model.get("license") or {}).get("slug"),
        "author": (model.get("user") or {}).get("displayName"),
        "source": f"https://sketchfab.com/3d-models/{model.get('slug', uid)}",
        "file": file_rel,
        "rigged": bool(model.get("animationCount")) or "rig" in name.lower(),
        "size_mb": round(os.path.getsize(os.path.join(ROOT, "models", file_rel)) / 1e6, 2),
    }


def main():
    args = sys.argv[1:]
    if not TOKEN:
        print("SKETCHFAB_API_TOKEN is not set. Add it under Settings -> Project settings -> Secrets.")
        return 2
    limit = None
    if "--limit" in args:
        limit = int(args[args.index("--limit") + 1])
    dry = "--dry-run" in args

    os.makedirs(OUT_DIR, exist_ok=True)
    manifest = json.load(open(MANIFEST)) if os.path.exists(MANIFEST) else {}
    seen = set(manifest)
    found = []
    for category, query in QUERIES:
        try:
            results = search(query, category)
        except Exception as e:
            print(f"SEARCH_FAIL {query}: {e}")
            continue
        print(f"SEARCH {query}: {len(results)} downloadable CC models")
        for m in results:
            if m["uid"] not in seen:
                seen.add(m["uid"])
                found.append(m)
    if limit:
        found = found[:limit]
    print(f"CANDIDATES {len(found)}")
    if dry:
        for m in found:
            print(" ", m["_category"], m["uid"], m.get("name"))
        return 0

    ok = 0
    for n, m in enumerate(found, 1):
        used_gb = sum(os.path.getsize(os.path.join(OUT_DIR, f)) for f in os.listdir(OUT_DIR)) / 1e9
        if used_gb > MAX_TOTAL_GB:
            print(f"BUDGET_REACHED {used_gb:.2f} GB, stopping")
            break
        try:
            entry = fetch_model(m)
            manifest[entry["id"]] = entry
            ok += 1
            print(f"[{n}/{len(found)}] OK {entry['name']} ({entry['license']}) {entry['size_mb']} MB")
        except Exception as e:
            print(f"[{n}/{len(found)}] FAILED {m['uid']} {m.get('name')}: {e}")
        with open(MANIFEST, "w") as f:
            json.dump(manifest, f, indent=1, ensure_ascii=False)
        time.sleep(0.4)
    print(f"DONE downloaded={ok} manifest={MANIFEST}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
