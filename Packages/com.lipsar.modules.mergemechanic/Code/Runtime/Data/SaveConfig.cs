using UnityEngine;

namespace Modules.MergeMechanic.Data
{
    [CreateAssetMenu(menuName = "Modules/MergeMechanic/SaveConfig", fileName = "SaveConfig")]
    public class SaveConfig : ScriptableObject
    {
        [Header("Merge unit")]
        public string TypeIDSaveKey;
        public string CurrentLevelSaveKey;
        public string TileIDSaveKey;

        [Header("Tile")]
        public string UnlockedTileKey;
    }
}
