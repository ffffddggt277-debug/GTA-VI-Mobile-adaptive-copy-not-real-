using System;
using System.Collections.Generic;
using UnityEngine;
using GTA6.Core;

namespace GTA6.Game
{
    public sealed class Vehicle
    {
        public GameObject Root;
        public VehicleDef Def;
        public Vector3 Pos;
        public float Heading;
        public float Speed;              // m/s, signed
        public float Steer;              // -1..1
        public float Health = 100f;
        public bool Occupied;
        public bool AiDriven;
        public float WheelSpin;
        public float Roll, Pitch;
        public Transform Body;
        public readonly List<Transform> Wheels = new List<Transform>();
        public float PeaceTimer;
        public float SirenPhase;
        public bool Destroyed { get { return Health <= 0f; } }
        public float SpeedKmh { get { return Mathf.Abs(Speed) * 3.6f; } }

        public Vector3 Forward { get { return Quaternion.Euler(0f, Heading, 0f) * Vector3.forward; } }

        static readonly Dictionary<string, Mesh> _bodyCache = new Dictionary<string, Mesh>();

        public static Vehicle Build(VehicleDef def, Vector3 pos, float heading, Transform parent, Palette palette)
        {
            Vehicle v = new Vehicle();
            v.Def = def;
            v.Pos = pos;
            v.Heading = heading;
            v.Root = Go.New("car_" + def.Id, parent);
            v.Body = v.Root.transform;

            Mesh body = BodyMesh(def, palette);
            Go.Mesh(v.Body, "body", body, def.Class == VehicleClass.Hyper || def.Class == VehicleClass.Sports
                ? MatLib.Glossy : MatLib.Matte, true);

            // wheels
            int wheels = def.Class == VehicleClass.Bike ? 2 : 4;
            float halfL = def.Length * 0.32f, halfW = def.Width * 0.5f + 0.02f;
            for (int i = 0; i < wheels; i++)
            {
                float x = (i % 2 == 0 ? -halfW : halfW);
                float z = i < 2 ? halfL : -halfL;
                MeshBuilder mb = new MeshBuilder(palette);
                int tyre = palette.Index(Col.Hex("16171A"));
                int rim = palette.Index(Col.Hex("B9BEC4"));
                mb.Cylinder(Vector3.zero, def.WheelRadius, 0.24f, 10, tyre, Quaternion.Euler(90f, 0f, 0f), def.WheelRadius, true);
                mb.Cylinder(new Vector3(0f, 0f, 0.02f), def.WheelRadius * 0.55f, 0.26f, 8, rim,
                    Quaternion.Euler(90f, 0f, 0f), def.WheelRadius * 0.55f, true);
                GameObject w = Go.New("wheel" + i, v.Body);
                MeshFilter mf = w.AddComponent<MeshFilter>();
                mf.sharedMesh = mb.ToMesh("wheel");
                MeshRenderer mr = w.AddComponent<MeshRenderer>();
                mr.sharedMaterial = MatLib.Matte;
                w.transform.localPosition = new Vector3(x, def.WheelRadius, z);
                v.Wheels.Add(w.transform);
            }

            // head and tail lights
            MeshBuilder lights = new MeshBuilder(palette);
            int head = palette.Index(Col.Hex("FFF6D0"));
            int tail = palette.Index(Col.Hex("E0403A"));
            float front = def.Length * 0.5f, back = -def.Length * 0.5f;
            float ly = def.Height * 0.42f;
            lights.Box(new Vector3(-def.Width * 0.34f, ly, front), new Vector3(0.34f, 0.16f, 0.08f), head);
            lights.Box(new Vector3(def.Width * 0.34f, ly, front), new Vector3(0.34f, 0.16f, 0.08f), head);
            lights.Box(new Vector3(-def.Width * 0.34f, ly, back), new Vector3(0.3f, 0.14f, 0.08f), tail);
            lights.Box(new Vector3(def.Width * 0.34f, ly, back), new Vector3(0.3f, 0.14f, 0.08f), tail);
            if (def.Class == VehicleClass.Police || def.Class == VehicleClass.Ambulance)
            {
                int bar = palette.Index(Col.Hex(def.Class == VehicleClass.Police ? "FF3B4A" : "3B7BFF"));
                lights.Box(new Vector3(0f, def.Height + 0.1f, 0f), new Vector3(def.Width * 0.7f, 0.18f, 0.3f), bar);
            }
            palette.Apply();
            GameObject lg = Go.New("lights", v.Body);
            MeshFilter lf = lg.AddComponent<MeshFilter>();
            lf.sharedMesh = lights.ToMesh("lights");
            MeshRenderer lr = lg.AddComponent<MeshRenderer>();
            lr.sharedMaterial = MatLib.Emissive;
            lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            v.Sync();
            return v;
        }

        static Mesh BodyMesh(VehicleDef def, Palette palette)
        {
            Mesh cached;
            if (_bodyCache.TryGetValue(def.Id, out cached) && cached != null) return cached;

            MeshBuilder mb = new MeshBuilder(palette);
            int body = palette.Index(Col.Hex(def.BodyHex));
            int bodyDark = palette.Index(Col.Scale(Col.Hex(def.BodyHex), 0.78f));
            int accent = palette.Index(Col.Hex(def.AccentHex));
            int glass = palette.Index(Col.Hex("20303A"));
            float L = def.Length, W = def.Width, H = def.Height;

            if (def.Class == VehicleClass.Bike)
            {
                mb.Box(new Vector3(0f, H * 0.52f, 0f), new Vector3(W * 0.5f, H * 0.3f, L * 0.7f), body);
                mb.Box(new Vector3(0f, H * 0.72f, -L * 0.1f), new Vector3(W * 0.42f, H * 0.16f, L * 0.35f), accent);
                mb.Box(new Vector3(0f, H * 0.86f, -L * 0.28f), new Vector3(W * 0.36f, 0.12f, 0.5f), accent);
            }
            else
            {
                float cabinH = H * 0.52f;
                mb.Box(new Vector3(0f, H * 0.18f, 0f), new Vector3(W, H * 0.42f, L), body);
                mb.Box(new Vector3(0f, H * 0.45f, L * 0.06f), new Vector3(W * 0.92f, cabinH, L * 0.52f), bodyDark);
                // windows
                mb.Box(new Vector3(0f, H * 0.48f, L * 0.06f + L * 0.27f), new Vector3(W * 0.82f, cabinH * 0.72f, 0.08f), glass);
                mb.Box(new Vector3(0f, H * 0.48f, L * 0.06f - L * 0.27f), new Vector3(W * 0.82f, cabinH * 0.72f, 0.08f), glass);
                mb.Box(new Vector3(W * 0.46f, H * 0.48f, L * 0.06f), new Vector3(0.08f, cabinH * 0.66f, L * 0.42f), glass);
                mb.Box(new Vector3(-W * 0.46f, H * 0.48f, L * 0.06f), new Vector3(0.08f, cabinH * 0.66f, L * 0.42f), glass);
                // bumpers and sills
                mb.Box(new Vector3(0f, H * 0.13f, L * 0.5f), new Vector3(W * 0.98f, H * 0.13f, 0.18f), accent);
                mb.Box(new Vector3(0f, H * 0.13f, -L * 0.5f), new Vector3(W * 0.98f, H * 0.13f, 0.18f), accent);
                mb.Box(new Vector3(W * 0.5f, H * 0.2f, 0f), new Vector3(0.06f, H * 0.14f, L * 0.72f), accent);
                mb.Box(new Vector3(-W * 0.5f, H * 0.2f, 0f), new Vector3(0.06f, H * 0.14f, L * 0.72f), accent);
                if (def.Class == VehicleClass.Taxi)
                    mb.Box(new Vector3(0f, H * 0.72f, 0f), new Vector3(W * 0.34f, 0.16f, 0.5f), palette.Index(Col.Hex("FFE9A8")));
                if (def.Class == VehicleClass.Police)
                {
                    mb.Box(new Vector3(0f, H * 0.4f, L * 0.06f), new Vector3(W * 1.01f, H * 0.1f, L * 0.6f), palette.Index(Col.Hex("14213A")));
                }
                if (def.Class == VehicleClass.Van)
                    mb.Box(new Vector3(0f, H * 0.55f, -L * 0.1f), new Vector3(W * 0.96f, H * 0.72f, L * 0.78f), body);
            }

            Mesh mesh = mb.ToMesh("car_" + def.Id);
            _bodyCache[def.Id] = mesh;
            return mesh;
        }

        public Vector3 SeatPosition(int seat)
        {
            float x = seat == 0 ? -Def.Width * 0.22f : Def.Width * 0.22f;
            return BodyTransform.TransformPoint(new Vector3(x, 0f, 0f));
        }

        Transform BodyTransform { get { return Root.transform; } }

        public void Sync()
        {
            if (Root == null) return;
            Root.transform.position = Pos;
            Root.transform.rotation = Quaternion.Euler(Pitch, Heading, Roll);
        }

        // ------------------------------------------------------------- driving

        public void Drive(float dt, InputState input, World world)
        {
            float throttle = input.Move.y;
            Steer = Mathf.MoveTowards(Steer, input.Move.x, dt * 5.5f);
            bool handbrake = input.Held("jump");

            float accel = Def.Accel;
            if (throttle > 0.05f)
            {
                if (Speed < 0f) Speed += Def.Brake * throttle * dt;
                else Speed += accel * throttle * dt * (1f - Mathf.Clamp01(Speed / Def.TopSpeed) * 0.6f);
            }
            else if (throttle < -0.05f)
            {
                if (Speed > 0.5f) Speed += Def.Brake * throttle * dt;
                else Speed += accel * 0.6f * throttle * dt;
            }
            else
            {
                Speed = Mathf.MoveTowards(Speed, 0f, Def.Brake * 0.22f * dt);
            }

            Speed -= Speed * 0.02f * dt * 10f;   // drag
            Speed = Mathf.Clamp(Speed, -Def.TopSpeed * 0.42f, Def.TopSpeed);

            float speedFactor = Mathf.Clamp01(Mathf.Abs(Speed) / 12f);
            float turn = Steer * (58f + 24f * Mathf.Clamp01(1f - Mathf.Abs(Speed) / Def.TopSpeed)) * speedFactor;
            if (handbrake) turn *= 1.5f;
            if (Speed < 0f) turn = -turn;
            Heading = Mathf.Repeat(Heading + turn * dt, 360f);

            Vector3 fwd = Forward;
            Pos += fwd * Speed * dt;

            Collide(world, dt);
            WheelSpin += Speed * dt / Mathf.Max(0.1f, Def.WheelRadius) * Mathf.Rad2Deg;
            Roll = Mathf.Lerp(Roll, -Steer * Mathf.Clamp01(Mathf.Abs(Speed) / 22f) * 5f, dt * 5f);
            Pitch = Mathf.Lerp(Pitch, Mathf.Clamp(throttle * -1.6f, -2.4f, 2.4f), dt * 5f);
            Sync();
            SpinWheels();
        }

        void SpinWheels()
        {
            for (int i = 0; i < Wheels.Count; i++)
            {
                Transform w = Wheels[i];
                if (w == null) continue;
                Vector3 e = w.localEulerAngles;
                w.localEulerAngles = new Vector3(WheelSpin, e.y, 0f);
                if (i >= 2) w.localRotation = Quaternion.Euler(WheelSpin, Steer * 22f, 0f);
            }
        }

        void Collide(World world, float dt)
        {
            List<int> cells = new List<int>(16);
            float radius = Def.Length * 0.5f;
            world.Colliders.Query(Pos.x, Pos.z, radius + 1f, cells);
            Vector3 normal;
            bool hit = false;
            for (int i = 0; i < cells.Count; i++)
            {
                Box3 b = world.Colliders.Boxes[cells[i]];
                if (b.ResolveCircle(ref Pos, radius * 0.7f, Def.Height, true, out normal))
                {
                    hit = true;
                    float impact = Mathf.Abs(Speed);
                    if (impact > 6f)
                    {
                        Damage(impact * 2.4f);
                        Fx.Impact(Pos + normal * radius, normal);
                        AudioDirector.I.PlayCrash(impact);
                    }
                    Speed *= 0.35f;
                }
            }
            if (Pos.x > World.OceanX - 4f) { Pos.x = World.OceanX - 4f; Speed *= 0.5f; }
            world.ClampToLand(ref Pos, 4f);
            if (hit && dt > 0f) { }
        }

        public void Damage(float amount)
        {
            if (Destroyed) return;
            Health = Mathf.Max(0f, Health - amount);
            if (Health <= 0f)
            {
                Fx.Explosion(Pos + new Vector3(0f, 1f, 0f));
                AudioDirector.I.PlayExplosion(Pos);
            }
        }

        public void RemoveDriver()
        {
            Occupied = false;
        }

        public void Place(Vector3 pos, float heading)
        {
            Pos = pos;
            Heading = heading;
            Speed = 0f;
            Sync();
        }

        // ---------------------------------------------------------- traffic

        public void DriveAi(float dt, World world, float aggression)
        {
            AiDriven = true;
            Vector3 fwd = Forward;
            // stay in the lane: steer back towards the nearest road axis
            Vector3 ahead = Pos + fwd * 14f;
            float dx = Mathf.Repeat(ahead.x + World.Spacing * 0.5f, World.Spacing) - World.Spacing * 0.5f;
            float dz = Mathf.Repeat(ahead.z + World.Spacing * 0.5f, World.Spacing) - World.Spacing * 0.5f;
            float target;
            if (Mathf.Abs(dx) < Mathf.Abs(dz)) target = Mathf.Atan2(-Mathf.Sign(dx) * 0.35f, 1f) * Mathf.Rad2Deg;
            else target = Mathf.Atan2(1f, -Mathf.Sign(dz) * 0.35f) * Mathf.Rad2Deg;
            if (Mathf.Abs(dx) < Mathf.Abs(dz)) target = Mathf.Atan2(-dx, 1f) * Mathf.Rad2Deg;
            else target = Mathf.Atan2(1f, -dz) * Mathf.Rad2Deg;

            float steer = Mathf.Clamp(M.AngleDelta(Heading, target) / 30f, -1f, 1f);
            Steer = Mathf.MoveTowards(Steer, steer, dt * 4f);

            float want = Def.TopSpeed * (0.34f + aggression * 0.4f);
            Speed = Mathf.MoveTowards(Speed, want, Def.Accel * dt);
            Heading = Mathf.Repeat(Heading + Steer * 46f * dt, 360f);
            Pos += Forward * Speed * dt;

            Collide(world, dt);
            WheelSpin += Speed * dt / Mathf.Max(0.1f, Def.WheelRadius) * Mathf.Rad2Deg;
            Sync();
            SpinWheels();
        }
    }

    /// <summary>Keeps the streets populated near the player and cleans up far away.</summary>
    public sealed class Traffic
    {
        public readonly List<Vehicle> Cars = new List<Vehicle>();
        Transform _parent;
        World _world;
        Rng _rng = new Rng(4242);
        float _timer;

        public void Init(Transform parent, World world)
        {
            _parent = parent;
            _world = world;
        }

        public void Update(float dt, Vector3 playerPos, int maxCars)
        {
            _timer -= dt;
            if (_timer <= 0f)
            {
                _timer = 1.1f;
                if (Cars.Count < maxCars)
                {
                    Vector3 p = playerPos + new Vector3(_rng.Range(-260f, 260f), 0f, _rng.Range(-260f, 260f));
                    Vector3 road = _world.NearestRoad(p, 4.5f);
                    road.y = 0f;
                    if (road.x < World.BeachX - 20f && Mathf.Abs(road.x - playerPos.x) + Mathf.Abs(road.z - playerPos.z) > 45f)
                    {
                        VehicleDef def = GameData.Vehicles[_rng.Range(0, 5)];
                        if (def.Class == VehicleClass.Police) def = GameData.Vehicle("sedan");
                        Vehicle v = Vehicle.Build(def, road, _rng.Range(0f, 360f), _parent, MatLib.Palette);
                        v.DriveAi(0.016f, _world, 0.4f);
                        Cars.Add(v);
                    }
                }
            }

            for (int i = Cars.Count - 1; i >= 0; i--)
            {
                Vehicle v = Cars[i];
                if (v.Occupied) continue;
                float d = Mathf.Abs(v.Pos.x - playerPos.x) + Mathf.Abs(v.Pos.z - playerPos.z);
                if (d > 340f || v.Destroyed)
                {
                    if (v.Root != null) UnityEngine.Object.Destroy(v.Root);
                    Cars.RemoveAt(i);
                    continue;
                }
                v.DriveAi(dt, _world, 0.35f);
            }
        }

        public Vehicle Nearest(Vector3 pos, float maxDist)
        {
            Vehicle best = null;
            float bestD = maxDist;
            for (int i = 0; i < Cars.Count; i++)
            {
                float d = Vector3.Distance(Cars[i].Pos, pos);
                if (d < bestD) { bestD = d; best = Cars[i]; }
            }
            return best;
        }

        public void Clear()
        {
            for (int i = 0; i < Cars.Count; i++) if (Cars[i].Root != null) UnityEngine.Object.Destroy(Cars[i].Root);
            Cars.Clear();
        }
    }
}
