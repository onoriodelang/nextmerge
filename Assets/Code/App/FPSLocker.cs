using UnityEngine;

namespace Modules.App
{
    public class FPSLocker : MonoBehaviour
    {
        [SerializeField] private int _targetFPS = 60;
        [SerializeField] private bool _lockInEditor = true;

        public void Awake()
        {
#if UNITY_EDITOR
            if (!_lockInEditor)
            {
                Destroy(this);
                return;
            }
#endif

            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = _targetFPS;
            Destroy(this);
        }
    }
}