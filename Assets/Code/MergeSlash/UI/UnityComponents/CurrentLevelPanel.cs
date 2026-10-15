using UnityEngine;
using Modules.PlayerLevel;
using TMPro;

namespace Modules.MergeSlash.UI
{
    public class CurrentLevelPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _view;
        [SerializeField] private string _levelPrefix;

        // called from external event(by default via OnShow of ECSUIScreen)
        public void UpdateView()
        {
            _view.text = $"{_levelPrefix}{PlayerLevel.ProgressionInfo.CurrentLevel+1}";
        }
    }
}