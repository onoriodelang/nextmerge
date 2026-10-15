using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
using UnityEngine;

public static class NeonAndroidBuild
{
    public static void Build()
    {
        try
        {
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android &&
                !EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android))
                throw new Exception("Could not select Android build target.");
            PlayerSettings.productName = "Neon Cyber Ninja";
            PlayerSettings.Android.useCustomKeystore = false;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            EditorUserBuildSettings.buildAppBundle = false;
            EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
            var icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Content/Icon/NeonCyberNinja.png");
            if (icon == null) throw new Exception("Generated launcher icon is missing.");
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new[] { icon });
            foreach (var kind in PlayerSettings.GetSupportedIconKindsForPlatform(BuildTargetGroup.Android))
            {
                var icons = PlayerSettings.GetPlatformIcons(BuildTargetGroup.Android, kind);
                foreach (var slot in icons)
                    slot.SetTextures(Enumerable.Repeat(icon, slot.maxLayerCount).ToArray());
                PlayerSettings.SetPlatformIcons(BuildTargetGroup.Android, kind, icons);
            }
            AssetDatabase.SaveAssets();
            if (AddressableAssetSettingsDefaultObject.Settings == null)
                throw new Exception("Addressables settings are missing.");
            AddressableAssetSettings.BuildPlayerContent(out var addressables);
            if (!string.IsNullOrEmpty(addressables.Error)) throw new Exception(addressables.Error);
            var output = Environment.GetEnvironmentVariable("NEON_APK_PATH");
            if (string.IsNullOrEmpty(output)) output = "Builds/Android/NeonCyberNinja.apk";
            Directory.CreateDirectory(Path.GetDirectoryName(output));
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray(),
                locationPathName = output,
                target = BuildTarget.Android,
                options = BuildOptions.None
            });
            if (report.summary.result != BuildResult.Succeeded)
                throw new Exception("Android build failed: " + report.summary.result);
            Debug.Log("APK ready: " + Path.GetFullPath(output));
            EditorApplication.Exit(0);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorApplication.Exit(1);
        }
    }
}
