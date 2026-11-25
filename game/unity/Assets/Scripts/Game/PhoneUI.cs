using System;
using System.Collections.Generic;
using UnityEngine;
using GTA6.Core;

namespace GTA6.Game
{
    public sealed class PhoneUI
    {
        public bool Open { get; private set; }
        public string AppId = "";
        float _anim;
        int _page;
        string _called;
        float _callTime;
        int _photoCount;
        float _scroll;

        public void Toggle(GameRoot g)
        {
            if (Open) Close(); else Show(g);
        }

        public void Show(GameRoot g)
        {
            Open = true;
            AppId = "";
            _anim = 0f;
            _page = 0;
            AudioDirector.I.PlayUi();
        }

        public void Close()
        {
            Open = false;
            _called = null;
            AudioDirector.I.PlayUi();
        }

        public void Update(GameRoot g, float dt)
        {
            if (Open) _anim = Mathf.MoveTowards(_anim, 1f, dt * 7f);
            else _anim = Mathf.MoveTowards(_anim, 0f, dt * 9f);
            if (_called != null) _callTime += dt;
        }

        public void Draw(GameRoot g)
        {
            if (_anim <= 0.01f) return;
            Rect full = new Rect(0, 0, Screen.width, Screen.height);
            Ui.Fill(full, new Color(0f, 0f, 0f, 0.45f * _anim));

            Rect r = Layout.PhoneRect;
            float rise = (1f - _anim) * Layout.Px(60f);
            r.y += rise;

            Ui.Round(r, new Color32(10, 11, 15, 250), Layout.Px(26f));
            Ui.Border(r, new Color32(60, 64, 72, 255), 2f);

            Rect header = new Rect(r.x + Layout.Px(12f), r.y + Layout.Px(10f), r.width - Layout.Px(24f), Layout.Px(28f));
            Ui.Text(new Rect(header.x, header.y, header.width * 0.4f, header.height), Loc.Hour(g.Clock),
                Mathf.RoundToInt(Layout.Px(14f)), Ui.TextColor, TextAnchor.MiddleLeft, FontStyle.Bold);
            Ui.Text(header, Loc.S("phone.signal"), Mathf.RoundToInt(Layout.Px(13f)), Ui.TextDimColor);
            Ui.Text(new Rect(header.x + header.width * 0.6f, header.y, header.width * 0.4f, header.height),
                "92%", Mathf.RoundToInt(Layout.Px(13f)), Ui.TextDimColor, TextAnchor.MiddleRight);

            Rect body = new Rect(r.x + Layout.Px(14f), r.y + Layout.Px(44f),
                r.width - Layout.Px(28f), r.height - Layout.Px(78f));

            if (_called != null) DrawCall(g, body);
            else if (string.IsNullOrEmpty(AppId)) DrawHome(g, body);
            else DrawApp(g, body);

            // home bar
            Rect bar = new Rect(r.x + r.width * 0.36f, r.yMax - Layout.Px(18f), r.width * 0.28f, Layout.Px(5f));
            Ui.Round(bar, new Color32(120, 124, 132, 200), Layout.Px(3f));
        }

        // -------------------------------------------------------------- homescreen

        void DrawHome(GameRoot g, Rect body)
        {
            if (Ui.Button(new Rect(body.xMax - Layout.Px(52f), body.y - Layout.Px(34f), Layout.Px(46f), Layout.Px(26f)),
                Loc.S("ui.close"), false, Mathf.RoundToInt(Layout.Px(12f))))
            {
                Close();
                return;
            }

            int cols = 4;
            float gap = Layout.Px(8f);
            float cellW = (body.width - gap * (cols - 1)) / cols;
            float cellH = cellW * 0.92f;
            for (int i = 0; i < GameData.Apps.Length; i++)
            {
                PhoneApp app = GameData.Apps[i];
                int cx = i % cols, cy = i / cols;
                Rect cell = new Rect(body.x + cx * (cellW + gap), body.y + cy * (cellH + gap * 2.4f), cellW, cellH);
                if (cell.yMax > body.yMax - Layout.Px(10f)) break;
                Rect icon = new Rect(cell.x + cell.width * 0.14f, cell.y, cell.width * 0.72f, cell.width * 0.72f);
                Color32 c = Col.Hex(app.ColorHex);
                Ui.Round(icon, c, Layout.Px(12f));
                Ui.Text(icon, app.Glyph, Mathf.RoundToInt(icon.width * 0.36f), new Color32(255, 255, 255, 240),
                    TextAnchor.MiddleCenter, FontStyle.Bold);
                Ui.Text(new Rect(cell.x - Layout.Px(4f), icon.yMax + Layout.Px(4f), cell.width + Layout.Px(8f), Layout.Px(16f)),
                    app.Title, Mathf.RoundToInt(Layout.Px(11f)), Ui.TextColor, TextAnchor.MiddleCenter);
                if (GUI.Button(cell, GUIContent.none, GUIStyle.none))
                {
                    AppId = app.Id;
                    _page = 0;
                    _scroll = 0f;
                    AudioDirector.I.PlayUi();
                }
            }

            Ui.Text(new Rect(body.x, body.yMax - Layout.Px(26f), body.width, Layout.Px(18f)),
                Loc.S("phone.help1"), Mathf.RoundToInt(Layout.Px(11f)), Ui.TextDimColor);
        }

        // ---------------------------------------------------------------- apps

        void DrawApp(GameRoot g, Rect body)
        {
            PhoneApp app = Find(AppId);
            string title = app != null ? app.Title : AppId;
            Rect head = new Rect(body.x, body.y, body.width, Layout.Px(34f));
            if (Ui.Button(new Rect(head.x, head.y, Layout.Px(56f), Layout.Px(28f)), "< " + Loc.S("ui.back"), false,
                Mathf.RoundToInt(Layout.Px(13f))))
            {
                AppId = "";
                _page = 0;
                AudioDirector.I.PlayUi();
                return;
            }
            Ui.Text(new Rect(head.x + Layout.Px(64f), head.y, head.width - Layout.Px(64f), head.height), title,
                Mathf.RoundToInt(Layout.Px(17f)), Ui.TextColor, TextAnchor.MiddleLeft, FontStyle.Bold);

            Rect content = new Rect(body.x, body.y + Layout.Px(40f), body.width, body.height - Layout.Px(48f));
            switch (AppId)
            {
                case "map": AppMap(g, content); break;
                case "contacts": AppContacts(g, content); break;
                case "messages": AppMessages(g, content); break;
                case "calls": AppCalls(g, content); break;
                case "radio": AppRadio(g, content); break;
                case "photo": AppPhoto(g, content); break;
                case "gallery": AppGallery(g, content); break;
                case "music": AppMusic(g, content); break;
                case "bank": AppBank(g, content); break;
                case "shop": AppShop(g, content); break;
                case "missions": AppMissions(g, content); break;
                case "garage": AppGarage(g, content); break;
                case "weather": AppWeather(g, content); break;
                case "news": AppNews(g, content); break;
                case "browser": AppBrowser(g, content); break;
                case "settings": AppSettings(g, content); break;
                default: AppBlank(content); break;
            }
        }

        static PhoneApp Find(string id)
        {
            for (int i = 0; i < GameData.Apps.Length; i++) if (GameData.Apps[i].Id == id) return GameData.Apps[i];
            return null;
        }

        void AppBlank(Rect r)
        {
            Ui.Text(r, Loc.S("ui.coming"), Mathf.RoundToInt(Layout.Px(16f)), Ui.TextDimColor);
        }

        void AppMap(GameRoot g, Rect r)
        {
            Rect view = new Rect(r.x, r.y, r.width, r.height - Layout.Px(46f));
            Ui.Round(view, new Color32(20, 24, 30, 255), Layout.Px(10f));
            if (g.World.MapTexture != null)
            {
                Vector3 p = g.Player.Pos;
                float viewMeters = 900f;
                float extent = World.Extent;
                Rect uv = new Rect((p.x - viewMeters * 0.5f + extent) / (extent * 2f),
                                   (p.z - viewMeters * 0.5f + extent) / (extent * 2f),
                                   viewMeters / (extent * 2f), viewMeters / (extent * 2f));
                GUI.DrawTextureWithTexCoords(view, g.World.MapTexture, uv, true);
            }
            District d = g.District;
            Ui.Text(new Rect(r.x, r.yMax - Layout.Px(42f), r.width, Layout.Px(20f)),
                d != null ? d.Name : "", Mathf.RoundToInt(Layout.Px(14f)), Ui.AccentColor, TextAnchor.MiddleLeft, FontStyle.Bold);
            if (Ui.Button(new Rect(r.xMax - Layout.Px(120f), r.yMax - Layout.Px(44f), Layout.Px(120f), Layout.Px(34f)),
                Loc.S("hud.map"), true, Mathf.RoundToInt(Layout.Px(14f))))
            {
                g.OpenMap();
                Close();
            }
        }

        void AppContacts(GameRoot g, Rect r)
        {
            int perPage = 7;
            int pages = Mathf.CeilToInt(GameData.Contacts.Length / (float)perPage);
            _page = M.Clamp(_page, 0, pages - 1);
            for (int i = 0; i < perPage; i++)
            {
                int idx = _page * perPage + i;
                if (idx >= GameData.Contacts.Length) break;
                Contact c = GameData.Contacts[idx];
                Rect row = new Rect(r.x, r.y + i * Layout.Px(56f), r.width, Layout.Px(50f));
                Ui.Round(row, new Color32(28, 32, 40, 255), Layout.Px(8f));
                Ui.Circle(new Vector2(row.x + Layout.Px(24f), row.center.y), Layout.Px(16f), Col.Hex("4E9E5A"));
                Ui.Text(new Rect(row.x + Layout.Px(46f), row.y + Layout.Px(4f), row.width - Layout.Px(56f), Layout.Px(22f)),
                    c.Name, Mathf.RoundToInt(Layout.Px(15f)), Ui.TextColor, TextAnchor.MiddleLeft, FontStyle.Bold);
                Ui.Text(new Rect(row.x + Layout.Px(46f), row.y + Layout.Px(24f), row.width - Layout.Px(56f), Layout.Px(20f)),
                    c.Role, Mathf.RoundToInt(Layout.Px(12f)), Ui.TextDimColor, TextAnchor.MiddleLeft);
                if (c.Callable && GUI.Button(new Rect(row.xMax - Layout.Px(62f), row.y + Layout.Px(10f), Layout.Px(56f), Layout.Px(30f)),
                    Loc.S("phone.call")))
                {
                    _called = c.Id;
                    _callTime = 0f;
                    AudioDirector.I.PlayUi();
                }
            }
            Pager(r, pages, perPage);
        }

        void AppMessages(GameRoot g, Rect r)
        {
            int shown = 0;
            for (int i = 0; i < GameData.Contacts.Length && shown < 6; i++)
            {
                Contact c = GameData.Contacts[i];
                string[] msgs = c.Messages;
                if (msgs == null || msgs.Length == 0) continue;
                Rect row = new Rect(r.x, r.y + shown * Layout.Px(72f), r.width, Layout.Px(66f));
                Ui.Round(row, new Color32(28, 32, 40, 255), Layout.Px(8f));
                Ui.Text(new Rect(row.x + Layout.Px(10f), row.y + Layout.Px(4f), row.width - Layout.Px(20f), Layout.Px(20f)),
                    c.Name, Mathf.RoundToInt(Layout.Px(14f)), Ui.AccentColor, TextAnchor.MiddleLeft, FontStyle.Bold);
                Ui.Text(new Rect(row.x + Layout.Px(10f), row.y + Layout.Px(22f), row.width - Layout.Px(20f), Layout.Px(40f)),
                    msgs[msgs.Length - 1], Mathf.RoundToInt(Layout.Px(12f)), Ui.TextColor, TextAnchor.UpperLeft, FontStyle.Normal, true);
                shown++;
            }
            if (shown == 0) Ui.Text(r, Loc.S("ui.empty"), Mathf.RoundToInt(Layout.Px(15f)), Ui.TextDimColor);
        }

        void AppCalls(GameRoot g, Rect r)
        {
            for (int i = 0; i < 8 && i < GameData.Contacts.Length; i++)
            {
                Contact c = GameData.Contacts[i];
                Rect row = new Rect(r.x, r.y + i * Layout.Px(44f), r.width, Layout.Px(40f));
                Ui.Round(row, new Color32(26, 30, 36, 255), Layout.Px(6f));
                Ui.Text(new Rect(row.x + Layout.Px(10f), row.y, row.width * 0.6f, row.height), c.Name,
                    Mathf.RoundToInt(Layout.Px(13f)), Ui.TextColor, TextAnchor.MiddleLeft);
                Ui.Text(new Rect(row.x + row.width * 0.55f, row.y, row.width * 0.42f, row.height), c.Number,
                    Mathf.RoundToInt(Layout.Px(11f)), Ui.TextDimColor, TextAnchor.MiddleRight);
                if (GUI.Button(row, GUIContent.none, GUIStyle.none)) { _called = c.Id; _callTime = 0f; }
            }
        }

        void AppRadio(GameRoot g, Rect r)
        {
            for (int i = 0; i < GameData.Stations.Length; i++)
            {
                RadioStation st = GameData.Stations[i];
                Rect row = new Rect(r.x, r.y + i * Layout.Px(54f), r.width, Layout.Px(48f));
                bool active = AudioDirector.I.Station == i;
                Ui.Round(row, active ? Col.Hex(st.ColorHex) : new Color32(28, 32, 40, 255), Layout.Px(8f));
                Ui.Text(new Rect(row.x + Layout.Px(12f), row.y + Layout.Px(2f), row.width - Layout.Px(24f), Layout.Px(24f)),
                    st.Name, Mathf.RoundToInt(Layout.Px(15f)), active ? new Color32(20, 18, 14, 255) : Ui.TextColor,
                    TextAnchor.MiddleLeft, FontStyle.Bold);
                Ui.Text(new Rect(row.x + Layout.Px(12f), row.y + Layout.Px(24f), row.width - Layout.Px(24f), Layout.Px(20f)),
                    st.Genre, Mathf.RoundToInt(Layout.Px(12f)), active ? new Color32(40, 36, 28, 255) : Ui.TextDimColor,
                    TextAnchor.MiddleLeft);
                if (GUI.Button(row, GUIContent.none, GUIStyle.none))
                {
                    if (active) AudioDirector.I.StopMusic();
                    else AudioDirector.I.SetStation(i);
                }
            }
            if (Ui.Button(new Rect(r.x, r.yMax - Layout.Px(44f), r.width, Layout.Px(40f)), Loc.S("radio.off"), false))
                AudioDirector.I.StopMusic();
        }

        void AppPhoto(GameRoot g, Rect r)
        {
            Ui.Round(r, new Color32(18, 20, 24, 255), Layout.Px(10f));
            Ui.Text(r, Loc.S("phone.photo"), Mathf.RoundToInt(Layout.Px(18f)), Ui.TextDimColor);
            if (Ui.Button(new Rect(r.x + r.width * 0.2f, r.yMax - Layout.Px(60f), r.width * 0.6f, Layout.Px(44f)),
                Loc.S("phone.photo"), true))
            {
                _photoCount++;
                g.Save.Photos++;
                g.Notify("+" + Loc.S("phone.photo"));
            }
            Ui.Text(new Rect(r.x, r.yMax - Layout.Px(110f), r.width, Layout.Px(24f)),
                Loc.Num(g.Save.Photos) + " / 999", Mathf.RoundToInt(Layout.Px(14f)), Ui.TextColor);
        }

        void AppGallery(GameRoot g, Rect r)
        {
            int cols = 3;
            float gap = Layout.Px(6f);
            float cell = (r.width - gap * (cols - 1)) / cols;
            int count = Mathf.Clamp(g.Save.Photos, 0, 9);
            for (int i = 0; i < 9; i++)
            {
                Rect c = new Rect(r.x + (i % cols) * (cell + gap), r.y + (i / cols) * (cell + gap), cell, cell);
                Color32 col = i < count ? Col.Hex(new[] { "3C8DBC", "D4472F", "4E9E5A", "C0803F", "8E5BD1", "38B6A0", "B0883A", "C2334D", "4A90C2" }[i])
                    : new Color32(30, 32, 38, 255);
                Ui.Round(c, col, Layout.Px(6f));
            }
            if (count == 0)
                Ui.Text(new Rect(r.x, r.yMax - Layout.Px(30f), r.width, Layout.Px(24f)), Loc.S("ui.empty"),
                    Mathf.RoundToInt(Layout.Px(13f)), Ui.TextDimColor);
        }

        void AppMusic(GameRoot g, Rect r)
        {
            Ui.Text(new Rect(r.x, r.y, r.width, Layout.Px(24f)), Loc.S("phone.music"),
                Mathf.RoundToInt(Layout.Px(13f)), Ui.TextDimColor, TextAnchor.MiddleLeft);
            for (int i = 0; i < GameData.Stations.Length; i++)
            {
                RadioStation st = GameData.Stations[i];
                Rect row = new Rect(r.x, r.y + Layout.Px(30f) + i * Layout.Px(50f), r.width, Layout.Px(44f));
                bool active = AudioDirector.I.Station == i;
                Ui.Round(row, active ? Col.Hex(st.ColorHex) : new Color32(28, 32, 40, 255), Layout.Px(8f));
                Ui.Text(new Rect(row.x + Layout.Px(10f), row.y, row.width - Layout.Px(20f), row.height),
                    st.Name + " — " + st.Genre, Mathf.RoundToInt(Layout.Px(14f)),
                    active ? new Color32(20, 18, 14, 255) : Ui.TextColor, TextAnchor.MiddleLeft, FontStyle.Bold);
                if (GUI.Button(row, GUIContent.none, GUIStyle.none)) AudioDirector.I.SetStation(i);
            }
        }

        void AppBank(GameRoot g, Rect r)
        {
            Ui.Round(new Rect(r.x, r.y, r.width, Layout.Px(96f)), new Color32(28, 52, 40, 255), Layout.Px(10f));
            Ui.Text(new Rect(r.x + Layout.Px(14f), r.y + Layout.Px(10f), r.width, Layout.Px(22f)), Loc.S("phone.balance"),
                Mathf.RoundToInt(Layout.Px(13f)), Ui.TextDimColor, TextAnchor.MiddleLeft);
            Ui.Text(new Rect(r.x + Layout.Px(14f), r.y + Layout.Px(32f), r.width, Layout.Px(44f)),
                "$" + Loc.Num(g.Player.Stats.Cash), Mathf.RoundToInt(Layout.Px(30f)), Ui.GoodColor,
                TextAnchor.MiddleLeft, FontStyle.Bold);
            string[] labels =
            {
                Loc.S("mis.prologue.title"), Loc.S("mis.cash.title"), Loc.S("mis.debt.title"),
                Loc.S("mis.ride.title"), Loc.S("mis.finale.title")
            };
            for (int i = 0; i < GameData.Missions.Length; i++)
            {
                Rect row = new Rect(r.x, r.y + Layout.Px(110f) + i * Layout.Px(42f), r.width, Layout.Px(38f));
                bool done = i < g.Save.StoryIndex;
                Ui.Text(new Rect(row.x, row.y, row.width * 0.6f, row.height), labels[i],
                    Mathf.RoundToInt(Layout.Px(13f)), done ? Ui.TextColor : Ui.TextDimColor, TextAnchor.MiddleLeft);
                Ui.Text(new Rect(row.x + row.width * 0.6f, row.y, row.width * 0.4f, row.height),
                    (done ? "+" : "") + "$" + Loc.Num(GameData.Missions[i].Reward),
                    Mathf.RoundToInt(Layout.Px(13f)), done ? Ui.GoodColor : Ui.TextDimColor, TextAnchor.MiddleRight);
            }
        }

        void AppShop(GameRoot g, Rect r)
        {
            for (int i = 0; i < GameData.ShopItems.Length; i++)
            {
                GameData.ShopItem item = GameData.ShopItems[i];
                Rect row = new Rect(r.x, r.y + i * Layout.Px(52f), r.width, Layout.Px(46f));
                Ui.Round(row, new Color32(28, 32, 40, 255), Layout.Px(8f));
                Ui.Text(new Rect(row.x + Layout.Px(10f), row.y, row.width * 0.5f, row.height), item.Name,
                    Mathf.RoundToInt(Layout.Px(14f)), Ui.TextColor, TextAnchor.MiddleLeft);
                Ui.Text(new Rect(row.x + row.width * 0.45f, row.y, row.width * 0.25f, row.height), "$" + Loc.Num(item.Price),
                    Mathf.RoundToInt(Layout.Px(13f)), Ui.AccentColor, TextAnchor.MiddleRight);
                bool afford = g.Player.Stats.Cash >= item.Price;
                if (Ui.Button(new Rect(row.xMax - Layout.Px(84f), row.y + Layout.Px(6f), Layout.Px(78f), Layout.Px(34f)),
                    afford ? Loc.S("phone.buy") : "—", afford, Mathf.RoundToInt(Layout.Px(13f))))
                {
                    if (!g.BuyItem(item.Id)) g.Notify(Loc.S("phone.poor"));
                }
            }
        }

        void AppMissions(GameRoot g, Rect r)
        {
            for (int i = 0; i < GameData.Missions.Length; i++)
            {
                MissionDef m = GameData.Missions[i];
                bool done = i < g.Save.StoryIndex;
                bool current = i == g.Save.StoryIndex;
                Rect row = new Rect(r.x, r.y + i * Layout.Px(70f), r.width, Layout.Px(64f));
                Ui.Round(row, current ? new Color32(70, 46, 24, 255) : new Color32(26, 30, 36, 255), Layout.Px(8f));
                Ui.Text(new Rect(row.x + Layout.Px(10f), row.y + Layout.Px(2f), row.width - Layout.Px(20f), Layout.Px(22f)),
                    m.Title, Mathf.RoundToInt(Layout.Px(15f)), done ? Ui.TextDimColor : Ui.TextColor,
                    TextAnchor.MiddleLeft, FontStyle.Bold);
                Ui.Text(new Rect(row.x + Layout.Px(10f), row.y + Layout.Px(24f), row.width - Layout.Px(20f), Layout.Px(38f)),
                    m.Desc, Mathf.RoundToInt(Layout.Px(11f)), Ui.TextDimColor, TextAnchor.UpperLeft, FontStyle.Normal, true);
                if (current && Ui.Button(new Rect(row.xMax - Layout.Px(100f), row.yMax - Layout.Px(28f), Layout.Px(92f), Layout.Px(24f)),
                    "$" + Loc.Num(m.Reward), false, Mathf.RoundToInt(Layout.Px(11f))))
                {
                    g.StartMission(m.Id);
                    Close();
                }
                if (done)
                    Ui.Text(new Rect(row.xMax - Layout.Px(100f), row.yMax - Layout.Px(28f), Layout.Px(92f), Layout.Px(24f)),
                        "OK", Mathf.RoundToInt(Layout.Px(13f)), Ui.GoodColor, TextAnchor.MiddleCenter, FontStyle.Bold);
            }
        }

        void AppGarage(GameRoot g, Rect r)
        {
            int shown = 0;
            for (int i = 0; i < GameData.Vehicles.Length && shown < 7; i++)
            {
                VehicleDef v = GameData.Vehicles[i];
                if (v.Price <= 0) continue;
                Rect row = new Rect(r.x, r.y + shown * Layout.Px(52f), r.width, Layout.Px(46f));
                Ui.Round(row, new Color32(28, 32, 40, 255), Layout.Px(8f));
                Ui.Round(new Rect(row.x + Layout.Px(8f), row.y + Layout.Px(10f), Layout.Px(26f), Layout.Px(26f)),
                    Col.Hex(v.BodyHex), Layout.Px(6f));
                Ui.Text(new Rect(row.x + Layout.Px(42f), row.y, row.width * 0.55f, row.height), v.Name,
                    Mathf.RoundToInt(Layout.Px(13f)), Ui.TextColor, TextAnchor.MiddleLeft);
                bool afford = g.Player.Stats.Cash >= v.Price;
                if (Ui.Button(new Rect(row.xMax - Layout.Px(96f), row.y + Layout.Px(6f), Layout.Px(90f), Layout.Px(34f)),
                    "$" + Ui.ShortNumber(v.Price), afford, Mathf.RoundToInt(Layout.Px(12f))))
                {
                    if (g.BuyVehicle(v.Id)) g.Notify(Loc.S("shop.bought"));
                    else g.Notify(Loc.S("phone.poor"));
                }
                shown++;
            }
        }

        void AppWeather(GameRoot g, Rect r)
        {
            Ui.Text(new Rect(r.x, r.y, r.width, Layout.Px(40f)),
                g.WeatherRain > 0.3f ? Loc.S("phone.weather.rain") : Loc.S("phone.weather.sun"),
                Mathf.RoundToInt(Layout.Px(22f)), Ui.TextColor, TextAnchor.MiddleLeft, FontStyle.Bold);
            Ui.Text(new Rect(r.x, r.y + Layout.Px(44f), r.width, Layout.Px(24f)),
                Loc.Hour(g.Clock) + "  •  " + g.District.Name,
                Mathf.RoundToInt(Layout.Px(14f)), Ui.TextDimColor, TextAnchor.MiddleLeft);
            for (int i = 0; i < 5; i++)
            {
                Rect row = new Rect(r.x, r.y + Layout.Px(84f) + i * Layout.Px(46f), r.width, Layout.Px(40f));
                Ui.Round(row, new Color32(26, 30, 38, 255), Layout.Px(8f));
                Ui.Text(new Rect(row.x + Layout.Px(12f), row.y, row.width * 0.3f, row.height),
                    Loc.DayName(g.DayNumber + i), Mathf.RoundToInt(Layout.Px(14f)), Ui.TextColor, TextAnchor.MiddleLeft);
                Ui.Text(new Rect(row.x + row.width * 0.35f, row.y, row.width * 0.3f, row.height),
                    "+" + (29 + i) + "°", Mathf.RoundToInt(Layout.Px(14f)), Ui.AccentColor);
                Ui.Text(new Rect(row.x + row.width * 0.7f, row.y, row.width * 0.28f, row.height),
                    (i % 2 == 0) ? "SUN" : "RAIN", Mathf.RoundToInt(Layout.Px(10f)), Ui.TextColor);
            }
        }

        void AppNews(GameRoot g, Rect r)
        {
            for (int i = 1; i <= 4; i++)
            {
                Rect row = new Rect(r.x, r.y + (i - 1) * Layout.Px(78f), r.width, Layout.Px(72f));
                Ui.Round(row, new Color32(26, 30, 38, 255), Layout.Px(8f));
                Ui.Text(new Rect(row.x + Layout.Px(10f), row.y + Layout.Px(4f), row.width - Layout.Px(20f), Layout.Px(18f)),
                    "LEONIDA NOW  •  " + (18 + i) + ":40", Mathf.RoundToInt(Layout.Px(10f)), Ui.AccentColor,
                    TextAnchor.MiddleLeft, FontStyle.Bold);
                Ui.Text(new Rect(row.x + Layout.Px(10f), row.y + Layout.Px(22f), row.width - Layout.Px(20f), Layout.Px(46f)),
                    Loc.S("news." + i), Mathf.RoundToInt(Layout.Px(11f)), Ui.TextColor,
                    TextAnchor.UpperLeft, FontStyle.Normal, true);
            }
        }

        void AppBrowser(GameRoot g, Rect r)
        {
            Rect bar = new Rect(r.x, r.y, r.width, Layout.Px(36f));
            Ui.Round(bar, new Color32(40, 44, 52, 255), Layout.Px(8f));
            Ui.Text(new Rect(bar.x + Layout.Px(10f), bar.y, bar.width - Layout.Px(20f), bar.height),
                "leonet.ld", Mathf.RoundToInt(Layout.Px(13f)), Ui.TextDimColor, TextAnchor.MiddleLeft);
            Ui.Text(new Rect(r.x + Layout.Px(8f), r.y + Layout.Px(56f), r.width - Layout.Px(16f), Layout.Px(80f)),
                Loc.S("phone.search"), Mathf.RoundToInt(Layout.Px(20f)), Ui.TextColor,
                TextAnchor.UpperLeft, FontStyle.Bold, true);
            for (int i = 0; i < 4; i++)
            {
                Rect row = new Rect(r.x, r.y + Layout.Px(150f) + i * Layout.Px(40f), r.width, Layout.Px(34f));
                Ui.Text(new Rect(row.x, row.y, row.width, row.height), "· " + Loc.S("news." + (i + 1)),
                    Mathf.RoundToInt(Layout.Px(11f)), Ui.TextDimColor, TextAnchor.MiddleLeft);
            }
        }

        void AppSettings(GameRoot g, Rect r)
        {
            float y = r.y;
            float h = Layout.Px(44f);
            string lang = Loc.Current == Lang.Ru ? "Русский" : "English";
            if (Ui.Button(new Rect(r.x, y, r.width, h), Loc.S("set.language") + ":  " + lang, false, 0))
            {
                Loc.ToggleLang();
                g.Settings.Language = (int)Loc.Current;
                g.SaveSettings();
            }
            y += h + Layout.Px(6f);
            string q = Loc.S(g.Settings.Quality >= 3 ? "set.quality.ultra" : g.Settings.Quality == 2
                ? "set.quality.high" : g.Settings.Quality == 1 ? "set.quality.med" : "set.quality.low");
            if (Ui.Button(new Rect(r.x, y, r.width, h), Loc.S("set.quality") + ":  " + q, false, 0))
            {
                g.Settings.Quality = (g.Settings.Quality + 1) % 4;
                g.ApplySettings();
            }
            y += h + Layout.Px(6f);
            if (Ui.Button(new Rect(r.x, y, r.width, h), Loc.S("set.sound") + ":  " + Mathf.RoundToInt(g.Settings.SoundVolume * 100f) + "%", false, 0))
            {
                g.Settings.SoundVolume = Mathf.Repeat(g.Settings.SoundVolume + 0.2f, 1.2f);
                if (g.Settings.SoundVolume > 1f) g.Settings.SoundVolume = 0f;
                g.ApplySettings();
            }
            y += h + Layout.Px(6f);
            if (Ui.Button(new Rect(r.x, y, r.width, h), Loc.S("set.music") + ":  " + Mathf.RoundToInt(g.Settings.MusicVolume * 100f) + "%", false, 0))
            {
                g.Settings.MusicVolume = Mathf.Repeat(g.Settings.MusicVolume + 0.2f, 1.2f);
                if (g.Settings.MusicVolume > 1f) g.Settings.MusicVolume = 0f;
                g.ApplySettings();
            }
            y += h + Layout.Px(6f);
            if (Ui.Button(new Rect(r.x, y, r.width, h), Loc.S("set.sens") + ":  " + g.Settings.Sensitivity.ToString("0.0"), false, 0))
            {
                g.Settings.Sensitivity = g.Settings.Sensitivity >= 2.4f ? 0.4f : g.Settings.Sensitivity + 0.2f;
                g.ApplySettings();
            }
            y += h + Layout.Px(6f);
            if (Ui.Button(new Rect(r.x, y, r.width, h), Loc.S("set.invert") + ":  " + Loc.S(g.Settings.InvertY ? "ui.on" : "ui.off"), false, 0))
            {
                g.Settings.InvertY = !g.Settings.InvertY;
                g.ApplySettings();
            }
            y += h + Layout.Px(6f);
            if (Ui.Button(new Rect(r.x, y, r.width, h), Loc.S("set.fps") + ":  " + Loc.S(g.Settings.ShowFps ? "ui.on" : "ui.off"), false, 0))
            {
                g.Settings.ShowFps = !g.Settings.ShowFps;
                g.ApplySettings();
            }
        }

        // ----------------------------------------------------------------- call

        void DrawCall(GameRoot g, Rect body)
        {
            Contact c = GameData.FindContact(_called);
            Ui.Text(new Rect(body.x, body.y + Layout.Px(30f), body.width, Layout.Px(30f)), c.Name,
                Mathf.RoundToInt(Layout.Px(24f)), Ui.TextColor, TextAnchor.MiddleCenter, FontStyle.Bold);
            Ui.Text(new Rect(body.x, body.y + Layout.Px(64f), body.width, Layout.Px(22f)), c.Number,
                Mathf.RoundToInt(Layout.Px(13f)), Ui.TextDimColor);
            Ui.Circle(new Vector2(body.center.x, body.y + Layout.Px(140f)), Layout.Px(38f), Col.Hex("2E6B47"));
            Ui.Text(new Rect(body.x, body.y + Layout.Px(126f), body.width, Layout.Px(28f)), "TEL",
                Mathf.RoundToInt(Layout.Px(22f)), Ui.TextColor, TextAnchor.MiddleCenter, FontStyle.Bold);

            string[] lines = c.Messages;
            int shown = Mathf.Clamp(Mathf.FloorToInt(_callTime / 1.6f), 0, lines != null ? lines.Length : 0);
            if (lines != null)
                for (int i = 0; i < shown && i < 4; i++)
                {
                    Rect row = new Rect(body.x, body.y + Layout.Px(200f) + i * Layout.Px(56f), body.width, Layout.Px(50f));
                    Ui.Round(row, new Color32(30, 34, 42, 255), Layout.Px(8f));
                    Ui.Text(new Rect(row.x + Layout.Px(10f), row.y + Layout.Px(4f), row.width - Layout.Px(20f), Layout.Px(42f)),
                        lines[i], Mathf.RoundToInt(Layout.Px(13f)), Ui.TextColor, TextAnchor.UpperLeft, FontStyle.Normal, true);
                }
            if (shown == 0)
                Ui.Text(new Rect(body.x, body.y + Layout.Px(210f), body.width, Layout.Px(24f)), Loc.S("phone.busy"),
                    Mathf.RoundToInt(Layout.Px(13f)), Ui.TextDimColor);

            if (Ui.Button(new Rect(body.x, body.yMax - Layout.Px(56f), body.width, Layout.Px(46f)),
                Loc.S("phone.hangup"), false))
            {
                _called = null;
                AudioDirector.I.PlayUi();
            }
        }

        void Pager(Rect r, int pages, int perPage)
        {
            if (pages <= 1) return;
            if (Ui.Button(new Rect(r.x, r.yMax - Layout.Px(36f), Layout.Px(64f), Layout.Px(32f)), "<", false))
                _page = (_page - 1 + pages) % pages;
            if (Ui.Button(new Rect(r.xMax - Layout.Px(64f), r.yMax - Layout.Px(36f), Layout.Px(64f), Layout.Px(32f)), ">", false))
                _page = (_page + 1) % pages;
            Ui.Text(new Rect(r.x + Layout.Px(70f), r.yMax - Layout.Px(36f), r.width - Layout.Px(140f), Layout.Px(32f)),
                (_page + 1) + " / " + pages, Mathf.RoundToInt(Layout.Px(12f)), Ui.TextDimColor);
        }
    }
}
