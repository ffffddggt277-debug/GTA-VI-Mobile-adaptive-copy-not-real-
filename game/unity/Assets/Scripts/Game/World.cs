using System;
using System.Collections.Generic;
using UnityEngine;
using GTA6.Core;

namespace GTA6.Game
{
    public struct Building
    {
        public Box3 Bounds;
        public int Color;
        public int Floors;
        public string District;
        public bool Tall;
    }

    /// <summary>Procedurally generated Leonida. Everything is merged into a few meshes per chunk.</summary>
    public sealed class World
    {
        public const float Spacing = 96f;          // road centre to road centre
        public const float RoadHalf = 9f;          // half the asphalt width
        public const float BlockMargin = 3f;       // sidewalk width inside a block
        public const float OceanX = 950f;          // everything east of this is sea
        public const float BeachX = 892f;
        public const float Extent = 1440f;         // half-size of the whole map

        public Transform Root;
        public BoxGrid Colliders;
        public readonly List<Building> Buildings = new List<Building>();
        public readonly List<Vector3> Lamps = new List<Vector3>();
        public readonly List<Vector3> Palms = new List<Vector3>();
        public readonly List<Vector3> RoadNodes = new List<Vector3>();
        public Texture2D MapTexture;
        public int Seed;

        readonly Dictionary<long, Chunk> _chunks = new Dictionary<long, Chunk>();
        readonly List<GameObject> _extra = new List<GameObject>();
        Palette _p;

        sealed class Chunk
        {
            public MeshBuilder Matte, Glossy, Emissive;
            public float X, Z;
        }

        // ------------------------------------------------------------- building

        public void Build(int seed, Settings settings)
        {
            Seed = seed;
            MatLib.Init();
            _p = MatLib.Palette;
            Root = Go.New("Leonida").transform;
            Colliders = new BoxGrid(32f);

            BuildGround();
            BuildOcean();

            int half = Mathf.CeilToInt(GameData.CityMax / Spacing);
            for (int ix = -half; ix < half; ix++)
            {
                for (int iz = -half; iz < half; iz++)
                {
                    float cx = ix * Spacing + Spacing * 0.5f;
                    float cz = iz * Spacing + Spacing * 0.5f;
                    if (cx > BeachX - 20f) continue;
                    BuildBlock(ix, iz, cx, cz);
                }
            }

            BuildBeachProps();
            BuildStreetLamps(half);
            FlushChunks();
            BuildHills();
            MapTexture = MakeMap(1024);
        }

        Chunk GetChunk(float x, float z)
        {
            int cx = Mathf.FloorToInt(x / 480f), cz = Mathf.FloorToInt(z / 480f);
            long key = (long)cx * 100003L + cz;
            Chunk c;
            if (!_chunks.TryGetValue(key, out c))
            {
                c = new Chunk
                {
                    Matte = new MeshBuilder(_p),
                    Glossy = new MeshBuilder(_p),
                    Emissive = new MeshBuilder(_p),
                    X = cx * 480f,
                    Z = cz * 480f
                };
                _chunks[key] = c;
            }
            return c;
        }

        void FlushChunks()
        {
            _p.Apply();
            foreach (KeyValuePair<long, Chunk> kv in _chunks)
            {
                Chunk c = kv.Value;
                string n = "chunk_" + Mathf.RoundToInt(c.X) + "_" + Mathf.RoundToInt(c.Z);
                Transform t = Go.New(n, Root).transform;
                if (!c.Matte.Empty) Go.Mesh(t, n + "_m", c.Matte.ToMesh(n + "_m"), MatLib.Matte);
                if (!c.Glossy.Empty) Go.Mesh(t, n + "_g", c.Glossy.ToMesh(n + "_g"), MatLib.Glossy);
                if (!c.Emissive.Empty) Go.Mesh(t, n + "_e", c.Emissive.ToMesh(n + "_e"), MatLib.Emissive, false);
            }
        }

        // ----------------------------------------------------------------- land

        void BuildGround()
        {
            float e = Extent;
            int asphalt = _p.Index(new Color32(52, 54, 58, 255));
            int pavement = _p.Index(new Color32(122, 120, 116, 255));
            int dirt = _p.Index(new Color32(96, 86, 66, 255));

            MeshBuilder g = new MeshBuilder(_p);
            // Base ground: land west of the beach, sea floor goes to BuildOcean.
            g.Quad(new Vector3(-e, 0f, -e), new Vector3(BeachX, 0f, -e),
                   new Vector3(BeachX, 0f, e), new Vector3(-e, 0f, e), Vector3.up, dirt);
            g.Quad(new Vector3(-e, 0.001f, -e), new Vector3(BeachX + 8f, 0.001f, -e),
                   new Vector3(BeachX + 8f, 0.001f, e), new Vector3(-e, 0.001f, e), Vector3.up, asphalt);
            Go.Mesh(Root, "ground", g.ToMesh("ground"), MatLib.Matte, false);

            // Road markings along every second grid line, plus crosswalks at every other junction.
            MeshBuilder marks = new MeshBuilder(_p);
            int paint = _p.Index(new Color32(226, 226, 214, 255));
            int yellow = _p.Index(new Color32(214, 178, 74, 255));
            for (int k = -15; k <= 15; k++)
            {
                float c = k * Spacing;
                if (c > BeachX - 20f) continue;
                if (k % 2 == 0)
                {
                    for (float s = -Extent; s < BeachX - 20f; s += 12f)
                    {
                        marks.Quad(new Vector3(s, 0.02f, c - 0.16f), new Vector3(s + 5.5f, 0.02f, c - 0.16f),
                                   new Vector3(s + 5.5f, 0.02f, c + 0.16f), new Vector3(s, 0.02f, c + 0.16f),
                                   Vector3.up, yellow);
                    }
                }
                for (int j = -15; j <= 15; j++)
                {
                    float z = j * Spacing;
                    if (z > Extent - 40f) continue;
                    // zebra crossing stripes
                    for (int s = -3; s <= 3; s++)
                    {
                        float off = s * 1.6f;
                        marks.Quad(new Vector3(c + off - 0.6f, 0.021f, z - RoadHalf - 2.2f),
                                   new Vector3(c + off + 0.6f, 0.021f, z - RoadHalf - 2.2f),
                                   new Vector3(c + off + 0.6f, 0.021f, z - RoadHalf + 1.6f),
                                   new Vector3(c + off - 0.6f, 0.021f, z - RoadHalf + 1.6f), Vector3.up, paint);
                    }
                }
            }
            if (!marks.Empty) Go.Mesh(Root, "markings", marks.ToMesh("markings"), MatLib.Matte, false);

            // A thin line of pavement along the beach.
            MeshBuilder beach = new MeshBuilder(_p);
            int sand = _p.Index(new Color32(226, 205, 156, 255));
            beach.Quad(new Vector3(BeachX, 0.012f, -Extent), new Vector3(OceanX, 0.012f, -Extent),
                       new Vector3(OceanX, 0.012f, Extent), new Vector3(BeachX, 0.012f, Extent), Vector3.up, sand);
            Go.Mesh(Root, "beach", beach.ToMesh("beach"), MatLib.Matte, false);

            // Sidewalk curbs for every block
            MeshBuilder curbs = new MeshBuilder(_p);
            int curb = _p.Index(new Color32(146, 143, 136, 255));
            int half = Mathf.CeilToInt(Extent / Spacing);
            for (int ix = -half; ix < half; ix++)
                for (int iz = -half; iz < half; iz++)
                {
                    float x = ix * Spacing + Spacing * 0.5f, z = iz * Spacing + Spacing * 0.5f;
                    if (x > BeachX - 20f) continue;
                    float inner = Spacing - RoadHalf * 2f;
                    curbs.Box(new Vector3(x, 0.09f, z), new Vector3(inner, 0.18f, inner), curb);
                }
            if (!curbs.Empty) Go.Mesh(Root, "curbs", curbs.ToMesh("curbs"), MatLib.Matte, false);
        }

        void BuildOcean()
        {
            MeshBuilder water = new MeshBuilder(_p);
            int deep = _p.Index(new Color32(24, 72, 104, 255));
            water.Quad(new Vector3(OceanX - 2f, -0.35f, -Extent * 2f), new Vector3(Extent * 2f, -0.35f, -Extent * 2f),
                       new Vector3(Extent * 2f, -0.35f, Extent * 2f), new Vector3(OceanX - 2f, -0.35f, Extent * 2f),
                       Vector3.up, deep);
            Go.Mesh(Root, "ocean", water.ToMesh("ocean"), MatLib.Glossy, false);

            // Marina pier at the Keys
            MeshBuilder pier = new MeshBuilder(_p);
            int wood = _p.Index(new Color32(126, 100, 70, 255));
            for (int i = 0; i < 8; i++)
            {
                float z = -160f + i * 22f;
                pier.Box(new Vector3(1000f, 0.4f, z), new Vector3(90f, 0.8f, 6f), wood);
            }
            Go.Mesh(Root, "pier", pier.ToMesh("pier"), MatLib.Matte);
        }

        void BuildHills()
        {
            // Distant hills give the map an edge instead of a hard cut.
            Rng rng = new Rng(Seed + 7717);
            for (int i = 0; i < 9; i++)
            {
                float x = rng.Range(-Extent, -900f);
                float z = rng.Range(-Extent, Extent);
                float r = rng.Range(160f, 320f);
                MeshBuilder mb = new MeshBuilder(_p);
                int cac = _p.Index(new Color32(74, 96, 62, 255));
                mb.Ball(Vector3.zero, r, 12, 6, cac);
                Mesh mesh = mb.ToMesh("hill" + i);
                MeshRenderer mr = Go.Mesh(Root, "hill" + i, mesh, MatLib.Matte, false);
                mr.transform.position = new Vector3(x, -r * 0.78f, z);
                mr.transform.localScale = new Vector3(1f, 0.4f, 1f);
                _extra.Add(mr.gameObject);
            }
        }

        // -------------------------------------------------------------- blocks

        void BuildBlock(int ix, int iz, float cx, float cz)
        {
            District d = GameData.DistrictAt(cx, cz);
            Rng rng = new Rng(Seed * 31 + ix * 7919 + iz * 104729 + 17);
            float inner = Spacing - RoadHalf * 2f - BlockMargin * 2f;

            // Grassrivers stays swampy, Kalaga is hills only.
            if (d.Id == "kalaga") { if (rng.Chance(0.55f)) return; }
            if (d.Id == "grassrivers" && rng.Chance(0.35f))
            {
                AddPalm(cx + rng.Range(-20f, 20f), cz + rng.Range(-20f, 20f), rng.Range(4f, 8f));
                return;
            }

            switch (d.Id)
            {
                case "downtown":
                    Tower(cx, cz, inner, rng, d, rng.Range(45f, 125f));
                    if (rng.Chance(0.4f)) Tower(cx + rng.Range(-26f, 26f), cz + rng.Range(-26f, 26f), inner * 0.4f, rng, d, rng.Range(25f, 60f));
                    break;
                case "vice_beach":
                case "ocean_drive":
                    Hotel(cx, cz, rng, d, rng.Range(14f, 38f));
                    break;
                case "starfish":
                    Villa(cx + rng.Range(-14f, 14f), cz + rng.Range(-14f, 14f), rng, d, 26f);
                    if (rng.Chance(0.5f)) AddPalm(cx + rng.Range(-32f, 32f), cz + rng.Range(-32f, 32f), rng.Range(7f, 11f));
                    break;
                case "gellhorn":
                case "yorktown":
                    Warehouse(cx + rng.Range(-10f, 10f), cz + rng.Range(-10f, 10f), rng, d);
                    if (rng.Chance(0.6f))
                        Container(cx + rng.Range(-32f, 32f), cz + rng.Range(-32f, 32f), rng);
                    break;
                case "ambrosia":
                    if (rng.Chance(0.5f)) Warehouse(cx + rng.Range(-12f, 12f), cz + rng.Range(-12f, 12f), rng, d);
                    else SmallHouse(cx + rng.Range(-18f, 18f), cz + rng.Range(-18f, 18f), rng, d);
                    break;
                case "keys":
                    SmallHouse(cx + rng.Range(-16f, 16f), cz + rng.Range(-16f, 16f), rng, d);
                    if (rng.Chance(0.5f)) AddPalm(cx + rng.Range(-30f, 30f), cz + rng.Range(-30f, 30f), rng.Range(6f, 9f));
                    break;
                default:
                    SmallHouse(cx + rng.Range(-16f, 16f), cz + rng.Range(-16f, 16f), rng, d);
                    if (rng.Chance(0.6f))
                        SmallHouse(cx + rng.Range(-18f, 18f), cz + rng.Range(-18f, 18f), rng, d);
                    if (rng.Chance(0.45f)) AddPalm(cx + rng.Range(-30f, 30f), cz + rng.Range(-30f, 30f), rng.Range(6f, 10f));
                    break;
            }

            if (rng.Chance(0.25f)) AddBush(cx + rng.Range(-34f, 34f), cz + rng.Range(-34f, 34f), rng);
        }

        static Color32 DistrictWallColor(District d, Rng rng)
        {
            switch (d.Id)
            {
                case "downtown": return Col.Scale(Col.Hex("8FA6BC"), rng.Range(0.72f, 1.05f));
                case "vice_beach":
                case "ocean_drive": return Col.Scale(Col.Hex("EBD5B0"), rng.Range(0.85f, 1.05f));
                case "starfish": return Col.Scale(Col.Hex("E8E0CE"), rng.Range(0.9f, 1.02f));
                case "havana": return Col.Scale(Col.Hex("D9A187"), rng.Range(0.8f, 1.05f));
                case "gerss": return Col.Hex("B08060");
                case "gellhorn":
                case "yorktown": return Col.Scale(Col.Hex("9AA0A6"), rng.Range(0.8f, 1f));
                case "grassrivers": return Col.Scale(Col.Hex("8E7A5E"), rng.Range(0.8f, 1f));
                case "ambrosia": return Col.Scale(Col.Hex("C9BBA0"), rng.Range(0.8f, 1f));
                case "waning_sands": return Col.Scale(Col.Hex("DCC49A"), rng.Range(0.85f, 1f));
                case "keys": return Col.Scale(Col.Hex("9BC2C4"), rng.Range(0.85f, 1f));
                case "kalaga": return Col.Scale(Col.Hex("9C8A70"), rng.Range(0.8f, 1f));
                default: return Col.Scale(Col.Hex("C4B49A"), rng.Range(0.8f, 1f));
            }
        }

        void Tower(float cx, float cz, float inner, Rng rng, District d, float height)
        {
            float w = Mathf.Clamp(inner * rng.Range(0.45f, 0.8f), 18f, inner);
            float depth = Mathf.Clamp(inner * rng.Range(0.45f, 0.8f), 18f, inner);
            Color32 wall = DistrictWallColor(d, rng);
            int wc = _p.Index(wall);
            int glass = _p.Index(Col.Scale(Col.Hex("274B63"), rng.Range(0.9f, 1.2f)));
            int trim = _p.Index(Col.Scale(wall, 0.7f));
            Chunk c = GetChunk(cx, cz);

            c.Matte.Box(new Vector3(cx, height * 0.5f, cz), new Vector3(w, height, depth), wc);
            // window bands every floor
            int floors = Mathf.Max(2, Mathf.RoundToInt(height / 3.4f));
            for (int f = 1; f < floors; f++)
            {
                float y = f * 3.4f - 0.9f;
                c.Matte.Box(new Vector3(cx, y, cz + depth * 0.5f + 0.06f), new Vector3(w * 0.92f, 1.9f, 0.12f), glass);
                c.Matte.Box(new Vector3(cx, y, cz - depth * 0.5f - 0.06f), new Vector3(w * 0.92f, 1.9f, 0.12f), glass);
                c.Matte.Box(new Vector3(cx + w * 0.5f + 0.06f, y, cz), new Vector3(0.12f, 1.9f, depth * 0.92f), glass);
                c.Matte.Box(new Vector3(cx - w * 0.5f - 0.06f, y, cz), new Vector3(0.12f, 1.9f, depth * 0.92f), glass);
            }
            // roof crown
            c.Matte.Box(new Vector3(cx, height + 0.6f, cz), new Vector3(w * 1.06f, 1.2f, depth * 1.06f), trim);
            c.Matte.Box(new Vector3(cx, height + 3.2f, cz), new Vector3(w * 0.45f, 4.2f, depth * 0.45f), trim);
            if (rng.Chance(0.5f))
                c.Emissive.Box(new Vector3(cx, height + 1.4f, cz + depth * 0.53f), new Vector3(w * 0.7f, 0.7f, 0.14f),
                    _p.Index(Col.Hex("FFD98A")));

            Buildings.Add(new Building
            {
                Bounds = Box3.FromCenter(new Vector3(cx, height * 0.5f, cz), new Vector3(w, height, depth)),
                Color = wc, Floors = floors, District = d.Id, Tall = true
            });
            Colliders.Add(Buildings[Buildings.Count - 1].Bounds);
        }

        void Hotel(float cx, float cz, Rng rng, District d, float height)
        {
            float w = rng.Range(30f, 46f), depth = rng.Range(22f, 32f);
            Color32 wall = DistrictWallColor(d, rng);
            int wc = _p.Index(wall);
            int glass = _p.Index(Col.Hex("2C4F63"));
            int balcony = _p.Index(Col.Scale(wall, 1.12f));
            int neon = _p.Index(Col.Hex(rng.Chance(0.5f) ? "FF5FA2" : "5FE3FF"));
            Chunk c = GetChunk(cx, cz);

            c.Matte.Box(new Vector3(cx, height * 0.5f, cz), new Vector3(w, height, depth), wc);
            int floors = Mathf.Max(2, Mathf.RoundToInt(height / 3.2f));
            for (int f = 1; f < floors; f++)
            {
                float y = f * 3.2f - 0.8f;
                c.Matte.Box(new Vector3(cx, y, cz + depth * 0.5f + 0.5f), new Vector3(w * 0.9f, 0.25f, 1.2f), balcony);
                c.Matte.Box(new Vector3(cx, y + 0.9f, cz + depth * 0.5f + 0.06f), new Vector3(w * 0.86f, 1.6f, 0.1f), glass);
                c.Matte.Box(new Vector3(cx, y + 0.9f, cz - depth * 0.5f - 0.06f), new Vector3(w * 0.86f, 1.6f, 0.1f), glass);
            }
            // roof
            c.Matte.Box(new Vector3(cx, height + 0.5f, cz), new Vector3(w * 1.04f, 1f, depth * 1.04f), _p.Index(Col.Scale(wall, 0.8f)));
            if (rng.Chance(0.7f))
                c.Emissive.Box(new Vector3(cx, height + 2.2f, cz + depth * 0.5f), new Vector3(w * 0.55f, Mathf.Max(1.2f, height * 0.18f), 0.3f), neon);
            // ground floor awning
            c.Matte.Box(new Vector3(cx, 3.2f, cz + depth * 0.5f + 1.2f), new Vector3(w * 0.95f, 0.3f, 2.6f), balcony);

            Buildings.Add(new Building
            {
                Bounds = Box3.FromCenter(new Vector3(cx, height * 0.5f, cz), new Vector3(w, height, depth)),
                Color = wc, Floors = floors, District = d.Id, Tall = height > 28f
            });
            Colliders.Add(Buildings[Buildings.Count - 1].Bounds);
        }

        void Villa(float cx, float cz, Rng rng, District d, float width)
        {
            float h = rng.Range(6f, 9f);
            Color32 wall = DistrictWallColor(d, rng);
            int wc = _p.Index(wall);
            int roof = _p.Index(Col.Hex("B4553F"));
            int trim = _p.Index(Col.Hex("F2EDE2"));
            Chunk c = GetChunk(cx, cz);

            c.Matte.Box(new Vector3(cx, h * 0.5f, cz), new Vector3(width, h, width * 0.75f), wc);
            c.Matte.Box(new Vector3(cx, h + 0.35f, cz), new Vector3(width * 1.08f, 0.7f, width * 0.83f), roof);
            c.Matte.Box(new Vector3(cx + width * 0.5f + 0.05f, 1.2f, cz), new Vector3(0.14f, 2.4f, 1.6f), trim);
            // pool
            int pool = _p.Index(Col.Hex("63C6D8"));
            c.Glossy.Box(new Vector3(cx + width * 0.2f, 0.12f, cz + width * 0.85f), new Vector3(width * 0.5f, 0.24f, width * 0.4f), pool);
            Buildings.Add(new Building
            {
                Bounds = Box3.FromCenter(new Vector3(cx, h * 0.5f, cz), new Vector3(width, h, width * 0.75f)),
                Color = wc, Floors = 2, District = d.Id, Tall = false
            });
            Colliders.Add(Buildings[Buildings.Count - 1].Bounds);
        }

        void SmallHouse(float cx, float cz, Rng rng, District d)
        {
            float w = rng.Range(12f, 17f), depth = rng.Range(10f, 14f), h = rng.Range(4.4f, 7f);
            Color32 wall = DistrictWallColor(d, rng);
            int wc = _p.Index(wall);
            int roof = _p.Index(Col.Scale(Col.Hex(rng.Chance(0.5f) ? "6E4A3A" : "4A4E52"), rng.Range(0.9f, 1.15f)));
            int door = _p.Index(Col.Hex("3A2E28"));
            int glass = _p.Index(Col.Hex("2A4652"));
            Chunk c = GetChunk(cx, cz);

            c.Matte.Box(new Vector3(cx, h * 0.5f, cz), new Vector3(w, h, depth), wc);
            // simple gable made of two tilted slabs
            float slope = Mathf.Atan2(1.8f, depth * 0.5f) * Mathf.Rad2Deg;
            c.Matte.Box(new Vector3(cx, h + 0.9f, cz + depth * 0.25f), new Vector3(w + 0.6f, 0.28f, depth * 0.56f),
                roof, Quaternion.Euler(-slope, 0f, 0f));
            c.Matte.Box(new Vector3(cx, h + 0.9f, cz - depth * 0.25f), new Vector3(w + 0.6f, 0.28f, depth * 0.56f),
                roof, Quaternion.Euler(slope, 0f, 0f));
            c.Matte.Box(new Vector3(cx + w * 0.3f, 1.1f, cz + depth * 0.5f + 0.05f), new Vector3(1.2f, 2.2f, 0.12f), door);
            c.Matte.Box(new Vector3(cx - w * 0.22f, 2.1f, cz + depth * 0.5f + 0.06f), new Vector3(1.6f, 1.3f, 0.1f), glass);
            c.Matte.Box(new Vector3(cx + w * 0.5f + 0.06f, 2.1f, cz), new Vector3(0.1f, 1.3f, 1.5f), glass);

            Buildings.Add(new Building
            {
                Bounds = Box3.FromCenter(new Vector3(cx, h * 0.5f, cz), new Vector3(w, h, depth)),
                Color = wc, Floors = 1, District = d.Id, Tall = false
            });
            Colliders.Add(Buildings[Buildings.Count - 1].Bounds);
        }

        void Warehouse(float cx, float cz, Rng rng, District d)
        {
            float w = rng.Range(34f, 48f), depth = rng.Range(24f, 34f), h = rng.Range(7f, 12f);
            int wc = _p.Index(DistrictWallColor(d, rng));
            int metal = _p.Index(Col.Scale(Col.Hex("8B9199"), rng.Range(0.85f, 1.1f)));
            int door = _p.Index(Col.Hex("43474C"));
            Chunk c = GetChunk(cx, cz);

            c.Matte.Box(new Vector3(cx, h * 0.5f, cz), new Vector3(w, h, depth), wc);
            c.Matte.Box(new Vector3(cx, h + 0.25f, cz), new Vector3(w * 1.02f, 0.5f, depth * 1.02f), metal);
            c.Matte.Box(new Vector3(cx, 2.6f, cz + depth * 0.5f + 0.08f), new Vector3(w * 0.35f, 5.2f, 0.16f), door);
            if (rng.Chance(0.5f))
                c.Emissive.Box(new Vector3(cx, h + 1.4f, cz + depth * 0.5f + 0.1f), new Vector3(w * 0.4f, 0.8f, 0.14f),
                    _p.Index(Col.Hex("FFE2A0")));

            Buildings.Add(new Building
            {
                Bounds = Box3.FromCenter(new Vector3(cx, h * 0.5f, cz), new Vector3(w, h, depth)),
                Color = wc, Floors = 1, District = d.Id, Tall = false
            });
            Colliders.Add(Buildings[Buildings.Count - 1].Bounds);
        }

        void Container(float cx, float cz, Rng rng)
        {
            Color32 c32 = Col.Hex(rng.Chance(0.5f) ? "B8552F" : "3E6E8E");
            int ci = _p.Index(Col.Scale(c32, rng.Range(0.8f, 1.05f)));
            float h = rng.Range(2.4f, 2.8f);
            Chunk ch = GetChunk(cx, cz);
            ch.Matte.Box(new Vector3(cx, h * 0.5f, cz), new Vector3(12f, h, 2.6f), ci,
                Quaternion.Euler(0f, rng.Chance(0.5f) ? 90f : 0f, 0f));
        }

        void AddPalm(float x, float z, float height)
        {
            int trunk = _p.Index(new Color32(120, 96, 68, 255));
            int leaf = _p.Index(Col.Scale(new Color32(64, 116, 62, 255), 0.9f + (x * 0.001f % 0.2f)));
            Chunk c = GetChunk(x, z);
            float lean = Mathf.Sin(x * 0.13f) * 0.1f;
            c.Matte.Cylinder(new Vector3(x, 0f, z), 0.34f, height, 6, trunk, Quaternion.Euler(lean * 20f, 0f, lean * 20f), 0.22f);
            Vector3 top = new Vector3(x + lean * height, height, z + lean * height * 0.4f);
            for (int i = 0; i < 7; i++)
            {
                float a = i * (360f / 7f);
                Quaternion q = Quaternion.Euler(0f, a, -22f);
                c.Matte.Box(top + q * new Vector3(0f, 0.1f, 1.15f), new Vector3(0.36f, 0.1f, 2.3f), leaf, q);
            }
            Palms.Add(new Vector3(x, 0f, z));
            Colliders.Add(Box3.FromCenter(new Vector3(x, 1.5f, z), new Vector3(0.9f, 3f, 0.9f)));
        }

        void AddBush(float x, float z, Rng rng)
        {
            int green = _p.Index(Col.Scale(new Color32(58, 92, 52, 255), rng.Range(0.85f, 1.2f)));
            Chunk c = GetChunk(x, z);
            c.Matte.Ball(new Vector3(x, 0.5f, z), rng.Range(0.7f, 1.4f), 6, 4, green);
        }

        void BuildBeachProps()
        {
            Rng rng = new Rng(Seed + 99);
            for (float z = -Extent + 40f; z < Extent - 40f; z += 16f)
            {
                if (rng.Chance(0.25f)) continue;
                AddPalm(BeachX + rng.Range(6f, 26f), z + rng.Range(-3f, 3f), rng.Range(7f, 12f));
            }
            // promenade railing
            MeshBuilder rail = new MeshBuilder(_p);
            int rc = _p.Index(new Color32(210, 208, 200, 255));
            for (float z = -Extent + 40f; z < Extent - 40f; z += 6f)
                rail.Box(new Vector3(BeachX + 2f, 0.55f, z), new Vector3(0.16f, 1.1f, 0.16f), rc);
            Go.Mesh(Root, "railing", rail.ToMesh("railing"), MatLib.Matte, false);
        }

        void BuildStreetLamps(int half)
        {
            int pole = _p.Index(new Color32(70, 74, 78, 255));
            int bulb = _p.Index(new Color32(255, 226, 156, 255));
            for (int ix = -half; ix <= half; ix += 2)
            {
                for (int iz = -half; iz <= half; iz += 2)
                {
                    float x = ix * Spacing, z = iz * Spacing;
                    if (x > BeachX - 10f) continue;
                    float off = RoadHalf + 1.6f;
                    for (int s = -1; s <= 1; s += 2)
                    {
                        Chunk c = GetChunk(x + s * off, z + s * off);
                        Vector3 basePos = new Vector3(x + s * off, 0f, z + s * off);
                        c.Matte.Cylinder(basePos, 0.16f, 7.4f, 5, pole);
                        c.Matte.Box(basePos + new Vector3(0f, 7.4f, 0f), new Vector3(0.3f, 0.24f, 1.6f), pole);
                        c.Emissive.Box(basePos + new Vector3(0f, 7.1f, 0.6f * s), new Vector3(0.5f, 0.18f, 0.9f), bulb);
                        Lamps.Add(basePos + new Vector3(0f, 7f, 0f));
                    }
                }
            }
        }

        // --------------------------------------------------------------- query

        public bool IsWater(float x, float z) { return x > OceanX + 1f; }

        public bool OnBeach(float x, float z) { return x > BeachX && x <= OceanX; }

        public bool IsRoad(float x, float z)
        {
            float mx = Mathf.Repeat(x + Spacing * 0.5f, Spacing) - Spacing * 0.5f;
            float mz = Mathf.Repeat(z + Spacing * 0.5f, Spacing) - Spacing * 0.5f;
            return Mathf.Abs(mx) <= RoadHalf || Mathf.Abs(mz) <= RoadHalf;
        }

        /// <summary>Centre of the nearest road, useful for spawning traffic and pedestrians.</summary>
        public Vector3 NearestRoad(Vector3 p, float lane = 0f)
        {
            float rx = Mathf.Round((p.x - lane) / Spacing) * Spacing + lane;
            float rz = Mathf.Round((p.z - lane) / Spacing) * Spacing + lane;
            if (Mathf.Abs(p.x - rx) <= Mathf.Abs(p.z - rz)) return new Vector3(rx, 0f, p.z);
            return new Vector3(p.x, 0f, rz);
        }

        public Vector3 RandomRoad(Rng rng)
        {
            int cell = rng.Range(-14, 14);
            bool alongX = rng.Chance(0.5f);
            float c = cell * Spacing;
            float t = rng.Range(-Extent + 60f, BeachX - 60f);
            if (alongX) return new Vector3(t, 0f, c);
            return new Vector3(c, 0f, t);
        }

        public void ClampToLand(ref Vector3 p, float margin = 2f)
        {
            p.x = Mathf.Clamp(p.x, -Extent + margin, BeachX + (OceanX - BeachX) * 0.5f);
            p.z = Mathf.Clamp(p.z, -Extent + margin, Extent - margin);
        }

        // ----------------------------------------------------------------- map

        public Texture2D MakeMap(int res)
        {
            Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
            tex.name = "leonida_map";
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Trilinear;
            Color32[] px = new Color32[res * res];
            float span = Extent * 2f;
            Color32 ocean = Col.Hex("17435F");
            Color32 shallow = Col.Hex("2A6C90");
            Color32 sand = Col.Hex("D8C089");
            Color32 road = Col.Hex("8A8A84");
            Color32 land = Col.Hex("5E6E52");
            for (int y = 0; y < res; y++)
            {
                float wz = -Extent + (y + 0.5f) / res * span;
                for (int x = 0; x < res; x++)
                {
                    float wx = -Extent + (x + 0.5f) / res * span;
                    Color32 c;
                    if (wx > OceanX) c = Col.Mix(shallow, ocean, Mathf.Clamp01((wx - OceanX) / 260f));
                    else if (wx > BeachX) c = sand;
                    else if (IsRoad(wx, wz)) c = road;
                    else
                    {
                        District d = GameData.DistrictAt(wx, wz);
                        c = Col.Scale(Col.Hex(d.ColorHex), 0.55f);
                        if (Mathf.Repeat(wx, Spacing) < 1.6f || Mathf.Repeat(wz, Spacing) < 1.6f)
                            c = Col.Mix(c, land, 0.5f);
                    }
                    px[y * res + x] = c;
                }
            }
            tex.SetPixels32(px);
            tex.Apply(false, false);
            return tex;
        }

        public void Destroy()
        {
            if (Root != null) UnityEngine.Object.Destroy(Root.gameObject);
            for (int i = 0; i < _extra.Count; i++) if (_extra[i] != null) UnityEngine.Object.Destroy(_extra[i]);
            _extra.Clear();
            Buildings.Clear(); Lamps.Clear(); Palms.Clear(); RoadNodes.Clear();
            _chunks.Clear();
            if (MapTexture != null) UnityEngine.Object.Destroy(MapTexture);
        }
    }
}
