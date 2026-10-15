# NextMerge — Neon Cyber Ninja

A Unity 3D merge-and-throw game with a neon cyber-ninja visual theme.

<img src="Assets/Content/Icon/NeonCyberNinja.png" alt="Neon Cyber Ninja app icon" width="240">

## Features

- Dark navy arenas with cyan circuit-grid surfaces.
- Six neon weapon-tier colors and energy edge shading.
- Updated armor, merge tile states, UI accents, and gameplay labels.
- Generated Android launcher icon configured in project settings.
- Automated APK build script with Addressables content generation.

The reskin retains existing meshes, animation rigs, colliders, and gameplay systems. See [NEON-RESKIN.md](NEON-RESKIN.md) for details.

## Requirements

- Unity **2021.3.18f1**, the version recorded by the project.
- Android Build Support, including Android SDK/NDK and OpenJDK.
- An activated Unity Editor license.
- Access to the private Lipsar package registry in `Packages/manifest.json`, or a cache of the required packages.

Unity 6 migration has not been validated.

## Run

1. Clone this repository and add its folder to Unity Hub.
2. Open with Unity 2021.3.18f1 and let packages resolve and assets import.
3. Select Android as the build target.
4. Open `Assets/Scenes/ProdLoad.unity` and enter Play mode.

The enabled scenes are `ProdLoad` and `ProdGame`. Levels load through Addressables.

## Build an Android APK

Run from PowerShell in the project folder:

```powershell
.\Build-Android.ps1
```

For a different installation path:

```powershell
.\Build-Android.ps1 -UnityEditor 'D:/Unity/2021.3.18f1/Editor/Unity.exe'
```

The script assigns the generated icon, builds Addressables content, and produces a debug-key-signed APK for sideload testing at `Builds/Android/NeonCyberNinja.apk`. Logs are saved to `Builds/Android/build.log`. Release distribution requires your own signing setup. Signing keys are excluded from version control.

## Verification status

Static checks passed for 73 edited YAML assets, neon shader properties, and project GUID references in edited scenes and prefabs. Shader compilation, gameplay testing, and an APK build have not completed. The attempted build stopped before compilation because the installed Unity Editor had no active license.

## Project layout

| Path | Contents |
| --- | --- |
| `Assets/Scenes` | Loading and gameplay scenes |
| `Assets/Prefabs` | Levels, player, weapons, enemies, and UI |
| `Assets/Content` | Models, materials, textures, icon, and neon shader |
| `Assets/Editor/NeonAndroidBuild.cs` | Android batch build entry point |
| `Packages` | Manifest and embedded merge/level modules |
| `ProjectSettings` | Unity project configuration |

## Dependencies and attribution

The original project includes Unity Addressables, TextMeshPro, Cinemachine, DOTween, More Mountains Nice Vibrations, LeoECS, EzySlice, Epic Toon FX, and Lipsar modules. Existing third-party notices and licenses remain in their asset folders. This README grants no additional license for third-party assets.

Builds, Unity caches, local reskin backups, and signing material are excluded by `.gitignore`. The reskin procedure is recorded in `Apply-NeonReskin.ps1`; review it before rerunning over custom art changes.
