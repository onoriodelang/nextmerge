using UnityEngine;

namespace Modules.MergeMechanic.Data
{
    [CreateAssetMenu(menuName = "Modules/MergeMechanic/HintConfig", fileName = "HintConfig")]
    public class HintConfig : ScriptableObject
    {
        public float HintDelay;
    }
}
