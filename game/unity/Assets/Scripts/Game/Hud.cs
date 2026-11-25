using System;
using System.Collections.Generic;
using UnityEngine;
using GTA6.Core;

namespace GTA6.Game
{
    public struct Notice
    {
        public string Text;
        public float Life;
        public float Max;
        public Color32 Color;
    }

    public sealed class Hud
    {
        public void Draw(GameRoot g)
        {
            Ui.Init();
            if (g.Phase != Phase.Playing && g.Phase != Phase.Paused && g.Phase != Phase.Dead &&
                g.Phase != Phase.Busted && g.Phase != Phase.MissionPassed && g.Phase != Phase.MissionFailed)
                return;
            if (g.Phone != null && g.Phone.Open) return;

            float s = Layout.S;
            DrawMinimap(g);
            DrawStatus(g);
            DrawWanted(g);
            DrawObjective(g);
            DrawButtons(g);
            DrawNotifications(g);
            DrawSubtitle(g);
            if (g.Phase == Phase.Dead) DrawWasted(g, false);
            else if (g.Phase == Phase.Busted) DrawWasted(g, true);
            if (g.Settings.ShowFps)
                Ui.Text(new Rect(Layout.Px(10f), Screen.height - Layout.Px(34f), Layout.Px(160f), Layout.Px(24f)),
                    Mathf.RoundToInt(1f / Mathf.Max(0.0001f, g.FrameDelta)) + " fps",
                    Mathf.RoundToInt(Layout.Px(16f)), Ui.TextDimColor, TextAnchor.MiddleLeft);
        }

        // ------------------------------------------------------------- minimap

        void DrawMinimap(GameRoot g)
        {
            Rect r = Layout.MinimapRect;
            float pad = Layout.Px(4f);
            Ui.Round(new Rect(r.x - pad, r.y - pad, r.width + pad * 2f, r.height + pad * 2f),
                new Color32(10, 12, 16, 210), Layout.Px(14f));

            const float viewMeters = 240f;
            float extent = World.Extent;
            Vector3 p = g.Player.Pos;
            float ux = (p.x - viewMeters * 0.5f + extent) / (extent * 2f);
            float uy = (p.z - viewMeters * 0.5f + extent) / (extent * 2f);
            float uw = viewMeters / (extent * 2f);
            Rect uv = new Rect(ux, uy, uw, uw);
            if (g.World.MapTexture != null)
                Ui.RotatedMap(r, g.World.MapTexture, uv, -g.Player.Heading, r.center.x, r.center.y);

            // blips
            Vector2 center = r.center;
            float half = r.width * 0.5f;
            float scale = half / (viewMeters * 0.5f);
            float rad = -g.Player.Heading * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad), sin = Mathf.Sin(rad);

            DrawBlip(g, center, scale, cos, sin, g.Player.Pos, Col.Hex("F2A03D"), 6f, true);
            if (g.MissionHasTarget)
                DrawBlip(g, center, scale, cos, sin, g.MissionTarget, Col.Hex("F2D24E"), 5.5f, false);

            for (int i = 0; i < g.Traffic.Cars.Count; i++)
                DrawBlip(g, center, scale, cos, sin, g.Traffic.Cars[i].Pos, new Color32(214, 214, 214, 190), 2.4f, false);
            for (int i = 0; i < g.Police.Cruisers.Count; i++)
                DrawBlip(g, center, scale, cos, sin, g.Police.Cruisers[i].Pos, Col.Hex("3B7BFF"), 3.2f, false);
            for (int i = 0; i < Actors.Cops.Count; i++)
                DrawBlip(g, center, scale, cos, sin, Actors.Cops[i].Pos, Col.Hex("3B7BFF"), 2.6f, false);

            Ui.Arrow(center, Layout.Px(9f), Col.Hex("FFFFFF"), 0f);
        }

        void DrawBlip(GameRoot g, Vector2 center, float scale, float cos, float sin, Vector3 world, Color32 color, float size, bool clampToEdge)
        {
            float dx = (world.x - g.Player.Pos.x) * scale;
            float dy = -(world.z - g.Player.Pos.z) * scale;
            float sx = dx * cos - dy * sin;
            float sy = dx * sin + dy * cos;
            Vector2 pos = center + new Vector2(sx, sy);
            float radius = Layout.Px(size);
            float maxR = Layout.MinimapRect.width * 0.47f;
            if (clampToEdge)
            {
                Vector2 d = pos - center;
                if (d.magnitude > maxR) pos = center + d.normalized * maxR;
            }
            else if ((pos - center).magnitude > maxR) return;
            Ui.Dot(pos, radius, color);
        }

        // -------------------------------------------------------------- status

        void DrawStatus(GameRoot g)
        {
            Rect mm = Layout.MinimapRect;
            float y = mm.yMax + Layout.Px(10f);
            float w = mm.width;
            int ts = Mathf.RoundToInt(Layout.Px(15f));

            // health
            Rect hb = new Rect(mm.x, y, w * 0.62f, Layout.Px(10f));
            Ui.Bar(hb, g.Player.Stats.Health / 100f, Ui.GoodColor, new Color32(24, 28, 26, 210));
            // armour above the health bar
            Rect ab = new Rect(mm.x, y - Layout.Px(13f), w * 0.62f, Layout.Px(7f));
            Ui.Bar(ab, g.Player.Stats.Armor / 100f, Col.Hex("6FA8DC"), new Color32(24, 26, 34, 200));

            // cash
            Ui.TextShadow(new Rect(mm.x + w * 0.66f, y - Layout.Px(12f), w * 0.6f, Layout.Px(24f)),
                "$" + Loc.Num(g.Player.Stats.Cash), ts, Ui.GoodColor, TextAnchor.MiddleLeft, FontStyle.Bold);

            // weapon and ammo, bottom left of the minimap column
            WeaponState wp = g.Player.Weapon;
            if (wp != null)
            {
                string ammo = wp.Melee ? "-" : (wp.Magazine + " / " + wp.Reserve);
                Ui.TextShadow(new Rect(mm.x, y + Layout.Px(14f), w * 1.4f, Layout.Px(22f)),
                    wp.Def.Name, ts, Ui.TextColor, TextAnchor.MiddleLeft);
                Ui.TextShadow(new Rect(mm.x, y + Layout.Px(32f), w * 0.8f, Layout.Px(20f)),
                    ammo, Mathf.RoundToInt(Layout.Px(14f)), Ui.TextDimColor, TextAnchor.MiddleLeft);
            }

            // clock and district, top right under the map button
            string clock = Loc.Hour(g.Clock);
            District d = g.District;
            float rx = Screen.width - Layout.Px(24f);
            Ui.TextShadow(new Rect(rx - Layout.Px(260f), Layout.Px(18f), Layout.Px(236f), Layout.Px(24f)),
                clock + "  •  " + Loc.DayName(g.DayNumber), Mathf.RoundToInt(Layout.Px(17f)),
                Ui.TextColor, TextAnchor.MiddleRight, FontStyle.Bold);
            Ui.TextShadow(new Rect(rx - Layout.Px(260f), Layout.Px(40f), Layout.Px(236f), Layout.Px(22f)),
                d != null ? d.Name : "", Mathf.RoundToInt(Layout.Px(14f)), Ui.TextDimColor, TextAnchor.MiddleRight);
        }

        void DrawWanted(GameRoot g)
        {
            int stars = g.Wanted.Stars;
            float size = Layout.Px(15f);
            float x = Layout.Px(20f);
            float y = Layout.MinimapRect.yMax + Layout.Px(40f) + Layout.Px(20f);
            for (int i = 0; i < 5; i++)
            {
                Color32 c = i < stars ? Col.Hex("F2D24E") : new Color32(70, 70, 74, 170);
                if (i < stars && Mathf.Repeat(Time.time * 3f, 1f) > 0.5f)
                    c = i < stars ? Col.Hex("FFF0A0") : c;
                Ui.Star(new Vector2(x + i * size * 2.1f, y), size, c);
            }
        }

        void DrawObjective(GameRoot g)
        {
            MissionStep step = g.Mission.Step;
            if (step == null || !g.Mission.Active) return;
            float w = Mathf.Min(Screen.width * 0.62f, Layout.Px(520f));
            Rect r = new Rect((Screen.width - w) * 0.5f, Layout.Px(16f), w, Layout.Px(52f));
            if (g.Phase == Phase.Playing) Ui.Panel(r, false);
            string text = step.Text;
            float dist = M.Dist2D(g.Player.Pos.x, g.Player.Pos.z, step.X, step.Z);
            string sub = Mathf.RoundToInt(dist) + " " + Loc.S("hud.distance");
            if (step.Kind == StepKind.Kill) sub = g.Mission.Kills + " / " + step.Count;
            else if (step.Kind == StepKind.Survive) sub = Loc.Clock(Mathf.Max(0f, step.Seconds - g.Mission.Timer));
            else if (step.Kind == StepKind.Escape) sub = Loc.Clock(Mathf.Max(0f, step.Seconds - g.Mission.Timer));

            Ui.Text(new Rect(r.x + Layout.Px(14f), r.y + Layout.Px(6f), w - Layout.Px(28f), Layout.Px(22f)),
                text, Mathf.RoundToInt(Layout.Px(16f)), Ui.TextColor, TextAnchor.MiddleLeft, FontStyle.Bold);
            Ui.Text(new Rect(r.x + Layout.Px(14f), r.y + Layout.Px(26f), w - Layout.Px(28f), Layout.Px(20f)),
                sub, Mathf.RoundToInt(Layout.Px(14f)), Ui.AccentColor, TextAnchor.MiddleLeft);
            Ui.Bar(new Rect(r.x + Layout.Px(14f), r.yMax - Layout.Px(8f), w - Layout.Px(28f), Layout.Px(4f)),
                g.Mission.Progress, Ui.AccentColor, new Color32(40, 40, 44, 200));
        }

        // ------------------------------------------------------------- buttons

        void DrawButtons(GameRoot g)
        {
            InputState input = g.Input;
            bool driving = g.Player.Vehicle != null;
            float radius = Layout.Btn("fire").width * 0.5f;

            DrawTouchButton(input, "fire", "FIRE", Col.Hex("D9543C"), true);
            DrawTouchButton(input, "aim", g.Player.Aiming ? "AIM*" : "AIM", Col.Hex("4A6E8E"), false);
            DrawTouchButton(input, "reload", "RLD", Col.Hex("4E6B52"), false);
            if (driving)
            {
                DrawTouchButton(input, "car", "EXIT", Col.Hex("6E5A3C"), false);
                DrawTouchButton(input, "horn", "HORN", Col.Hex("5A5A62"), false);
            }
            else
            {
                DrawTouchButton(input, "jump", "JUMP", Col.Hex("5A5A62"), false);
                bool near = g.NearbyVehicle != null;
                DrawTouchButton(input, "car", near ? "CAR" : "CAR", near ? Col.Hex("3F7A5E") : Col.Hex("3A3F46"), false);
                DrawTouchButton(input, "weapon", "SWAP", Col.Hex("4A4A52"), false);
            }

            DrawTouchButton(input, "phone", "TEL", Col.Hex("3E5A78"), false);
            DrawTouchButton(input, "map", "MAP", Col.Hex("3E5A78"), false);
            DrawTouchButton(input, "pause", "II", Col.Hex("44474E"), false);
            if (g.CanStartMission) DrawTouchButton(input, "mission", "JOB", Col.Hex("B0752E"), false);

            // joystick
            if (input.StickVisible)
            {
                float r = Layout.StickRadius;
                Ui.Circle(input.StickCenter, r, new Color32(20, 22, 28, 90));
                Ui.Ring(input.StickCenter, r, new Color32(240, 240, 240, 110));
                Vector2 knob = input.StickCenter + input.StickKnob * r;
                Ui.Circle(knob, r * 0.38f, new Color32(242, 160, 61, 190));
            }
            else
            {
                Vector2 home = Layout.StickHome;
                Ui.Circle(home, Layout.StickRadius * 0.85f, new Color32(20, 22, 28, 60));
                Ui.Ring(home, Layout.StickRadius * 0.85f, new Color32(240, 240, 240, 60));
                Ui.Text(new Rect(home.x - 60f, home.y - 10f, 120f, 20f), "MOVE",
                    Mathf.RoundToInt(Layout.Px(13f)), new Color32(220, 220, 220, 110));
            }

            // interaction prompt over the car
            if (g.NearbyVehicle != null && g.Player.Vehicle == null)
            {
                Ui.TextShadow(new Rect(Screen.width * 0.5f - Layout.Px(150f), Screen.height - Layout.Px(120f),
                    Layout.Px(300f), Layout.Px(26f)), Loc.S("hud.enter") + "  [CAR]",
                    Mathf.RoundToInt(Layout.Px(16f)), Ui.AccentColor);
            }

            if (radius <= 0f) return;
        }

        void DrawTouchButton(InputState input, string id, string label, Color32 color, bool big)
        {
            Rect r = Layout.Btn(id);
            if (r.width <= 1f) return;
            Vector2 center = r.center;
            float radius = r.width * 0.5f;
            bool held = input.Held(id);
            Color32 c = held ? Ui.Lighten(color, 0.18f) : color;
            Ui.Circle(center, radius, new Color32(c.r, c.g, c.b, (byte)(held ? 235 : 180)));
            Ui.Ring(center, radius, new Color32(255, 255, 255, (byte)(held ? 150 : 70)));
            Ui.Text(new Rect(center.x - radius, center.y - radius * 0.5f, radius * 2f, radius),
                label, Mathf.RoundToInt(radius * (big ? 0.46f : 0.42f)), new Color32(255, 255, 255, 240),
                TextAnchor.MiddleCenter, FontStyle.Bold);
        }

        // -------------------------------------------------------- text overlays

        void DrawNotifications(GameRoot g)
        {
            float y = Screen.height * 0.30f;
            for (int i = g.Notifications.Count - 1; i >= 0; i--)
            {
                Notice n = g.Notifications[i];
                float a = Mathf.Clamp01(n.Life / 0.6f);
                float w = Mathf.Min(Screen.width * 0.7f, Layout.Px(420f));
                Rect r = new Rect((Screen.width - w) * 0.5f, y, w, Layout.Px(34f));
                Ui.Round(r, new Color32(12, 14, 18, (byte)(190 * a)));
                Ui.Text(r, n.Text, Mathf.RoundToInt(Layout.Px(17f)), Ui.Fade(n.Color, a), TextAnchor.MiddleCenter, FontStyle.Bold);
                y -= Layout.Px(40f);
            }
        }

        void DrawSubtitle(GameRoot g)
        {
            if (string.IsNullOrEmpty(g.Subtitle) || g.SubtitleTimer <= 0f) return;
            float w = Mathf.Min(Screen.width * 0.86f, Layout.Px(720f));
            Rect r = new Rect((Screen.width - w) * 0.5f, Screen.height - Layout.Px(150f), w, Layout.Px(60f));
            Ui.Round(r, new Color32(10, 12, 16, 190));
            Ui.Text(new Rect(r.x + Layout.Px(12f), r.y, r.width - Layout.Px(24f), r.height), g.Subtitle,
                Mathf.RoundToInt(Layout.Px(17f)), Ui.TextColor, TextAnchor.MiddleCenter, FontStyle.Normal, true);
        }

        void DrawWasted(GameRoot g, bool busted)
        {
            Rect full = new Rect(0, 0, Screen.width, Screen.height);
            Ui.Fill(full, new Color(0f, 0f, 0f, 0.55f));
            float w = Screen.width;
            Ui.TextShadow(new Rect(0f, Screen.height * 0.34f, w, Layout.Px(70f)),
                Loc.S(busted ? "hud.busted" : "hud.wasted"), Mathf.RoundToInt(Layout.Px(46f)),
                busted ? Col.Hex("7FB3D5") : Col.Hex("C0392B"), TextAnchor.MiddleCenter, FontStyle.Bold);

            float bw = Mathf.Min(Screen.width * 0.6f, Layout.Px(320f));
            Rect b = new Rect((Screen.width - bw) * 0.5f, Screen.height * 0.52f, bw, Layout.Px(60f));
            if (Ui.Button(b, Loc.S("death.retry"), true))
                g.RespawnPlayer();
        }
    }
}
