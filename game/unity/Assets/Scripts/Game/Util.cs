using System;
using System.Collections.Generic;
using UnityEngine;

namespace GTA6.Game
{
    // ------------------------------------------------------------------ colour

    public static class Col
    {
        public static Color32 Hex(string hex, float alpha = 1f)
        {
            if (string.IsNullOrEmpty(hex)) return new Color32(255, 255, 255, 255);
            if (hex[0] == '#') hex = hex.Substring(1);
            int r = 255, g = 255, b = 255;
            try
            {
                if (hex.Length >= 6)
                {
                    r = Convert.ToInt32(hex.Substring(0, 2), 16);
                    g = Convert.ToInt32(hex.Substring(2, 2), 16);
                    b = Convert.ToInt32(hex.Substring(4, 2), 16);
                }
            }
            catch (Exception) { }
            return new Color32((byte)r, (byte)g, (byte)b, (byte)Mathf.Clamp(Mathf.RoundToInt(alpha * 255f), 0, 255));
        }

        public static Color32 Scale(Color32 c, float k)
        {
            return new Color32(
                (byte)Mathf.Clamp(Mathf.RoundToInt(c.r * k), 0, 255),
                (byte)Mathf.Clamp(Mathf.RoundToInt(c.g * k), 0, 255),
                (byte)Mathf.Clamp(Mathf.RoundToInt(c.b * k), 0, 255), c.a);
        }

        public static Color32 Mix(Color32 a, Color32 b, float t)
        {
            t = Mathf.Clamp01(t);
            return new Color32(
                (byte)Mathf.RoundToInt(a.r + (b.r - a.r) * t),
                (byte)Mathf.RoundToInt(a.g + (b.g - a.g) * t),
                (byte)Mathf.RoundToInt(a.b + (b.b - a.b) * t),
                (byte)Mathf.RoundToInt(a.a + (b.a - a.a) * t));
        }

        public static Color ToColor(Color32 c) { return c; }

        public static readonly Color32 Clear = new Color32(255, 255, 255, 0);
    }

    /// <summary>
    /// A 16x16 point-filtered palette texture. Every flat-coloured part of the world
    /// points its UVs at one texel, so thousands of differently coloured boxes still
    /// render in a single draw call.
    /// </summary>
    public sealed class Palette
    {
        public const int Size = 64;
        public readonly Texture2D Texture;
        readonly Color32[] _pixels = new Color32[Size * Size];
        readonly Dictionary<int, int> _lookup = new Dictionary<int, int>();
        int _used;
        bool _dirty = true;

        public Palette()
        {
            Texture = new Texture2D(Size, Size, TextureFormat.RGBA32, false);
            Texture.name = "palette";
            Texture.filterMode = FilterMode.Point;
            Texture.wrapMode = TextureWrapMode.Clamp;
            for (int i = 0; i < _pixels.Length; i++) _pixels[i] = new Color32(255, 0, 255, 255);
        }

        public int Index(Color32 c)
        {
            int key = (c.r << 24) | (c.g << 16) | (c.b << 8) | c.a;
            int idx;
            if (_lookup.TryGetValue(key, out idx)) return idx;
            if (_used >= _pixels.Length) return 0;
            idx = _used++;
            _pixels[idx] = c;
            _lookup[key] = idx;
            _dirty = true;
            return idx;
        }

        public Vector2 UV(int index)
        {
            if (index < 0) index = 0;
            int x = index % Size, y = index / Size;
            return new Vector2((x + 0.5f) / Size, (y + 0.5f) / Size);
        }

        public void Apply()
        {
            if (!_dirty) return;
            Texture.SetPixels32(_pixels);
            Texture.Apply(false, false);
            _dirty = false;
        }
    }

    // ------------------------------------------------------------------- meshes

    /// <summary>Accumulates flat-shaded geometry with palette UVs into one mesh.</summary>
    public sealed class MeshBuilder
    {
        readonly List<Vector3> _v = new List<Vector3>(1024);
        readonly List<Vector3> _n = new List<Vector3>(1024);
        readonly List<Vector2> _uv = new List<Vector2>(1024);
        readonly List<int> _t = new List<int>(2048);
        readonly Palette _palette;

        public MeshBuilder(Palette palette) { _palette = palette; }

        public int VertexCount { get { return _v.Count; } }
        public bool Empty { get { return _t.Count == 0; } }

        public void Clear()
        {
            _v.Clear(); _n.Clear(); _uv.Clear(); _t.Clear();
        }

        void Vert(Vector3 p, Vector3 normal, Vector2 uv)
        {
            _v.Add(p); _n.Add(normal); _uv.Add(uv);
        }

        public void Quad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 normal, int color)
        {
            Vector2 uv = _palette.UV(color);
            int i = _v.Count;
            Vert(a, normal, uv); Vert(b, normal, uv); Vert(c, normal, uv); Vert(d, normal, uv);
            // Clockwise when seen from the front, which is Unity's front face.
            _t.Add(i); _t.Add(i + 2); _t.Add(i + 1);
            _t.Add(i); _t.Add(i + 3); _t.Add(i + 2);
        }

        /// <summary>Textured quad for roads and markings (uses the material's own texture).</summary>
        public void QuadUV(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 normal,
            Vector2 ua, Vector2 ub, Vector2 uc, Vector2 ud)
        {
            int i = _v.Count;
            Vert(a, normal, ua); Vert(b, normal, ub); Vert(c, normal, uc); Vert(d, normal, ud);
            _t.Add(i); _t.Add(i + 2); _t.Add(i + 1);
            _t.Add(i); _t.Add(i + 3); _t.Add(i + 2);
        }

        /// <summary>Axis-aligned box. Rotation is applied about the centre when given.</summary>
        public void Box(Vector3 center, Vector3 size, int color, Quaternion? rotation = null, bool bottom = false)
        {
            Vector3 h = size * 0.5f;
            Quaternion q = rotation ?? Quaternion.identity;
            Vector3 p0 = center + q * new Vector3(-h.x, -h.y, -h.z);
            Vector3 p1 = center + q * new Vector3(h.x, -h.y, -h.z);
            Vector3 p2 = center + q * new Vector3(h.x, -h.y, h.z);
            Vector3 p3 = center + q * new Vector3(-h.x, -h.y, h.z);
            Vector3 p4 = center + q * new Vector3(-h.x, h.y, -h.z);
            Vector3 p5 = center + q * new Vector3(h.x, h.y, -h.z);
            Vector3 p6 = center + q * new Vector3(h.x, h.y, h.z);
            Vector3 p7 = center + q * new Vector3(-h.x, h.y, h.z);

            Quad(p4, p5, p6, p7, q * Vector3.up, color);               // top
            if (bottom) Quad(p0, p1, p2, p3, q * Vector3.down, color);
            Quad(p3, p2, p6, p7, q * Vector3.forward, color);          // +z
            Quad(p1, p0, p4, p5, q * Vector3.back, color);             // -z
            Quad(p2, p1, p5, p6, q * Vector3.right, color);            // +x
            Quad(p0, p3, p7, p4, q * Vector3.left, color);             // -x
        }

        /// <summary>Box whose four sides use a texture, useful for facades.</summary>
        public void BoxTiled(Vector3 center, Vector3 size, int color, float tilesY, float tilesX = 1f)
        {
            Vector3 h = size * 0.5f;
            Vector3 p0 = center + new Vector3(-h.x, -h.y, -h.z);
            Vector3 p1 = center + new Vector3(h.x, -h.y, -h.z);
            Vector3 p2 = center + new Vector3(h.x, -h.y, h.z);
            Vector3 p3 = center + new Vector3(-h.x, -h.y, h.z);
            Vector3 p4 = center + new Vector3(-h.x, h.y, -h.z);
            Vector3 p5 = center + new Vector3(h.x, h.y, -h.z);
            Vector3 p6 = center + new Vector3(h.x, h.y, h.z);
            Vector3 p7 = center + new Vector3(-h.x, h.y, h.z);
            // sides
            QuadUV(p3, p2, p6, p7, Vector3.forward, new Vector2(0, 0), new Vector2(tilesX, 0), new Vector2(tilesX, tilesY), new Vector2(0, tilesY));
            QuadUV(p1, p0, p4, p5, Vector3.back, new Vector2(0, 0), new Vector2(tilesX, 0), new Vector2(tilesX, tilesY), new Vector2(0, tilesY));
            QuadUV(p2, p1, p5, p6, Vector3.right, new Vector2(0, 0), new Vector2(tilesX, 0), new Vector2(tilesX, tilesY), new Vector2(0, tilesY));
            QuadUV(p0, p3, p7, p4, Vector3.left, new Vector2(0, 0), new Vector2(tilesX, 0), new Vector2(tilesX, tilesY), new Vector2(0, tilesY));
            Vector2 uv = _palette.UV(color);
            Quad(p4, p5, p6, p7, Vector3.up, color);
            Quad(p0, p1, p2, p3, Vector3.down, color);
        }

        public void Cylinder(Vector3 baseCenter, float radius, float height, int sides, int color,
            Quaternion? rotation = null, float topRadius = -1f, bool caps = true)
        {
            if (topRadius < 0f) topRadius = radius;
            Quaternion q = rotation ?? Quaternion.identity;
            Vector3 up = q * Vector3.up;
            for (int i = 0; i < sides; i++)
            {
                float a0 = i * Mathf.PI * 2f / sides;
                float a1 = (i + 1) * Mathf.PI * 2f / sides;
                Vector3 d0 = q * new Vector3(Mathf.Cos(a0), 0, Mathf.Sin(a0));
                Vector3 d1 = q * new Vector3(Mathf.Cos(a1), 0, Mathf.Sin(a1));
                Vector3 b0 = baseCenter + d0 * radius, b1 = baseCenter + d1 * radius;
                Vector3 t0 = baseCenter + up * height + d0 * topRadius;
                Vector3 t1 = baseCenter + up * height + d1 * topRadius;
                Vector3 nrm = Vector3.Cross(b1 - b0, t0 - b0).normalized;
                if (nrm.sqrMagnitude < 0.001f) nrm = d0;
                Quad(b0, b1, t1, t0, nrm, color);
                if (caps)
                {
                    Quad(baseCenter, b0, b1, baseCenter, -up, color);
                    Quad(baseCenter + up * height, t1, t0, baseCenter + up * height, up, color);
                }
            }
        }

        public void Ball(Vector3 center, float radius, int segments, int rings, int color)
        {
            for (int r = 0; r < rings; r++)
            {
                float p0 = Mathf.PI * r / rings, p1 = Mathf.PI * (r + 1) / rings;
                for (int s = 0; s < segments; s++)
                {
                    float a0 = Mathf.PI * 2f * s / segments, a1 = Mathf.PI * 2f * (s + 1) / segments;
                    Vector3 v00 = center + new Vector3(Mathf.Sin(p0) * Mathf.Cos(a0), Mathf.Cos(p0), Mathf.Sin(p0) * Mathf.Sin(a0)) * radius;
                    Vector3 v01 = center + new Vector3(Mathf.Sin(p0) * Mathf.Cos(a1), Mathf.Cos(p0), Mathf.Sin(p0) * Mathf.Sin(a1)) * radius;
                    Vector3 v10 = center + new Vector3(Mathf.Sin(p1) * Mathf.Cos(a0), Mathf.Cos(p1), Mathf.Sin(p1) * Mathf.Sin(a0)) * radius;
                    Vector3 v11 = center + new Vector3(Mathf.Sin(p1) * Mathf.Cos(a1), Mathf.Cos(p1), Mathf.Sin(p1) * Mathf.Sin(a1)) * radius;
                    Quad(v00, v10, v11, v01, (v00 - center).normalized, color);
                }
            }
        }

        public Mesh ToMesh(string name)
        {
            Mesh m = new Mesh();
            m.name = name;
            if (_v.Count > 65000) m.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            m.SetVertices(_v);
            m.SetNormals(_n);
            m.SetUVs(0, _uv);
            m.SetTriangles(_t, 0, true);
            m.RecalculateBounds();
            return m;
        }
    }

    // --------------------------------------------------------------- materials

    public static class MatLib
    {
        public static Palette Palette;
        public static Material Matte;
        public static Material Glossy;
        public static Material Emissive;
        public static Material Glass;
        public static bool Ready { get { return Matte != null; } }

        public static void Init()
        {
            if (Ready) return;
            Palette = new Palette();
            Shader std = Shader.Find("Standard");
            if (std == null) std = Shader.Find("Diffuse");
            Matte = New(std, "matte", 0f, 0.18f);
            Glossy = New(std, "glossy", 0.55f, 0.72f);
            Emissive = New(std, "emissive", 0.2f, 0.3f);
            Glass = New(std, "glass", 0.9f, 0.9f);
            Glass.color = new Color(0.55f, 0.66f, 0.72f, 0.42f);
            Emissive.EnableKeyword("_EMISSION");
            Emissive.SetColor("_EmissionColor", new Color(0.45f, 0.38f, 0.22f));
        }

        static Material New(Shader shader, string name, float metallic, float smoothness)
        {
            Material m = new Material(shader);
            m.name = name;
            m.SetTexture("_MainTex", Palette.Texture);
            m.color = Color.white;
            m.SetFloat("_Metallic", metallic);
            m.SetFloat("_Glossiness", smoothness);
            return m;
        }

        public static void SetEmission(float strength)
        {
            if (Emissive == null) return;
            Color c = new Color(1.0f, 0.86f, 0.58f) * Mathf.Clamp01(strength);
            Emissive.SetColor("_EmissionColor", c);
        }
    }

    // ------------------------------------------------------------ bounding box

    public struct Box3
    {
        public Vector3 Min, Max;

        public Box3(Vector3 min, Vector3 max) { Min = min; Max = max; }

        public static Box3 FromCenter(Vector3 c, Vector3 size)
        {
            return new Box3(c - size * 0.5f, c + size * 0.5f);
        }

        public Vector3 Center { get { return (Min + Max) * 0.5f; } }
        public Vector3 Size { get { return Max - Min; } }
        public float RadiusXZ { get { Vector3 s = Size; return Mathf.Max(s.x, s.z) * 0.5f; } }

        public bool ContainsXZ(float x, float z, float pad = 0f)
        {
            return x >= Min.x - pad && x <= Max.x + pad && z >= Min.z - pad && z <= Max.z + pad;
        }

        public bool Overlaps(Box3 o)
        {
            return Min.x <= o.Max.x && Max.x >= o.Min.x
                && Min.y <= o.Max.y && Max.y >= o.Min.y
                && Min.z <= o.Max.z && Max.z >= o.Min.z;
        }

        /// <summary>Ray/AABB slab test. Returns distance or -1.</summary>
        public float Raycast(Vector3 origin, Vector3 dir, float maxDist)
        {
            float tmin = 0f, tmax = maxDist;
            for (int a = 0; a < 3; a++)
            {
                float o = origin[a], d = dir[a];
                float lo = Min[a], hi = Max[a];
                if (Mathf.Abs(d) < 1e-6f)
                {
                    if (o < lo || o > hi) return -1f;
                }
                else
                {
                    float t1 = (lo - o) / d, t2 = (hi - o) / d;
                    if (t1 > t2) { float tmp = t1; t1 = t2; t2 = tmp; }
                    if (t1 > tmin) tmin = t1;
                    if (t2 < tmax) tmax = t2;
                    if (tmin > tmax) return -1f;
                }
            }
            return tmin;
        }

        /// <summary>Pushes a circle out of the box (2D, XZ plane).</summary>
        public bool ResolveCircle(ref Vector3 pos, float radius, float height, bool grounded, out Vector3 normal)
        {
            normal = Vector3.zero;
            if (pos.y > Max.y || pos.y + height < Min.y) return false;
            float cx = Mathf.Clamp(pos.x, Min.x, Max.x);
            float cz = Mathf.Clamp(pos.z, Min.z, Max.z);
            float dx = pos.x - cx, dz = pos.z - cz;
            float d2 = dx * dx + dz * dz;
            if (d2 > radius * radius) return false;
            if (d2 > 0.000001f)
            {
                float d = Mathf.Sqrt(d2);
                normal = new Vector3(dx / d, 0f, dz / d);
                pos.x = cx + normal.x * radius;
                pos.z = cz + normal.z * radius;
            }
            else
            {
                float left = pos.x - Min.x, right = Max.x - pos.x;
                float back = pos.z - Min.z, front = Max.z - pos.z;
                float m = Mathf.Min(Mathf.Min(left, right), Mathf.Min(back, front));
                if (m == left) { pos.x = Min.x - radius; normal = Vector3.left; }
                else if (m == right) { pos.x = Max.x + radius; normal = Vector3.right; }
                else if (m == back) { pos.z = Min.z - radius; normal = Vector3.back; }
                else { pos.z = Max.z + radius; normal = Vector3.forward; }
            }
            return true;
        }
    }

    /// <summary>Uniform grid of AABBs for cheap collision and bullet queries.</summary>
    public sealed class BoxGrid
    {
        readonly float _cell;
        readonly Dictionary<int, List<int>> _cells = new Dictionary<int, List<int>>();
        public readonly List<Box3> Boxes = new List<Box3>();

        public BoxGrid(float cellSize) { _cell = cellSize; }

        int Key(int x, int z) { return x * 73856093 ^ z * 19349663; }

        public void Add(Box3 b)
        {
            int idx = Boxes.Count;
            Boxes.Add(b);
            int x0 = Mathf.FloorToInt(b.Min.x / _cell), x1 = Mathf.FloorToInt(b.Max.x / _cell);
            int z0 = Mathf.FloorToInt(b.Min.z / _cell), z1 = Mathf.FloorToInt(b.Max.z / _cell);
            for (int x = x0; x <= x1; x++)
                for (int z = z0; z <= z1; z++)
                {
                    int k = Key(x, z);
                    List<int> list;
                    if (!_cells.TryGetValue(k, out list)) { list = new List<int>(4); _cells[k] = list; }
                    list.Add(idx);
                }
        }

        public void Clear() { Boxes.Clear(); _cells.Clear(); }

        public void Query(float x, float z, float radius, List<int> result)
        {
            result.Clear();
            int x0 = Mathf.FloorToInt((x - radius) / _cell), x1 = Mathf.FloorToInt((x + radius) / _cell);
            int z0 = Mathf.FloorToInt((z - radius) / _cell), z1 = Mathf.FloorToInt((z + radius) / _cell);
            for (int cx = x0; cx <= x1; cx++)
                for (int cz = z0; cz <= z1; cz++)
                {
                    List<int> list;
                    if (!_cells.TryGetValue(Key(cx, cz), out list)) continue;
                    for (int i = 0; i < list.Count; i++)
                        if (!result.Contains(list[i])) result.Add(list[i]);
                }
        }

        public float Raycast(Vector3 origin, Vector3 dir, float maxDist, out Box3 hit)
        {
            hit = default(Box3);
            float best = maxDist;
            int x0 = Mathf.FloorToInt(Mathf.Min(origin.x, origin.x + dir.x * maxDist) / _cell) - 1;
            int x1 = Mathf.FloorToInt(Mathf.Max(origin.x, origin.x + dir.x * maxDist) / _cell) + 1;
            int z0 = Mathf.FloorToInt(Mathf.Min(origin.z, origin.z + dir.z * maxDist) / _cell) - 1;
            int z1 = Mathf.FloorToInt(Mathf.Max(origin.z, origin.z + dir.z * maxDist) / _cell) + 1;
            for (int cx = x0; cx <= x1; cx++)
                for (int cz = z0; cz <= z1; cz++)
                {
                    List<int> list;
                    if (!_cells.TryGetValue(Key(cx, cz), out list)) continue;
                    for (int i = 0; i < list.Count; i++)
                    {
                        Box3 b = Boxes[list[i]];
                        float t = b.Raycast(origin, dir, best);
                        if (t >= 0f && t < best) { best = t; hit = b; }
                    }
                }
            return best < maxDist ? best : -1f;
        }
    }

    // ------------------------------------------------------------------ objects

    public static class Go
    {
        public static GameObject New(string name, Transform parent = null)
        {
            GameObject g = new GameObject(name);
            if (parent != null) g.transform.SetParent(parent, false);
            return g;
        }

        public static MeshRenderer Mesh(Transform parent, string name, Mesh mesh, Material material, bool shadows = true)
        {
            GameObject g = New(name, parent);
            MeshFilter mf = g.AddComponent<MeshFilter>();
            mf.sharedMesh = mesh;
            MeshRenderer mr = g.AddComponent<MeshRenderer>();
            mr.sharedMaterial = material;
            mr.shadowCastingMode = shadows ? UnityEngine.Rendering.ShadowCastingMode.On
                                           : UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.receiveShadows = shadows;
            return mr;
        }

        public static void Destroy(UnityEngine.Object o)
        {
            if (o != null) UnityEngine.Object.Destroy(o);
        }
    }
}
