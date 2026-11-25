using System;
using System.Collections.Generic;
using GTA6.Core;

namespace GTA6.Tests
{
    /// <summary>
    /// Tests for the pure rules layer. Compiled and executed with Unity's bundled
    /// dotnet runtime by tools/run-core-tests.sh, so they run without a Unity licence.
    /// </summary>
    public static class CoreTests
    {
        static int _passed, _failed;
        static readonly List<string> _failures = new List<string>();

        static void Check(bool condition, string what)
        {
            if (condition) { _passed++; return; }
            _failed++;
            _failures.Add(what);
            Console.WriteLine("  FAIL  " + what);
        }

        static void Eq<T>(T expected, T actual, string what)
        {
            bool ok = EqualityComparer<T>.Default.Equals(expected, actual);
            Check(ok, what + " (expected " + expected + ", got " + actual + ")");
        }

        static void Near(float expected, float actual, float eps, string what)
        {
            Check(Math.Abs(expected - actual) <= eps, what + " (expected " + expected + ", got " + actual + ")");
        }

        public static int Main(string[] args)
        {
            Console.WriteLine("=== GTA VI: Leonida — core rules tests ===");
            Run("Loc", TestLoc);
            Run("GameData", TestGameData);
            Run("Settings", TestSettings);
            Run("SaveData", TestSave);
            Run("PlayerStats", TestPlayerStats);
            Run("WeaponState", TestWeapons);
            Run("Wanted", TestWanted);
            Run("Rng", TestRng);
            Run("Missions", TestMissions);
            Run("Mission failures", TestMissionFailures);

            Console.WriteLine();
            Console.WriteLine("passed: " + _passed + "   failed: " + _failed);
            if (_failed > 0)
            {
                Console.WriteLine("failing checks:");
                foreach (string f in _failures) Console.WriteLine("  - " + f);
                return 1;
            }
            Console.WriteLine("ALL TESTS PASSED");
            return 0;
        }

        static void Run(string name, Action test)
        {
            int before = _failed;
            try { test(); }
            catch (Exception e)
            {
                _failed++;
                _failures.Add(name + " threw " + e.GetType().Name + ": " + e.Message);
                Console.WriteLine("  EXCEPTION in " + name + ": " + e);
            }
            Console.WriteLine((_failed == before ? "ok    " : "FAILED") + " " + name);
        }

        // ---------------------------------------------------------------- tests

        static void TestLoc()
        {
            Check(Loc.KeyCount > 160, "at least 160 localised keys, got " + Loc.KeyCount);
            foreach (Lang lang in new[] { Lang.Ru, Lang.En })
            {
                Loc.SetLang(lang);
                string[] keys =
                {
                    "menu.newgame", "menu.settings", "hud.health", "hud.busted", "app.map", "app.settings",
                    "mis.finale.title", "step.escape", "set.language", "phone.title", "hud.mission.pass",
                };
                foreach (string k in keys)
                {
                    string v = Loc.S(k);
                    Check(!string.IsNullOrEmpty(v), lang + " value for " + k);
                    Check(v != k, lang + " translation exists for " + k);
                }
            }
            Loc.SetLang(Lang.Ru);
            Eq("Нет ключа", Loc.S("Нет ключа"), "unknown key falls back to itself");
            Eq("1 250", Loc.Num(1250), "ru number grouping");
            Loc.SetLang(Lang.En);
            Eq("1,250", Loc.Num(1250), "en number grouping");
            Eq("3:07", Loc.Clock(187f), "clock formatting");
            Eq("07:05", Loc.Hour(7.0833f), "hour formatting");
            Eq("23:00", Loc.Hour(23f), "hour formatting at night");
            Eq("00:30", Loc.Hour(24.5f), "hour wraps past midnight");
            Eq("Mon", Loc.DayName(0), "day name");
            Loc.SetLang(Lang.Ru);
            Eq("Пн", Loc.DayName(0), "russian day name");
        }

        static void TestGameData()
        {
            Eq(12, GameData.Districts.Length, "district count");
            Eq(5, GameData.Weapons.Length, "weapon count");
            Eq(9, GameData.Vehicles.Length, "vehicle count");
            Eq(16, GameData.Apps.Length, "phone app count");
            Eq(22, GameData.Contacts.Length, "contact count");
            Eq(5, GameData.Missions.Length, "mission count");
            Eq(5, GameData.StoryOrder.Length, "story order length");

            HashSet<string> ids = new HashSet<string>();
            foreach (District d in GameData.Districts)
            {
                Check(ids.Add(d.Id), "unique district id " + d.Id);
                Check(!string.IsNullOrEmpty(d.NameRu) && !string.IsNullOrEmpty(d.NameEn), "district names " + d.Id);
                Check(d.Danger >= 1 && d.Danger <= 5, "district danger range " + d.Id);
                Check(d.X > GameData.CityMin && d.X < GameData.OceanX, "district inside land " + d.Id);
                Check(d.Z > GameData.CityMin && d.Z < GameData.CityMax, "district inside map " + d.Id);
                Check(d.Streets != null && d.Streets.Length >= 3, "street names " + d.Id);
            }

            foreach (string mid in GameData.StoryOrder)
            {
                MissionDef def = GameData.Mission(mid);
                Eq(mid, def.Id, "story order resolves " + mid);
                Check(def.Steps != null && def.Steps.Length >= 3, "mission has steps " + mid);
                Check(def.Reward > 0, "mission reward " + mid);
                foreach (MissionStep s in def.Steps)
                {
                    Check(s.X > GameData.CityMin && s.X < GameData.OceanX, mid + " step x on land");
                    Check(s.Z > GameData.CityMin && s.Z < GameData.CityMax, mid + " step z on map");
                    Check(s.Count > 0, mid + " step count");
                    Check(!string.IsNullOrEmpty(s.TextRu) && !string.IsNullOrEmpty(s.TextEn), mid + " step text");
                }
            }

            foreach (Contact c in GameData.Contacts)
            {
                Check(!string.IsNullOrEmpty(c.Name), "contact name " + c.Id);
                Check(!string.IsNullOrEmpty(c.Number), "contact number " + c.Id);
            }
            Check(GameData.Contacts[0].Messages.Length > 0, "donny has messages");

            foreach (PhoneApp a in GameData.Apps)
            {
                Check(!string.IsNullOrEmpty(a.Title), "app title " + a.Id);
                Check(a.Glyph.Length >= 1 && a.Glyph.Length <= 4, "app glyph " + a.Id);
                Check(a.ColorHex.Length == 6, "app colour " + a.Id);
            }

            foreach (VehicleDef v in GameData.Vehicles)
            {
                Check(v.TopSpeed > 10f && v.TopSpeed < 100f, "vehicle top speed " + v.Id);
                Check(v.Seats >= 1, "vehicle seats " + v.Id);
                Check(v.Mass > 100f, "vehicle mass " + v.Id);
            }

            foreach (WeaponDef w in GameData.Weapons)
            {
                Check(w.RoundsPerMinute > 0, "weapon rpm " + w.Id);
                if (w.Class != WeaponClass.Melee) Check(w.Magazine > 0 && w.Reserve > 0, "weapon ammo " + w.Id);
            }

            Eq("downtown", GameData.DistrictAt(0, 250).Id, "district lookup at downtown");
            Eq("vice_beach", GameData.DistrictAt(700, 700).Id, "district lookup at the beach");
            Eq("havana", GameData.DistrictAt(-470, 180).Id, "district lookup in little havana");
            Check(GameData.DistrictAt(0, 0) != null, "district lookup at the centre");
            Eq(16f, GameData.RoadWidth, "road width unchanged");
            Eq(9, GameData.Vehicles.Length, "all vehicle classes present");
            Check(GameData.Weapon("nope").Id == "fists", "unknown weapon falls back to fists");
            Check(GameData.Vehicle("nope").Id == "sedan", "unknown vehicle falls back to sedan");
        }

        static void TestSettings()
        {
            Settings s = new Settings();
            s.Quality = 3; s.SoundVolume = 0.25f; s.MusicVolume = 0.7f; s.Sensitivity = 1.75f;
            s.InvertY = true; s.Shadows = false; s.ShowFps = true; s.AutoAim = false;
            s.Vibration = false; s.Language = 1;
            Settings back = Settings.FromText(s.ToText());
            Eq(3, back.Quality, "settings quality round-trip");
            Near(0.25f, back.SoundVolume, 0.001f, "settings sound round-trip");
            Check(back.InvertY, "settings invert round-trip");
            Check(!back.Shadows, "settings shadows round-trip");
            Check(back.ShowFps, "settings fps round-trip");
            Check(!back.AutoAim, "settings autoaim round-trip");
            Eq(1, back.Language, "settings language round-trip");

            Settings junk = Settings.FromText("quality=99\nsound=abc\nsens=-5\nlang=7\ngarbage line");
            Eq(3, junk.Quality, "settings clamp quality");
            Near(0.85f, junk.SoundVolume, 0.001f, "settings ignore bad float");
            Near(0.2f, junk.Sensitivity, 0.001f, "settings clamp sensitivity");
            Eq(1, Settings.FromText("lang=1").Language, "settings language 1");

            SaveData d = new SaveData();
            d.SetAmmo("pistol", 42);
            SaveData copy = SaveData.FromText(d.ToText());
            Eq(42, copy.AmmoOf("pistol"), "save ammo preserved");
        }

        static void TestSave()
        {
            SaveData d = new SaveData();
            d.Cash = 12345;
            d.Health = 61.5f;
            d.Armor = 20f;
            d.PosX = -12.25f;
            d.PosZ = 998.5f;
            d.Heading = 271.5f;
            d.TimeOfDay = 18.25f;
            d.Day = 4;
            d.StoryIndex = 2;
            d.CurrentMission = "debt";
            d.CurrentStep = 1;
            d.WantedHeat = 180f;
            d.OwnedWeapons.Add("smg");
            d.OwnedWeapons.Add("shotgun");
            d.SetAmmo("smg", 120);
            d.SetAmmo("shotgun", 18);
            d.ShotsFired = 999;
            d.Photos = 3;

            SaveData b = SaveData.FromText(d.ToText());
            Eq(12345, b.Cash, "save cash");
            Near(61.5f, b.Health, 0.01f, "save health");
            Near(20f, b.Armor, 0.01f, "save armor");
            Near(-12.25f, b.PosX, 0.01f, "save x");
            Near(998.5f, b.PosZ, 0.01f, "save z");
            Near(18.25f, b.TimeOfDay, 0.01f, "save time");
            Eq(4, b.Day, "save day");
            Eq(2, b.StoryIndex, "save story index");
            Eq("debt", b.CurrentMission, "save mission");
            Eq(1, b.CurrentStep, "save step");
            Near(180f, b.WantedHeat, 0.01f, "save heat");
            Eq(4, b.OwnedWeapons.Count, "save weapon count");
            Check(b.HasWeapon("smg") && b.HasWeapon("shotgun") && b.HasWeapon("pistol"), "save weapons");
            Eq(120, b.AmmoOf("smg"), "save smg ammo");
            Eq(18, b.AmmoOf("shotgun"), "save shotgun ammo");
            Eq(999, b.ShotsFired, "save shots");
            Eq(3, b.Photos, "save photos");
            Eq("debt", b.NextMissionId(), "next mission id");

            SaveData empty = SaveData.FromText("");
            Eq(500, empty.Cash, "empty save defaults");
            Eq("prologue", empty.NextMissionId(), "empty save starts the story");

            SaveData done = SaveData.FromText("story=5\n");
            Eq("", done.NextMissionId(), "story complete has no next mission");

            SaveData clamped = SaveData.FromText("health=5000\nheat=9999\nx=0\n");
            Check(clamped.Health <= 100f, "save clamps health");
            Check(clamped.WantedHeat <= 600f, "save clamps heat");
        }

        static void TestPlayerStats()
        {
            PlayerStats p = new PlayerStats();
            Eq(100f, p.Health, "starts at full health");
            p.Damage(30f);
            Near(70f, p.Health, 0.01f, "damage applies fully without armour");
            p.AddArmor(50f);
            p.Damage(30f);
            Near(30.2f, p.Armor, 0.01f, "armour absorbs two thirds");
            Near(59.8f, p.Health, 0.01f, "only a third goes through");
            p.Damage(1000f);
            Check(p.Dead, "lethal damage kills");
            Check(p.Armor < 0.001f, "armour is destroyed");
            p.Revive();
            Check(p.Health > 0f && p.Health < 100f, "revive at reduced health");

            p.Cash = 100;
            Check(p.Spend(60), "spend within budget");
            Eq(40, p.Cash, "cash after spending");
            Check(!p.Spend(1000), "cannot overspend");
            Eq(40, p.Cash, "cash unchanged after a refused purchase");
            p.Earn(10);
            Eq(50, p.Cash, "cash earned");

            p.Stamina = 100f;
            for (int i = 0; i < 100; i++) p.Tick(0.1f, true, true);
            Check(p.Stamina < 100f, "sprinting drains stamina");
            for (int i = 0; i < 200; i++) p.Tick(0.1f, false, false);
            Near(100f, p.Stamina, 0.01f, "stamina recovers");
        }

        static void TestWeapons()
        {
            WeaponState pistol = new WeaponState(GameData.Weapon("pistol"), 1, 60);
            Eq(12, pistol.Magazine, "magazine starts full");
            Check(pistol.CanFire(), "can fire when loaded");
            Check(pistol.TryFire(), "first shot fires");
            Eq(11, pistol.Magazine, "round consumed");
            Check(!pistol.TryFire(), "cooldown blocks the second shot");
            pistol.Tick(1f);
            Check(pistol.TryFire(), "fires again after cooldown");

            for (int i = 0; i < 20; i++) { pistol.Tick(1f); pistol.TryFire(); }
            Eq(0, pistol.Magazine, "magazine empties");
            Check(!pistol.CanFire(), "empty gun cannot fire");
            Check(pistol.Reload(), "reload works");
            Eq(12, pistol.Magazine, "reload refills the magazine");
            Check(pistol.Reserve < 60, "reload takes from the reserve");

            WeaponState smg = new WeaponState(GameData.Weapon("smg"), 1, 0);
            Check(smg.AddReserve(60), "reserve can be topped up");
            Eq(60, smg.Reserve, "reserve topped up");

            WeaponState fists = new WeaponState(GameData.Weapon("fists"), 1, 0);
            Check(fists.Melee && fists.CanFire(), "fists always ready");
            Check(fists.TryFire(), "fists punch");
            Check(!fists.Reload(), "fists cannot reload");

            WeaponState shotgun = new WeaponState(GameData.Weapon("shotgun"), 1, 36);
            shotgun.Refill();
            Eq(6, shotgun.Magazine, "shotgun refill");
            Eq(36, shotgun.Reserve, "shotgun reserve refill");
            Check(GameData.Weapon("shotgun").PelletCount == 8, "shotgun fires eight pellets");
        }

        static void TestWanted()
        {
            WantedSystem w = new WantedSystem();
            Eq(0, w.Stars, "no stars at rest");
            w.Crime(30f);
            Eq(1, w.Stars, "one star after a crime");
            w.Crime(320f);
            Eq(4, w.Stars, "four stars");
            w.Crime(10000f);
            Eq(5, w.Stars, "heat is capped at five stars");
            for (int i = 0; i < 100; i++) w.Tick(1f, true);
            Eq(5, w.Stars, "heat does not fall while seen");
            for (int i = 0; i < 2000; i++) w.Tick(1f, false);
            Eq(0, w.Stars, "heat bleeds away when unseen");
            Near(0f, w.Heat, 0.01f, "heat reaches zero");
            w.Crime(120f);
            Check(w.Wanted, "wanted again");
            w.Clear();
            Check(!w.Wanted, "cleared");
            Check(w.PoliceAggression > 0.3f, "police aggression scales");
        }

        static void TestRng()
        {
            Rng a = new Rng(1234), b = new Rng(1234), c = new Rng(999);
            bool same = true;
            for (int i = 0; i < 100; i++) if (a.NextUInt() != b.NextUInt()) same = false;
            Check(same, "same seed gives the same sequence");
            Check(c.NextUInt() != new Rng(1234).NextUInt(), "different seed differs");
            Rng r = new Rng(7);
            int min = 99, max = -1;
            for (int i = 0; i < 2000; i++)
            {
                int v = r.Range(0, 12);
                if (v < min) min = v;
                if (v > max) max = v;
            }
            Eq(0, min, "range reaches its lower bound");
            Eq(11, max, "range reaches its upper bound");
            for (int i = 0; i < 100; i++) Check(r.Value >= 0f && r.Value < 1f, "value in 0..1");
        }

        static void TestMissions()
        {
            foreach (string id in GameData.StoryOrder)
            {
                MissionDef def = GameData.Mission(id);
                MissionState ms = new MissionState();
                Eq(MissionStatus.Inactive, ms.Status, id + ": starts inactive");
                ms.Start(def);
                Eq(MissionStatus.Active, ms.Status, id + ": active after start");
                Eq(0, ms.StepIndex, id + ": starts at the first step");

                int guard = 0;
                while (ms.Status == MissionStatus.Active && guard++ < 40000)
                {
                    MissionStep s = ms.Step;
                    float px = s.X, pz = s.Z;
                    bool inCar = s.Kind == StepKind.Deliver;
                    if (s.Kind == StepKind.Kill)
                    {
                        int needed = s.Count;
                        for (int k = 0; k < needed; k++) ms.ReportKill();
                    }
                    MissionEvent e = ms.Tick(0.25f, px, pz, inCar, 100f, false);
                    if (e == MissionEvent.Failed) break;
                }
                Eq(MissionStatus.Passed, ms.Status, id + ": can be completed");
                Near(1f, ms.Progress, 0.001f, id + ": progress reaches 100%");
                Check(guard < 40000, id + ": completes in finite time");
            }

            MissionState s2 = new MissionState();
            s2.Start(GameData.Mission("prologue"));
            Eq(0f, s2.Progress, "progress starts at zero");
            MissionStep first = s2.Step;
            s2.Tick(0.1f, first.X + 500f, first.Z, false, 100f, false);
            Eq(0, s2.StepIndex, "staying away does not advance");
            s2.Tick(0.1f, first.X, first.Z, false, 100f, false);
            Eq(1, s2.StepIndex, "arriving advances");
            Eq(MissionEvent.None, s2.ReportKill(), "kills outside a kill step are ignored");
        }

        static void TestMissionFailures()
        {
            // Delivering a wrecked car fails the ride mission.
            MissionState ride = new MissionState();
            ride.Start(GameData.Mission("ride"));
            MissionStep s = ride.Step;
            ride.Tick(0.1f, s.X, s.Z, true, 100f, false);      // reach the car
            Eq(1, ride.StepIndex, "ride: reached the second step");
            ride.Tick(0.1f, 0f, 0f, true, 0f, false);
            Eq(MissionStatus.Failed, ride.Status, "ride: wrecked car fails the mission");
            Eq("step.fail.car", ride.FailKey, "ride: failure reports the car");

            // Escape timers reset whenever the police can see the player.
            MissionState cash = new MissionState();
            cash.Start(GameData.Mission("cash"));
            MissionStep cashStep = cash.Step;
            cash.Tick(0.1f, cashStep.X, cashStep.Z, true, 100f, false);
            while (cash.Step.Kind != StepKind.Escape) cash.Advance();
            int escapeIndex = cash.StepIndex;
            float need = cash.Step.Seconds;
            for (int i = 0; i < (int)(need * 4f) + 4; i++) cash.Tick(0.25f, 0f, 0f, false, 100f, true);
            Eq(escapeIndex, cash.StepIndex, "escape: staying seen keeps the step active");
            for (int i = 0; i < (int)(need * 4f) + 20; i++) cash.Tick(0.25f, 0f, 0f, false, 100f, false);
            Check(cash.StepIndex > escapeIndex || cash.Status == MissionStatus.Passed,
                "escape: completing it while unseen advances");

            // Failing twice is idempotent.
            MissionState f = new MissionState();
            f.Start(GameData.Mission("debt"));
            f.Fail("step.fail.dead");
            MissionEvent after = f.Tick(0.1f, 0f, 0f, false, 0f, false);
            Eq(MissionEvent.None, after, "failed mission stops ticking");
            Eq(MissionStatus.Failed, f.Status, "mission stays failed");
        }
    }
}
