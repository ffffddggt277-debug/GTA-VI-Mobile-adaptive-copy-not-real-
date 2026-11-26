using System;
using System.Collections.Generic;
using UnityEngine;
using GTA6.Core;

namespace GTA6.Game
{
    public struct Look
    {
        public Color32 Skin, Shirt, Pants, Shoes, Hair;

        public static Look Random(Rng rng)
        {
            Color32[] skins = { Col.Hex("F1C7A4"), Col.Hex("E0AE86"), Col.Hex("C58C63"), Col.Hex("8D5B3C"), Col.Hex("6B4229") };
            Color32[] shirts =
            {
                Col.Hex("E8E4DA"), Col.Hex("C8453C"), Col.Hex("3F5C8A"), Col.Hex("4E7A52"),
                Col.Hex("D8A03D"), Col.Hex("6E4B7A"), Col.Hex("2E2E33"), Col.Hex("D77BA2")
            };
            Color32[] pants = { Col.Hex("3A3F4A"), Col.Hex("2E4A62"), Col.Hex("5A4A3A"), Col.Hex("1F2124"), Col.Hex("7A7468") };
            Color32[] hair = { Col.Hex("1B1512"), Col.Hex("3A2A1C"), Col.Hex("6B4A2A"), Col.Hex("B5883F"), Col.Hex("8A8A8A") };
            Look l;
            l.Skin = skins[rng.Range(0, skins.Length)];
            l.Shirt = shirts[rng.Range(0, shirts.Length)];
            l.Pants = pants[rng.Range(0, pants.Length)];
            l.Shoes = Col.Hex("1C1C1F");
            l.Hair = hair[rng.Range(0, hair.Length)];
            return l;
        }

        public static Look Player()
        {
            Look l;
            l.Skin = Col.Hex("E3B58F");
            l.Shirt = Col.Hex("E9E3D6");
            l.Pants = Col.Hex("2F3A4C");
            l.Shoes = Col.Hex("F0F0EC");
            l.Hair = Col.Hex("241A14");
            return l;
        }

        public static Look Police()
        {
            Look l;
            l.Skin = Col.Hex("D9A277");
            l.Shirt = Col.Hex("22335C");
            l.Pants = Col.Hex("1A2438");
            l.Shoes = Col.Hex("141416");
            l.Hair = Col.Hex("1B1512");
            return l;
        }
    }

    /// <summary>A blocky humanoid assembled from five meshes so the limbs can swing.</summary>
    public sealed class CharacterView
    {
        public GameObject Root;
        public Transform Torso, Head, ArmL, ArmR, LegL, LegR;
        public Look Look;
        float _bob;

        public static CharacterView Create(Transform parent, string name, Look look, Palette palette)
        {
            CharacterView v = new CharacterView();
            v.Look = look;
            v.Root = Go.New(name, parent);
            int skin = palette.Index(look.Skin);
            int shirt = palette.Index(look.Shirt);
            int pants = palette.Index(look.Pants);
            int shoes = palette.Index(look.Shoes);
            int hair = palette.Index(look.Hair);

            MeshBuilder torso = new MeshBuilder(palette);
            torso.Box(new Vector3(0f, 1.18f, 0f), new Vector3(0.42f, 0.56f, 0.24f), shirt);
            torso.Box(new Vector3(0f, 0.92f, 0f), new Vector3(0.36f, 0.2f, 0.22f), pants);
            torso.Box(new Vector3(0f, 1.49f, 0f), new Vector3(0.16f, 0.1f, 0.16f), skin);
            v.Torso = TorsoOf(v.Root, "torso", torso, palette);

            MeshBuilder head = new MeshBuilder(palette);
            head.Ball(new Vector3(0f, 1.63f, 0f), 0.125f, 10, 7, skin);
            head.Box(new Vector3(0f, 1.71f, -0.01f), new Vector3(0.26f, 0.09f, 0.25f), hair);
            head.Box(new Vector3(0f, 1.64f, 0.12f), new Vector3(0.06f, 0.05f, 0.04f), skin);
            head.Box(new Vector3(0.055f, 1.66f, 0.115f), new Vector3(0.03f, 0.03f, 0.02f), palette.Index(Col.Hex("1A1A1A")));
            head.Box(new Vector3(-0.055f, 1.66f, 0.115f), new Vector3(0.03f, 0.03f, 0.02f), palette.Index(Col.Hex("1A1A1A")));
            // The head mesh is already baked around y = 1.63, so its transform stays
            // at the character root; there is no child object to re-centre.
            v.Head = TorsoOf(v.Root, "head", head, palette);

            MeshBuilder arm = new MeshBuilder(palette);
            arm.Box(new Vector3(0f, -0.22f, 0f), new Vector3(0.13f, 0.46f, 0.14f), shirt);
            arm.Box(new Vector3(0f, -0.55f, 0f), new Vector3(0.11f, 0.26f, 0.12f), skin);
            v.ArmL = TorsoOf(v.Root, "armL", arm, palette);
            v.ArmL.localPosition = new Vector3(0.27f, 1.4f, 0f);
            v.ArmR = TorsoOf(v.Root, "armR", arm, palette);
            v.ArmR.localPosition = new Vector3(-0.27f, 1.4f, 0f);

            MeshBuilder leg = new MeshBuilder(palette);
            leg.Box(new Vector3(0f, -0.24f, 0f), new Vector3(0.17f, 0.5f, 0.18f), pants);
            leg.Box(new Vector3(0f, -0.65f, 0f), new Vector3(0.15f, 0.34f, 0.16f), pants);
            leg.Box(new Vector3(0f, -0.85f, 0.04f), new Vector3(0.17f, 0.1f, 0.26f), shoes);
            v.LegL = TorsoOf(v.Root, "legL", leg, palette);
            v.LegL.localPosition = new Vector3(0.11f, 0.92f, 0f);
            v.LegR = TorsoOf(v.Root, "legR", leg, palette);
            v.LegR.localPosition = new Vector3(-0.11f, 0.92f, 0f);

            return v;
        }

        static Transform TorsoOf(GameObject parent, string name, MeshBuilder mb, Palette palette)
        {
            palette.Apply();
            GameObject g = Go.New(name, parent.transform);
            MeshFilter mf = g.AddComponent<MeshFilter>();
            mf.sharedMesh = mb.ToMesh(name);
            MeshRenderer mr = g.AddComponent<MeshRenderer>();
            mr.sharedMaterial = MatLib.Matte;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            return g.transform;
        }

        public void SetActive(bool on)
        {
            if (Root != null) Root.SetActive(on);
        }

        public void Place(Vector3 pos, float heading)
        {
            Root.transform.position = pos;
            Root.transform.rotation = Quaternion.Euler(0f, heading, 0f);
        }

        /// <summary>Procedural walk/run cycle plus a subtle idle breath.</summary>
        public void Animate(float speed01, float phase, float dt, bool aiming, bool onGround)
        {
            if (Root == null) return;
            float amp = Mathf.Lerp(18f, 52f, Mathf.Clamp01(speed01));
            float swing = Mathf.Sin(phase) * amp;
            float swing2 = Mathf.Sin(phase + Mathf.PI) * amp;
            if (aiming)
            {
                ArmR.localRotation = Quaternion.Euler(-78f, 0f, -6f);
                ArmL.localRotation = Quaternion.Euler(-70f, 0f, 8f);
                LegL.localRotation = Quaternion.Euler(swing * 0.35f, 0f, 0f);
                LegR.localRotation = Quaternion.Euler(swing2 * 0.35f, 0f, 0f);
            }
            else
            {
                ArmL.localRotation = Quaternion.Euler(-swing * 0.8f, 0f, 3f);
                ArmR.localRotation = Quaternion.Euler(-swing2 * 0.8f, 0f, -3f);
                LegL.localRotation = Quaternion.Euler(swing, 0f, 0f);
                LegR.localRotation = Quaternion.Euler(swing2, 0f, 0f);
            }
            _bob = Mathf.MoveTowards(_bob, Mathf.Abs(Mathf.Sin(phase)) * 0.045f * speed01, dt * 0.4f);
            Torso.localPosition = new Vector3(0f, _bob, 0f);
            Head.localPosition = new Vector3(0f, 1.63f + _bob, 0f);
            if (!onGround) { LegL.localRotation = Quaternion.Euler(20f, 0f, 0f); LegR.localRotation = Quaternion.Euler(-10f, 0f, 0f); }
        }

        public void Destroy()
        {
            if (Root != null) UnityEngine.Object.Destroy(Root);
        }
    }

    // ----------------------------------------------------------------- player

    public sealed class Player
    {
        public const float Height = 1.8f;
        public const float Radius = 0.34f;
        public const float WalkSpeed = 3.1f;
        public const float RunSpeed = 6.5f;
        public const float AimSpeed = 2.2f;

        public CharacterView View;
        public Vector3 Pos;
        public float Heading = 180f;
        public float VerticalSpeed;
        public bool Grounded = true;
        public float MoveAmount;
        public bool Aiming;
        public bool Alive { get { return Stats.Health > 0f; } }

        public PlayerStats Stats = new PlayerStats();
        public readonly List<WeaponState> Weapons = new List<WeaponState>();
        public int WeaponIndex;
        public Vehicle Vehicle;
        public float Phase;
        public float Recoil;         // camera kick, decays

        public WeaponState Weapon
        {
            get
            {
                if (Weapons.Count == 0) return null;
                return Weapons[M.Clamp(WeaponIndex, 0, Weapons.Count - 1)];
            }
        }

        public Vector3 Eye { get { return Pos + new Vector3(0f, 1.62f, 0f); } }
        public Vector3 Chest { get { return Pos + new Vector3(0f, 1.2f, 0f); } }

        public void Setup(Transform parent, Look look, SaveData save)
        {
            if (View != null && View.Root != null) UnityEngine.Object.Destroy(View.Root);
            View = CharacterView.Create(parent, "Player", look, MatLib.Palette);
            Pos = new Vector3(save.PosX, 0f, save.PosZ);
            Heading = save.Heading;
            Stats.Health = Mathf.Clamp(save.Health, 1f, 100f);
            Stats.Armor = save.Armor;
            Stats.Cash = save.Cash;

            Weapons.Clear();
            for (int i = 0; i < GameData.Weapons.Length; i++)
            {
                WeaponDef def = GameData.Weapons[i];
                if (!save.HasWeapon(def.Id) && def.Id != "fists") continue;
                int ammo = save.AmmoOf(def.Id);
                WeaponState st = new WeaponState(def, 1, def.Id == "fists" ? 0 : Mathf.Max(ammo, def.Reserve));
                if (def.Id != "fists" && ammo > 0) { st.Magazine = Mathf.Min(def.Magazine, ammo); st.Reserve = Mathf.Max(0, ammo - st.Magazine); }
                Weapons.Add(st);
            }
            if (Weapons.Count == 0) Weapons.Add(new WeaponState(GameData.Weapon("fists"), 1, 0));
            WeaponIndex = 0;
            Sync();
        }

        public void AddWeapon(string id, int reserve)
        {
            for (int i = 0; i < Weapons.Count; i++)
            {
                if (Weapons[i].Def.Id != id) continue;
                Weapons[i].AddReserve(reserve);
                if (Weapons[i].Magazine <= 0) Weapons[i].Reload();
                WeaponIndex = i;
                return;
            }
            WeaponDef def = GameData.Weapon(id);
            WeaponState st = new WeaponState(def, 1, Mathf.Max(reserve, def.Reserve));
            Weapons.Add(st);
            WeaponIndex = Weapons.Count - 1;
        }

        public void NextWeapon()
        {
            if (Weapons.Count <= 1) return;
            WeaponIndex = (WeaponIndex + 1) % Weapons.Count;
        }

        public void Tick(float dt, InputState input, World world, CamRig cam, Settings settings)
        {
            if (!Alive) return;
            Stats.Tick(dt, input.Sprint, MoveAmount > 0.1f);

            if (Vehicle != null)
            {
                Pos = Vehicle.SeatPosition(0);
                Heading = Vehicle.Heading;
                Aiming = false;
                MoveAmount = 0f;
                View.SetActive(false);
                return;
            }

            View.SetActive(true);

            Vector3 move = Vector3.zero;
            if (input.Move.sqrMagnitude > 0.0004f)
            {
                Vector3 fwd = cam.FlatForward;
                Vector3 right = cam.FlatRight;
                move = (fwd * input.Move.y + right * input.Move.x);
                if (move.sqrMagnitude > 1f) move.Normalize();
            }

            Aiming = input.AimHeld && Weapon != null && !Weapon.Melee;
            float speed = Aiming ? AimSpeed : (input.Sprint && !Stats.Tired ? RunSpeed : WalkSpeed);
            MoveAmount = Mathf.MoveTowards(MoveAmount, move.magnitude * speed, dt * 12f);

            if (move.sqrMagnitude > 0.001f)
            {
                Pos += move * MoveAmount * dt;
                float targetHeading = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
                Heading = Mathf.MoveTowardsAngle(Heading, targetHeading, dt * 520f);
            }

            if (Aiming) Heading = Mathf.MoveTowardsAngle(Heading, cam.Yaw, dt * 720f);

            // gravity and jumping
            bool jumpPressed = input.Pressed("jump");
            if (Grounded && jumpPressed && !input.FireHeld)
            {
                VerticalSpeed = 5.1f;
                Grounded = false;
            }
            VerticalSpeed -= 18.5f * dt;
            Pos.y += VerticalSpeed * dt;
            if (Pos.y <= 0f) { Pos.y = 0f; VerticalSpeed = 0f; Grounded = true; }

            ResolveCollisions(world);
            Sync();

            // animation
            float phaseSpeed = 3.4f + MoveAmount * 1.7f;
            Phase += dt * phaseSpeed * Mathf.Clamp(MoveAmount / 2f, 0.12f, 2.2f);
            View.Animate(Mathf.Clamp01(MoveAmount / RunSpeed), Phase, dt, Aiming, Grounded);
            View.Place(Pos, Heading);

            if (Recoil > 0f) Recoil = Mathf.Max(0f, Recoil - dt * 8f);
        }

        void ResolveCollisions(World world)
        {
            List<int> cells = new List<int>(32);
            world.Colliders.Query(Pos.x, Pos.z, Radius + 2f, cells);
            Vector3 normal;
            for (int i = 0; i < cells.Count; i++)
            {
                Box3 b = world.Colliders.Boxes[cells[i]];
                if (Pos.y > b.Max.y - 0.2f) continue;
                b.ResolveCircle(ref Pos, Radius, Height, Grounded, out normal);
            }
            world.ClampToLand(ref Pos, 2f);
        }

        public void Sync()
        {
            if (View != null && View.Root != null)
            {
                View.Root.transform.position = Pos;
                View.Root.transform.rotation = Quaternion.Euler(0f, Heading, 0f);
            }
        }

        public void Damage(float amount, Vector3 from)
        {
            if (!Alive) return;
            Stats.Damage(amount);
            if (Stats.Health <= 0f) Stats.Health = 0f;
        }

        public void Respawn(Vector3 at)
        {
            Pos = at;
            Pos.y = 0f;
            VerticalSpeed = 0f;
            Stats.Revive();
            Heading = 0f;
            if (Vehicle != null) { Vehicle.RemoveDriver(); Vehicle = null; }
            Sync();
        }
    }

    // ----------------------------------------------------------- pedestrians

    public sealed class ActorHit
    {
        public Npc Ped;
        public Cop Officer;
        public float Distance;
        public Vector3 Point;

        public float Health
        {
            get { return Ped != null ? Ped.Health : (Officer != null ? Officer.Health : 0f); }
        }

        public void Damage(float amount, Vector3 from)
        {
            if (Ped != null) Ped.Damage(amount, from);
            else if (Officer != null) Officer.Damage(amount, from);
        }

        public bool Dead
        {
            get { return Ped != null ? Ped.Dead : (Officer != null ? Officer.Dead : true); }
        }

        public bool Valid { get { return Ped != null || Officer != null; } }
    }

    public static class Actors
    {
        public static readonly List<Npc> Peds = new List<Npc>();
        public static readonly List<Cop> Cops = new List<Cop>();
        /// <summary>Mission enemies. They use the officer behaviour but are not police.</summary>
        public static readonly List<Cop> Thugs = new List<Cop>();

        public static void Clear()
        {
            for (int i = 0; i < Peds.Count; i++) if (Peds[i].View != null) Peds[i].View.Destroy();
            for (int i = 0; i < Cops.Count; i++) if (Cops[i].View != null) Cops[i].View.Destroy();
            for (int i = 0; i < Thugs.Count; i++) if (Thugs[i].View != null) Thugs[i].View.Destroy();
            Peds.Clear();
            Cops.Clear();
            Thugs.Clear();
        }

        /// <summary>Closest living actor along a ray, used by every weapon.</summary>
        public static ActorHit Raycast(Vector3 origin, Vector3 dir, float maxDist)
        {
            ActorHit best = new ActorHit();
            float bestD = maxDist;
            for (int i = 0; i < Peds.Count; i++)
            {
                Npc p = Peds[i];
                if (p.Dead) continue;
                float d = RaySphere(origin, dir, p.Pos + new Vector3(0f, 1f, 0f), 0.55f);
                if (d >= 0f && d < bestD) { bestD = d; best = new ActorHit { Ped = p, Distance = d, Point = origin + dir * d }; }
            }
            for (int i = 0; i < Cops.Count; i++)
            {
                Cop c = Cops[i];
                if (c.Dead) continue;
                float d = RaySphere(origin, dir, c.Pos + new Vector3(0f, 1f, 0f), 0.6f);
                if (d >= 0f && d < bestD) { bestD = d; best = new ActorHit { Officer = c, Distance = d, Point = origin + dir * d }; }
            }
            for (int i = 0; i < Thugs.Count; i++)
            {
                Cop c = Thugs[i];
                if (c.Dead) continue;
                float d = RaySphere(origin, dir, c.Pos + new Vector3(0f, 1f, 0f), 0.6f);
                if (d >= 0f && d < bestD) { bestD = d; best = new ActorHit { Officer = c, Distance = d, Point = origin + dir * d }; }
            }
            return best;
        }

        public static float RaySphere(Vector3 origin, Vector3 dir, Vector3 center, float radius)
        {
            Vector3 oc = center - origin;
            float t = Vector3.Dot(oc, dir);
            if (t < 0f) return -1f;
            float d2 = oc.sqrMagnitude - t * t;
            float r2 = radius * radius;
            if (d2 > r2) return -1f;
            float thc = Mathf.Sqrt(r2 - d2);
            float t0 = t - thc;
            return t0 < 0f ? t + thc : t0;
        }
    }

    public sealed class Npc
    {
        public CharacterView View;
        public Vector3 Pos, Target;
        public float Heading;
        public float Health = 40f;
        public float Speed = 1.4f;
        public float Phase;
        public bool Panic;
        public float PanicTimer;
        public bool Dead { get { return Health <= 0f; } }
        public float DeathTimer;
        static readonly List<Vector3> _targets = new List<Vector3>();

        public void Setup(Transform parent, Rng rng, Vector3 spawn, Look look)
        {
            View = CharacterView.Create(parent, "ped", look, MatLib.Palette);
            Pos = spawn;
            Heading = rng.Range(0f, 360f);
            Speed = rng.Range(1.1f, 1.7f);
            PickTarget(rng);
        }

        public void PickTarget(Rng rng)
        {
            if (rng == null) rng = new Rng((int)(Pos.x * 13f + Pos.z * 7f) + 3);
            float ang = rng.Range(0f, Mathf.PI * 2f);
            float dist = rng.Range(14f, 46f);
            Target = Pos + new Vector3(Mathf.Cos(ang) * dist, 0f, Mathf.Sin(ang) * dist);
            Target.y = 0f;
        }

        public void Update(float dt, World world, Vector3 playerPos, Rng rng)
        {
            if (Dead)
            {
                DeathTimer += dt;
                if (View != null && View.Root != null)
                {
                    Vector3 p = View.Root.transform.position;
                    p.y = Mathf.Max(0f, p.y - dt * 2.4f);
                    View.Root.transform.position = p;
                    View.Root.transform.rotation = Quaternion.Slerp(View.Root.transform.rotation,
                        Quaternion.Euler(0f, Heading, 84f), dt * 5f);
                }
                return;
            }

            if (PanicTimer > 0f)
            {
                PanicTimer -= dt;
                Panic = PanicTimer > 0f;
            }

            Vector3 toTarget = Target - Pos;
            toTarget.y = 0f;
            if (toTarget.magnitude < 1.5f)
            {
                PickTarget(null);
                return;
            }

            Vector3 dir = toTarget.normalized;
            if (Panic)
            {
                Vector3 away = Pos - playerPos;
                away.y = 0f;
                if (away.sqrMagnitude > 0.01f) dir = (dir * 0.35f + away.normalized * 0.9f).normalized;
            }

            float speed = Panic ? Speed * 3.4f : Speed;
            Pos += dir * speed * dt;
            Heading = Mathf.MoveTowardsAngle(Heading, Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg, dt * 360f);

            // keep pedestrians on land and out of buildings
            List<int> cells = new List<int>(8);
            world.Colliders.Query(Pos.x, Pos.z, 1.2f, cells);
            Vector3 normal;
            for (int i = 0; i < cells.Count; i++)
                world.Colliders.Boxes[cells[i]].ResolveCircle(ref Pos, 0.36f, 1.8f, true, out normal);
            world.ClampToLand(ref Pos, 3f);

            Phase += dt * (Panic ? 9f : 4.6f);
            View.Animate(Panic ? 1f : 0.35f, Phase, dt, false, true);
            View.Place(Pos, Heading);
        }

        public void Damage(float amount, Vector3 from)
        {
            if (Dead) return;
            Health -= amount;
            Panic = true;
            PanicTimer = 6f;
            Vector3 away = Pos - from;
            away.y = 0f;
            if (away.sqrMagnitude > 0.01f) Target = Pos + away.normalized * 30f;
            if (Dead && View != null)
                Heading = Mathf.Atan2(away.x, away.z) * Mathf.Rad2Deg;
        }
    }

    public sealed class Cop
    {
        public CharacterView View;
        public Vector3 Pos;
        public float Heading;
        public float Health = 70f;
        public bool Criminal;
        public float FireCooldown;
        public float Phase;
        public bool Dead { get { return Health <= 0f; } }
        public float DeathTimer;

        public void Setup(Transform parent, Vector3 spawn, Look look)
        {
            View = CharacterView.Create(parent, "cop", look, MatLib.Palette);
            Pos = spawn;
            FireCooldown = 1.2f;
        }

        public void Update(float dt, World world, Player player, float aggression)
        {
            if (Dead) { DeathTimer += dt; return; }

            Vector3 to = player.Pos - Pos;
            to.y = 0f;
            float dist = to.magnitude;
            Vector3 dir = dist > 0.01f ? to / dist : Vector3.forward;

            if (dist > 9f) Pos += dir * 4.4f * dt;
            else if (dist < 4f) Pos -= dir * 2.2f * dt;
            else Pos += new Vector3(-dir.z, 0f, dir.x) * 1.4f * dt;

            List<int> cells = new List<int>(8);
            world.Colliders.Query(Pos.x, Pos.z, 1.2f, cells);
            Vector3 normal;
            for (int i = 0; i < cells.Count; i++)
                world.Colliders.Boxes[cells[i]].ResolveCircle(ref Pos, 0.36f, 1.8f, true, out normal);
            world.ClampToLand(ref Pos, 3f);

            Heading = Mathf.MoveTowardsAngle(Heading, Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg, dt * 480f);
            Phase += dt * 8f;
            View.Animate(1f, Phase, dt, true, true);
            View.Place(Pos, Heading);

            FireCooldown -= dt;
            if (FireCooldown <= 0f && dist < 42f && player.Vehicle == null && player.Alive)
            {
                FireCooldown = Mathf.Lerp(1.5f, 0.55f, aggression) + UnityEngine.Random.value * 0.4f;
                Vector3 origin = Pos + new Vector3(0f, 1.35f, 0f);
                Vector3 target = player.Chest + UnityEngine.Random.insideUnitSphere * 0.35f;
                Vector3 shotDir = (target - origin).normalized;
                Fx.Tracer(origin, origin + shotDir * 40f, Col.Hex("FFD27F"));
                Fx.Muzzle(origin + shotDir * 0.5f, shotDir);
                float d = Actors.RaySphere(origin, shotDir, player.Chest, 0.62f);
                if (d > 0f)
                {
                    player.Damage(UnityEngine.Random.Range(4f, 9f), Pos);
                    Fx.Blood(player.Chest);
                }
                AudioDirector.I.PlayShot(WeaponClass.Pistol, origin);
            }
        }

        public void Damage(float amount, Vector3 from)
        {
            if (Dead) return;
            Health -= amount;
            if (Dead) Heading = Mathf.Atan2(Pos.x - from.x, Pos.z - from.z) * Mathf.Rad2Deg;
        }
    }
}
