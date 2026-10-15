using UnityEngine;

namespace Modules.MergeMechanic.Data
{
    [CreateAssetMenu(menuName = "Modules/MergeMechanic/TileConfig", fileName = "TileConfig")]
    public class TileConfig : ScriptableObject
    {
        public LayerMask TileLayer;

        [Header("Position offset")]
        public float UnitOffsetX;
        public float UnitOffsetY;
        public float UnitOffsetZ;
    }
}
