using UnityEngine;
using Leopotam.Ecs;
using Modules.EventGroup;

namespace Modules.MergeSlash.EventHandlers
{
    /// <summary>
    /// when player lost the level
    /// </summary>
    public class OnRoundFailedEnter: IEcsRunSystem
    {
        readonly EcsFilter<RoundFailedState, StateEnter> _signal;
        readonly EcsWorld _world;

        public void Run()
        {
            if (_signal.IsEmpty())
                return;

            // show round failed screen
            ref var screen = ref _world.NewEntity().Get<UICoreECS.ShowScreenTag>();
            screen.ID = (int)UI.MainScreens.RoundFailed;
            screen.Layer = (int)UI.Layers.MainLayer;
        }
    }
}