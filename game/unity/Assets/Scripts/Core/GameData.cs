using System;
using System.Collections.Generic;

namespace GTA6.Core
{
    // ---------------------------------------------------------------- districts

    public sealed class District
    {
        public string Id;
        public string NameRu;
        public string NameEn;
        public float X, Z;          // centre, metres
        public float Radius;        // rough extent
        public int Danger;          // 1..5, drives police density and pedestrian wealth
        public string ColorHex;     // map tint
        public string FlavourRu;
        public string FlavourEn;
        public string[] Streets;

        public string Name { get { return Loc.Current == Lang.Ru ? NameRu : NameEn; } }
        public string Flavour { get { return Loc.Current == Lang.Ru ? FlavourRu : FlavourEn; } }
    }

    // ------------------------------------------------------------------ weapons

    public enum WeaponClass { Melee, Pistol, Smg, Shotgun, Rifle }

    public sealed class WeaponDef
    {
        public string Id;
        public string NameRu;
        public string NameEn;
        public WeaponClass Class;
        public int Damage;
        public float RoundsPerMinute;
        public int Magazine;
        public int Reserve;
        public float Range;
        public float Spread;        // radians at 10 m
        public float Recoil;
        public int Price;
        public bool Automatic;
        public int PelletCount = 1;
        public string TracerHex = "FFD27F";

        public string Name { get { return Loc.Current == Lang.Ru ? NameRu : NameEn; } }
        public float SecondsBetweenShots { get { return 60f / Math.Max(1f, RoundsPerMinute); } }
    }

    // ----------------------------------------------------------------- vehicles

    public enum VehicleClass { Sedan, Sports, Suv, Taxi, Police, Bike, Van, Hyper, Ambulance }

    public sealed class VehicleDef
    {
        public string Id;
        public string NameRu;
        public string NameEn;
        public VehicleClass Class;
        public float TopSpeed;      // m/s
        public float Accel;         // m/s^2
        public float Brake;
        public float Grip;          // lateral grip 0..1
        public float Mass;
        public int Seats;
        public int Price;
        public float Length = 4.4f, Width = 1.9f, Height = 1.45f;
        public float WheelRadius = 0.34f;
        public string BodyHex = "C8352B";
        public string AccentHex = "1A1A1E";

        public string Name { get { return Loc.Current == Lang.Ru ? NameRu : NameEn; } }
    }

    // ------------------------------------------------------------------- radio

    public sealed class RadioStation
    {
        public string Id;
        public string Name;
        public string GenreRu;
        public string GenreEn;
        public float RootHz = 110f;
        public float Tempo = 100f;
        public int Pattern;         // selects the arpeggio table in the synth
        public string ColorHex = "D4472F";

        public string Genre { get { return Loc.Current == Lang.Ru ? GenreRu : GenreEn; } }
    }

    // ------------------------------------------------------------------- phone

    public sealed class PhoneApp
    {
        public string Id;
        public string TitleKey;
        public string DescKey;
        public string Glyph;        // 2-4 letters drawn on the icon
        public string ColorHex;
        public bool Enabled = true;
        public string Title { get { return Loc.S(TitleKey); } }
        public string Desc { get { return Loc.S(DescKey); } }
    }

    public sealed class Contact
    {
        public string Id;
        public string Name;
        public string Number;
        public string RoleRu;
        public string RoleEn;
        public bool Callable = true;
        public string[] MessagesRu = new string[0];
        public string[] MessagesEn = new string[0];
        public string Role { get { return Loc.Current == Lang.Ru ? RoleRu : RoleEn; } }
        public string[] Messages { get { return Loc.Current == Lang.Ru ? MessagesRu : MessagesEn; } }
    }

    // ---------------------------------------------------------------- missions

    public enum StepKind { GoTo, Kill, Survive, Deliver, Escape, Talk }

    public sealed class MissionStep
    {
        public StepKind Kind;
        public string TextRu;
        public string TextEn;
        public float X, Z;
        public float Radius = 22f;
        public int Count = 1;
        public float Seconds = 30f;
        public string FailRu = "";
        public string FailEn = "";
        public string Text { get { return Loc.Current == Lang.Ru ? TextRu : TextEn; } }
        public string FailText { get { return Loc.Current == Lang.Ru ? FailRu : FailEn; } }
    }

    public sealed class MissionDef
    {
        public string Id;
        public string TitleRu;
        public string TitleEn;
        public string DescRu;
        public string DescEn;
        public int Reward;
        public string Giver;
        public MissionStep[] Steps;
        public string Title { get { return Loc.Current == Lang.Ru ? TitleRu : TitleEn; } }
        public string Desc { get { return Loc.Current == Lang.Ru ? DescRu : DescEn; } }
    }

    // -------------------------------------------------------------- game data

    public static class GameData
    {
        public const float CityMin = -1200f;
        public const float CityMax = 1200f;
        public const float OceanX = 950f;      // x > this is water
        public const float BeachX = 895f;
        public const float RoadSpacing = 96f;   // city block pitch
        public const float RoadWidth = 16f;
        public const int SaveVersion = 1;

        // ---------------------------------------------------------- districts
        public static readonly District[] Districts =
        {
            new District { Id = "vice_beach", NameRu = "Вайс-Бич", NameEn = "Vice Beach",
                X = 690, Z = 700, Radius = 320, Danger = 2, ColorHex = "F2C46B",
                FlavourRu = "Пляж, променад и вечные каникулы.",
                FlavourEn = "Beach, boardwalk and an endless vacation.",
                Streets = new[] { "Ocean Drive", "Collins Ave", "Espanola Way", "Washington Ave" } },
            new District { Id = "ocean_drive", NameRu = "Оушен-Драйв", NameEn = "Ocean Drive",
                X = 480, Z = 760, Radius = 260, Danger = 2, ColorHex = "EF8A5B",
                FlavourRu = "Неоновые отели и арт-деко на первой линии.",
                FlavourEn = "Neon hotels and art deco on the waterfront.",
                Streets = new[] { "Ocean Drive", "Lummus Park", "7th Street", "5th Street" } },
            new District { Id = "starfish", NameRu = "Старфиш-Айленд", NameEn = "Starfish Island",
                X = 210, Z = 790, Radius = 220, Danger = 2, ColorHex = "8FC9A2",
                FlavourRu = "Особняки, бассейны, никого не видно за заборами.",
                FlavourEn = "Mansions, pools, and nobody in sight behind the walls.",
                Streets = new[] { "Starfish Causeway", "Hibiscus Lane", "Mansion Row" } },
            new District { Id = "downtown", NameRu = "Даунтаун Леониды", NameEn = "Downtown Leonida",
                X = 0, Z = 250, Radius = 380, Danger = 3, ColorHex = "7FB3D5",
                FlavourRu = "Стеклянные башни, банки и самая злая полиция штата.",
                FlavourEn = "Glass towers, banks and the meanest police in the state.",
                Streets = new[] { "Brickell Ave", "Flagler Street", "Biscayne Blvd", "SE 2nd Ave" } },
            new District { Id = "havana", NameRu = "Литл-Гавана", NameEn = "Little Havana",
                X = -470, Z = 180, Radius = 300, Danger = 4, ColorHex = "D98880",
                FlavourRu = "Палисадники, домино и долги, которые никто не забыл.",
                FlavourEn = "Porches, dominoes, and debts nobody forgot.",
                Streets = new[] { "Calle Ocho", "SW 8th Street", "Domino Park", "Calle 12" } },
            new District { Id = "gellhorn", NameRu = "Порт-Геллхорн", NameEn = "Port Gellhorn",
                X = -780, Z = 880, Radius = 300, Danger = 3, ColorHex = "B0A18A",
                FlavourRu = "Контейнеры, краны и люди, которым лучше не задавать вопросов.",
                FlavourEn = "Containers, cranes, and people you should not ask questions.",
                Streets = new[] { "Dock 7", "Crane Row", "Harbour Road" } },
            new District { Id = "grassrivers", NameRu = "Грассриверс", NameEn = "Grassrivers",
                X = -930, Z = -520, Radius = 330, Danger = 2, ColorHex = "6E8B5A",
                FlavourRu = "Болота, катера и аллигаторы крупнее, чем вы думаете.",
                FlavourEn = "Swamps, airboats and alligators bigger than you think.",
                Streets = new[] { "River Route 4", "Muddy Creek", "Alligator Alley" } },
            new District { Id = "ambrosia", NameRu = "Амброзия", NameEn = "Ambrosia",
                X = -330, Z = -830, Radius = 300, Danger = 2, ColorHex = "C7B37F",
                FlavourRu = "Пыльные склады и развязки, где легко уйти от хвоста.",
                FlavourEn = "Dusty warehouses and interchanges, easy to shake a tail.",
                Streets = new[] { "Ambrosia Rd", "Freight Line", "Warehouse Row" } },
            new District { Id = "waning_sands", NameRu = "Уэйнинг-Сэндс", NameEn = "Waning Sands",
                X = 470, Z = -320, Radius = 290, Danger = 3, ColorHex = "E0C48A",
                FlavourRu = "Мотели, автосалоны и казино-развалюхи.",
                FlavourEn = "Motels, car lots and crumbling casinos.",
                Streets = new[] { "Sands Blvd", "Route 9", "Motel Strip" } },
            new District { Id = "yorktown", NameRu = "Йорктаун", NameEn = "Yorktown",
                X = 720, Z = -740, Radius = 300, Danger = 3, ColorHex = "9AA7B4",
                FlavourRu = "Промзона с эхом и пустыми ангарами.",
                FlavourEn = "An industrial zone with echo and empty hangars.",
                Streets = new[] { "Yorktown Ave", "Dock 22", "Foundry Lane" } },
            new District { Id = "kalaga", NameRu = "Маунт-Калага", NameEn = "Mount Kalaga",
                X = -1030, Z = 300, Radius = 260, Danger = 1, ColorHex = "7D8A6A",
                FlavourRu = "Холмы, серпантин и вид на весь штат.",
                FlavourEn = "Hills, switchbacks and a view of the whole state.",
                Streets = new[] { "Kalaga Pass", "Ridge Rd", "Fire Lookout" } },
            new District { Id = "keys", NameRu = "Кис-Марина", NameEn = "Keys Marina",
                X = 860, Z = -120, Radius = 220, Danger = 2, ColorHex = "6FB3B8",
                FlavourRu = "Лодки, пеликаны и закат, который стоит катера.",
                FlavourEn = "Boats, pelicans and a sunset worth a boat.",
                Streets = new[] { "Marina Mile", "Keys Causeway", "Pelican Pier" } },
        };

        /// <summary>District whose centre is closest (weighted by radius).</summary>
        public static District DistrictAt(float x, float z)
        {
            District best = Districts[0];
            float bestD = float.MaxValue;
            for (int i = 0; i < Districts.Length; i++)
            {
                District d = Districts[i];
                float dx = x - d.X, dz = z - d.Z;
                float dist = (float)Math.Sqrt(dx * dx + dz * dz) / Math.Max(0.01f, d.Radius);
                if (dist < bestD) { bestD = dist; best = d; }
            }
            return best;
        }

        public static string StreetAt(float x, float z, int seed)
        {
            District d = DistrictAt(x, z);
            int i = Math.Abs((int)((x * 0.031f + z * 0.017f + seed)) % d.Streets.Length);
            return d.Streets[i];
        }

        // ------------------------------------------------------------ weapons
        public static readonly WeaponDef[] Weapons =
        {
            new WeaponDef { Id = "fists", NameRu = "Кулаки", NameEn = "Fists", Class = WeaponClass.Melee,
                Damage = 12, RoundsPerMinute = 120, Magazine = 0, Reserve = 0, Range = 1.7f,
                Spread = 0f, Recoil = 0f, Price = 0, Automatic = true },
            new WeaponDef { Id = "pistol", NameRu = "Пистолет Cerbera", NameEn = "Cerbera Pistol",
                Class = WeaponClass.Pistol, Damage = 26, RoundsPerMinute = 300, Magazine = 12,
                Reserve = 96, Range = 55f, Spread = 0.011f, Recoil = 1.4f, Price = 350,
                Automatic = false, TracerHex = "FFE1A3" },
            new WeaponDef { Id = "smg", NameRu = "ПП Vom Feuer", NameEn = "Vom Feuer SMG",
                Class = WeaponClass.Smg, Damage = 17, RoundsPerMinute = 750, Magazine = 30,
                Reserve = 240, Range = 45f, Spread = 0.026f, Recoil = 0.9f, Price = 1900,
                Automatic = true, TracerHex = "FFC46B" },
            new WeaponDef { Id = "shotgun", NameRu = "Дробовик Slick", NameEn = "Slick Shotgun",
                Class = WeaponClass.Shotgun, Damage = 11, RoundsPerMinute = 75, Magazine = 6,
                Reserve = 36, Range = 18f, Spread = 0.075f, Recoil = 4.2f, Price = 2400,
                Automatic = false, PelletCount = 8, TracerHex = "FFB347" },
            new WeaponDef { Id = "rifle", NameRu = "Карабин Kalaga", NameEn = "Kalaga Carbine",
                Class = WeaponClass.Rifle, Damage = 34, RoundsPerMinute = 520, Magazine = 25,
                Reserve = 150, Range = 140f, Spread = 0.009f, Recoil = 1.8f, Price = 5200,
                Automatic = true, TracerHex = "FFF0B3" },
        };

        public static WeaponDef Weapon(string id)
        {
            for (int i = 0; i < Weapons.Length; i++) if (Weapons[i].Id == id) return Weapons[i];
            return Weapons[0];
        }

        // ----------------------------------------------------------- vehicles
        public static readonly VehicleDef[] Vehicles =
        {
            new VehicleDef { Id = "sedan", NameRu = "Willard Sedan", NameEn = "Willard Sedan",
                Class = VehicleClass.Sedan, TopSpeed = 42f, Accel = 7.5f, Brake = 16f, Grip = 0.86f,
                Mass = 1500f, Seats = 4, Price = 24000, BodyHex = "B9C4CC", AccentHex = "22262B" },
            new VehicleDef { Id = "sports", NameRu = "Pegassi Torero", NameEn = "Pegassi Torero",
                Class = VehicleClass.Sports, TopSpeed = 66f, Accel = 12.5f, Brake = 22f, Grip = 0.93f,
                Mass = 1280f, Seats = 2, Price = 148000, Length = 4.5f, Width = 2.0f, Height = 1.12f,
                WheelRadius = 0.32f, BodyHex = "E23E57", AccentHex = "111114" },
            new VehicleDef { Id = "suv", NameRu = "Bravado Bison", NameEn = "Bravado Bison",
                Class = VehicleClass.Suv, TopSpeed = 40f, Accel = 6.8f, Brake = 15f, Grip = 0.79f,
                Mass = 2200f, Seats = 4, Price = 46000, Length = 5.0f, Width = 2.1f, Height = 1.85f,
                WheelRadius = 0.42f, BodyHex = "2E4A62", AccentHex = "191C21" },
            new VehicleDef { Id = "taxi", NameRu = "Vapid Такси", NameEn = "Vapid Taxi",
                Class = VehicleClass.Taxi, TopSpeed = 39f, Accel = 7f, Brake = 16f, Grip = 0.85f,
                Mass = 1550f, Seats = 4, Price = 12000, BodyHex = "F2C14E", AccentHex = "2B2B2B" },
            new VehicleDef { Id = "police", NameRu = "Патруль LPD", NameEn = "LPD Cruiser",
                Class = VehicleClass.Police, TopSpeed = 54f, Accel = 10.5f, Brake = 20f, Grip = 0.9f,
                Mass = 1800f, Seats = 4, Price = 0, BodyHex = "F4F6F8", AccentHex = "14213A" },
            new VehicleDef { Id = "bike", NameRu = "Maibatsu Sanchez", NameEn = "Maibatsu Sanchez",
                Class = VehicleClass.Bike, TopSpeed = 58f, Accel = 13f, Brake = 18f, Grip = 0.72f,
                Mass = 210f, Seats = 1, Price = 32000, Length = 2.1f, Width = 0.7f, Height = 1.1f,
                WheelRadius = 0.31f, BodyHex = "E8672B", AccentHex = "151515" },
            new VehicleDef { Id = "van", NameRu = "Brute Boxville", NameEn = "Brute Boxville",
                Class = VehicleClass.Van, TopSpeed = 34f, Accel = 5.2f, Brake = 12f, Grip = 0.7f,
                Mass = 2800f, Seats = 2, Price = 38000, Length = 5.6f, Width = 2.2f, Height = 2.5f,
                WheelRadius = 0.42f, BodyHex = "DDE2E6", AccentHex = "2A2E33" },
            new VehicleDef { Id = "hyper", NameRu = "Grotti Cheetah", NameEn = "Grotti Cheetah",
                Class = VehicleClass.Hyper, TopSpeed = 84f, Accel = 16f, Brake = 26f, Grip = 0.96f,
                Mass = 1150f, Seats = 2, Price = 620000, Length = 4.6f, Width = 2.05f, Height = 1.05f,
                WheelRadius = 0.33f, BodyHex = "F5D020", AccentHex = "101012" },
            new VehicleDef { Id = "ambulance", NameRu = "Ambulance", NameEn = "Ambulance",
                Class = VehicleClass.Ambulance, TopSpeed = 44f, Accel = 8f, Brake = 16f, Grip = 0.8f,
                Mass = 2600f, Seats = 2, Price = 0, Length = 5.4f, Width = 2.2f, Height = 2.4f,
                WheelRadius = 0.42f, BodyHex = "F2F4F6", AccentHex = "C0392B" },
        };

        public static VehicleDef Vehicle(string id)
        {
            for (int i = 0; i < Vehicles.Length; i++) if (Vehicles[i].Id == id) return Vehicles[i];
            return Vehicles[0];
        }

        // -------------------------------------------------------------- radio
        public static readonly RadioStation[] Stations =
        {
            new RadioStation { Id = "leo_fm", Name = "LEO-FM 101.5", GenreRu = "Поп", GenreEn = "Pop",
                RootHz = 130.81f, Tempo = 112f, Pattern = 0, ColorHex = "E8A33D" },
            new RadioStation { Id = "vice_wave", Name = "Vice Wave 92.1", GenreRu = "Синтвейв", GenreEn = "Synthwave",
                RootHz = 98f, Tempo = 96f, Pattern = 1, ColorHex = "D4472F" },
            new RadioStation { Id = "cadence", Name = "Cadence 88.8", GenreRu = "Хип-хоп", GenreEn = "Hip-hop",
                RootHz = 87.31f, Tempo = 88f, Pattern = 2, ColorHex = "8E5BD1" },
            new RadioStation { Id = "kalaga", Name = "Kalaga Country", GenreRu = "Кантри", GenreEn = "Country",
                RootHz = 146.83f, Tempo = 104f, Pattern = 3, ColorHex = "B78647" },
            new RadioStation { Id = "neon", Name = "Neon FM", GenreRu = "Хаус", GenreEn = "House",
                RootHz = 110f, Tempo = 124f, Pattern = 4, ColorHex = "38B6A0" },
            new RadioStation { Id = "pirate", Name = "Пиратское радио", GenreRu = "Регги", GenreEn = "Reggae",
                RootHz = 123.47f, Tempo = 78f, Pattern = 5, ColorHex = "5FA84A" },
        };

        // -------------------------------------------------------------- apps
        public static readonly PhoneApp[] Apps =
        {
            new PhoneApp { Id = "map", TitleKey = "app.map", DescKey = "", Glyph = "MAP", ColorHex = "3C8DBC" },
            new PhoneApp { Id = "contacts", TitleKey = "app.contacts", DescKey = "", Glyph = "AB", ColorHex = "4E9E5A" },
            new PhoneApp { Id = "messages", TitleKey = "app.messages", DescKey = "", Glyph = "MSG", ColorHex = "59B36A" },
            new PhoneApp { Id = "calls", TitleKey = "app.calls", DescKey = "", Glyph = "TEL", ColorHex = "4FA845" },
            new PhoneApp { Id = "radio", TitleKey = "app.radio", DescKey = "", Glyph = "FM", ColorHex = "D4472F" },
            new PhoneApp { Id = "photo", TitleKey = "app.photo", DescKey = "", Glyph = "CAM", ColorHex = "6C6F73" },
            new PhoneApp { Id = "gallery", TitleKey = "app.gallery", DescKey = "", Glyph = "IMG", ColorHex = "C0803F" },
            new PhoneApp { Id = "music", TitleKey = "app.music", DescKey = "", Glyph = "MUS", ColorHex = "C2334D" },
            new PhoneApp { Id = "bank", TitleKey = "app.bank", DescKey = "", Glyph = "$", ColorHex = "2E8B57" },
            new PhoneApp { Id = "shop", TitleKey = "app.shop", DescKey = "", Glyph = "SHOP", ColorHex = "B0883A" },
            new PhoneApp { Id = "missions", TitleKey = "app.missions", DescKey = "", Glyph = "JOB", ColorHex = "C9743A" },
            new PhoneApp { Id = "garage", TitleKey = "app.garage", DescKey = "", Glyph = "CAR", ColorHex = "3E6E9E" },
            new PhoneApp { Id = "weather", TitleKey = "app.weather", DescKey = "", Glyph = "WX", ColorHex = "4A90C2" },
            new PhoneApp { Id = "news", TitleKey = "app.news", DescKey = "", Glyph = "NEWS", ColorHex = "9B4F4F" },
            new PhoneApp { Id = "browser", TitleKey = "app.browser", DescKey = "", Glyph = "WWW", ColorHex = "3F9FA8" },
            new PhoneApp { Id = "settings", TitleKey = "app.settings", DescKey = "", Glyph = "SET", ColorHex = "6B6E72" },
        };

        // ------------------------------------------------------------ contacts
        public static readonly Contact[] Contacts =
        {
            new Contact { Id = "donny", Name = "Donny Vega", Number = "+1 305 555 0110",
                RoleRu = "Друг детства, трейлерный парк", RoleEn = "Childhood friend, trailer park",
                MessagesRu = new[] { "Брат, ты где? Я жду у трейлеров.", "Взял работу: сумка в порту. Не опаздывай." },
                MessagesEn = new[] { "Bro, where are you? I'm at the trailers.", "Got us a job: bag at the docks. Don't be late." } },
            new Contact { Id = "lucia", Name = "Lucía Caminos", Number = "+1 305 555 0142",
                RoleRu = "Напарница", RoleEn = "Partner",
                MessagesRu = new[] { "Я на крыше отеля. Приходи один.", "Если что-то пойдёт не так — уходи через кухню." },
                MessagesEn = new[] { "I'm on the hotel roof. Come alone.", "If it goes sideways, take the kitchen exit." } },
            new Contact { Id = "ravi", Name = "Ravi 'Rocket' Patel", Number = "+1 305 555 0188",
                RoleRu = "Механик", RoleEn = "Mechanic",
                MessagesRu = new[] { "Привози тачку — сделаю из неё конфету." },
                MessagesEn = new[] { "Bring the car by — I'll make it candy." } },
            new Contact { Id = "mama", Name = "Мама", Number = "+1 305 555 0101",
                RoleRu = "Семья", RoleEn = "Family",
                MessagesRu = new[] { "Ты поел? Позвони, я волнуюсь." },
                MessagesEn = new[] { "Did you eat? Call me, I worry." } },
            new Contact { Id = "diaz", Name = "Rick Diaz", Number = "+1 305 555 0177",
                RoleRu = "Должник", RoleEn = "Debtor" },
            new Contact { Id = "carla", Name = "Carla Mendes", Number = "+1 305 555 0123",
                RoleRu = "Бармен в Литл-Гаване", RoleEn = "Bartender in Little Havana" },
            new Contact { Id = "tony", Name = "Tony 'Eight' Russo", Number = "+1 305 555 0166",
                RoleRu = "Скупщик", RoleEn = "Fence" },
            new Contact { Id = "nina", Name = "Nina Okafor", Number = "+1 305 555 0154",
                RoleRu = "Журналистка", RoleEn = "Journalist" },
            new Contact { Id = "sergei", Name = "Sergei Volkov", Number = "+1 305 555 0132",
                RoleRu = "Порт", RoleEn = "Docks" },
            new Contact { Id = "mila", Name = "Mila Reyes", Number = "+1 305 555 0119",
                RoleRu = "Угонщица", RoleEn = "Car thief" },
            new Contact { Id = "gus", Name = "Gus Halvorsen", Number = "+1 305 555 0193",
                RoleRu = "Рыбак из Грассриверс", RoleEn = "Grassrivers fisherman" },
            new Contact { Id = "dee", Name = "Dee Watkins", Number = "+1 305 555 0171",
                RoleRu = "Радиоведущая", RoleEn = "Radio host" },
            new Contact { Id = "omar", Name = "Omar Haddad", Number = "+1 305 555 0135",
                RoleRu = "Владелец магазина", RoleEn = "Shop owner" },
            new Contact { Id = "jax", Name = "Jax Turner", Number = "+1 305 555 0186",
                RoleRu = "Уличный гонщик", RoleEn = "Street racer" },
            new Contact { Id = "rosa", Name = "Rosa Alvarez", Number = "+1 305 555 0148",
                RoleRu = "Медсестра", RoleEn = "Nurse" },
            new Contact { Id = "kev", Name = "Kev Doyle", Number = "+1 305 555 0190",
                RoleRu = "Коп из ЛПД", RoleEn = "LPD cop", Callable = false },
            new Contact { Id = "ivy", Name = "Ivy Chen", Number = "+1 305 555 0160",
                RoleRu = "Хакер", RoleEn = "Hacker" },
            new Contact { Id = "bruno", Name = "Bruno Costa", Number = "+1 305 555 0127",
                RoleRu = "Охранник в порту", RoleEn = "Dock guard" },
            new Contact { Id = "sam", Name = "Sam Whitlock", Number = "+1 305 555 0104",
                RoleRu = "Риелтор", RoleEn = "Realtor" },
            new Contact { Id = "lea", Name = "Lea Moreau", Number = "+1 305 555 0157",
                RoleRu = "Гид по Калага", RoleEn = "Kalaga guide" },
            new Contact { Id = "vic", Name = "Vic Sorrentino", Number = "+1 305 555 0181",
                RoleRu = "Букмекер", RoleEn = "Bookmaker" },
            new Contact { Id = "911", Name = "Экстренные службы", Number = "911",
                RoleRu = "Полиция / скорая", RoleEn = "Police / ambulance", Callable = false },
        };

        public static Contact FindContact(string id)
        {
            for (int i = 0; i < Contacts.Length; i++) if (Contacts[i].Id == id) return Contacts[i];
            return Contacts[0];
        }

        // ------------------------------------------------------------ missions
        public static readonly MissionDef[] Missions =
        {
            new MissionDef
            {
                Id = "prologue", Reward = 250, Giver = "donny",
                TitleRu = "Первый день", TitleEn = "First Day",
                DescRu = "Донни ждёт у трейлерного парка на окраине Амброзии.",
                DescEn = "Donny is waiting by the trailer park on the edge of Ambrosia.",
                Steps = new[]
                {
                    new MissionStep { Kind = StepKind.GoTo, X = -330, Z = -980, Radius = 18,
                        TextRu = "Дойдите до трейлерного парка", TextEn = "Walk to the trailer park" },
                    new MissionStep { Kind = StepKind.Talk, X = -330, Z = -980, Radius = 6,
                        TextRu = "Поговорите с Донни", TextEn = "Talk to Donny" },
                    new MissionStep { Kind = StepKind.GoTo, X = -260, Z = -860, Radius = 16,
                        TextRu = "Дойдите до склада", TextEn = "Walk to the warehouse" },
                    new MissionStep { Kind = StepKind.Kill, X = -260, Z = -860, Radius = 30, Count = 3,
                        TextRu = "Разберитесь с должниками", TextEn = "Deal with the debtors" },
                }
            },
            new MissionDef
            {
                Id = "cash", Reward = 1200, Giver = "sergei",
                TitleRu = "Наличные", TitleEn = "Cash Flow",
                DescRu = "Сумка в седьмом доке Порт-Геллхорна. Забери её первым.",
                DescEn = "A bag at dock seven in Port Gellhorn. Get to it first.",
                Steps = new[]
                {
                    new MissionStep { Kind = StepKind.Deliver, X = -780, Z = 900, Radius = 20,
                        TextRu = "Доедьте до седьмого дока", TextEn = "Drive to dock seven" },
                    new MissionStep { Kind = StepKind.Kill, X = -780, Z = 900, Radius = 34, Count = 4,
                        TextRu = "Отбейте сумку", TextEn = "Fight for the bag" },
                    new MissionStep { Kind = StepKind.Escape, X = -700, Z = 800, Radius = 40, Seconds = 20,
                        TextRu = "Уйдите от полиции", TextEn = "Lose the police" },
                }
            },
            new MissionDef
            {
                Id = "debt", Reward = 2400, Giver = "diaz",
                TitleRu = "Долг", TitleEn = "The Debt",
                DescRu = "Рик Диаз задолжал всем. Сначала поговорите, потом стреляйте.",
                DescEn = "Rick Diaz owes everyone. Talk first, shoot later.",
                Steps = new[]
                {
                    new MissionStep { Kind = StepKind.GoTo, X = -470, Z = 180, Radius = 18,
                        TextRu = "Придите в Литл-Гавану", TextEn = "Go to Little Havana" },
                    new MissionStep { Kind = StepKind.Talk, X = -470, Z = 180, Radius = 8,
                        TextRu = "Поговорите с Риком", TextEn = "Talk to Rick" },
                    new MissionStep { Kind = StepKind.Survive, X = -470, Z = 180, Radius = 30, Seconds = 45,
                        TextRu = "Держитесь, пока не приедет Тони", TextEn = "Hold out until Tony arrives" },
                    new MissionStep { Kind = StepKind.Kill, X = -470, Z = 180, Radius = 40, Count = 5,
                        TextRu = "Уложите всех нападавших", TextEn = "Put down every attacker" },
                }
            },
            new MissionDef
            {
                Id = "ride", Reward = 3600, Giver = "mila",
                TitleRu = "Угон", TitleEn = "Ride Out",
                DescRu = "Украдите тачку на Оушен-Драйв и довезите её целой до гаража.",
                DescEn = "Steal a car on Ocean Drive and deliver it to the garage in one piece.",
                Steps = new[]
                {
                    new MissionStep { Kind = StepKind.GoTo, X = 480, Z = 760, Radius = 20,
                        TextRu = "Найдите машину на Оушен-Драйв", TextEn = "Find the car on Ocean Drive" },
                    new MissionStep { Kind = StepKind.Deliver, X = 720, Z = -740, Radius = 20, Seconds = 150,
                        TextRu = "Доставьте машину в Йорктаун", TextEn = "Deliver the car to Yorktown",
                        FailRu = "Машина уничтожена", FailEn = "The vehicle was destroyed" },
                    new MissionStep { Kind = StepKind.Escape, X = 720, Z = -740, Radius = 40, Seconds = 24,
                        TextRu = "Уйдите от полиции", TextEn = "Lose the police" },
                }
            },
            new MissionDef
            {
                Id = "finale", Reward = 12000, Giver = "lucia",
                TitleRu = "Финал", TitleEn = "Finale",
                DescRu = "Крыша отеля на Вайс-Бич. Последний разговор.",
                DescEn = "A hotel roof in Vice Beach. The last conversation.",
                Steps = new[]
                {
                    new MissionStep { Kind = StepKind.GoTo, X = 690, Z = 700, Radius = 16,
                        TextRu = "Поднимитесь к отелю", TextEn = "Get to the hotel" },
                    new MissionStep { Kind = StepKind.Talk, X = 690, Z = 700, Radius = 8,
                        TextRu = "Поговорите с Люсией", TextEn = "Talk to Lucía" },
                    new MissionStep { Kind = StepKind.Kill, X = 690, Z = 700, Radius = 45, Count = 8,
                        TextRu = "Отбейте крышу", TextEn = "Defend the roof" },
                    new MissionStep { Kind = StepKind.Escape, X = 690, Z = 700, Radius = 50, Seconds = 35,
                        TextRu = "Уходите с дня на день", TextEn = "Get out before the day ends" },
                }
            },
        };

        public static MissionDef Mission(string id)
        {
            for (int i = 0; i < Missions.Length; i++) if (Missions[i].Id == id) return Missions[i];
            return Missions[0];
        }

        /// <summary>Mission ids in story order.</summary>
        public static readonly string[] StoryOrder = { "prologue", "cash", "debt", "ride", "finale" };

        // ------------------------------------------------------------- shops
        public sealed class ShopItem
        {
            public string Id;
            public string NameKey;
            public int Price;
            public string Name { get { return Loc.S(NameKey); } }
        }

        public static readonly ShopItem[] ShopItems =
        {
            new ShopItem { Id = "medkit", NameKey = "shop.health", Price = 120 },
            new ShopItem { Id = "armor", NameKey = "shop.armor", Price = 450 },
            new ShopItem { Id = "ammo_pistol", NameKey = "shop.ammo", Price = 60 },
            new ShopItem { Id = "ammo_smg", NameKey = "shop.ammo", Price = 180 },
            new ShopItem { Id = "ammo_shotgun", NameKey = "shop.ammo", Price = 220 },
            new ShopItem { Id = "ammo_rifle", NameKey = "shop.ammo", Price = 400 },
            new ShopItem { Id = "respray", NameKey = "shop.paint", Price = 900 },
        };
    }
}
