using Leopotam.Ecs;
using UnityEngine;
using Modules.PlayerLevel;
using TMPro;
using UICoreECS;

namespace Modules.MergeSlash.UI
{
    public class CurrentGoldPanel : AUIEntity
    {
        [SerializeField] private TextMeshProUGUI _view;
        [SerializeField] private string _goldPrefix;

        private EcsWorld _world;

        public override void Init(EcsWorld world, EcsEntity screen)
        {
            _world = world;
            screen.Get<CurrentGoldPanelView>().View = this;
        }

        public void SetGold(int gold)
        {
            _view.text = $"{_goldPrefix}{gold}";
        }
    }

    public struct CurrentGoldPanelView
    {
        public CurrentGoldPanel View;
    }
}