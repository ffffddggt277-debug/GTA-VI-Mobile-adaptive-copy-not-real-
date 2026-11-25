using System;
using System.Collections.Generic;
using System.Text;

namespace GTA6.Core
{
    // ------------------------------------------------------------------- maths

    public static class M
    {
        public static float Clamp(float v, float a, float b) { return v < a ? a : (v > b ? b : v); }
        public static int Clamp(int v, int a, int b) { return v < a ? a : (v > b ? b : v); }
        public static float Clamp01(float v) { return Clamp(v, 0f, 1f); }
        public static float Lerp(float a, float b, float t) { return a + (b - a) * Clamp01(t); }
        public static float Smooth(float t) { t = Clamp01(t); return t * t * (3f - 2f * t); }
        public static float Dist2D(float ax, float az, float bx, float bz)
        {
            float dx = ax - bx, dz = az - bz;
            return (float)Math.Sqrt(dx * dx + dz * dz);
        }
        public static float WrappedAngle(float deg)
        {
            deg %= 360f;
            if (deg < 0f) deg += 360f;
            return deg;
        }
        public static float AngleDelta(float a, float b)
        {
            float d = WrappedAngle(b) - WrappedAngle(a);
            if (d > 180f) d -= 360f;
            if (d < -180f) d += 360f;
            return d;
        }
        public static float MoveTowards(float current, float target, float maxDelta)
        {
            if (Math.Abs(target - current) <= maxDelta) return target;
            return current + Math.Sign(target - current) * maxDelta;
        }
    }

    /// <summary>Deterministic xorshift so a seed always rebuilds the same city.</summary>
    public sealed class Rng
    {
        uint _s;
        public Rng(int seed)
        {
            _s = (uint)seed;
            if (_s == 0) _s = 0x9E3779B9u;
            for (int i = 0; i < 4; i++) NextUInt();
        }
        public uint NextUInt()
        {
            _s ^= _s << 13; _s ^= _s >> 17; _s ^= _s << 5;
            return _s;
        }
        public float Value { get { return (NextUInt() & 0xFFFFFF) / 16777216f; } }
        public float Range(float a, float b) { return a + Value * (b - a); }
        public int Range(int a, int b) { return b <= a ? a : a + (int)(NextUInt() % (uint)(b - a)); }
        public bool Chance(float p) { return Value < p; }
        public float Sign { get { return Value < 0.5f ? -1f : 1f; } }
        public T Pick<T>(IList<T> list) { return list[Range(0, list.Count)]; }
    }

    // ------------------------------------------------------------- save model

    public sealed class Settings
    {
        public int Quality = 2;          // 0 low, 1 medium, 2 high, 3 ultra
        public float SoundVolume = 0.85f;
        public float MusicVolume = 0.55f;
        public float Sensitivity = 1f;
        public bool InvertY;
        public bool Shadows = true;
        public bool ShowFps;
        public bool AutoAim = true;
        public bool Vibration = true;
        public int Language;             // 0 ru, 1 en

        public Settings Clone()
        {
            return (Settings)MemberwiseClone();
        }

        public string ToText()
        {
            StringBuilder b = new StringBuilder();
            b.Append("quality=").Append(Quality).Append('\n');
            b.Append("sound=").Append(SoundVolume.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture)).Append('\n');
            b.Append("music=").Append(MusicVolume.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture)).Append('\n');
            b.Append("sens=").Append(Sensitivity.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture)).Append('\n');
            b.Append("invert=").Append(InvertY ? 1 : 0).Append('\n');
            b.Append("shadows=").Append(Shadows ? 1 : 0).Append('\n');
            b.Append("fps=").Append(ShowFps ? 1 : 0).Append('\n');
            b.Append("autoaim=").Append(AutoAim ? 1 : 0).Append('\n');
            b.Append("vibration=").Append(Vibration ? 1 : 0).Append('\n');
            b.Append("lang=").Append(Language).Append('\n');
            return b.ToString();
        }

        public static Settings FromText(string text)
        {
            Settings s = new Settings();
            if (string.IsNullOrEmpty(text)) return s;
            foreach (KeyValuePair<string, string> kv in Parse(text))
            {
                float f;
                switch (kv.Key)
                {
                    case "quality": s.Quality = M.Clamp(ParseInt(kv.Value, 2), 0, 3); break;
                    case "sound": if (TryFloat(kv.Value, out f)) s.SoundVolume = M.Clamp01(f); break;
                    case "music": if (TryFloat(kv.Value, out f)) s.MusicVolume = M.Clamp01(f); break;
                    case "sens": if (TryFloat(kv.Value, out f)) s.Sensitivity = M.Clamp(f, 0.2f, 2.5f); break;
                    case "invert": s.InvertY = kv.Value == "1"; break;
                    case "shadows": s.Shadows = kv.Value == "1"; break;
                    case "fps": s.ShowFps = kv.Value == "1"; break;
                    case "autoaim": s.AutoAim = kv.Value == "1"; break;
                    case "vibration": s.Vibration = kv.Value == "1"; break;
                    case "lang": s.Language = kv.Value == "1" ? 1 : 0; break;
                }
            }
            return s;
        }

        internal static int ParseInt(string s, int fallback)
        {
            int v;
            return int.TryParse(s, System.Globalization.NumberStyles.Integer,
                System.Globalization.CultureInfo.InvariantCulture, out v) ? v : fallback;
        }

        internal static bool TryFloat(string s, out float v)
        {
            return float.TryParse(s, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out v);
        }

        internal static IEnumerable<KeyValuePair<string, string>> Parse(string text)
        {
            string[] lines = text.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                int eq = line.IndexOf('=');
                if (eq <= 0) continue;
                yield return new KeyValuePair<string, string>(line.Substring(0, eq), line.Substring(eq + 1));
            }
        }
    }

    public sealed class SaveData
    {
        public int Version = GameData.SaveVersion;
        public int Cash = 500;
        public float Health = 100f;
        public float Armor;
        public float PosX = -330f, PosZ = -1060f;
        public float Heading;
        public float TimeOfDay = 8.5f;        // hours 0..24
        public int Day = 1;
        public float PlaySeconds;
        public int StoryIndex;                 // index into GameData.StoryOrder
        public string CurrentMission = "";
        public int CurrentStep;
        public float WantedHeat;
        public int ShotsFired;
        public int Photos;
        public string LastVehicle = "";
        public int RadioStation;
        public List<string> OwnedWeapons = new List<string> { "fists", "pistol" };
        public Dictionary<string, int> Ammo = new Dictionary<string, int>();

        public SaveData()
        {
            Ammo["pistol"] = 60;
        }

        public bool HasWeapon(string id) { return OwnedWeapons.Contains(id); }

        public int AmmoOf(string id)
        {
            int v;
            return Ammo.TryGetValue(id, out v) ? v : 0;
        }

        public void SetAmmo(string id, int value) { Ammo[id] = Math.Max(0, value); }

        public string NextMissionId()
        {
            if (StoryIndex < 0) return "";
            if (StoryIndex >= GameData.StoryOrder.Length) return "";
            return GameData.StoryOrder[StoryIndex];
        }

        public string ToText()
        {
            StringBuilder b = new StringBuilder(512);
            b.Append("version=").Append(Version).Append('\n');
            b.Append("cash=").Append(Cash).Append('\n');
            b.Append("health=").Append(F(Health)).Append('\n');
            b.Append("armor=").Append(F(Armor)).Append('\n');
            b.Append("x=").Append(F(PosX)).Append('\n');
            b.Append("z=").Append(F(PosZ)).Append('\n');
            b.Append("heading=").Append(F(Heading)).Append('\n');
            b.Append("time=").Append(F(TimeOfDay)).Append('\n');
            b.Append("day=").Append(Day).Append('\n');
            b.Append("play=").Append(F(PlaySeconds)).Append('\n');
            b.Append("story=").Append(StoryIndex).Append('\n');
            b.Append("mission=").Append(CurrentMission).Append('\n');
            b.Append("step=").Append(CurrentStep).Append('\n');
            b.Append("heat=").Append(F(WantedHeat)).Append('\n');
            b.Append("shots=").Append(ShotsFired).Append('\n');
            b.Append("photos=").Append(Photos).Append('\n');
            b.Append("car=").Append(LastVehicle).Append('\n');
            b.Append("radio=").Append(RadioStation).Append('\n');
            for (int i = 0; i < OwnedWeapons.Count; i++) b.Append("have=").Append(OwnedWeapons[i]).Append('\n');
            foreach (KeyValuePair<string, int> kv in Ammo) b.Append("ammo.").Append(kv.Key).Append('=').Append(kv.Value).Append('\n');
            return b.ToString();
        }

        static string F(float v)
        {
            return v.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture);
        }

        public static SaveData FromText(string text)
        {
            SaveData d = new SaveData();
            bool weaponsSeen = false;
            if (string.IsNullOrEmpty(text)) return d;
            foreach (KeyValuePair<string, string> kv in Settings.Parse(text))
            {
                string k = kv.Key, v = kv.Value;
                float f;
                switch (k)
                {
                    case "version": d.Version = Settings.ParseInt(v, GameData.SaveVersion); break;
                    case "cash": d.Cash = Settings.ParseInt(v, d.Cash); break;
                    case "health": if (Settings.TryFloat(v, out f)) d.Health = M.Clamp(f, 0f, 100f); break;
                    case "armor": if (Settings.TryFloat(v, out f)) d.Armor = M.Clamp(f, 0f, 100f); break;
                    case "x": if (Settings.TryFloat(v, out f)) d.PosX = f; break;
                    case "z": if (Settings.TryFloat(v, out f)) d.PosZ = f; break;
                    case "heading": if (Settings.TryFloat(v, out f)) d.Heading = f; break;
                    case "time": if (Settings.TryFloat(v, out f)) d.TimeOfDay = M.Clamp(f, 0f, 24f); break;
                    case "day": d.Day = Math.Max(1, Settings.ParseInt(v, 1)); break;
                    case "play": if (Settings.TryFloat(v, out f)) d.PlaySeconds = f; break;
                    case "story": d.StoryIndex = M.Clamp(Settings.ParseInt(v, 0), 0, GameData.StoryOrder.Length); break;
                    case "mission": d.CurrentMission = v; break;
                    case "step": d.CurrentStep = Math.Max(0, Settings.ParseInt(v, 0)); break;
                    case "heat": if (Settings.TryFloat(v, out f)) d.WantedHeat = M.Clamp(f, 0f, 600f); break;
                    case "shots": d.ShotsFired = Settings.ParseInt(v, 0); break;
                    case "photos": d.Photos = Settings.ParseInt(v, 0); break;
                    case "car": d.LastVehicle = v; break;
                    case "radio": d.RadioStation = Settings.ParseInt(v, 0); break;
                    case "have":
                        if (!weaponsSeen) { d.OwnedWeapons.Clear(); weaponsSeen = true; }
                        if (!string.IsNullOrEmpty(v) && !d.OwnedWeapons.Contains(v)) d.OwnedWeapons.Add(v);
                        break;
                    default:
                        if (k.StartsWith("ammo.", StringComparison.Ordinal))
                            d.Ammo[k.Substring(5)] = Math.Max(0, Settings.ParseInt(v, 0));
                        break;
                }
            }
            if (!d.OwnedWeapons.Contains("fists")) d.OwnedWeapons.Insert(0, "fists");
            return d;
        }
    }

    // ----------------------------------------------------------------- stats

    public sealed class PlayerStats
    {
        public float Health = 100f;
        public float Armor;
        public float Stamina = 100f;
        public int Cash = 500;

        public bool Dead { get { return Health <= 0f; } }
        public bool Tired { get { return Stamina <= 1f; } }

        /// <summary>Armour soaks two thirds of incoming damage until it breaks.</summary>
        public float Damage(float amount)
        {
            if (amount <= 0f) return 0f;
            float absorbed = 0f;
            if (Armor > 0f)
            {
                absorbed = Math.Min(Armor, amount * 0.66f);
                Armor -= absorbed;
            }
            Health = M.Clamp(Health - (amount - absorbed), 0f, 100f);
            return amount - absorbed;
        }

        public void Heal(float amount) { Health = M.Clamp(Health + amount, 0f, 100f); }
        public void AddArmor(float amount) { Armor = M.Clamp(Armor + amount, 0f, 100f); }

        public bool Spend(int amount)
        {
            if (amount <= 0) return true;
            if (Cash < amount) return false;
            Cash -= amount;
            return true;
        }

        public void Earn(int amount) { Cash = Math.Max(0, Cash + amount); }

        public void Tick(float dt, bool sprinting, bool moving)
        {
            if (sprinting && moving) Stamina = M.Clamp(Stamina - dt * 14f, 0f, 100f);
            else Stamina = M.Clamp(Stamina + dt * (moving ? 9f : 16f), 0f, 100f);
        }

        public void Revive()
        {
            Health = 60f;
            Armor = 0f;
            Stamina = 100f;
        }
    }

    // ---------------------------------------------------------------- weapons

    public sealed class WeaponState
    {
        public WeaponDef Def;
        public int Magazine;
        public int Reserve;
        public int Owned;
        float _cooldown;

        public WeaponState(WeaponDef def, int owned, int reserve)
        {
            Def = def;
            Owned = owned;
            Magazine = owned > 0 ? def.Magazine : 0;
            Reserve = reserve;
        }

        public bool Melee { get { return Def.Class == WeaponClass.Melee; } }
        public bool Empty { get { return !Melee && Magazine <= 0; } }
        public float Cooldown { get { return _cooldown; } }

        public bool CanFire()
        {
            if (!Melee && Magazine <= 0) return false;
            return _cooldown <= 0f;
        }

        /// <summary>Consumes one round and starts the cooldown. Returns false when it cannot fire.</summary>
        public bool TryFire()
        {
            if (!CanFire()) return false;
            if (!Melee) Magazine--;
            _cooldown = Def.SecondsBetweenShots;
            return true;
        }

        public void Tick(float dt) { if (_cooldown > 0f) _cooldown = Math.Max(0f, _cooldown - dt); }

        public bool Reload()
        {
            if (Melee) return false;
            int need = Def.Magazine - Magazine;
            if (need <= 0 || Reserve <= 0) return false;
            int take = Math.Min(need, Reserve);
            Magazine += take;
            Reserve -= take;
            _cooldown = Math.Max(_cooldown, 1.35f);
            return true;
        }

        public bool AddReserve(int rounds)
        {
            if (rounds <= 0 || Melee) return false;
            Reserve = Math.Min(Def.Reserve * 3, Reserve + rounds);
            return true;
        }

        public void Refill()
        {
            Magazine = Def.Magazine;
            Reserve = Def.Reserve;
        }
    }

    // ----------------------------------------------------------------- wanted

    public sealed class WantedSystem
    {
        public const float StarStep = 100f;
        public float Heat;

        public int Stars
        {
            get
            {
                if (Heat <= 0.5f) return 0;
                return M.Clamp((int)(Heat / StarStep) + 1, 1, 5);
            }
        }

        public bool Wanted { get { return Stars > 0; } }

        public void Crime(float heat)
        {
            Heat = M.Clamp(Heat + heat, 0f, StarStep * 5f - 1f);
        }

        /// <summary>When no officer can see the player the heat bleeds away, faster at low stars.</summary>
        public void Tick(float dt, bool seen)
        {
            if (seen) return;
            if (Heat <= 0f) { Heat = 0f; return; }
            float rate = 6f - Stars;                 // 5/s at one star, 1/s at five
            Heat = Math.Max(0f, Heat - rate * dt);
        }

        public void Clear() { Heat = 0f; }

        public float PoliceAggression { get { return 0.35f + Stars * 0.22f; } }
    }

    // ---------------------------------------------------------------- missions

    public enum MissionStatus { Inactive, Active, Passed, Failed }
    public enum MissionEvent { None, StepStarted, StepDone, Passed, Failed }

    public sealed class MissionState
    {
        public MissionDef Def;
        public MissionStatus Status = MissionStatus.Inactive;
        public int StepIndex;
        public float Timer;
        public int Kills;
        public string FailKey = "";
        public int ElapsedSteps { get { return StepIndex; } }

        public MissionStep Step
        {
            get
            {
                if (Def == null || Def.Steps == null || Def.Steps.Length == 0) return null;
                if (StepIndex < 0 || StepIndex >= Def.Steps.Length) return null;
                return Def.Steps[StepIndex];
            }
        }

        public bool Active { get { return Status == MissionStatus.Active; } }

        public MissionEvent Start(MissionDef def)
        {
            Def = def;
            Status = MissionStatus.Active;
            StepIndex = 0;
            Timer = 0f;
            Kills = 0;
            FailKey = "";
            return MissionEvent.StepStarted;
        }

        public void Fail(string key)
        {
            if (Status != MissionStatus.Active) return;
            Status = MissionStatus.Failed;
            FailKey = key;
        }

        /// <summary>Advance to the next step or finish the mission.</summary>
        public MissionEvent Advance()
        {
            if (Status != MissionStatus.Active) return MissionEvent.None;
            StepIndex++;
            Timer = 0f;
            Kills = 0;
            if (StepIndex >= Def.Steps.Length)
            {
                Status = MissionStatus.Passed;
                return MissionEvent.Passed;
            }
            return MissionEvent.StepDone;
        }

        public MissionEvent Tick(float dt, float px, float pz, bool inVehicle, float vehicleHealth, bool seenByPolice)
        {
            if (Status != MissionStatus.Active) return MissionEvent.None;
            MissionStep s = Step;
            if (s == null) return Advance();
            Timer += dt;
            float dist = M.Dist2D(px, pz, s.X, s.Z);

            switch (s.Kind)
            {
                case StepKind.GoTo:
                    if (dist <= s.Radius) return Advance();
                    break;
                case StepKind.Talk:
                    if (dist <= s.Radius) return Advance();
                    break;
                case StepKind.Deliver:
                    if (inVehicle && vehicleHealth <= 0f) { Fail("step.fail.car"); return MissionEvent.Failed; }
                    if (inVehicle && dist <= s.Radius) return Advance();
                    break;
                case StepKind.Kill:
                    if (Kills >= s.Count) return Advance();
                    break;
                case StepKind.Survive:
                    if (Timer >= s.Seconds) return Advance();
                    break;
                case StepKind.Escape:
                    if (!seenByPolice && Timer >= s.Seconds) return Advance();
                    if (seenByPolice) Timer = 0f;
                    break;
            }
            return MissionEvent.None;
        }

        /// <summary>Called by the combat system for every enemy killed while a mission is active.</summary>
        public MissionEvent ReportKill()
        {
            if (Status != MissionStatus.Active) return MissionEvent.None;
            MissionStep s = Step;
            if (s == null) return MissionEvent.None;
            if (s.Kind != StepKind.Kill) return MissionEvent.None;
            Kills++;
            if (Kills >= s.Count) return Advance();
            return MissionEvent.None;
        }

        public float Progress
        {
            get
            {
                if (Def == null || Def.Steps == null || Def.Steps.Length == 0) return 0f;
                MissionStep s = Step;
                float within = 0f;
                if (s != null)
                {
                    if (s.Kind == StepKind.Kill) within = s.Count <= 0 ? 1f : M.Clamp01(Kills / (float)s.Count);
                    else if (s.Kind == StepKind.Survive) within = M.Clamp01(Timer / Math.Max(0.01f, s.Seconds));
                    else if (s.Kind == StepKind.Escape) within = M.Clamp01(Timer / Math.Max(0.01f, s.Seconds));
                }
                return (StepIndex + within) / Def.Steps.Length;
            }
        }
    }
}
