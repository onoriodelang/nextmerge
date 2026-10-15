using UnityEngine;
using Leopotam.Ecs;
using Modules.EventGroup;
using Modules.MergeSlash.UI;
using Modules.ST;

namespace Modules.MergeSlash.EventHandlers
{
    /// <summary>
    /// process enter at a regular gameplay state(after click on TTS)
    /// </summary>
    public class OnGamePlayStateEnter : IEcsRunSystem
    {
        readonly EcsFilter<GamePlayState, StateEnter> _signal;
        readonly EcsFilter<GameplayButtonsPanelView> _panelView;
        readonly EcsWorld _world;

        public void Run()
        {
            if (_signal.IsEmpty())
                return;

            // show gameplay screen
            ref var screen = ref _world.NewEntity().Get<UICoreECS.ShowScreenTag>();
            screen.ID = (int)UI.MainScreens.GamePlay;
            screen.Layer = (int)UI.Layers.MainLayer;
            
            ref var screen2 = ref _world.NewEntity().Get<UICoreECS.ShowScreenTag>();
            screen2.ID = (int)UI.SecondScreens.BloodScreen;
            screen2.Layer = (int)UI.Layers.SecondLayer;
            
            foreach (var viewIdx in _panelView)
            {
                _panelView.Get1(viewIdx).View.EnableTilesButtons();
            }
            
            
        }
    }
}