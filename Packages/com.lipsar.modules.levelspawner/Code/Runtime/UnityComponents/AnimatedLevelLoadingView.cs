using UnityEngine;
using UnityEngine.UI;

namespace Modules.LevelSpawner
{
    /// <summary>
    /// view for level loading panel
    /// animated via unity animator
    /// </summary>
    public class AnimatedLevelLoadingView : LevelLoadingView
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Slider _progress;
        [SerializeField] private bool _hasProgressView;
        [SerializeField] private string _showCode = "Show";
        [SerializeField] private string _hideCode = "Hide";
        [SerializeField] private bool _shown = true;

        /// <summary>
        /// recommended to call after animation via animator
        /// </summary>
        public void Disable() 
        {
            this.gameObject.SetActive(false);
        }

        public void Enable() 
        {
            this.gameObject.SetActive(true);
            if (_hasProgressView)
                _progress.value = 0.0f;
        }

        public override void Hide()
        {
            _animator.Play(_hideCode, 0, 0.0f);
            _shown = false;
        }

        public override void RecordProgress(float progress)
        {
            if (_hasProgressView)
                _progress.value = progress;
        }

        public override void Show()
        {
            Enable();
            if (!_shown)
            {
                _animator.Play(_showCode, 0, 0.0f);
                _shown = true;
            }
        }
    }
}