using UnityEngine;
using Leopotam.Ecs;
using Modules.EventGroup;
using Modules.Currency;

namespace Modules.MergeSlash.EventHandlers
{
    /// <summary>
    /// at tap to start screen, handle level transition at these state enter
    /// </summary>
    public class OnNextLevelEnter : IEcsRunSystem
    {
        readonly EcsFilter<NextLevelState, EventGroup.StateEnter> _signal;
        readonly EcsFilter<LevelSpawner.LevelSpawnedSignal> _levelSpawned;
        readonly EcsFilter<NextLevelState> _nextLevel;
        readonly EcsWorld _world;
        readonly WalletsFacade _wallets;

        public void Run()
        {
            // show tap to start screen when level loaded and we at next level state
            if (!_levelSpawned.IsEmpty() && !_nextLevel.IsEmpty())
            {
                // show tts screen
                ref var screen = ref _world.NewEntity().Get<UICoreECS.ShowScreenTag>();
                screen.ID = (int)UI.MainScreens.TTS;
                screen.Layer = (int)UI.Layers.MainLayer;
            }

            // check if we're entering the next level state
            if (_signal.IsEmpty())
                return;

            // spawn next level
            // previous will be automatically cleared
            _world.NewEntity().Get<LevelSpawner.SpawnLevelSignal>().LevelID = PlayerLevel.ProgressionInfo.CurrentLevel;

            // clear reward from prev round
            _wallets.ClearReward();

            // hide main layer screens
            ref var hide = ref _world.NewEntity().Get<UICoreECS.ShowScreenTag>();
            hide.ID = -100;
            hide.Layer = (int)UI.Layers.MainLayer;

            // update ui
            _world.NewEntity().Get<UICoreECS.UIUpdate>();
        }
    }
}