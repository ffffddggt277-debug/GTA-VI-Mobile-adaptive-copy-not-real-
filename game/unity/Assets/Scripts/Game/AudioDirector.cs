using System;
using System.Collections.Generic;
using UnityEngine;
using GTA6.Core;

namespace GTA6.Game
{
    public enum Sfx { Pistol, Smg, Shotgun, Rifle, Melee, Crash, Explosion, Hit, Step, Ui, Cash, Siren, Engine, Rain }

    /// <summary>
    /// Every sound in the game is synthesised at runtime, so the build carries no audio assets.
    /// </summary>
    public sealed class AudioDirector : MonoBehaviour
    {
        public static AudioDirector I;

        const int Rate = 22050;
        readonly Dictionary<Sfx, AudioClip> _clips = new Dictionary<Sfx, AudioClip>();
        readonly Dictionary<string, AudioClip> _music = new Dictionary<string, AudioClip>();
        AudioSource[] _pool;
        int _next;
        AudioSource _musicSource, _rainSource, _sirenSource;
        AudioSource _engineSource;
        float _sfxVolume = 0.85f, _musicVolume = 0.55f;
        int _station = -1;
        Transform _listener;

        void Awake()
        {
            I = this;
            AudioListener listener = FindAnyObjectByType<AudioListener>();
            if (listener != null) _listener = listener.transform;
            _pool = new AudioSource[10];
            for (int i = 0; i < _pool.Length; i++)
            {
                AudioSource s = gameObject.AddComponent<AudioSource>();
                s.playOnAwake = false;
                s.spatialBlend = 0.65f;
                s.rolloffMode = AudioRolloffMode.Linear;
                s.minDistance = 6f;
                s.maxDistance = 90f;
                _pool[i] = s;
            }
            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.loop = true;
            _musicSource.spatialBlend = 0f;
            _musicSource.volume = _musicVolume;
            _rainSource = gameObject.AddComponent<AudioSource>();
            _rainSource.loop = true;
            _rainSource.spatialBlend = 0f;
            _rainSource.volume = 0f;
            _sirenSource = gameObject.AddComponent<AudioSource>();
            _sirenSource.loop = true;
            _sirenSource.spatialBlend = 0f;
            _sirenSource.volume = 0f;
            _engineSource = gameObject.AddComponent<AudioSource>();
            _engineSource.loop = true;
            _engineSource.spatialBlend = 0f;
            _engineSource.volume = 0f;
            BuildClips();
        }

        void BuildClips()
        {
            _clips[Sfx.Pistol] = Gunshot("pistol", 0.28f, 0.55f, 140f, 0f);
            _clips[Sfx.Smg] = Gunshot("smg", 0.2f, 0.42f, 180f, 0f);
            _clips[Sfx.Shotgun] = Gunshot("shotgun", 0.45f, 0.85f, 90f, 0f);
            _clips[Sfx.Rifle] = Gunshot("rifle", 0.26f, 0.62f, 120f, 0.15f);
            _clips[Sfx.Melee] = Thump("melee", 0.14f, 90f, 0.5f);
            _clips[Sfx.Crash] = Thump("crash", 0.4f, 60f, 0.7f);
            _clips[Sfx.Explosion] = ExplosionClip("boom");
            _clips[Sfx.Hit] = Thump("hit", 0.16f, 120f, 0.35f);
            _clips[Sfx.Step] = Thump("step", 0.09f, 70f, 0.25f);
            _clips[Sfx.Ui] = Beep("ui", 900f, 0.06f, 0.3f);
            _clips[Sfx.Cash] = Beep("cash", 1400f, 0.18f, 0.4f);
            _clips[Sfx.Siren] = SirenLoop("siren");
            _clips[Sfx.Engine] = EngineLoop("engine");
            _clips[Sfx.Rain] = RainLoop("rain");

            _engineSource.clip = _clips[Sfx.Engine];
            _engineSource.Play();
            _sirenSource.clip = _clips[Sfx.Siren];
            _sirenSource.Play();
            _rainSource.clip = _clips[Sfx.Rain];
            _rainSource.Play();
        }

        // ------------------------------------------------------------ synthesis

        static float[] Buffer(float seconds) { return new float[Mathf.Max(16, (int)(seconds * Rate))]; }

        static AudioClip Make(string name, float[] data)
        {
            AudioClip c = AudioClip.Create(name, data.Length, 1, Rate, false);
            c.SetData(data, 0);
            return c;
        }

        static AudioClip Gunshot(string name, float seconds, float body, float lowHz, float tail)
        {
            float[] d = Buffer(seconds);
            Rng rng = new Rng(name.GetHashCode());
            float env = 1f;
            for (int i = 0; i < d.Length; i++)
            {
                float t = i / (float)Rate;
                env = Mathf.Exp(-t * (18f - body * 10f));
                float crack = (rng.Value * 2f - 1f) * env * body;
                float thump = Mathf.Sin(t * lowHz * Mathf.PI * 2f) * Mathf.Exp(-t * 26f) * 0.7f;
                float air = Mathf.Sin(t * 2400f * Mathf.PI * 2f) * Mathf.Exp(-t * 55f) * 0.15f;
                d[i] = Mathf.Clamp(crack + thump + air, -1f, 1f);
            }
            if (tail > 0f)
            {
                for (int i = (int)(d.Length * 0.4f); i < d.Length; i++)
                {
                    float t = i / (float)Rate;
                    d[i] += (rng.Value * 2f - 1f) * Mathf.Exp(-t * 6f) * tail * 0.4f;
                }
            }
            return Make(name, d);
        }

        static AudioClip Thump(string name, float seconds, float hz, float gain)
        {
            float[] d = Buffer(seconds);
            Rng rng = new Rng(name.GetHashCode() + 7);
            for (int i = 0; i < d.Length; i++)
            {
                float t = i / (float)Rate;
                float e = Mathf.Exp(-t * 22f);
                d[i] = (Mathf.Sin(t * hz * Mathf.PI * 2f) * 0.7f + (rng.Value * 2f - 1f) * 0.3f) * e * gain;
            }
            return Make(name, d);
        }

        static AudioClip Beep(string name, float hz, float seconds, float gain)
        {
            float[] d = Buffer(seconds);
            for (int i = 0; i < d.Length; i++)
            {
                float t = i / (float)Rate;
                float e = Mathf.Min(1f, t * 60f) * Mathf.Exp(-t * 16f);
                d[i] = Mathf.Sin(t * hz * Mathf.PI * 2f) * e * gain;
            }
            return Make(name, d);
        }

        static AudioClip ExplosionClip(string name)
        {
            float[] d = Buffer(1.4f);
            Rng rng = new Rng(1234);
            float low = 0f;
            for (int i = 0; i < d.Length; i++)
            {
                float t = i / (float)Rate;
                float noise = rng.Value * 2f - 1f;
                low += (noise - low) * 0.02f;
                float e = Mathf.Exp(-t * 3.2f);
                d[i] = Mathf.Clamp((noise * 0.35f + Mathf.Sin(t * 48f * Mathf.PI * 2f) * 0.5f + low * 2.2f) * e, -1f, 1f);
            }
            return Make(name, d);
        }

        static AudioClip SirenLoop(string name)
        {
            float[] d = Buffer(2.4f);
            for (int i = 0; i < d.Length; i++)
            {
                float t = i / (float)Rate;
                float sweep = 0.5f + 0.5f * Mathf.Sin(t * Mathf.PI * 0.83f);
                float hz = Mathf.Lerp(660f, 1050f, sweep);
                float phase = Mathf.Sin(t * hz * Mathf.PI * 2f);
                float e = Mathf.Clamp01(Mathf.Sin(t * Mathf.PI / 2.4f) * 3f) * 0.35f;
                d[i] = phase * e;
            }
            return Make(name, d);
        }

        static AudioClip EngineLoop(string name)
        {
            float[] d = Buffer(1f);
            for (int i = 0; i < d.Length; i++)
            {
                float t = i / (float)Rate;
                float f = 62f;
                float saw = Mathf.Repeat(t * f, 1f) * 2f - 1f;
                float sub = Mathf.Sin(t * f * 0.5f * Mathf.PI * 2f);
                float phase = (float)i / Rate;
                float loop = Mathf.Sin(phase * Mathf.PI * 2f) * 0.5f + 0.5f; // seamless-ish envelope
                d[i] = (saw * 0.25f + sub * 0.3f) * (0.7f + loop * 0.3f) * 0.8f;
            }
            return Make(name, d);
        }

        static AudioClip RainLoop(string name)
        {
            float[] d = Buffer(3f);
            Rng rng = new Rng(5511);
            float a = 0f, b = 0f;
            for (int i = 0; i < d.Length; i++)
            {
                float n = rng.Value * 2f - 1f;
                a += (n - a) * 0.35f;
                b += (a - b) * 0.06f;
                d[i] = (b * 1.4f + n * 0.1f) * 0.8f;
            }
            // crossfade the ends so the loop is seamless
            int fade = Rate / 8;
            for (int i = 0; i < fade; i++)
            {
                float k = i / (float)fade;
                d[i] = d[i] * k + d[d.Length - fade + i] * (1f - k);
            }
            return Make(name, d);
        }

        /// <summary>Builds an eight second loop for a station: bass, arpeggio and hats.</summary>
        AudioClip StationClip(int index)
        {
            RadioStation st = GameData.Stations[M.Clamp(index, 0, GameData.Stations.Length - 1)];
            if (_music.ContainsKey(st.Id)) return _music[st.Id];

            float seconds = 8f;
            float[] d = Buffer(seconds);
            Rng rng = new Rng(st.Id.GetHashCode());
            int[][] patterns =
            {
                new[] { 0, 4, 7, 12, 7, 4, 0, 7 },
                new[] { 0, 7, 12, 7, 3, 10, 7, 3 },
                new[] { 0, 3, 7, 10, 12, 10, 7, 3 },
                new[] { 0, 5, 7, 12, 10, 7, 5, 0 },
                new[] { 0, 7, 12, 16, 12, 7, 4, 7 },
                new[] { 0, 5, 7, 5, 12, 7, 5, 0 },
            };
            int[] pat = patterns[M.Clamp(st.Pattern, 0, patterns.Length - 1)];
            float beat = 60f / st.Tempo;
            int step = 0;
            int totalSteps = Mathf.FloorToInt(seconds / (beat * 0.5f));
            for (int s = 0; s < totalSteps; s++)
            {
                float t0 = s * beat * 0.5f;
                int note = pat[step % pat.Length];
                step++;
                float hz = st.RootHz * Mathf.Pow(2f, note / 12f);
                float len = beat * (patternLen(st.Pattern) ? 0.9f : 0.5f);
                int start = Mathf.Clamp(Mathf.RoundToInt(t0 * Rate), 0, d.Length - 2);
                int count = Mathf.Min(Mathf.RoundToInt(len * Rate), d.Length - start);
                for (int i = 0; i < count; i++)
                {
                    float t = i / (float)Rate;
                    float e = Mathf.Exp(-t * 6f);
                    float wave = Wave(st.Pattern, hz, t);
                    d[start + i] += wave * e * 0.16f;
                }
                // bass on every beat
                if (s % 2 == 0)
                {
                    float bassHz = st.RootHz * 0.5f;
                    int bstart = start;
                    int bcount = Mathf.Min(Mathf.RoundToInt(beat * Rate), d.Length - bstart);
                    for (int i = 0; i < bcount; i++)
                    {
                        float t = i / (float)Rate;
                        float e = Mathf.Exp(-t * 3.4f);
                        d[bstart + i] += Mathf.Sin(t * bassHz * Mathf.PI * 2f) * e * 0.24f;
                    }
                }
                // hat
                if (s % 2 == 1)
                {
                    int hstart = start;
                    int hcount = Mathf.Min(Mathf.RoundToInt(0.05f * Rate), d.Length - hstart);
                    for (int i = 0; i < hcount; i++)
                    {
                        float t = i / (float)Rate;
                        d[hstart + i] += (rng.Value * 2f - 1f) * Mathf.Exp(-t * 90f) * 0.07f;
                    }
                }
            }
            for (int i = 0; i < d.Length; i++) d[i] = Mathf.Clamp(d[i], -1f, 1f);
            AudioClip clip = Make("radio_" + st.Id, d);
            _music[st.Id] = clip;
            return clip;
        }

        static bool patternLen(int p) { return p == 1 || p == 4; }

        static float Wave(int pattern, float hz, float t)
        {
            float phase = t * hz;
            switch (pattern)
            {
                case 1: // saw, synthwave
                    return (Mathf.Repeat(phase, 1f) * 2f - 1f) * 0.8f + Mathf.Sin(phase * Mathf.PI * 2f) * 0.2f;
                case 2: // square, hip-hop
                    return Mathf.Sign(Mathf.Sin(phase * Mathf.PI * 2f)) * 0.6f;
                case 3: // plucky triangle, country
                    return Mathf.PingPong(phase * 2f, 1f) * 2f - 1f;
                case 4: // house stab
                    return Mathf.Sin(phase * Mathf.PI * 2f) * 0.7f + Mathf.Sin(phase * Mathf.PI * 4f) * 0.3f;
                case 5: // reggae organ
                    return Mathf.Sin(phase * Mathf.PI * 2f) * 0.5f + Mathf.Sin(phase * Mathf.PI * 3f) * 0.3f;
                default:
                    return Mathf.Sin(phase * Mathf.PI * 2f);
            }
        }

        // --------------------------------------------------------------- playing

        public void Play(Sfx id, Vector3 pos, float volume = 1f, float pitch = 1f)
        {
            AudioClip clip;
            if (!_clips.TryGetValue(id, out clip) || clip == null) return;
            AudioSource s = _pool[_next];
            _next = (_next + 1) % _pool.Length;
            s.transform.position = pos;
            s.pitch = pitch;
            s.volume = Mathf.Clamp01(volume * _sfxVolume);
            s.PlayOneShot(clip, 1f);
        }

        public void PlayShot(WeaponClass cls, Vector3 pos)
        {
            Sfx id = Sfx.Pistol;
            switch (cls)
            {
                case WeaponClass.Smg: id = Sfx.Smg; break;
                case WeaponClass.Shotgun: id = Sfx.Shotgun; break;
                case WeaponClass.Rifle: id = Sfx.Rifle; break;
                case WeaponClass.Melee: id = Sfx.Melee; break;
            }
            Play(id, pos, 1f, UnityEngine.Random.Range(0.94f, 1.06f));
        }

        public void PlayCrash(float impact) { Play(Sfx.Crash, Camera.main != null ? Camera.main.transform.position : Vector3.zero, Mathf.Clamp01(impact / 20f)); }
        public void PlayExplosion(Vector3 pos) { Play(Sfx.Explosion, pos, 1f, UnityEngine.Random.Range(0.9f, 1.1f)); }
        public void PlayHit(Vector3 pos) { Play(Sfx.Hit, pos, 0.8f); }
        public void PlayFootstep(Vector3 pos, bool running) { Play(Sfx.Step, pos, running ? 0.55f : 0.35f, UnityEngine.Random.Range(0.85f, 1.15f)); }
        public void PlayUi() { Play(Sfx.Ui, Vector3.zero, 0.6f); }

        /// <summary>Null-safe click, used by the UI toolkit from any screen.</summary>
        public static void Click() { if (I != null) I.PlayUi(); }
        public void PlayCash() { Play(Sfx.Cash, Vector3.zero, 0.7f); }
        public void PlayStep() { }

        public void SetStation(int index)
        {
            if (index < 0)
            {
                StopMusic();
                return;
            }
            index = M.Clamp(index, 0, GameData.Stations.Length - 1);
            if (_station == index && _musicSource.isPlaying) return;
            _station = index;
            _musicSource.clip = StationClip(index);
            _musicSource.volume = _musicVolume;
            _musicSource.Play();
        }

        public void StopMusic()
        {
            _station = -1;
            _musicSource.Stop();
        }

        public int Station { get { return _station; } }

        public void SetVolumes(float sfx, float music)
        {
            _sfxVolume = Mathf.Clamp01(sfx);
            _musicVolume = Mathf.Clamp01(music);
            _musicSource.volume = _musicVolume;
        }

        public void SetRain(float amount, float wind)
        {
            _rainSource.volume = Mathf.Clamp01(amount) * 0.5f * _sfxVolume;
        }

        public void SetSiren(bool on, float volume)
        {
            _sirenSource.volume = on ? Mathf.Clamp01(volume) * 0.35f * _sfxVolume : 0f;
        }

        public void SetEngine(bool on, float speed01)
        {
            _engineSource.volume = on ? (0.18f + speed01 * 0.22f) * _sfxVolume : 0f;
            _engineSource.pitch = 0.75f + speed01 * 1.1f;
        }
    }
}
