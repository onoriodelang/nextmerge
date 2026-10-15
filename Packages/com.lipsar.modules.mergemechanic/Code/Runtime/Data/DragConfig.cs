using UnityEngine;

namespace Modules.MergeMechanic.Data
{
    [CreateAssetMenu(menuName = "Modules/MergeMechanic/DragConfig", fileName = "DragConfig")]
    public class DragConfig : ScriptableObject
    {
        public float DragSpeedX;
        public float DragSpeedZ;

        public float XMinPos;
        public float XMaxPos;
        public float ZMinPos;
        public float ZMaxPos;

        public float YOffset;
    }
}
