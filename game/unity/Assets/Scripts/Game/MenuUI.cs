using System;
using UnityEngine;
using GTA6.Core;

namespace GTA6.Game
{
    public sealed class MenuUI
    {
        public void Draw(GameRoot g)
        {
            Ui.Init();
            switch (g.Phase)
            {
                case Phase.Loading: DrawLoading(g); break;
                case Phase.MainMenu: DrawMain(g); break;
                case Phase.Settings: DrawSettings(g); break;
                case Phase.Controls: DrawControls(g); break;
                case Phase.About: DrawAbout(g); break;
                case Phase.Paused: DrawPause(g); break;
                case Phase.MissionPassed: DrawMissionResult(g, true); break;
                case Phase.MissionFailed: DrawMissionResult(g, false); break;
            }
        }

        static void Backdrop()
        {
            Ui.Fill(new Rect(0, 0, Screen.width, Screen.height), new Color(0.04f, 0.05f, 0.07f, 0.94f));
            // warm gradient band
            for (int i = 0; i < 6; i++)
            {
                float a = 0.06f - i * 0.009f;
                Ui.Fill(new Rect(0, Screen.height * 0.28f + i * Screen.height * 0.05f, Screen.width, Screen.height * 0.05f),
                    new Color(0.95f, 0.55f, 0.25f, Mathf.Max(0f, a)));
            }
        }

        static Rect Column(float widthPx, float startY, float h)
        {
            float w = Mathf.Min(Screen.width * 0.82f, widthPx);
            return new Rect((Screen.width - w) * 0.5f, startY, w, h);
        }

        void DrawLoading(GameRoot g)
        {
            Ui.Fill(new Rect(0, 0, Screen.width, Screen.height), new Color(0.03f, 0.04f, 0.06f, 1f));
            Ui.TextShadow(new Rect(0, Screen.height * 0.42f, Screen.width, Layout.Px(40f)), Loc.S("game.title"),
                Mathf.RoundToInt(Layout.Px(30f)), Ui.TextColor, TextAnchor.MiddleCenter, FontStyle.Bold);
            Ui.Text(new Rect(0, Screen.height * 0.42f + Layout.Px(44f), Screen.width, Layout.Px(28f)),
                Loc.S("ui.loading") + " " + Mathf.RoundToInt(g.LoadProgress * 100f) + "%",
                Mathf.RoundToInt(Layout.Px(15f)), Ui.TextDimColor);
            Ui.Bar(new Rect(Screen.width * 0.25f, Screen.height * 0.42f + Layout.Px(76f), Screen.width * 0.5f, Layout.Px(6f)),
                g.LoadProgress, Ui.AccentColor, new Color32(40, 44, 52, 255));
        }

        void DrawMain(GameRoot g)
        {
            Backdrop();
            int titleSize = Mathf.RoundToInt(Layout.Px(46f));
            Ui.TextShadow(new Rect(0, Screen.height * 0.14f, Screen.width, Layout.Px(60f)), "LEONIDA",
                titleSize, Ui.TextColor, TextAnchor.MiddleCenter, FontStyle.Bold);
            Ui.Text(new Rect(0, Screen.height * 0.14f + Layout.Px(56f), Screen.width, Layout.Px(28f)),
                Loc.S("game.subtitle"), Mathf.RoundToInt(Layout.Px(15f)), Ui.AccentColor);
            Ui.Text(new Rect(0, Screen.height * 0.14f + Layout.Px(80f), Screen.width, Layout.Px(22f)),
                Loc.S("menu.legal"), Mathf.RoundToInt(Layout.Px(10f)), Ui.TextDimColor);

            float y = Screen.height * 0.40f;
            float h = Layout.Px(52f), gap = Layout.Px(12f), w = Mathf.Min(Screen.width * 0.7f, Layout.Px(380f));
            if (Ui.Button(new Rect((Screen.width - w) * 0.5f, y, w, h), Loc.S("menu.newgame"), true, 0))
                g.StartNewGame();
            y += h + gap;
            if (g.HasSave)
            {
                if (Ui.Button(new Rect((Screen.width - w) * 0.5f, y, w, h), Loc.S("menu.continue"), false, 0))
                    g.ContinueGame();
                y += h + gap;
            }
            if (Ui.Button(new Rect((Screen.width - w) * 0.5f, y, w, h), Loc.S("menu.settings"), false, 0)) g.SetPhase(Phase.Settings);
            y += h + gap;
            if (Ui.Button(new Rect((Screen.width - w) * 0.5f, y, w, h), Loc.S("menu.controls"), false, 0)) g.SetPhase(Phase.Controls);
            y += h + gap;
            if (Ui.Button(new Rect((Screen.width - w) * 0.5f, y, w, h), Loc.S("menu.credits"), false, 0)) g.SetPhase(Phase.About);

            Ui.Text(new Rect(0, Screen.height - Layout.Px(44f), Screen.width, Layout.Px(24f)),
                "v1.0  •  Unity " + Application.unityVersion, Mathf.RoundToInt(Layout.Px(11f)), Ui.TextDimColor);
        }

        void DrawSettings(GameRoot g)
        {
            Backdrop();
            float w = Mathf.Min(Screen.width * 0.88f, Layout.Px(520f));
            float x = (Screen.width - w) * 0.5f;
            float y = Screen.height * 0.10f;
            Ui.Text(new Rect(x, y, w, Layout.Px(34f)), Loc.S("menu.settings"),
                Mathf.RoundToInt(Layout.Px(24f)), Ui.TextColor, TextAnchor.MiddleLeft, FontStyle.Bold);
            y += Layout.Px(46f);
            float rowH = Layout.Px(46f), gap = Layout.Px(8f);

            Setting(g, Loc.S("set.quality"), Loc.S(g.Settings.Quality >= 3 ? "set.quality.ultra"
                : g.Settings.Quality == 2 ? "set.quality.high" : g.Settings.Quality == 1 ? "set.quality.med"
                : "set.quality.low"), x, ref y, w, rowH, gap, () =>
            {
                g.Settings.Quality = (g.Settings.Quality + 1) % 4;
                g.ApplySettings();
            });
            Setting(g, Loc.S("set.language"), Loc.Current == Lang.Ru ? "Русский" : "English", x, ref y, w, rowH, gap, () =>
            {
                Loc.ToggleLang();
                g.Settings.Language = (int)Loc.Current;
                g.ApplySettings();
            });
            Slider(g, Loc.S("set.sound"), g.Settings.SoundVolume, x, ref y, w, rowH, gap, v =>
            {
                g.Settings.SoundVolume = v;
                g.ApplySettings();
            });
            Slider(g, Loc.S("set.music"), g.Settings.MusicVolume, x, ref y, w, rowH, gap, v =>
            {
                g.Settings.MusicVolume = v;
                g.ApplySettings();
            });
            Slider(g, Loc.S("set.sens"), (g.Settings.Sensitivity - 0.2f) / 2.3f, x, ref y, w, rowH, gap, v =>
            {
                g.Settings.Sensitivity = 0.2f + v * 2.3f;
                g.ApplySettings();
            });
            Toggle(g, Loc.S("set.invert"), g.Settings.InvertY, x, ref y, w, rowH, gap, () =>
            {
                g.Settings.InvertY = !g.Settings.InvertY;
                g.ApplySettings();
            });
            Toggle(g, Loc.S("set.shadows"), g.Settings.Shadows, x, ref y, w, rowH, gap, () =>
            {
                g.Settings.Shadows = !g.Settings.Shadows;
                g.ApplySettings();
            });
            Toggle(g, Loc.S("set.autoaim"), g.Settings.AutoAim, x, ref y, w, rowH, gap, () =>
            {
                g.Settings.AutoAim = !g.Settings.AutoAim;
                g.ApplySettings();
            });
            Toggle(g, Loc.S("set.fps"), g.Settings.ShowFps, x, ref y, w, rowH, gap, () =>
            {
                g.Settings.ShowFps = !g.Settings.ShowFps;
                g.ApplySettings();
            });

            Rect back = new Rect(x, y + Layout.Px(6f), w, Layout.Px(50f));
            if (Ui.Button(back, Loc.S("ui.back"), true, 0)) g.CloseOverlay();
        }

        void Setting(GameRoot g, string label, string value, float x, ref float y, float w, float h, float gap, Action act)
        {
            Rect r = new Rect(x, y, w, h);
            Ui.Panel(r, true);
            Ui.Text(new Rect(r.x + Layout.Px(14f), r.y, r.width * 0.6f, r.height), label,
                Mathf.RoundToInt(Layout.Px(15f)), Ui.TextColor, TextAnchor.MiddleLeft);
            Ui.Text(new Rect(r.x + r.width * 0.55f, r.y, r.width * 0.4f, r.height), value,
                Mathf.RoundToInt(Layout.Px(15f)), Ui.AccentColor, TextAnchor.MiddleRight, FontStyle.Bold);
            if (GUI.Button(r, GUIContent.none, GUIStyle.none)) act();
            y += h + gap;
        }

        void Slider(GameRoot g, string label, float value, float x, ref float y, float w, float h, float gap, Action<float> set)
        {
            Rect r = new Rect(x, y, w, h);
            Ui.Panel(r, true);
            Ui.Text(new Rect(r.x + Layout.Px(14f), r.y, r.width * 0.5f, r.height), label,
                Mathf.RoundToInt(Layout.Px(15f)), Ui.TextColor, TextAnchor.MiddleLeft);
            Rect track = new Rect(r.x + r.width * 0.5f, r.center.y - Layout.Px(4f), r.width * 0.46f - Layout.Px(14f), Layout.Px(8f));
            Ui.Bar(track, value, Ui.AccentColor, new Color32(44, 48, 56, 255));
            Event e = Event.current;
            if ((e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && track.Contains(e.mousePosition))
            {
                float v = Mathf.Clamp01((e.mousePosition.x - track.x) / track.width);
                set(v);
                e.Use();
            }
            y += h + gap;
        }

        void Toggle(GameRoot g, string label, bool on, float x, ref float y, float w, float h, float gap, Action act)
        {
            Rect r = new Rect(x, y, w, h);
            Ui.Panel(r, true);
            Ui.Text(new Rect(r.x + Layout.Px(14f), r.y, r.width * 0.6f, r.height), label,
                Mathf.RoundToInt(Layout.Px(15f)), Ui.TextColor, TextAnchor.MiddleLeft);
            Rect pill = new Rect(r.xMax - Layout.Px(76f), r.center.y - Layout.Px(13f), Layout.Px(56f), Layout.Px(26f));
            Ui.Round(pill, on ? Ui.AccentColor : new Color32(56, 60, 68, 255), pill.height * 0.5f);
            Ui.Circle(new Vector2(on ? pill.xMax - pill.height * 0.5f : pill.x + pill.height * 0.5f, pill.center.y),
                pill.height * 0.38f, new Color32(250, 250, 250, 255));
            if (GUI.Button(r, GUIContent.none, GUIStyle.none)) act();
            y += h + gap;
        }

        void DrawControls(GameRoot g)
        {
            Backdrop();
            float w = Mathf.Min(Screen.width * 0.88f, Layout.Px(560f));
            float x = (Screen.width - w) * 0.5f;
            float y = Screen.height * 0.12f;
            Ui.Text(new Rect(x, y, w, Layout.Px(34f)), Loc.S("menu.controls"),
                Mathf.RoundToInt(Layout.Px(24f)), Ui.TextColor, TextAnchor.MiddleLeft, FontStyle.Bold);
            y += Layout.Px(46f);
            string[] keys = { "ctl.move", "ctl.look", "ctl.run", "ctl.jump", "ctl.fire", "ctl.aim", "ctl.car", "ctl.phone", "ctl.keys" };
            for (int i = 0; i < keys.Length; i++)
            {
                Rect r = new Rect(x, y, w, Layout.Px(38f));
                Ui.Text(new Rect(r.x, r.y, r.width, r.height), "•  " + Loc.S(keys[i]),
                    Mathf.RoundToInt(Layout.Px(13f)), i == keys.Length - 1 ? Ui.AccentColor : Ui.TextColor,
                    TextAnchor.MiddleLeft, FontStyle.Normal, true);
                y += Layout.Px(34f);
            }
            if (Ui.Button(new Rect(x, y + Layout.Px(18f), w, Layout.Px(50f)), Loc.S("ui.back"), true, 0)) g.CloseOverlay();
        }

        void DrawAbout(GameRoot g)
        {
            Backdrop();
            float w = Mathf.Min(Screen.width * 0.88f, Layout.Px(560f));
            float x = (Screen.width - w) * 0.5f;
            float y = Screen.height * 0.14f;
            Ui.Text(new Rect(x, y, w, Layout.Px(34f)), Loc.S("game.title"),
                Mathf.RoundToInt(Layout.Px(26f)), Ui.TextColor, TextAnchor.MiddleLeft, FontStyle.Bold);
            y += Layout.Px(46f);
            string[] lines =
            {
                Loc.S("game.subtitle"),
                Loc.S("menu.legal"),
                "Unity " + Application.unityVersion,
                Loc.S("menu.legal"),
            };
            for (int i = 0; i < lines.Length; i++)
            {
                Ui.Text(new Rect(x, y, w, Layout.Px(56f)), lines[i], Mathf.RoundToInt(Layout.Px(13f)),
                    Ui.TextColor, TextAnchor.UpperLeft, FontStyle.Normal, true);
                y += Layout.Px(52f);
            }
            string stats = "districts: " + GameData.Districts.Length + "   •   missions: " + GameData.Missions.Length
                + "   •   vehicles: " + GameData.Vehicles.Length + "   •   apps: " + GameData.Apps.Length
                + "   •   contacts: " + GameData.Contacts.Length;
            Ui.Text(new Rect(x, y, w, Layout.Px(60f)), stats, Mathf.RoundToInt(Layout.Px(11f)),
                Ui.TextDimColor, TextAnchor.UpperLeft, FontStyle.Normal, true);
            if (Ui.Button(new Rect(x, y + Layout.Px(70f), w, Layout.Px(50f)), Loc.S("ui.back"), true, 0)) g.CloseOverlay();
        }

        void DrawPause(GameRoot g)
        {
            Ui.Fill(new Rect(0, 0, Screen.width, Screen.height), new Color(0.02f, 0.03f, 0.04f, 0.82f));
            float w = Mathf.Min(Screen.width * 0.7f, Layout.Px(380f));
            float x = (Screen.width - w) * 0.5f;
            float y = Screen.height * 0.24f;
            Ui.Text(new Rect(0, y - Layout.Px(60f), Screen.width, Layout.Px(40f)), Loc.S("ui.pause"),
                Mathf.RoundToInt(Layout.Px(28f)), Ui.TextColor, TextAnchor.MiddleCenter, FontStyle.Bold);
            float h = Layout.Px(50f), gap = Layout.Px(12f);
            if (Ui.Button(new Rect(x, y, w, h), Loc.S("ui.resume"), true, 0)) g.Resume();
            y += h + gap;
            if (Ui.Button(new Rect(x, y, w, h), Loc.S("ui.save"), false, 0)) { g.SaveGame(); g.Notify(Loc.S("ui.saved"), Ui.GoodColor); }
            y += h + gap;
            if (Ui.Button(new Rect(x, y, w, h), Loc.S("menu.settings"), false, 0)) g.SetPhase(Phase.Settings);
            y += h + gap;
            if (Ui.Button(new Rect(x, y, w, h), Loc.S("menu.controls"), false, 0)) g.SetPhase(Phase.Controls);
            y += h + gap;
            if (Ui.Button(new Rect(x, y, w, h), Loc.S("menu.quit"), false, 0)) g.QuitToMenu();
        }

        void DrawMissionResult(GameRoot g, bool passed)
        {
            Ui.Fill(new Rect(0, 0, Screen.width, Screen.height), new Color(0f, 0f, 0f, 0.6f));
            string title = Loc.S(passed ? "hud.mission.pass" : "hud.mission.fail");
            Ui.TextShadow(new Rect(0, Screen.height * 0.28f, Screen.width, Layout.Px(50f)), title,
                Mathf.RoundToInt(Layout.Px(32f)), passed ? Ui.GoodColor : Ui.BadColor, TextAnchor.MiddleCenter, FontStyle.Bold);
            string name = g.Mission.Def != null ? g.Mission.Def.Title : "";
            Ui.Text(new Rect(0, Screen.height * 0.28f + Layout.Px(50f), Screen.width, Layout.Px(28f)), name,
                Mathf.RoundToInt(Layout.Px(16f)), Ui.TextColor);
            if (passed)
                Ui.Text(new Rect(0, Screen.height * 0.28f + Layout.Px(80f), Screen.width, Layout.Px(30f)),
                    Loc.S("hud.reward") + ": $" + Loc.Num(g.LastReward), Mathf.RoundToInt(Layout.Px(18f)), Ui.AccentColor);
            else
                Ui.Text(new Rect(0, Screen.height * 0.28f + Layout.Px(80f), Screen.width, Layout.Px(30f)),
                    Loc.S(string.IsNullOrEmpty(g.FailureKey) ? "step.fail.dead" : g.FailureKey),
                    Mathf.RoundToInt(Layout.Px(15f)), Ui.TextDimColor);

            float w = Mathf.Min(Screen.width * 0.66f, Layout.Px(320f));
            float x = (Screen.width - w) * 0.5f;
            float y = Screen.height * 0.56f;
            if (Ui.Button(new Rect(x, y, w, Layout.Px(52f)), passed ? Loc.S("ui.confirm") : Loc.S("ui.restart"), true, 0))
            {
                if (passed) g.ContinueAfterMission();
                else g.RetryMission();
            }
            y += Layout.Px(64f);
            if (Ui.Button(new Rect(x, y, w, Layout.Px(46f)), Loc.S("menu.quit"), false, 0)) g.QuitToMenu();
        }
    }
}
