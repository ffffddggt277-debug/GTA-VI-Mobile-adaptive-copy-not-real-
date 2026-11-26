#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace GTA6.Build
{
    /// <summary>
    /// Opens the empty game scene and enters play mode; GTA6.Game.SmokeProbe (enabled
    /// with GTA6_SMOKE=1) reports the outcome and exits the editor.
    /// Run: GTA6_SMOKE=1 Unity -batchmode -nographics -projectPath game/unity \
    ///      -executeMethod GTA6.Build.SmokePlay.Run -logFile /tmp/smoke.log
    /// </summary>
    public static class SmokePlay
    {
        public static void Run()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Main.unity", OpenSceneMode.Single);
            Debug.Log("[Smoke] entering play mode");
            EditorApplication.EnterPlaymode();
        }
    }
}
#endif
