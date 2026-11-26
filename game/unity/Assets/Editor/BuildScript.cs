using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace GTA6.Build
{
    /// <summary>Android build entry point: -executeMethod GTA6.Build.BuildScript.BuildApk</summary>
    public static class BuildScript
    {
        public const string BundleId = "com.leonida.gta6concept";
        public const string ProductName = "GTA VI Leonida";
        const string ScenePath = "Assets/Scenes/Main.unity";
        public const string ApkPath = "Builds/GTA6-Leonida.apk";

        [MenuItem("Leonida/Build Android APK")]
        public static void BuildApk()
        {
            int exitCode = 0;
            try
            {
                ConfigureAndroidTools();
                ConfigurePlayer();
                string scene = EnsureScene();

                string dir = Path.GetDirectoryName(ApkPath);
                if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

                BuildPlayerOptions options = new BuildPlayerOptions
                {
                    scenes = new[] { scene },
                    locationPathName = ApkPath,
                    target = BuildTarget.Android,
                    options = BuildOptions.None,
                };

                BuildReport report = BuildPipeline.BuildPlayer(options);
                BuildSummary summary = report.summary;
                Debug.Log(string.Format(
                    "[Leonida] build {0}: {1} bytes, {2} errors, {3} warnings, {4:0.0}s -> {5}",
                    summary.result, summary.totalSize, summary.totalErrors, summary.totalWarnings,
                    summary.totalTime.TotalSeconds, summary.outputPath));
                if (summary.result != BuildResult.Succeeded) exitCode = 1;
                if (summary.outputPath != null && File.Exists(summary.outputPath))
                    Debug.Log("[Leonida] apk size: " + new FileInfo(summary.outputPath).Length + " bytes");
            }
            catch (Exception e)
            {
                Debug.LogError("[Leonida] build failed: " + e);
                Debug.LogException(e);
                exitCode = 2;
            }

            if (Application.isBatchMode) EditorApplication.Exit(exitCode);
        }

        // --------------------------------------------------------- android tools

        /// <summary>
        /// Points the Android toolchain at the JDK and SDK installed by
        /// tools/setup-unity-android.sh. Both the legacy EditorPrefs keys and the
        /// current AndroidExternalToolsSettings API are set, so this keeps working
        /// across editor versions.
        /// </summary>
        public static void ConfigureAndroidTools()
        {
            string jdk = Environment.GetEnvironmentVariable("JAVA_HOME");
            string sdk = Environment.GetEnvironmentVariable("ANDROID_HOME");
            if (string.IsNullOrEmpty(sdk)) sdk = Environment.GetEnvironmentVariable("ANDROID_SDK_ROOT");
            string ndk = FindNdk(sdk);

            if (!string.IsNullOrEmpty(jdk))
            {
                EditorPrefs.SetString("JdkPath", jdk);
                Debug.Log("[Leonida] JDK: " + jdk);
            }
            if (!string.IsNullOrEmpty(sdk))
            {
                EditorPrefs.SetString("AndroidSdkRoot", sdk);
                Debug.Log("[Leonida] Android SDK: " + sdk);
            }
            // Unity 6000+ supports NDK r27c (27.2.12479018) and looks for it in
            // AndroidPlayer/NDK, which the Linux module does not ship. Point the
            // editor at the side-by-side NDK installed by sdkmanager instead.
            if (!string.IsNullOrEmpty(ndk))
            {
                EditorPrefs.SetString("AndroidNdkRoot", ndk);
                Debug.Log("[Leonida] Android NDK: " + ndk);
            }
            else
            {
                Debug.LogWarning("[Leonida] Android NDK not found under the SDK; " +
                                 "install it with: sdkmanager \"ndk;27.2.12479018\"");
                EditorPrefs.SetString("AndroidNdkRoot", "");
            }

            try
            {
                Type settings = Type.GetType("UnityEditor.Android.AndroidExternalToolsSettings, UnityEditor.Android.Extensions");
                if (settings != null)
                {
                    SetStatic(settings, "jdkRootPath", jdk);
                    SetStatic(settings, "sdkRootPath", sdk);
                    SetStatic(settings, "ndkRootPath", ndk);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("[Leonida] could not set external tools through the API: " + e.Message);
            }
        }

        /// <summary>
        /// Finds the side-by-side NDK under ANDROID_HOME/ndk, preferring the
        /// r27c build Unity 6000 reports as its supported version.
        /// </summary>
        static string FindNdk(string sdk)
        {
            if (string.IsNullOrEmpty(sdk)) return null;
            string root = Path.Combine(sdk, "ndk");
            if (!Directory.Exists(root)) return null;

            string preferred = Path.Combine(root, "27.2.12479018");
            if (Directory.Exists(preferred)) return preferred;

            string[] versions = Directory.GetDirectories(root);
            Array.Sort(versions, StringComparer.OrdinalIgnoreCase);
            return versions.Length > 0 ? versions[versions.Length - 1] : null;
        }

        static void SetStatic(Type type, string property, string value)
        {
            if (string.IsNullOrEmpty(value)) return;
            PropertyInfo p = type.GetProperty(property, BindingFlags.Public | BindingFlags.Static);
            if (p != null && p.CanWrite) { p.SetValue(null, value, null); return; }
            FieldInfo f = type.GetField(property, BindingFlags.Public | BindingFlags.Static);
            if (f != null) f.SetValue(null, value);
        }

        // ------------------------------------------------------------- settings

        public static void ConfigurePlayer()
        {
            PlayerSettings.companyName = "Leonida Fan Project";
            PlayerSettings.productName = ProductName;
            PlayerSettings.bundleVersion = "1.0.1";
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, BundleId);

            PlayerSettings.Android.bundleVersionCode = 2;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel25;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;
            PlayerSettings.Android.forceInternetPermission = false;
            PlayerSettings.Android.forceSDCardPermission = false;
            PlayerSettings.Android.startInFullscreen = true;
            PlayerSettings.Android.renderOutsideSafeArea = true;

            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;

            PlayerSettings.colorSpace = ColorSpace.Gamma;
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { GraphicsDeviceType.OpenGLES3 });
            PlayerSettings.MTRendering = false;
            PlayerSettings.stereoRenderingPath = StereoRenderingPath.SinglePass;

            // Mono on Android cannot produce 64-bit ARM code, and modern phones are
            // arm64-only, so the APK is built with IL2CPP for both ABIs.
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetIl2CppCompilerConfiguration(NamedBuildTarget.Android, Il2CppCompilerConfiguration.Release);
            PlayerSettings.SetApiCompatibilityLevel(NamedBuildTarget.Android, ApiCompatibilityLevel.NET_Standard);
            PlayerSettings.SetManagedStrippingLevel(NamedBuildTarget.Android, ManagedStrippingLevel.Low);

            EnsureShaderMaterials();

            QualitySettings.shadows = ShadowQuality.All;
            QualitySettings.shadowResolution = ShadowResolution.Medium;
            QualitySettings.antiAliasing = 0;
            QualitySettings.vSyncCount = 0;

            EditorUserBuildSettings.buildAppBundle = false;
            EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
            EditorUserBuildSettings.development = false;

            Debug.Log("[Leonida] player settings configured for Android");
        }

        /// <summary>
        /// The build ships an empty scene, so nothing references the built-in shaders
        /// the game creates at runtime (Standard, Skybox/Procedural) and Unity strips
        /// them: Shader.Find then returns null on the device, leaving a black sky and
        /// invisible models. A material kept in Resources pulls its shader into the
        /// build. Always Included Shaders would do it too, but writing that list breaks
        /// the player build on 6000.3 ("Failed to write unity_builtin_extra").
        /// </summary>
        static void EnsureShaderMaterials()
        {
            const string dir = "Assets/Resources";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                AssetDatabase.Refresh();
            }

            EnsureMaterial(dir + "/LeonidaStandard.mat", "Standard");
            EnsureMaterial(dir + "/LeonidaSky.mat", "Skybox/Procedural");
            AssetDatabase.SaveAssets();
        }

        static void EnsureMaterial(string path, string shaderName)
        {
            if (File.Exists(path)) return;

            Shader shader = Shader.Find(shaderName);
            if (shader == null)
            {
                Debug.LogWarning("[Leonida] shader not found: " + shaderName);
                return;
            }

            Material material = new Material(shader);
            material.name = Path.GetFileNameWithoutExtension(path);
            AssetDatabase.CreateAsset(material, path);
            Debug.Log("[Leonida] resources material: " + path + " (" + shaderName + ")");
        }

        static string EnsureScene()
        {
            string dir = Path.GetDirectoryName(ScenePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);

            if (!File.Exists(ScenePath))
            {
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                EditorSceneManager.SaveScene(scene, ScenePath);
                Debug.Log("[Leonida] created empty scene " + ScenePath);
            }
            else
            {
                EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            }

            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            return ScenePath;
        }
    }
}
