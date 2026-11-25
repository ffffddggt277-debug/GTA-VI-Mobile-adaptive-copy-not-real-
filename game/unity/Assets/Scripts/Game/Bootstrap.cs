using UnityEngine;

namespace GTA6.Game
{
    /// <summary>
    /// The build contains one empty scene; the whole game is created here, so the
    /// project has no scene or prefab files to keep in sync.
    /// </summary>
    public static class Bootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Create()
        {
            if (GameRoot.I != null) return;
            GameObject go = new GameObject("GameRoot");
            Object.DontDestroyOnLoad(go);
            go.AddComponent<GameRoot>();
            Application.targetFrameRate = 60;
        }
    }
}
