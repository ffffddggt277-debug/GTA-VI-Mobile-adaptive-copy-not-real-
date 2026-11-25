using System;
using System.Collections.Generic;
using UnityEngine;

namespace GTA6.Game
{
    /// <summary>Small IMGUI toolkit. No uGUI package, no prefabs, no imported assets.</summary>
    public static class Ui
    {
        public static readonly Color32 AccentColor = new Color32(242, 160, 61, 255);
        public static readonly Color32 AccentDim = new Color32(150, 96, 30, 255);
        public static readonly Color32 PanelColor = new Color32(16, 18, 22, 216);
        public static readonly Color32 PanelLight = new Color32(34, 38, 46, 226);
        public static readonly Color32 TextColor = new Color32(240, 240, 236, 255);
        public static readonly Color32 TextDimColor = new Color32(160, 164, 170, 255);
        public static readonly Color32 GoodColor = new Color32(94, 196, 118, 255);
        public static readonly Color32 BadColor = new Color32(214, 78, 74, 255);
        public static readonly Color32 SkyColor = new Color32(70, 86, 112, 255);

        static Texture2D _white, _circle, _ring, _star, _arrow;
        static readonly Dictionary<string, GUIStyle> _styles = new Dictionary<string, GUIStyle>();
        static bool _init;

        public static void Init()
        {
            if (_init) return;
            _init = true;
            _white = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            _white.SetPixel(0, 0, Color.white);
            _white.Apply();
            _white.wrapMode = TextureWrapMode.Clamp;

            _circle = Disc(96, 1f);
            _ring = Disc(96, 0.16f);
            _star = StarTexture(64);
            _arrow = ArrowTexture(64);
        }

        static Texture2D Disc(int size, float hardness)
        {
            Texture2D t = new Texture2D(size, size, TextureFormat.RGBA32, false);
            t.wrapMode = TextureWrapMode.Clamp;
            Color32[] px = new Color32[size * size];
            float c = (size - 1) * 0.5f;
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    float d = Mathf.Sqrt((x - c) * (x - c) + (y - c) * (y - c)) / c;
                    float a;
                    if (hardness >= 1f) a = d <= 1f ? 1f : 0f;
                    else a = d >= 1f ? 0f : (d <= 1f - hardness ? 1f : (1f - d) / hardness);
                    px[y * size + x] = new Color32(255, 255, 255, (byte)Mathf.RoundToInt(a * 255f));
                }
            t.SetPixels32(px);
            t.Apply();
            return t;
        }

        static Texture2D StarTexture(int size)
        {
            Texture2D t = new Texture2D(size, size, TextureFormat.RGBA32, false);
            t.wrapMode = TextureWrapMode.Clamp;
            Color32[] px = new Color32[size * size];
            float c = (size - 1) * 0.5f;
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    float dx = (x - c) / c, dy = (y - c) / c;
                    float ang = Mathf.Atan2(dy, dx);
                    float rad = Mathf.Sqrt(dx * dx + dy * dy);
                    float spikes = 0.62f + 0.38f * Mathf.Cos(ang * 5f);
                    float a = rad <= spikes ? 1f : 0f;
                    px[y * size + x] = new Color32(255, 255, 255, (byte)(a * 255f));
                }
            t.SetPixels32(px);
            t.Apply();
            return t;
        }

        static Texture2D ArrowTexture(int size)
        {
            Texture2D t = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Color32[] px = new Color32[size * size];
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    float nx = (x - size * 0.5f) / (size * 0.5f);
                    float ny = (y - size * 0.4f) / (size * 0.5f);
                    float a = 0f;
                    if (ny < 0.9f && Mathf.Abs(nx) < (0.9f - ny) * 0.8f) a = 1f;
                    px[y * size + x] = new Color32(255, 255, 255, (byte)(a * 255f));
                }
            t.SetPixels32(px);
            t.Apply();
            return t;
        }

        public static Texture2D White { get { Init(); return _white; } }
        public static Texture2D CircleTex { get { Init(); return _circle; } }
        public static Texture2D RingTex { get { Init(); return _ring; } }
        public static Texture2D StarTex { get { Init(); return _star; } }
        public static Texture2D ArrowTex { get { Init(); return _arrow; } }

        public static void Fill(Rect r, Color color)
        {
            Color old = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(r, White);
            GUI.color = old;
        }

        public static void Fill(Rect r, Color32 color) { Fill(r, (Color)color); }

        public static void Round(Rect r, Color color, float radius = -1f)
        {
            if (radius <= 0f) radius = Mathf.Min(r.width, r.height) * 0.18f;
            // A cheap rounded look: body plus four discs at the corners.
            Fill(new Rect(r.x + radius, r.y, r.width - radius * 2f, r.height), color);
            Fill(new Rect(r.x, r.y + radius, r.width, r.height - radius * 2f), color);
            Color old = GUI.color;
            GUI.color = color;
            float d = radius * 2f;
            GUI.DrawTexture(new Rect(r.x, r.y, d, d), CircleTex);
            GUI.DrawTexture(new Rect(r.xMax - d, r.y, d, d), CircleTex);
            GUI.DrawTexture(new Rect(r.x, r.yMax - d, d, d), CircleTex);
            GUI.DrawTexture(new Rect(r.xMax - d, r.yMax - d, d, d), CircleTex);
            GUI.color = old;
        }

        public static void Panel(Rect r, bool light = false)
        {
            Color32 c = light ? PanelLight : PanelColor;
            Round(r, c, Mathf.Min(Layout.Px(14f), Mathf.Min(r.width, r.height) * 0.25f));
        }

        public static void Border(Rect r, Color32 color, float thickness = 1f)
        {
            Fill(new Rect(r.x, r.y, r.width, thickness), color);
            Fill(new Rect(r.x, r.yMax - thickness, r.width, thickness), color);
            Fill(new Rect(r.x, r.y, thickness, r.height), color);
            Fill(new Rect(r.xMax - thickness, r.y, thickness, r.height), color);
        }

        static GUIStyle Style(int size, TextAnchor anchor, FontStyle font, bool wrap)
        {
            string key = size + "|" + (int)anchor + "|" + (int)font + "|" + wrap;
            GUIStyle s;
            if (_styles.TryGetValue(key, out s) && s.fontSize != 0) return s;
            s = new GUIStyle(GUI.skin.label);
            s.fontSize = size;
            s.alignment = anchor;
            s.fontStyle = font;
            s.wordWrap = wrap;
            s.richText = false;
            s.clipping = TextClipping.Clip;
            s.normal.textColor = Color.white;
            s.hover.textColor = Color.white;
            s.active.textColor = Color.white;
            s.focused.textColor = Color.white;
            _styles[key] = s;
            return s;
        }

        public static void Text(Rect r, string text, int size, Color32 color, TextAnchor anchor = TextAnchor.MiddleCenter, FontStyle font = FontStyle.Normal, bool wrap = false)
        {
            if (string.IsNullOrEmpty(text)) return;
            GUIStyle s = Style(size, anchor, font, wrap);
            Color old = GUI.color;
            GUI.color = color;
            s.normal.textColor = color;
            GUI.Label(r, text, s);
            GUI.color = old;
        }

        public static void TextShadow(Rect r, string text, int size, Color32 color, TextAnchor anchor = TextAnchor.MiddleCenter, FontStyle font = FontStyle.Normal)
        {
            Text(new Rect(r.x + 1f, r.y + 1f, r.width, r.height), text, size, new Color32(0, 0, 0, 150), anchor, font);
            Text(r, text, size, color, anchor, font);
        }

        public static bool Button(Rect r, string label, bool accent = false, int size = 0, bool enabled = true)
        {
            if (size <= 0) size = Mathf.RoundToInt(Layout.Px(20f));
            bool hover = r.Contains(Event.current.mousePosition);
            Color32 bg = accent ? AccentColor : PanelLight;
            if (!enabled) bg = new Color32(40, 42, 46, 200);
            else if (hover) bg = accent ? new Color32(255, 186, 96, 255) : new Color32(58, 64, 74, 240);
            Round(r, bg, Layout.Px(10f));
            Color32 fg = accent ? new Color32(24, 20, 14, 255) : TextColor;
            if (!enabled) fg = new Color32(110, 112, 118, 255);
            Text(r, label, size, fg, TextAnchor.MiddleCenter, FontStyle.Bold);
            if (!enabled) return false;
            bool clicked = GUI.Button(r, GUIContent.none, GUIStyle.none);
            if (clicked) AudioDirector.Click();
            return clicked;
        }

        public static bool IconButton(Rect r, string glyph, bool accent = false)
        {
            bool clicked = Button(r, "", accent);
            int size = Mathf.RoundToInt(Mathf.Min(r.width, r.height) * 0.42f);
            Text(r, glyph, size, accent ? new Color32(24, 20, 14, 255) : TextColor, TextAnchor.MiddleCenter, FontStyle.Bold);
            return clicked;
        }

        public static void Bar(Rect r, float value01, Color32 fill, Color32 back)
        {
            Round(r, back, r.height * 0.5f);
            float w = Mathf.Clamp01(value01) * (r.width - 2f);
            if (w > 1f) Round(new Rect(r.x + 1f, r.y + 1f, w, r.height - 2f), fill, (r.height - 2f) * 0.5f);
        }

        public static void Circle(Vector2 center, float radius, Color32 color)
        {
            Color old = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(new Rect(center.x - radius, center.y - radius, radius * 2f, radius * 2f), CircleTex);
            GUI.color = old;
        }

        public static void Ring(Vector2 center, float radius, Color32 color)
        {
            Color old = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(new Rect(center.x - radius, center.y - radius, radius * 2f, radius * 2f), RingTex);
            GUI.color = old;
        }

        public static void Star(Vector2 center, float radius, Color32 color)
        {
            Color old = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(new Rect(center.x - radius, center.y - radius, radius * 2f, radius * 2f), StarTex);
            GUI.color = old;
        }

        public static void Arrow(Vector2 center, float radius, Color32 color, float rotation)
        {
            Matrix4x4 old = GUI.matrix;
            GUIUtility.RotateAroundPivot(rotation, center);
            Color c = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(new Rect(center.x - radius, center.y - radius, radius * 2f, radius * 2f), ArrowTex);
            GUI.color = c;
            GUI.matrix = old;
        }

        public static void Dot(Vector2 center, float radius, Color32 color)
        {
            Circle(center, radius, color);
            Ring(center, radius, new Color32(0, 0, 0, 120));
        }

        /// <summary>Draws a cropped, rotated piece of a texture (used by the minimap).</summary>
        public static void RotatedMap(Rect dest, Texture2D tex, Rect uv, float rotationDeg, float pivotX, float pivotY)
        {
            Matrix4x4 old = GUI.matrix;
            GUIUtility.RotateAroundPivot(rotationDeg, new Vector2(pivotX, pivotY));
            GUI.DrawTextureWithTexCoords(dest, tex, uv, true);
            GUI.matrix = old;
        }

        public static string ShortNumber(int value)
        {
            if (value >= 1000000) return (value / 1000000f).ToString("0.#") + "M";
            if (value >= 10000) return (value / 1000f).ToString("0.#") + "K";
            return value.ToString();
        }

        public static Color32 Lighten(Color32 c, float amount)
        {
            return new Color32(
                (byte)Mathf.Clamp(c.r + Mathf.RoundToInt(255f * amount), 0, 255),
                (byte)Mathf.Clamp(c.g + Mathf.RoundToInt(255f * amount), 0, 255),
                (byte)Mathf.Clamp(c.b + Mathf.RoundToInt(255f * amount), 0, 255), c.a);
        }

        public static Color32 Fade(Color32 c, float alpha)
        {
            return new Color32(c.r, c.g, c.b, (byte)Mathf.Clamp(Mathf.RoundToInt(c.a * alpha), 0, 255));
        }
    }
}
