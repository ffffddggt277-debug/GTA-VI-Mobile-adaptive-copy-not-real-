using System;
using UnityEngine;
using GTA6.Core;

namespace GTA6.Game
{
    public sealed class MapScreen
    {
        public bool Open { get; private set; }
        public float Zoom = 1f;
        Vector2 _center;
        Vector2 _dragStart;
        bool _dragging;
        float _viewKm = 3.2f;

        public void Toggle(GameRoot g)
        {
            if (Open) Close(); else Show(g);
        }

        public void Show(GameRoot g)
        {
            Open = true;
            _center = new Vector2(g.Player.Pos.x, g.Player.Pos.z);
            AudioDirector.I.PlayUi();
        }

        public void Close()
        {
            Open = false;
            AudioDirector.I.PlayUi();
        }

        public void Update(GameRoot g, float dt)
        {
            if (!Open) return;
            _center = Vector2.Lerp(_center, new Vector2(g.Player.Pos.x, g.Player.Pos.z), 1f - Mathf.Exp(-dt * 2.5f));
        }

        public void Draw(GameRoot g)
        {
            if (!Open) return;
            Ui.Fill(new Rect(0, 0, Screen.width, Screen.height), new Color(0.03f, 0.04f, 0.05f, 0.97f));
            Rect r = Layout.FullmapRect;

            float extent = World.Extent;
            float meters = _viewKm * 1000f / Zoom;
            float u = (_center.x - meters * 0.5f + extent) / (extent * 2f);
            float v = (_center.y - meters * 0.5f + extent) / (extent * 2f);
            Rect uv = new Rect(u, v, meters / (extent * 2f), meters / (extent * 2f));

            Ui.Round(new Rect(r.x - 4f, r.y - 4f, r.width + 8f, r.height + 8f), new Color32(8, 10, 14, 255), Layout.Px(12f));
            if (g.World.MapTexture != null)
                GUI.DrawTextureWithTexCoords(r, g.World.MapTexture, uv, true);

            // district labels
            for (int i = 0; i < GameData.Districts.Length; i++)
            {
                District d = GameData.Districts[i];
                Vector2 p = ToScreen(r, meters, d.X, d.Z);
                if (!r.Contains(p)) continue;
                Ui.Text(new Rect(p.x - Layout.Px(70f), p.y - Layout.Px(9f), Layout.Px(140f), Layout.Px(18f)),
                    d.Name, Mathf.RoundToInt(Layout.Px(10f)), new Color32(255, 255, 255, 120), TextAnchor.MiddleCenter, FontStyle.Bold);
            }

            // blips
            DrawBlip(g, r, meters, g.Player.Pos, Col.Hex("F2A03D"), 7f, true);
            if (g.MissionHasTarget) DrawBlip(g, r, meters, g.MissionTarget, Col.Hex("F2D24E"), 6f, false);
            if (g.HasWaypoint) DrawBlip(g, r, meters, g.Waypoint, Col.Hex("7FD1E8"), 6f, false);
            for (int i = 0; i < g.Police.Cruisers.Count; i++) DrawBlip(g, r, meters, g.Police.Cruisers[i].Pos, Col.Hex("3B7BFF"), 3.5f, false);
            for (int i = 0; i < Actors.Cops.Count; i++) DrawBlip(g, r, meters, Actors.Cops[i].Pos, Col.Hex("3B7BFF"), 3f, false);
            for (int i = 0; i < g.Traffic.Cars.Count; i++) DrawBlip(g, r, meters, g.Traffic.Cars[i].Pos, new Color32(210, 210, 210, 160), 2.4f, false);

            // interaction: tap to set a waypoint, drag to pan
            Event e = Event.current;
            Vector2 mouse = e.mousePosition;
            if (e.type == EventType.MouseDown && r.Contains(mouse))
            {
                _dragging = true;
                _dragStart = mouse;
            }
            else if (e.type == EventType.MouseDrag && _dragging)
            {
                Vector2 delta = mouse - _dragStart;
                _dragStart = mouse;
                _center -= new Vector2(delta.x, -delta.y) / r.width * meters;
                _center.x = Mathf.Clamp(_center.x, -World.Extent, World.Extent);
                _center.y = Mathf.Clamp(_center.y, -World.Extent, World.Extent);
            }
            else if (e.type == EventType.MouseUp && _dragging)
            {
                _dragging = false;
                if ((mouse - _dragStart).magnitude < Layout.Px(14f) && r.Contains(mouse))
                {
                    float wx = _center.x + (mouse.x - r.center.x) / r.width * meters;
                    float wz = _center.y - (mouse.y - r.center.y) / r.height * meters;
                    g.SetWaypoint(new Vector3(wx, 0f, wz));
                }
            }

            // chrome
            Rect close = new Rect(r.xMax - Layout.Px(60f), r.y + Layout.Px(10f), Layout.Px(50f), Layout.Px(40f));
            if (Ui.Button(close, "X", true, Mathf.RoundToInt(Layout.Px(16f)))) Close();
            Rect zin = new Rect(r.x + Layout.Px(10f), r.yMax - Layout.Px(108f), Layout.Px(50f), Layout.Px(44f));
            Rect zout = new Rect(r.x + Layout.Px(10f), r.yMax - Layout.Px(56f), Layout.Px(50f), Layout.Px(44f));
            if (Ui.Button(zin, "+", false, Mathf.RoundToInt(Layout.Px(20f)))) Zoom = Mathf.Clamp(Zoom * 1.6f, 0.6f, 6f);
            if (Ui.Button(zout, "-", false, Mathf.RoundToInt(Layout.Px(20f)))) Zoom = Mathf.Clamp(Zoom / 1.6f, 0.6f, 6f);

            District here = g.District;
            Ui.Panel(new Rect(r.x, r.y - Layout.Px(58f), r.width, Layout.Px(50f)));
            Ui.Text(new Rect(r.x + Layout.Px(14f), r.y - Layout.Px(56f), r.width * 0.6f, Layout.Px(24f)),
                here != null ? here.Name : "", Mathf.RoundToInt(Layout.Px(17f)), Ui.TextColor,
                TextAnchor.MiddleLeft, FontStyle.Bold);
            Ui.Text(new Rect(r.x + Layout.Px(14f), r.y - Layout.Px(34f), r.width * 0.8f, Layout.Px(22f)),
                here != null ? here.Flavour : "", Mathf.RoundToInt(Layout.Px(11f)), Ui.TextDimColor, TextAnchor.MiddleLeft);
            if (g.HasWaypoint)
                Ui.Text(new Rect(r.x + r.width * 0.55f, r.y - Layout.Px(56f), r.width * 0.42f, Layout.Px(48f)),
                    "WAYPOINT  •  " + Loc.S("ui.close") + " X", Mathf.RoundToInt(Layout.Px(11f)), Ui.AccentColor,
                    TextAnchor.MiddleRight);
        }

        Vector2 ToScreen(Rect r, float meters, float x, float z)
        {
            float sx = (x - _center.x) / meters * r.width + r.center.x;
            float sy = -(z - _center.y) / meters * r.height + r.center.y;
            return new Vector2(sx, sy);
        }

        void DrawBlip(GameRoot g, Rect r, float meters, Vector3 world, Color32 color, float size, bool player)
        {
            Vector2 p = ToScreen(r, meters, world.x, world.z);
            if (!r.Contains(p)) return;
            if (player) Ui.Arrow(p, Layout.Px(size), color, g.Player.Heading - g.Cam.Yaw);
            else Ui.Dot(p, Layout.Px(size), color);
        }
    }
}
