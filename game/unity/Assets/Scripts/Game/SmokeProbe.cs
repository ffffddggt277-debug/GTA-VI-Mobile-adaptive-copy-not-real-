#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace GTA6.Game
{
    /// <summary>
    /// Editor-only smoke probe: launches with GTA6_SMOKE=1 and reports whether the
    /// game gets past the loading phase, so a headless play-mode run can catch
    /// startup exceptions. It is compiled out of player builds.
    /// </summary>
    public sealed class SmokeProbe : MonoBehaviour
    {
        float _time;
        bool _reported;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Spawn()
        {
            if (Environment.GetEnvironmentVariable("GTA6_SMOKE") != "1") return;
            GameObject go = new GameObject("SmokeProbe");
            UnityEngine.Object.DontDestroyOnLoad(go);
            go.AddComponent<SmokeProbe>();
            Debug.Log("[Smoke] probe started");
        }

        void Update()
        {
            if (_reported) return;
            _time += Time.unscaledDeltaTime;

            GameRoot root = GameRoot.I;
            if (Time.frameCount % 60 == 1)
                Debug.Log("[Smoke] t=" + _time.ToString("0.0") + "s frame=" + Time.frameCount +
                          " root=" + (root == null ? "null" : "present") +
                          " phase=" + (root == null ? "-" : root.Phase.ToString()) +
                          " progress=" + (root == null ? "-" : root.LoadProgress.ToString("0.00")));

            bool ready = root != null && root.Phase != Phase.Loading;
            if (!ready && _time < 30f) return;

            _reported = true;
            if (root == null)
                Debug.LogError("[Smoke] FAIL: GameRoot was never created");
            else if (!ready)
                Debug.LogError("[Smoke] FAIL: stuck in " + root.Phase + " at " +
                               (root.LoadProgress * 100f).ToString("0") + "%");
            else
                Debug.Log("[Smoke] PASS: phase " + root.Phase + ", world=" +
                          (root.World == null ? "null" : "built") + ", player=" +
                          (root.Player == null ? "null" : "ok") + ", cam=" +
                          (root.Cam == null ? "null" : "ok"));

            EditorApplication.Exit(ready ? 0 : 1);
        }
    }
}
#endif
