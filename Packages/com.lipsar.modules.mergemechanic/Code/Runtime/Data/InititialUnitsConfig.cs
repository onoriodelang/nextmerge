using UnityEngine;

namespace Modules.MergeMechanic.Data
{
    [CreateAssetMenu(menuName = "Modules/MergeMechanic/InititialUnitsConfig", fileName = "InititialUnitsConfig")]
    public class InititialUnitsConfig : ScriptableObject
    {
        public EmptyMergeUnit[] UnitViews;
    }

    [System.Serializable]
    public class EmptyMergeUnit
    {
        public int TypeID;
        public int Level;
        public int TileID;
    }
}
