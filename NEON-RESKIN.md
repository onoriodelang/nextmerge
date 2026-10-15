# Neon cyber-ninja reskin

Applied directly to the existing assets: 73 materials, scenes and UI prefabs.
The original meshes, animation rigs, colliders and gameplay scripts are retained.

- Navy arena architecture with procedural cyan circuit lines on floors and tables.
- Six weapon energy colors: cyan, mint, violet, magenta, amber and hot pink.
- Neon rim shading on weapons, armor and arena props using the new built-in-renderer shader at `Assets/Content/NeonCyber.shader`.
- Distinct merge tile states, cyan/pink UI accents, dark camera background and fog.
- Start and gameplay labels use cyber-ninja terminology.

## Open and verify

Use Unity **2021.3.18f1**, Android platform, and start from `Assets/Scenes/ProdLoad.unity`.
Rebuild Addressables content before building the player, since levels use Addressables.
Check each weapon tier, merge hover/locked/available states, enemy visibility and UI text fit on the intended phone aspect ratio.
The shader produces colored energy edges without requiring bloom or a postprocessing package.

Static checks verified YAML headers, shader properties and preservation of project references in edited scenes and prefabs. Shader compilation and a visual playtest have not been performed: only Unity 6 is installed on this machine, while the project requires 2021.3.18f1.

## Files and recovery

`NeonReskin-files.json` lists every edited existing asset. Original versions are stored outside Assets in `.reskin-backup`, with their original relative paths. To revert an asset, copy its backup over the corresponding project file; keep the existing `.meta` file. The new shader has its own stable GUID.

`Apply-NeonReskin.ps1` records the reskin procedure. Do not rerun it after making further art changes without reviewing those changes first.
