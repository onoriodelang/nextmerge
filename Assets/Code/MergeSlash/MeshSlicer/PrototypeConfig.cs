using Leopotam.Ecs;
using Modules.ViewHub;
using UnityEngine;

namespace Modules.ST
{
    [CreateAssetMenu(fileName = "PrototypeConfig", menuName = "Modules/MergeSlash/PrototypeConfig")]
    public class PrototypeConfig : ScriptableObject
    {
        public float ScaryEmoteDistance;
        [Header("Gold")]
        public int WeaponCost;
        public int SlotCost;
        [Header("Timings")] 
        public float CameraReturnTiming;
        public float LooseWindowTiming;
        public float EnemyShootTiming;
        public float EnemySmileTiming;
        public float WinWindowTiming;
        public float WinAnimationTiming;
        public float ThrowButtonsEnable;
    }

    
}