using System;
using System.Collections.Generic;
using UnityEngine;
using GTA6.Core;

namespace GTA6.Game
{
    // ---------------------------------------------------------------- effects

    public static class Fx
    {
        sealed class Item
        {
            public GameObject Go;
            public float Life, MaxLife;
            public Vector3 Vel;
            public float Gravity;
            public Vector3 GrowScale;
            public bool Fade;
            public LineRenderer Line;
        }

        static Transform _root;
        static readonly List<Item> _items = new List<Item>();
        static readonly List<Item> _pool = new List<Item>();
        static Mesh _ball, _cube;
        static Material _spark, _blood, _smoke, _fire, _glass;
        static readonly List<Item> _lines = new List<Item>();
        static readonly List<Item> _linePool = new List<Item>();

        public static int LiveCount { get { return _items.Count + _lines.Count; } }

        public static void Init(Transform root)
        {
            _root = root;
            if (_ball != null) return;
            MeshBuilder mb = new MeshBuilder(MatLib.Palette);
            int white = MatLib.Palette.Index(new Color32(255, 255, 255, 255));
            mb.Ball(Vector3.zero, 0.5f, 6, 4, white);
            _ball = mb.ToMesh("fx_ball");
            mb.Clear();
            mb.Box(Vector3.zero, Vector3.one, white);
            _cube = mb.ToMesh("fx_cube");
            MatLib.Palette.Apply();

            _spark = TintedColour("FFC46B");
            _blood = TintedColour("B02020");
            _smoke = TintedColour("9AA0A6");
            _fire = TintedColour("FF8A2A");
            _glass = TintedColour("CFE8F0");
        }

        static Material TintedColour(string hex)
        {
            // Vertex colours are not used by the Standard shader, so each effect colour
            // gets its own tiny palette entry baked into its own material.
            Palette p = new Palette();
            int idx = p.Index(Col.Hex(hex));
            p.UV(idx);
            p.Apply();
            Material m = new Material(MatLib.Matte);
            m.SetTexture("_MainTex", p.Texture);
            return m;
        }

        static Item Get()
        {
            Item it;
            if (_pool.Count > 0) { it = _pool[_pool.Count - 1]; _pool.RemoveAt(_pool.Count - 1); }
            else
            {
                it = new Item { Go = Go.New("fx", _root), Fade = false };
            }
            it.Go.SetActive(true);
            return it;
        }

        static void Spawn(Vector3 pos, Vector3 scale, Material mat, float life, Vector3 vel, float gravity, Vector3 grow, Mesh mesh = null)
        {
            if (_root == null) return;
            Item it = Get();
            MeshFilter mf = it.Go.GetComponent<MeshFilter>();
            if (mf == null) mf = it.Go.AddComponent<MeshFilter>();
            mf.sharedMesh = mesh != null ? mesh : _ball;
            MeshRenderer mr = it.Go.GetComponent<MeshRenderer>();
            if (mr == null) mr = it.Go.AddComponent<MeshRenderer>();
            mr.sharedMaterial = mat;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.receiveShadows = false;
            it.Go.transform.position = pos;
            it.Go.transform.localScale = scale;
            it.Go.transform.rotation = Quaternion.Euler(UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f), 0f);
            it.Life = life;
            it.MaxLife = life;
            it.Vel = vel;
            it.Gravity = gravity;
            it.GrowScale = grow;
            _items.Add(it);
        }

        public static void Tracer(Vector3 from, Vector3 to, Color32 colour)
        {
            if (_root == null) return;
            Item it;
            if (_linePool.Count > 0) { it = _linePool[_linePool.Count - 1]; _linePool.RemoveAt(_linePool.Count - 1); it.Go.SetActive(true); }
            else
            {
                it = new Item { Go = Go.New("tracer", _root) };
                LineRenderer lr = it.Go.AddComponent<LineRenderer>();
                lr.useWorldSpace = true;
                lr.positionCount = 2;
                lr.widthMultiplier = 0.06f;
                lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                lr.material = TintedColour("FFF0C0");
                it.Line = lr;
            }
            it.Line.SetPosition(0, from);
            it.Line.SetPosition(1, to);
            it.Life = 0.06f;
            it.MaxLife = 0.06f;
            _lines.Add(it);
        }

        public static void Muzzle(Vector3 pos, Vector3 dir)
        {
            Spawn(pos, Vector3.one * 0.22f, _fire, 0.06f, dir.normalized * 2f, 0f, new Vector3(0.4f, 0.4f, 0.4f));
        }

        public static void Impact(Vector3 pos, Vector3 normal)
        {
            for (int i = 0; i < 3; i++)
                Spawn(pos, Vector3.one * 0.07f, _spark, 0.5f, normal * 2f + UnityEngine.Random.insideUnitSphere * 2.5f, 9f, Vector3.zero, _cube);
            Spawn(pos + normal * 0.05f, Vector3.one * 0.18f, _smoke, 0.4f, normal * 0.6f, -0.2f, new Vector3(1.6f, 1.6f, 1.6f));
        }

        public static void Blood(Vector3 pos)
        {
            for (int i = 0; i < 4; i++)
                Spawn(pos, Vector3.one * 0.1f, _blood, 0.55f, UnityEngine.Random.insideUnitSphere * 2f + Vector3.up, 7f, Vector3.zero, _cube);
        }

        public static void Glass(Vector3 pos)
        {
            for (int i = 0; i < 5; i++)
                Spawn(pos, Vector3.one * 0.08f, _glass, 0.9f, UnityEngine.Random.insideUnitSphere * 3f + Vector3.up, 8f, Vector3.zero, _cube);
        }

        public static void Explosion(Vector3 pos)
        {
            Spawn(pos, Vector3.one * 2f, _fire, 0.75f, Vector3.up * 3f, -1f, new Vector3(5f, 5f, 5f));
            for (int i = 0; i < 8; i++)
                Spawn(pos, Vector3.one * 0.6f, _smoke, 1.4f, UnityEngine.Random.insideUnitSphere * 5f + Vector3.up * 3f, -0.5f, new Vector3(2f, 2f, 2f));
            for (int i = 0; i < 10; i++)
                Spawn(pos, Vector3.one * 0.16f, _spark, 0.8f, UnityEngine.Random.insideUnitSphere * 9f, 12f, Vector3.zero, _cube);
        }

        public static void Update(float dt)
        {
            for (int i = _items.Count - 1; i >= 0; i--)
            {
                Item it = _items[i];
                it.Life -= dt;
                if (it.Life <= 0f)
                {
                    it.Go.SetActive(false);
                    _items.RemoveAt(i);
                    _pool.Add(it);
                    continue;
                }
                it.Vel.y -= it.Gravity * dt;
                it.Go.transform.position += it.Vel * dt;
                if (it.GrowScale.sqrMagnitude > 0.001f)
                    it.Go.transform.localScale += it.GrowScale * dt;
                float t = it.Life / it.MaxLife;
                if (it.GrowScale.sqrMagnitude > 0.001f)
                    it.Go.transform.localScale *= Mathf.Lerp(0.96f, 1f, t);
                if (it.Go.transform.position.y < 0.02f) it.Vel.y = Mathf.Abs(it.Vel.y) * 0.2f;
            }

            for (int i = _lines.Count - 1; i >= 0; i--)
            {
                Item it = _lines[i];
                it.Life -= dt;
                if (it.Life <= 0f)
                {
                    it.Go.SetActive(false);
                    _lines.RemoveAt(i);
                    _linePool.Add(it);
                }
            }
        }
    }

    // --------------------------------------------------------------- shooting

    public struct ShotHit
    {
        public bool Hit;
        public Vector3 Point;
        public Vector3 Normal;
        public ActorHit Actor;
        public bool BlockedByWorld;
    }

    public static class Shooting
    {
        /// <summary>Raised with true when the victim was an officer. GameRoot uses it for crimes and missions.</summary>
        public static Action<bool> OnKill;

        public static ShotHit Trace(Vector3 origin, Vector3 dir, float maxDist, World world)
        {
            ShotHit r = new ShotHit();
            float worldDist = maxDist;
            Vector3 normal = -dir;
            Box3 box;
            float wd = world.Colliders.Raycast(origin, dir, maxDist, out box);
            if (wd >= 0f) { worldDist = wd; normal = (origin + dir * wd - box.Center).normalized; }

            ActorHit actor = Actors.Raycast(origin, dir, Mathf.Min(maxDist, worldDist + 0.4f));
            if (actor.Valid && actor.Distance <= worldDist)
            {
                r.Hit = true;
                r.Actor = actor;
                r.Point = actor.Point;
                r.Normal = -dir;
                return r;
            }
            if (wd >= 0f)
            {
                r.Hit = true;
                r.BlockedByWorld = true;
                r.Point = origin + dir * wd;
                r.Normal = normal;
            }
            return r;
        }

        /// <summary>Nearest living target inside a cone, for the mobile auto-aim.</summary>
        public static bool AutoAim(Vector3 origin, Vector3 forward, float maxAngle, float maxDist, out ActorHit target)
        {
            target = new ActorHit();
            float bestScore = float.MaxValue;
            bool found = false;
            for (int pass = 0; pass < 3; pass++)
            {
                int count = pass == 0 ? Actors.Peds.Count : pass == 1 ? Actors.Cops.Count : Actors.Thugs.Count;
                for (int i = 0; i < count; i++)
                {
                    Vector3 p;
                    bool dead;
                    if (pass == 0) { Npc n = Actors.Peds[i]; p = n.Pos + Vector3.up * 1f; dead = n.Dead; }
                    else if (pass == 1) { Cop c = Actors.Cops[i]; p = c.Pos + Vector3.up * 1f; dead = c.Dead; }
                    else { Cop c = Actors.Thugs[i]; p = c.Pos + Vector3.up * 1f; dead = c.Dead; }
                    if (dead) continue;
                    Vector3 to = p - origin;
                    float dist = to.magnitude;
                    if (dist > maxDist || dist < 0.5f) continue;
                    float angle = Vector3.Angle(forward, to);
                    if (angle > maxAngle) continue;
                    float score = angle / maxAngle + dist / maxDist * 0.35f;
                    if (score < bestScore)
                    {
                        bestScore = score;
                        if (pass == 0) target = new ActorHit { Ped = Actors.Peds[i], Distance = dist, Point = p };
                        else if (pass == 1) target = new ActorHit { Officer = Actors.Cops[i], Distance = dist, Point = p };
                        else target = new ActorHit { Officer = Actors.Thugs[i], Distance = dist, Point = p };
                        found = true;
                    }
                }
            }
            return found;
        }

        public static void FireWeapon(Player player, World world, Vector3 origin, Vector3 aimPoint, WeaponState weapon, bool autoAim)
        {
            WeaponDef def = weapon.Def;
            if (!weapon.TryFire()) return;

            Vector3 dir = (aimPoint - origin).normalized;
            if (autoAim && !def.Automatic)
            {
                ActorHit target;
                if (AutoAim(origin, dir, 12f, def.Range, out target))
                    dir = (target.Point - origin).normalized;
            }

            AudioDirector.I.PlayShot(def.Class, origin);
            Fx.Muzzle(origin + dir * 0.4f, dir);

            for (int pellet = 0; pellet < Mathf.Max(1, def.PelletCount); pellet++)
            {
                Vector3 pdir = dir;
                if (def.Spread > 0.0001f)
                {
                    Vector3 right = Vector3.Cross(Vector3.up, dir).normalized;
                    Vector3 up = Vector3.Cross(dir, right);
                    pdir = (dir + right * UnityEngine.Random.Range(-def.Spread, def.Spread)
                                + up * UnityEngine.Random.Range(-def.Spread, def.Spread)).normalized;
                }
                ShotHit hit = Trace(origin, pdir, def.Range, world);
                Vector3 end = hit.Hit ? hit.Point : origin + pdir * def.Range;
                if (pellet == 0 || UnityEngine.Random.value < 0.4f)
                    Fx.Tracer(origin + dir * 0.6f + Vector3.down * 0.05f, end, Col.Hex(def.TracerHex));
                if (!hit.Hit) continue;
                if (hit.Actor.Valid && !hit.Actor.Dead)
                {
                    bool wasCop = hit.Actor.Officer != null && !hit.Actor.Officer.Criminal;
                    hit.Actor.Damage(def.Damage, origin);
                    Fx.Blood(hit.Point);
                    if (hit.Actor.Dead)
                    {
                        AudioDirector.I.PlayHit(origin);
                        if (OnKill != null) OnKill(wasCop);
                    }
                }
                else if (!hit.BlockedByWorld || UnityEngine.Random.value < 0.6f)
                {
                    Fx.Impact(hit.Point, hit.Normal);
                }
            }
        }
    }

    // ---------------------------------------------------------------- police

    public sealed class PoliceForce
    {
        public readonly List<Vehicle> Cruisers = new List<Vehicle>();
        public bool Seen;
        public float SeenTimer;
        float _spawnTimer;
        float _copTimer;
        Transform _parent;
        Rng _rng = new Rng(9182);

        public void Init(Transform parent) { _parent = parent; }

        public int OfficerCount { get { return Actors.Cops.Count; } }

        public void Update(float dt, World world, Player player, WantedSystem wanted, MissionState mission, bool playerInMission)
        {
            if (_parent == null) return;
            int stars = wanted.Stars;

            // ---- sight check
            Seen = false;
            float bestDist = 999f;
            for (int i = 0; i < Actors.Cops.Count; i++)
            {
                Cop c = Actors.Cops[i];
                if (c.Dead) continue;
                float d = Vector3.Distance(c.Pos, player.Pos);
                if (d < 55f && HasSight(c.Pos + Vector3.up * 1.6f, player.Chest, world)) { Seen = true; bestDist = Mathf.Min(bestDist, d); }
            }
            for (int i = 0; i < Cruisers.Count; i++)
            {
                Vehicle v = Cruisers[i];
                if (v.Destroyed) continue;
                float d = Vector3.Distance(v.Pos, player.Pos);
                if (d < 110f && HasSight(v.Pos + Vector3.up * 1.5f, player.Chest, world)) { Seen = true; bestDist = Mathf.Min(bestDist, d); }
            }
            if (Seen) SeenTimer = 4f; else SeenTimer = Mathf.Max(0f, SeenTimer - dt);
            bool visible = Seen || SeenTimer > 0f;

            wanted.Tick(dt, visible);

            // ---- cruisers
            _spawnTimer -= dt;
            int maxCruisers = stars == 0 ? 0 : Mathf.Clamp(stars + 1, 2, 6);
            if (stars > 0 && Cruisers.Count < maxCruisers && _spawnTimer <= 0f)
            {
                _spawnTimer = 5.5f;
                Vector3 spawn = world.RandomRoad(_rng);
                if (Vector3.Distance(spawn, player.Pos) > 140f && Vector3.Distance(spawn, player.Pos) < 420f)
                {
                    Vehicle v = Vehicle.Build(GameData.Vehicle("police"), spawn, _rng.Range(0f, 360f), _parent, MatLib.Palette);
                    Cruisers.Add(v);
                }
            }

            for (int i = Cruisers.Count - 1; i >= 0; i--)
            {
                Vehicle v = Cruisers[i];
                float d = Vector3.Distance(v.Pos, player.Pos);
                if (v.Destroyed || d > 520f || stars == 0)
                {
                    if (v.Root != null) UnityEngine.Object.Destroy(v.Root);
                    Cruisers.RemoveAt(i);
                    continue;
                }
                if (v.Occupied) continue;
                ChaseVehicle(v, dt, world, player, stars);
                v.SirenPhase += dt * 6f;
            }

            // ---- officers on foot leave the car when the player is close
            _copTimer -= dt;
            if (stars > 0 && _copTimer <= 0f && Actors.Cops.Count < 3 + stars * 2)
            {
                _copTimer = 4.5f;
                for (int i = 0; i < Cruisers.Count; i++)
                {
                    Vehicle v = Cruisers[i];
                    if (v.Occupied || v.Destroyed) continue;
                    if (Vector3.Distance(v.Pos, player.Pos) < 38f)
                    {
                        Vector3 off = v.Forward * 2.2f;
                        Cop cop = new Cop();
                        cop.Setup(_parent, v.Pos + off, Look.Police());
                        Actors.Cops.Add(cop);
                        break;
                    }
                }
            }

            for (int i = Actors.Cops.Count - 1; i >= 0; i--)
            {
                Cop c = Actors.Cops[i];
                float d = Vector3.Distance(c.Pos, player.Pos);
                if (c.Dead && c.DeathTimer > 24f) { c.View.Destroy(); Actors.Cops.RemoveAt(i); continue; }
                if (!c.Dead && (d > 170f || stars == 0))
                {
                    c.View.Destroy();
                    Actors.Cops.RemoveAt(i);
                    continue;
                }
                if (!c.Dead) c.Update(dt, world, player, wanted.PoliceAggression);
            }
        }

        void ChaseVehicle(Vehicle v, float dt, World world, Player player, int stars)
        {
            Vector3 to = player.Pos - v.Pos;
            to.y = 0f;
            float dist = to.magnitude;
            Vector3 dir = dist > 0.1f ? to / dist : Vector3.forward;
            float targetHeading = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            float steer = Mathf.Clamp(M.AngleDelta(v.Heading, targetHeading) / 35f, -1f, 1f);
            v.Steer = Mathf.MoveTowards(v.Steer, steer, dt * 4f);
            float want = v.Def.TopSpeed * (dist > 24f ? 0.85f : 0.2f);
            v.Speed = Mathf.MoveTowards(v.Speed, want, v.Def.Accel * dt);
            v.Heading = Mathf.Repeat(v.Heading + v.Steer * 52f * dt, 360f);
            v.Pos += v.Forward * v.Speed * dt;
            world.ClampToLand(ref v.Pos, 4f);
            v.Sync();
        }

        static bool HasSight(Vector3 from, Vector3 to, World world)
        {
            Vector3 dir = to - from;
            float d = dir.magnitude;
            if (d < 0.1f) return true;
            dir /= d;
            Box3 box;
            float hit = world.Colliders.Raycast(from, dir, d, out box);
            return hit < 0f;
        }

        public void Clear()
        {
            for (int i = 0; i < Cruisers.Count; i++) if (Cruisers[i].Root != null) UnityEngine.Object.Destroy(Cruisers[i].Root);
            Cruisers.Clear();
            for (int i = 0; i < Actors.Cops.Count; i++) if (Actors.Cops[i].View != null) Actors.Cops[i].View.Destroy();
            Actors.Cops.Clear();
            Seen = false; SeenTimer = 0f;
        }
    }
}
