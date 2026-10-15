using UnityEngine;
using Leopotam.Ecs;
using Modules.EventGroup;

namespace Modules.MergeSlash.EventHandlers
{
    /// <summary>
    /// when player sucessfully passed the level
    /// </summary>
    public class OnRoundCompletedEnter : IEcsRunSystem
    {
        readonly EcsFilter<RoundCompletedState, StateEnter> _signal;
        readonly EcsWorld _world;
        readonly Currency.WalletsFacade _wallets;

        public void Run()
        {
            if (_signal.IsEmpty())
                return;

            // show sucess screen
            ref var screen = ref _world.NewEntity().Get<UICoreECS.ShowScreenTag>();
            screen.ID = (int) UI.MainScreens.RoundCompleted;
            screen.Layer = (int) UI.Layers.MainLayer;

            // add reward to player wallet
            _wallets.TransitRewardToPlayer();

            // increase current level
            PlayerLevel.ProgressionInfo.CurrentLevel += 1;
            PlayerLevel.ProgressionInfo.Save();
        }
    }
}