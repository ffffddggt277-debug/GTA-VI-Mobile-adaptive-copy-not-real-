using System;
using UnityEngine;
using GTA6.Core;

namespace GTA6.Game
{
    /// <summary>Screen layout shared by the HUD (drawing) and the input (touch picking).</summary>
    public static class Layout
    {
        static int _w = -1, _h = -1;
        static float _s = 1f;

        public static float S { get { Ensure(); return _s; } }
        public static float W { get { Ensure(); return _w; } }
        public static float H { get { Ensure(); return _h; } }

        public static void Ensure()
        {
            if (_w == Screen.width && _h == Screen.height && _w > 0) return;
            _w = Mathf.Max(1, Screen.width);
            _h = Mathf.Max(1, Screen.height);
            // Follow the physical density when the device reports it, so text keeps
            // the same apparent size on a phone and on a desktop monitor.
            float dpi = Screen.dpi;
            float s = dpi > 60f ? dpi / 160f : Mathf.Min(_w / 1100f, _h / 620f);
            _s = Mathf.Clamp(s, 0.6f, 3.2f);
        }

        public static float Px(float value) { return value * S; }

        static Rect R(float x, float y, float w, float h)
        {
            Ensure();
            // Anchored to the bottom-right / bottom-left corners with a safe margin.
            return new Rect(x, y, w, h);
        }

        /// <summary>Circular button rect by id, in screen pixels (IMGUI coordinates).</summary>
        public static Rect Btn(string id)
        {
            Ensure();
            float w = _w, h = _h;
            float m = Px(22f);
            float big = Px(92f), mid = Px(64f), small = Px(50f);
            switch (id)
            {
                case "fire": return R(w - m - big, h - m - big * 1.55f - Px(70f), big, big);
                case "aim": return R(w - m - big - Px(16f) - mid, h - m - mid - Px(30f), mid, mid);
                case "reload": return R(w - m - mid, h - m - big * 1.55f - Px(70f) - mid - Px(12f), mid, mid);
                case "jump": return R(w - m - big - Px(16f) - mid - Px(10f) - mid, h - m - mid, mid, mid);
                case "car": return R(w - m - big - Px(8f), h - m - mid, mid, mid);
                case "weapon": return R(w - m - mid - Px(10f) - mid - Px(10f) - mid, h - m - mid, mid, mid);
                case "horn": return R(w - m - big * 0.5f - mid * 0.5f, h - m - big - Px(20f) - mid, mid, mid);
                case "phone": return R(w - m - small, m + Px(64f), small, small);
                case "map": return R(m, m + Px(64f), small, small);
                case "pause": return R(w - m - small, m, small, small);
                case "call": return R(w * 0.5f - Px(70f), h - m - Px(80f), Px(140f), Px(52f));
                case "interact": return R(w - m - big - Px(16f) - mid, h - m - mid, mid, mid);
                case "mission": return R(w - m - small, m + Px(64f) * 2f + Px(8f), small, small);
                default: return new Rect(-1000, -1000, 1, 1);
            }
        }

        public static Rect MinimapRect
        {
            get { Ensure(); return new Rect(Px(18f), Px(18f), Px(148f), Px(148f)); }
        }

        public static Rect PhoneRect
        {
            get
            {
                Ensure();
                float pw = Mathf.Min(_w * 0.94f, Px(430f));
                float ph = Mathf.Min(_h * 0.90f, Px(800f));
                return new Rect((_w - pw) * 0.5f, (_h - ph) * 0.5f, pw, ph);
            }
        }

        public static Rect FullmapRect
        {
            get
            {
                Ensure();
                float size = Mathf.Min(_w, _h) * 0.92f;
                return new Rect((_w - size) * 0.5f, (_h - size) * 0.5f, size, size);
            }
        }

        public static Vector2 StickHome
        {
            get { Ensure(); return new Vector2(Px(140f), _h - Px(140f)); }
        }

        public static float StickRadius { get { return Px(96f); } }
    }

    /// <summary>Unified touch and keyboard input. Touch is primary, keyboard is for editor testing.</summary>
    public sealed class InputState
    {
        public Vector2 Move;
        public Vector2 Look;
        public bool Sprint;
        public bool JumpPressed;
        public bool FireHeld;
        public bool AimHeld;
        public bool InteractPressed;
        public bool VehiclePressed;
        public bool PhonePressed;
        public bool PausePressed;
        public bool ReloadPressed;
        public bool WeaponPressed;
        public bool HornHeld;
        public bool MapPressed;
        public bool MissionPressed;
        public bool HoverFire;

        public bool StickVisible;
        public Vector2 StickCenter;
        public Vector2 StickKnob = new Vector2(0.5f, 0.5f);

        int _stickFinger = -1, _lookFinger = -1;
        readonly int[] _buttonFinger = new int[12];
        readonly bool[] _buttonHeld = new bool[12];
        static readonly string[] ButtonIds = { "fire", "aim", "jump", "car", "phone", "map", "pause", "reload", "weapon", "horn", "mission", "interact" };

        bool _mouseLook;

        public void Reset()
        {
            Move = Vector2.zero; Look = Vector2.zero;
            JumpPressed = InteractPressed = VehiclePressed = PhonePressed = PausePressed = false;
            ReloadPressed = WeaponPressed = MapPressed = MissionPressed = false;
            FireHeld = AimHeld = HornHeld = Sprint = false;
            StickVisible = false;
            _stickFinger = _lookFinger = -1;
            for (int i = 0; i < _buttonFinger.Length; i++) { _buttonFinger[i] = -1; _buttonHeld[i] = false; }
        }

        public int IndexOf(string id)
        {
            for (int i = 0; i < ButtonIds.Length; i++) if (ButtonIds[i] == id) return i;
            return -1;
        }

        public void Update(bool gameplayActive)
        {
            Reset();
            if (!gameplayActive)
            {
                UpdateMouseButtonOnly(ref Look);
                return;
            }
            if (Input.touchSupported && Input.touchCount > 0) UpdateTouch();
            else UpdateKeyboard();
        }

        void UpdateMouseButtonOnly(ref Vector2 look)
        {
            // Menus and the phone still need a cursor, nothing else.
        }

        // ------------------------------------------------------------- touch

        void UpdateTouch()
        {
            float w = Screen.width;
            bool stickTaken = _stickFinger >= 0;

            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch t = Input.GetTouch(i);
                int id = t.fingerId;
                Vector2 pos = t.position;

                if (t.phase == TouchPhase.Began)
                {
                    int btn = HitButton(pos);
                    if (btn >= 0)
                    {
                        _buttonFinger[btn] = id;
                        _buttonHeld[btn] = true;
                        continue;
                    }
                    if (pos.x < w * 0.44f && !stickTaken)
                    {
                        _stickFinger = id;
                        StickVisible = true;
                        StickCenter = pos;
                        StickKnob = new Vector2(0.5f, 0.5f);
                        stickTaken = true;
                        continue;
                    }
                    if (_lookFinger < 0)
                    {
                        _lookFinger = id;
                        continue;
                    }
                }

                if (id == _stickFinger)
                {
                    if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
                    {
                        _stickFinger = -1;
                        StickVisible = false;
                        Move = Vector2.zero;
                    }
                    else
                    {
                        float radius = Layout.StickRadius;
                        Vector2 delta = (pos - StickCenter) / radius;
                        if (delta.magnitude > 1f) delta = delta.normalized;
                        Move = delta;
                        StickKnob = delta;
                    }
                }
                else if (id == _lookFinger)
                {
                    if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled) _lookFinger = -1;
                    else if (t.phase == TouchPhase.Moved) Look += t.deltaPosition;
                }
                else
                {
                    for (int b = 0; b < _buttonFinger.Length; b++)
                    {
                        if (_buttonFinger[b] != id) continue;
                        if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
                        {
                            _buttonFinger[b] = -1;
                            _buttonHeld[b] = false;
                        }
                    }
                }
            }

            for (int b = 0; b < _buttonFinger.Length; b++)
                if (_buttonFinger[b] < 0) _buttonHeld[b] = false;

            FireHeld = _buttonHeld[IndexOf("fire")];
            AimHeld = _buttonHeld[IndexOf("aim")];
            HornHeld = _buttonHeld[IndexOf("horn")];
            Sprint = Move.magnitude > 0.92f;

            // one-shot buttons are latched by the HUD when it draws them, see Pressed()
            JumpPressed = _buttonHeld[IndexOf("jump")];
            InteractPressed = _buttonHeld[IndexOf("interact")];
            VehiclePressed = _buttonHeld[IndexOf("car")];
            PhonePressed = _buttonHeld[IndexOf("phone")];
            MapPressed = _buttonHeld[IndexOf("map")];
            MissionPressed = _buttonHeld[IndexOf("mission")];
        }

        /// <summary>True while any finger is inside the button. Used by the HUD for its pressed look.</summary>
        public bool Held(string id)
        {
            int i = IndexOf(id);
            if (i < 0) return false;
            return _buttonFinger[i] >= 0 || KeyboardDown(id);
        }

        bool KeyboardDown(string id)
        {
            switch (id)
            {
                case "fire": return Input.GetKey(KeyCode.F) || _mouseFire;
                case "aim": return Input.GetMouseButton(1);
                case "jump": return Input.GetKey(KeyCode.Space);
                case "car": return Input.GetKey(KeyCode.E);
                case "interact": return Input.GetKey(KeyCode.E);
                case "phone": return Input.GetKey(KeyCode.Tab);
                case "map": return Input.GetKey(KeyCode.M);
                case "pause": return Input.GetKey(KeyCode.Escape);
                case "reload": return Input.GetKey(KeyCode.R);
                case "weapon": return Input.GetKey(KeyCode.Q);
                case "horn": return Input.GetKey(KeyCode.H);
                case "mission": return Input.GetKey(KeyCode.J);
                default: return false;
            }
        }

        readonly System.Collections.Generic.Dictionary<string, bool> _prev = new System.Collections.Generic.Dictionary<string, bool>();

        /// <summary>Rising edge of a button, so holding JUMP does not spam jumps.</summary>
        public bool Pressed(string id)
        {
            bool now = Held(id);
            bool was;
            _prev.TryGetValue(id, out was);
            _prev[id] = now;
            return now && !was;
        }

        public void ClearEdges()
        {
            _prev.Clear();
        }

        bool _mouseFire;

        int HitButton(Vector2 pos)
        {
            for (int i = 0; i < ButtonIds.Length; i++)
            {
                Rect r = Layout.Btn(ButtonIds[i]);
                if (r.width <= 1f && r.height <= 1f) continue;
                Vector2 d = pos - new Vector2(r.center.x, r.center.y);
                if (d.magnitude <= r.width * 0.5f + Layout.Px(10f)) return i;
            }
            return -1;
        }

        // ---------------------------------------------------------- keyboard

        void UpdateKeyboard()
        {
            float x = 0f, z = 0f;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) z += 1f;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) z -= 1f;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) x += 1f;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) x -= 1f;
            Move = new Vector2(x, z);
            if (Move.magnitude > 1f) Move = Move.normalized;
            Sprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            JumpPressed = Input.GetKeyDown(KeyCode.Space);
            InteractPressed = Input.GetKeyDown(KeyCode.E);
            VehiclePressed = Input.GetKeyDown(KeyCode.E);
            PhonePressed = Input.GetKeyDown(KeyCode.Tab);
            PausePressed = Input.GetKeyDown(KeyCode.Escape);
            ReloadPressed = Input.GetKeyDown(KeyCode.R);
            WeaponPressed = Input.GetKeyDown(KeyCode.Q);
            MapPressed = Input.GetKeyDown(KeyCode.M);
            MissionPressed = Input.GetKeyDown(KeyCode.J);
            _mouseFire = Input.GetMouseButton(0);
            FireHeld = _mouseFire;
            AimHeld = Input.GetMouseButton(1);
            HornHeld = Input.GetKey(KeyCode.H);

            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
                Look += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * 6f;
            // Right stick emulation for touch builds tested on desktop.
            if (Input.GetAxis("Horizontal") != 0f || Input.GetAxis("Vertical") != 0f)
            {
                float h = Input.GetAxis("Horizontal"), v = Input.GetAxis("Vertical");
                if (Mathf.Abs(h) > 0.2f || Mathf.Abs(v) > 0.2f) Look += new Vector2(h, -v) * 4f;
            }
        }

        public void ConsumeLook() { Look = Vector2.zero; }
    }
}
