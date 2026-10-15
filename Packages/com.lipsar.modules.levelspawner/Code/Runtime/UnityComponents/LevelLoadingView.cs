using UnityEngine;
namespace Modules.LevelSpawner
{
    /// <summary>
    /// abstract view for level loading module
    /// </summary>
    public abstract class LevelLoadingView : MonoBehaviour
    {
        public abstract void Show();
        public abstract void Hide();
        public abstract void RecordProgress(float progress);
    }
}
