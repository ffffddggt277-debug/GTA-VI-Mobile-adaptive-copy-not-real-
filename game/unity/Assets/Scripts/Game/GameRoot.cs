using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GTA6.Core;

namespace GTA6.Game
{
    public enum Phase
    {
        Loading, MainMenu, Playing, Paused, Dead, Busted,
        MissionPassed, MissionFailed, Settings, Controls, About
    }

    public sealed class GameRoot : MonoBehaviour
    {
        public static GameRoot I;

        public const string SaveKey = "gta6_leonida_save_v1";
        public const string SettingsKey = "gta6_leonida_settings_v1";
        public const int Seed = 20261125;

        public Settings Settings = new Settings();
        public SaveData Save = new SaveData();
        public Phase Phase = Phase.Loading;
        public float LoadProgress;

        public World World;
        public Player Player;
        public CamRig Cam;
        public Traffic Traffic = new Traffic();
        public PoliceForce Police = new PoliceForce();
        public WantedSystem Wanted = new WantedSystem();
        public MissionState Mission = new MissionState();
        public InputState Input = new InputState();
        public Hud Hud = new Hud();
        public PhoneUI Phone = new PhoneUI();
        public MapScreen Map = new MapScreen();
        public MenuUI Menu = new MenuUI();
        public AudioDirector Audio;

        public float Clock = 9f;
        public int DayNumber = 1;
        public float PlayTime;
        public float FrameDelta = 0.016f;
        public float WeatherRain;
        public float WeatherTarget;
        public float SubtitleTimer;
        public string Subtitle;
        public string FailureKey = "";
        public int LastReward;
        public bool CanStartMission;
        public bool MissionHasTarget;
        public Vector3 MissionTarget;
        public bool HasWaypoint;
        public Vector3 Waypoint;
        public Vehicle NearbyVehicle;
        public readonly List<Notice> Notifications = new List<Notice>();

        Transform _actorsRoot, _carsRoot, _fxRoot;
        Light _sun;
        Material _sky;
        Phase _overlayReturn = Phase.MainMenu;
        float _autosave;
        float _arrestTimer;
        float _weatherTimer = 40f;
        bool _environmentReady;

        public GTA6.Core.District District
        {
            get { return GameData.DistrictAt(Player.Pos.x, Player.Pos.z); }
        }

        public bool HasSave { get { return PlayerPrefs.HasKey(SaveKey); } }

        // ------------------------------------------------------------- lifecycle

        void Awake()
        {
            I = this;
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            LoadSettings();
            Loc.SetLang(Settings.Language == 1 ? Lang.En : Lang.Ru);
            MatLib.Init();
            Ui.Init();
            Audio = gameObject.AddComponent<AudioDirector>();
        }

        IEnumerator Start()
        {
            Phase = Phase.Loading;
            LoadProgress = 0.05f;
            yield return null;

            _fxRoot = Go.New("fx", transform).transform;
            Fx.Init(_fxRoot);
            _actorsRoot = Go.New("actors", transform).transform;
            _carsRoot = Go.New("cars", transform).transform;

            LoadProgress = 0.12f;
            yield return null;

            SetupSky();
            LoadProgress = 0.2f;
            yield return null;

            World = new World();
            World.Build(Seed, Settings);
            LoadProgress = 0.7f;
            yield return null;

            Cam = new CamRig();
            Cam.Init(transform);
            Cam.LookSensitivity = Settings.Sensitivity;

            Player = new Player();
            Player.Setup(_actorsRoot, Look.Player(), Save);
            Player.Pos = SafeSpawn(new Vector3(Save.PosX, 0f, Save.PosZ));
            Player.Sync();

            Traffic.Init(_carsRoot, World);
            Police.Init(_carsRoot);
            Shooting.OnKill = HandleKill;

            SeedTraffic();
            LoadProgress = 0.9f;
            yield return null;

            ApplySettings();
            UpdateEnvironment(0f);
            SnapCamera();
            LoadProgress = 1f;
            yield return null;

            Phase = Phase.MainMenu;
            Audio.SetVolumes(Settings.SoundVolume, Settings.MusicVolume);
        }

        void SeedTraffic()
        {
            Rng rng = new Rng(Seed + 5);
            for (int i = 0; i < 10; i++)
            {
                Vector3 road = World.RandomRoad(rng);
                if (Mathf.Abs(road.x - Player.Pos.x) + Mathf.Abs(road.z - Player.Pos.z) > 400f) continue;
                VehicleDef def = GameData.Vehicles[rng.Range(0, 5)];
                Vehicle v = Vehicle.Build(def, road, rng.Range(0f, 360f), _carsRoot, MatLib.Palette);
                v.DriveAi(0.016f, World, 0.35f);
                Traffic.Cars.Add(v);
            }
        }

        void SetupSky()
        {
            Shader skyShader = Shader.Find("Skybox/Procedural");
            if (skyShader != null)
            {
                _sky = new Material(skyShader);
                _sky.name = "LeonidaSky";
                RenderSettings.skybox = _sky;
            }
            GameObject sunGo = Go.New("Sun", transform);
            _sun = sunGo.AddComponent<Light>();
            _sun.type = LightType.Directional;
            _sun.shadows = LightShadows.Soft;
            _sun.shadowStrength = 0.72f;
            _sun.shadowBias = 0.04f;
            _sun.shadowNormalBias = 0.4f;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.fog = true;
            _environmentReady = true;
        }

        void OnDestroy()
        {
            if (Shooting.OnKill == HandleKill) Shooting.OnKill = null;
        }

        // ------------------------------------------------------------------ loop

        void Update()
        {
            FrameDelta = Mathf.Max(0.0001f, Time.unscaledDeltaTime);
            if (!_environmentReady)
            {
                Input.Update(false);
                return;
            }

            switch (Phase)
            {
                case Phase.MainMenu:
                case Phase.Settings:
                case Phase.Controls:
                case Phase.About:
                    Input.Update(false);
                    MenuCamera(FrameDelta);
                    break;

                case Phase.Playing:
                    Input.Update(!Phone.Open && !Map.Open);
                    if (Input.Pressed("pause")) { SetPhase(Phase.Paused); break; }
                    if (Input.Pressed("phone")) { if (Phone.Open) Phone.Close(); else Phone.Show(this); break; }
                    if (Input.Pressed("map")) { if (Map.Open) Map.Close(); else Map.Show(this); }
                    if (Phone.Open) { Phone.Update(this, FrameDelta); TickPassive(FrameDelta); break; }
                    if (Map.Open) { Map.Update(this, FrameDelta); TickPassive(FrameDelta); break; }
                    TickGameplay(FrameDelta);
                    break;

                case Phase.Paused:
                    Input.Update(true);
                    if (Input.Pressed("pause")) { SetPhase(Phase.Playing); break; }
                    break;

                case Phase.Dead:
                case Phase.Busted:
                    Input.Update(false);
                    TickPassive(FrameDelta);
                    break;

                case Phase.MissionPassed:
                case Phase.MissionFailed:
                    Input.Update(false);
                    break;
            }

            TickEffects(FrameDelta);
        }

        void TickPassive(float dt)
        {
            Fx.Update(dt);
            for (int i = Notifications.Count - 1; i >= 0; i--)
            {
                Notice n = Notifications[i];
                n.Life -= dt;
                Notifications[i] = n;
                if (n.Life <= 0f) Notifications.RemoveAt(i);
            }
            if (SubtitleTimer > 0f) SubtitleTimer = Mathf.Max(0f, SubtitleTimer - dt);
        }

        void TickEffects(float dt)
        {
            Fx.Update(dt);
            for (int i = Notifications.Count - 1; i >= 0; i--)
            {
                Notice n = Notifications[i];
                n.Life -= dt;
                Notifications[i] = n;
                if (n.Life <= 0f) Notifications.RemoveAt(i);
            }
            if (SubtitleTimer > 0f) SubtitleTimer = Mathf.Max(0f, SubtitleTimer - dt);
            if (Phase == Phase.Playing)
            {
                Audio.SetEngine(Player.Vehicle != null, Player.Vehicle != null ? Mathf.Clamp01(Player.Vehicle.SpeedKmh / 140f) : 0f);
                Audio.SetSiren(Police.Seen && Wanted.Stars > 0, 1f);
            }
            else Audio.SetEngine(false, 0f);
        }

        void TickGameplay(float dt)
        {
            PlayTime += dt;
            AdvanceClock(dt);
            TickWeather(dt);

            float driveSpeed = 0f;
            if (Player.Vehicle != null)
            {
                Player.Vehicle.Drive(dt, Input, World);
                driveSpeed = Mathf.Abs(Player.Vehicle.Speed);
            }
            Player.Tick(dt, Input, World, Cam, Settings);
            TickFootsteps();

            NearbyVehicle = Player.Vehicle != null ? null : Traffic.Nearest(Player.Pos, 5.2f);
            if (Input.Pressed("car")) HandleVehicleButton();

            TickCombat(dt);
            TickTraffic(dt);
            TickPeds(dt);
            UpdateMissionEnemies(dt);
            Police.Update(dt, World, Player, Wanted, Mission, Mission.Active);
            TickMission(dt);
            TickArrest(dt);
            CanStartMission = !Mission.Active && Save.StoryIndex < StoryLength && Save.StoryIndex >= 0;
            if (CanStartMission && Input.Pressed("mission")) StartMission(Save.NextMissionId());

            Cam.LookSensitivity = Settings.Sensitivity;
            Cam.InvertY = Settings.InvertY;
            Cam.Update(dt, Player, Input, World, Settings);
            UpdateEnvironment(dt);

            _autosave += dt;
            if (_autosave > 45f) { _autosave = 0f; SaveGame(); }

            if (Player.Stats.Dead && Phase == Phase.Playing) Die();
        }

        static int StoryLength { get { return GameData.StoryOrder.Length; } }

        void AdvanceClock(float dt)
        {
            Clock += dt * 0.05f;
            if (Clock >= 24f) { Clock -= 24f; DayNumber++; }
        }

        void TickWeather(float dt)
        {
            _weatherTimer -= dt;
            if (_weatherTimer <= 0f)
            {
                _weatherTimer = UnityEngine.Random.Range(110f, 260f);
                WeatherTarget = UnityEngine.Random.value < 0.35f ? UnityEngine.Random.Range(0.55f, 1f) : 0f;
            }
            WeatherRain = Mathf.MoveTowards(WeatherRain, WeatherTarget, dt * 0.08f);
            Audio.SetRain(WeatherRain, 0f);
        }

        void TickFootsteps()
        {
            if (Player.Vehicle != null) return;
            if (Player.MoveAmount < 0.6f) return;
            float rate = Player.MoveAmount > 4f ? 2.4f : 3.2f;
            _stepTimer -= FrameDelta;
            if (_stepTimer > 0f) return;
            _stepTimer = 1f / rate;
            Audio.PlayFootstep(Player.Pos, Player.MoveAmount > 4f);
        }

        float _stepTimer;

        void TickTraffic(float dt)
        {
            Traffic.Update(dt, Player.Pos, 12);
        }

        void TickPeds(float dt)
        {
            Rng rng = null;
            for (int i = Actors.Peds.Count - 1; i >= 0; i--)
            {
                Npc p = Actors.Peds[i];
                float d = Vector3.Distance(p.Pos, Player.Pos);
                if (p.Dead && p.DeathTimer > 30f) { p.View.Destroy(); Actors.Peds.RemoveAt(i); continue; }
                if (d > 120f) { p.View.Destroy(); Actors.Peds.RemoveAt(i); continue; }
                p.Update(dt, World, Player.Pos, rng);
            }

            _pedTimer -= dt;
            if (_pedTimer <= 0f && Actors.Peds.Count < 12)
            {
                _pedTimer = 1.6f;
                Vector3 spawn = World.NearestRoad(Player.Pos + new Vector3(UnityEngine.Random.Range(-90f, 90f), 0f,
                    UnityEngine.Random.Range(-90f, 90f)), 7.5f);
                spawn.y = 0f;
                if (spawn.x < World.BeachX - 10f && Vector3.Distance(spawn, Player.Pos) > 18f)
                {
                    Npc npc = new Npc();
                    Rng r = new Rng(UnityEngine.Random.Range(1, 999999));
                    npc.Setup(_actorsRoot, r, spawn, Look.Random(r));
                    Actors.Peds.Add(npc);
                }
            }
        }

        float _pedTimer = 2f;

        // --------------------------------------------------------------- combat

        void TickCombat(float dt)
        {
            WeaponState w = Player.Weapon;
            if (w == null) return;
            w.Tick(dt);
            if (Input.Pressed("weapon")) Player.NextWeapon();
            if (Input.Pressed("reload"))
            {
                if (w.Reload()) Notify(Loc.S("hud.ammo"), Ui.TextDimColor);
            }
            if (!Input.FireHeld || Phone.Open || Map.Open) return;
            if (Player.Vehicle != null) return;
            if (!w.CanFire()) return;

            Vector3 origin = Player.Chest + Vector3.up * 0.08f + Cam.FlatForward * 0.4f;
            Vector3 aim = Cam.AimPoint;
            Shooting.FireWeapon(Player, World, origin, aim, w, Settings.AutoAim);
            Cam.AddShake(w.Def.Recoil * 0.05f);
            Save.ShotsFired++;
            if (Wanted.Stars == 0 && Police.OfficerCount > 0) Crime(25f, "crime.shot");
        }

        void HandleKill(bool cop)
        {
            Crime(cop ? 130f : 105f, cop ? "crime.kill" : "crime.kill");
            MissionEvent e = Mission.ReportKill();
            HandleMissionEvent(e);
        }

        // -------------------------------------------------------------- missions

        void TickMission(float dt)
        {
            if (!Mission.Active) { MissionHasTarget = false; return; }
            MissionStep step = Mission.Step;
            if (step == null) { MissionHasTarget = false; return; }
            MissionHasTarget = true;
            MissionTarget = new Vector3(step.X, 0f, step.Z);

            float carHealth = Player.Vehicle != null ? Player.Vehicle.Health : 100f;
            MissionEvent e = Mission.Tick(dt, Player.Pos.x, Player.Pos.z, Player.Vehicle != null, carHealth, Police.Seen);
            HandleMissionEvent(e);
        }

        void HandleMissionEvent(MissionEvent e)
        {
            switch (e)
            {
                case MissionEvent.StepDone:
                    MissionStep next = Mission.Step;
                    if (next != null)
                    {
                        Say(next.Text, 4f);
                        SpawnEnemiesFor(next);
                    }
                    break;
                case MissionEvent.Passed:
                    CompleteMission();
                    break;
                case MissionEvent.Failed:
                    FailMission(Mission.FailKey);
                    break;
            }
        }

        public void StartMission(string id)
        {
            if (string.IsNullOrEmpty(id) || Mission.Active) return;
            MissionDef def = GameData.Mission(id);
            Mission.Start(def);
            Save.CurrentMission = def.Id;
            Save.CurrentStep = 0;
            MissionStep first = Mission.Step;
            if (first != null) { MissionTarget = new Vector3(first.X, 0f, first.Z); MissionHasTarget = true; SpawnEnemiesFor(first); }
            Say(def.Desc, 5f);
            Notify(def.Title, Ui.AccentColor);
            if (Phase == Phase.Paused) Phase = Phase.Playing;
        }

        readonly List<Cop> _missionEnemies = new List<Cop>();

        /// <summary>Spawns the enemies a mission step asks for. They are not police.</summary>
        void SpawnEnemiesFor(MissionStep step)
        {
            if (step == null || step.Kind != StepKind.Kill) return;
            ClearMissionEnemies();
            Rng rng = new Rng(Mathf.RoundToInt(step.X * 13f + step.Z * 7f) + 31);
            for (int i = 0; i < step.Count; i++)
            {
                float angle = i * Mathf.PI * 2f / Mathf.Max(1, step.Count);
                Vector3 p = SafeSpawn(new Vector3(step.X + Mathf.Cos(angle) * 11f, 0f, step.Z + Mathf.Sin(angle) * 11f));
                Cop thug = new Cop();
                thug.Setup(_actorsRoot, p, Look.Random(rng));
                thug.Criminal = true;
                thug.Health = 65f;
                Actors.Thugs.Add(thug);
                _missionEnemies.Add(thug);
            }
        }

        void ClearMissionEnemies()
        {
            for (int i = 0; i < _missionEnemies.Count; i++)
            {
                Cop c = _missionEnemies[i];
                Actors.Thugs.Remove(c);
                if (c.View != null) c.View.Destroy();
            }
            _missionEnemies.Clear();
        }

        void UpdateMissionEnemies(float dt)
        {
            for (int i = Actors.Thugs.Count - 1; i >= 0; i--)
            {
                Cop c = Actors.Thugs[i];
                if (c.Dead)
                {
                    c.DeathTimer += dt;
                    if (c.DeathTimer > 20f)
                    {
                        if (c.View != null) c.View.Destroy();
                        Actors.Thugs.RemoveAt(i);
                        _missionEnemies.Remove(c);
                    }
                    continue;
                }
                c.Update(dt, World, Player, 0.55f);
            }
        }

        void CompleteMission()
        {
            ClearMissionEnemies();
            LastReward = Mission.Def != null ? Mission.Def.Reward : 0;
            Player.Stats.Earn(LastReward);
            Save.StoryIndex = M.Clamp(Save.StoryIndex + 1, 0, StoryLength);
            Save.CurrentMission = "";
            Save.CurrentStep = 0;
            MissionHasTarget = false;
            FailureKey = "";
            SaveGame();
            Notify("+" + Loc.Money(LastReward), Ui.GoodColor);
            Audio.PlayCash();
            Phase = Phase.MissionPassed;
            Audio.SetEngine(false, 0f);
        }

        public void FailMission(string key)
        {
            ClearMissionEnemies();
            if (!Mission.Active && Phase != Phase.Playing) return;
            FailureKey = key;
            Mission.Fail(key);
            MissionHasTarget = false;
            Phase = Phase.MissionFailed;
        }

        public void ContinueAfterMission()
        {
            Mission = new MissionState();
            Phase = Phase.Playing;
            SnapCamera();
        }

        public void RetryMission()
        {
            string id = Save.NextMissionId();
            Mission = new MissionState();
            Phase = Phase.Playing;
            if (!string.IsNullOrEmpty(id)) StartMission(id);
            else SnapCamera();
        }

        // --------------------------------------------------------------- police

        void TickArrest(float dt)
        {
            if (Wanted.Stars == 0) { _arrestTimer = 0f; return; }
            bool close = false;
            for (int i = 0; i < Actors.Cops.Count; i++)
            {
                Cop c = Actors.Cops[i];
                if (c.Dead) continue;
                if (Vector3.Distance(c.Pos, Player.Pos) < 2.2f) { close = true; break; }
            }
            _arrestTimer = close ? _arrestTimer + dt : Mathf.Max(0f, _arrestTimer - dt * 2f);
            if (_arrestTimer > 1.4f) Busted();
        }

        void Busted()
        {
            _arrestTimer = 0f;
            Player.Stats.Spend(Mathf.Min(250, Player.Stats.Cash));
            Phase = Phase.Busted;
            if (Mission.Active) Mission.Fail("step.fail.busted");
            FailureKey = "step.fail.busted";
            Police.Clear();
            Audio.SetEngine(false, 0f);
        }

        void Die()
        {
            Phase = Phase.Dead;
            if (Mission.Active) Mission.Fail("step.fail.dead");
            FailureKey = "step.fail.dead";
            Audio.SetEngine(false, 0f);
        }

        public void RespawnPlayer()
        {
            Vector3 hospital = new Vector3(96f, 0f, 384f);
            if (Player.Vehicle != null) { Player.Vehicle.RemoveDriver(); Player.Vehicle = null; }
            Player.Respawn(SafeSpawn(hospital));
            Player.Stats.Spend(Mathf.Min(150, Player.Stats.Cash));
            Wanted.Clear();
            Police.Clear();
            Mission = new MissionState();
            MissionHasTarget = false;
            Phase = Phase.Playing;
            SnapCamera();
            Say(Loc.S("death.retry"), 3f);
        }

        public void Crime(float heat, string labelKey)
        {
            int before = Wanted.Stars;
            Wanted.Crime(heat);
            if (Wanted.Stars > before && Wanted.Stars > 0) Notify(Loc.S("hud.stars") + " " + Wanted.Stars, Ui.BadColor);
        }

        // -------------------------------------------------------------- vehicles

        void HandleVehicleButton()
        {
            if (Player.Vehicle != null)
            {
                Vehicle v = Player.Vehicle;
                v.RemoveDriver();
                Player.Vehicle = null;
                Vector3 side = Quaternion.Euler(0f, v.Heading, 0f) * new Vector3(v.Def.Width * 0.5f + 0.7f, 0f, 0f);
                Player.Pos = SafeSpawn(v.Pos + side);
                Player.Sync();
                Audio.SetEngine(false, 0f);
                return;
            }
            Vehicle target = NearbyVehicle;
            if (target == null || target.Destroyed) return;
            if (target.Def.Price > 0) Crime(65f, "crime.car");
            target.Occupied = true;
            Player.Vehicle = target;
            Player.View.SetActive(false);
            if (Police.Cruisers.Contains(target)) Police.Cruisers.Remove(target);
            Audio.SetEngine(true, 0f);
        }

        Vector3 SafeSpawn(Vector3 wanted)
        {
            Vector3 p = wanted;
            p.y = 0f;
            if (World != null)
            {
                World.ClampToLand(ref p, 4f);
                List<int> cells = new List<int>(16);
                World.Colliders.Query(p.x, p.z, 3f, cells);
                Vector3 normal;
                for (int i = 0; i < cells.Count; i++)
                    World.Colliders.Boxes[cells[i]].ResolveCircle(ref p, 1.2f, 1.8f, true, out normal);
            }
            return p;
        }

        // -------------------------------------------------------------- economy

        public bool BuyItem(string id)
        {
            GameData.ShopItem item = null;
            for (int i = 0; i < GameData.ShopItems.Length; i++)
                if (GameData.ShopItems[i].Id == id) item = GameData.ShopItems[i];
            if (item == null) return false;
            if (!Player.Stats.Spend(item.Price)) return false;
            switch (id)
            {
                case "medkit": Player.Stats.Heal(60f); break;
                case "armor": Player.Stats.AddArmor(60f); break;
                case "ammo_pistol": GiveAmmo("pistol", 48); break;
                case "ammo_smg": GiveAmmo("smg", 120); break;
                case "ammo_shotgun": GiveAmmo("shotgun", 24); break;
                case "ammo_rifle": GiveAmmo("rifle", 90); break;
                case "respray":
                    if (Player.Vehicle != null) { Player.Vehicle.Health = 100f; Wanted.Clear(); }
                    break;
            }
            Audio.PlayCash();
            return true;
        }

        void GiveAmmo(string weaponId, int rounds)
        {
            if (!Save.HasWeapon(weaponId) && !HasWeapon(weaponId))
            {
                Save.OwnedWeapons.Add(weaponId);
                Player.AddWeapon(weaponId, rounds);
            }
            for (int i = 0; i < Player.Weapons.Count; i++)
                if (Player.Weapons[i].Def.Id == weaponId) Player.Weapons[i].AddReserve(rounds);
        }

        bool HasWeapon(string id)
        {
            for (int i = 0; i < Player.Weapons.Count; i++) if (Player.Weapons[i].Def.Id == id) return true;
            return false;
        }

        public bool BuyVehicle(string id)
        {
            VehicleDef def = GameData.Vehicle(id);
            if (def.Price <= 0) return false;
            if (!Player.Stats.Spend(def.Price)) return false;
            Vector3 spawn = SafeSpawn(Player.Pos + Player.View.Root.transform.right * 4.5f);
            Vehicle v = Vehicle.Build(def, spawn, Player.Heading + 90f, _carsRoot, MatLib.Palette);
            Traffic.Cars.Add(v);
            Audio.PlayCash();
            return true;
        }

        // ----------------------------------------------------------------- clock

        void UpdateEnvironment(float dt)
        {
            float day = Mathf.Clamp01(Mathf.Sin((Clock - 5.6f) / 13.4f * Mathf.PI));
            float dusk = 1f - Mathf.Abs(Clock - 6.4f) / 2.2f;
            float dawn = 1f - Mathf.Abs(Clock - 19.2f) / 2.4f;
            float warm = Mathf.Clamp01(Mathf.Max(dusk, dawn));

            if (_sun != null)
            {
                _sun.transform.rotation = Quaternion.Euler(Clock / 24f * 360f - 90f, 38f, 0f);
                Color dayCol = Color.Lerp(new Color(1f, 0.62f, 0.34f), new Color(1f, 0.96f, 0.88f), 1f - warm);
                Color nightCol = new Color(0.42f, 0.48f, 0.72f);
                _sun.color = Color.Lerp(nightCol, dayCol, day);
                _sun.intensity = Mathf.Lerp(0.03f, 1.18f, day) * (1f - WeatherRain * 0.45f);
                _sun.shadows = Settings.Shadows && Settings.Quality > 0 ? LightShadows.Soft : LightShadows.None;
            }

            Color ambientDay = Color.Lerp(new Color(0.30f, 0.33f, 0.36f), new Color(0.46f, 0.47f, 0.47f), 1f - WeatherRain);
            Color ambientNight = new Color(0.07f, 0.09f, 0.15f);
            RenderSettings.ambientLight = Color.Lerp(ambientNight, ambientDay, day);

            Color fogDay = Color.Lerp(new Color(0.72f, 0.76f, 0.80f), new Color(0.52f, 0.56f, 0.60f), WeatherRain);
            Color fogNight = new Color(0.05f, 0.07f, 0.12f);
            RenderSettings.fogColor = Color.Lerp(fogNight, fogDay, day);
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = Mathf.Lerp(0.0042f, 0.0016f, day) + WeatherRain * 0.004f;

            if (_sky != null)
            {
                _sky.SetFloat("_AtmosphereThickness", Mathf.Lerp(0.55f, 1.35f, 1f - day) + WeatherRain * 0.4f);
                _sky.SetFloat("_Exposure", Mathf.Lerp(0.35f, 1.25f, day));
                _sky.SetFloat("_SunSize", 0.045f);
                _sky.SetColor("_SkyTint", Color.Lerp(new Color(0.18f, 0.24f, 0.42f), new Color(0.52f, 0.68f, 0.92f), day));
                _sky.SetColor("_GroundColor", Color.Lerp(new Color(0.02f, 0.03f, 0.05f), new Color(0.34f, 0.33f, 0.30f), day));
            }

            MatLib.SetEmission(1f - day + 0.15f);
            QualitySettings.shadowDistance = Settings.Quality >= 2 ? 160f : 90f;
        }

        void MenuCamera(float dt)
        {
            if (Cam == null) return;
            float t = Time.time * 0.06f;
            Vector3 target = new Vector3(700f + Mathf.Sin(t) * 60f, 24f, 640f + Mathf.Cos(t) * 80f);
            Cam.Cam.transform.position = Vector3.Lerp(Cam.Cam.transform.position, target, 1f - Mathf.Exp(-dt * 1.2f));
            Cam.Cam.transform.rotation = Quaternion.Slerp(Cam.Cam.transform.rotation,
                Quaternion.LookRotation(new Vector3(560f, 6f, 700f) - Cam.Cam.transform.position), 1f - Mathf.Exp(-dt * 1.5f));
            Cam.Cam.fieldOfView = Mathf.Lerp(Cam.Cam.fieldOfView, 58f, dt * 2f);
            UpdateEnvironment(dt);
        }

        void SnapCamera()
        {
            if (Cam == null || Player == null) return;
            Cam.Yaw = Player.Heading;
            Cam.Pitch = 14f;
            Cam.SnapTo(Player.Pos + Vector3.up * 1.45f);
            Cam.Cam.transform.position = Player.Pos + Quaternion.Euler(0f, Player.Heading, 0f) * new Vector3(0f, 2.2f, -5f);
        }

        // ------------------------------------------------------------ interface

        public void SetPhase(Phase p)
        {
            if (p == Phase.Settings || p == Phase.Controls || p == Phase.About)
            {
                if (Phase == Phase.Paused) _overlayReturn = Phase.Paused;
                else _overlayReturn = Phase.MainMenu;
            }
            if (Phase == Phase.Playing && p == Phase.Paused) SaveGame();
            Phase = p;
        }

        public void CloseOverlay() { Phase = _overlayReturn; }

        public void Resume() { Phase = Phase.Playing; SnapCamera(); }

        public void OpenMap() { Map.Show(this); }

        public void TogglePhone() { if (Phone.Open) Phone.Close(); else Phone.Show(this); }

        public void SetWaypoint(Vector3 p)
        {
            Waypoint = p;
            HasWaypoint = true;
            Notify("WAYPOINT", Ui.AccentColor);
        }

        public void Notify(string text) { Notify(text, Ui.TextColor); }

        public void Notify(string text, Color32 color)
        {
            if (string.IsNullOrEmpty(text)) return;
            if (Notifications.Count > 4) Notifications.RemoveAt(0);
            Notifications.Add(new Notice { Text = text, Life = 2.6f, Max = 2.6f, Color = color });
        }

        public void Say(string text, float seconds = 3.5f)
        {
            Subtitle = text;
            SubtitleTimer = seconds;
        }

        public void StartNewGame()
        {
            Save = new SaveData();
            Save.PosX = -330f;
            Save.PosZ = -1060f;
            ResetRuntime();
            SaveGame();
            Phase = Phase.Playing;
            Say(Loc.S("mis.prologue.desc"), 6f);
            Notify(Loc.S("game.title"), Ui.AccentColor);
        }

        public void ContinueGame()
        {
            if (!HasSave) { StartNewGame(); return; }
            Save = SaveData.FromText(PlayerPrefs.GetString(SaveKey, ""));
            ResetRuntime();
            Phase = Phase.Playing;
            string missionId = Save.CurrentMission;
            if (!string.IsNullOrEmpty(missionId)) StartMission(missionId);
        }

        void ResetRuntime()
        {
            Phone.Close();
            Map.Close();
            Mission = new MissionState();
            MissionHasTarget = false;
            ClearMissionEnemies();
            Wanted.Heat = Save.WantedHeat;
            Wanted.Clear();
            Clock = Mathf.Clamp(Save.TimeOfDay, 0f, 23.9f);
            DayNumber = Mathf.Max(1, Save.Day);
            PlayTime = Save.PlaySeconds;
            Police.Clear();

            Player.Setup(_actorsRoot, Look.Player(), Save);
            Player.Pos = SafeSpawn(new Vector3(Save.PosX, 0f, Save.PosZ));
            Player.Heading = Save.Heading;
            Player.Sync();
            NearbyVehicle = null;
            SnapCamera();
            ApplySettings();
            UpdateEnvironment(0f);
        }

        public void ApplySettings()
        {
            Loc.SetLang(Settings.Language == 1 ? Lang.En : Lang.Ru);
            Audio.SetVolumes(Settings.SoundVolume, Settings.MusicVolume);
            QualitySettings.shadows = Settings.Shadows ?
                (Settings.Quality >= 2 ? ShadowQuality.All : ShadowQuality.HardOnly) : ShadowQuality.Disable;
            QualitySettings.shadowResolution = Settings.Quality >= 2 ? ShadowResolution.High : ShadowResolution.Medium;
            QualitySettings.antiAliasing = Settings.Quality >= 3 ? 2 : 0;
            QualitySettings.pixelLightCount = Settings.Quality >= 2 ? 4 : 1;
            if (_sun != null)
                _sun.shadows = Settings.Shadows && Settings.Quality > 0 ? LightShadows.Soft : LightShadows.None;
            SaveSettings();
        }

        public void SaveSettings()
        {
            Settings.Language = (int)Loc.Current;
            PlayerPrefs.SetString(SettingsKey, Settings.ToText());
            PlayerPrefs.Save();
        }

        void LoadSettings()
        {
            Settings = Settings.FromText(PlayerPrefs.GetString(SettingsKey, ""));
            Loc.SetLang(Settings.Language == 1 ? Lang.En : Lang.Ru);
        }

        public void SaveGame()
        {
            if (Player == null) return;
            Save.Cash = Player.Stats.Cash;
            Save.Health = Mathf.Max(1f, Player.Stats.Health);
            Save.Armor = Player.Stats.Armor;
            Save.PosX = Player.Vehicle != null ? Player.Vehicle.Pos.x : Player.Pos.x;
            Save.PosZ = Player.Vehicle != null ? Player.Vehicle.Pos.z : Player.Pos.z;
            Save.Heading = Player.Heading;
            Save.TimeOfDay = Clock;
            Save.Day = DayNumber;
            Save.PlaySeconds = PlayTime;
            Save.WantedHeat = Wanted.Heat;
            Save.LastVehicle = Player.Vehicle != null ? Player.Vehicle.Def.Id : "";
            Save.OwnedWeapons.Clear();
            Save.Ammo.Clear();
            for (int i = 0; i < Player.Weapons.Count; i++)
            {
                WeaponState w = Player.Weapons[i];
                Save.OwnedWeapons.Add(w.Def.Id);
                if (!w.Melee) Save.SetAmmo(w.Def.Id, w.Magazine + w.Reserve);
            }
            PlayerPrefs.SetString(SaveKey, Save.ToText());
            PlayerPrefs.Save();
        }

        public void QuitToMenu()
        {
            SaveGame();
            Phone.Close();
            Map.Close();
            Mission = new MissionState();
            MissionHasTarget = false;
            Police.Clear();
            Wanted.Clear();
            Phase = Phase.MainMenu;
            Audio.SetEngine(false, 0f);
        }

        public void QuitApp()
        {
            Application.Quit();
            Phase = Phase.MainMenu;
        }

        // ----------------------------------------------------------------- gui

        void OnGUI()
        {
            if (World == null || Player == null)
            {
                Menu.Draw(this);
                return;
            }
            switch (Phase)
            {
                case Phase.Playing:
                case Phase.Paused:
                case Phase.Dead:
                case Phase.Busted:
                case Phase.MissionPassed:
                case Phase.MissionFailed:
                    Hud.Draw(this);
                    if (Map.Open) Map.Draw(this);
                    else if (Phone.Open) Phone.Draw(this);
                    if (Phase == Phase.Paused) Menu.Draw(this);
                    else if (Phase != Phase.Playing) Menu.Draw(this);
                    break;
                default:
                    Menu.Draw(this);
                    break;
            }
        }

        void OnApplicationPause(bool paused)
        {
            if (paused && Phase == Phase.Playing) { SaveGame(); Phase = Phase.Paused; }
        }
    }
}
