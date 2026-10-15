using Leopotam.Ecs;
using Modules.MergeMechanic.Components;
using Modules.MergeSlash.UI.UnityComponents;

namespace Modules.MergeSlash.Interactors
{
    public class BuySlotSystem : IEcsRunSystem
    {
        private EcsFilter<BuySlotButtonSignal> _signal;
        private EcsFilter<GoldWallet> _wallet;
        private EcsFilter<TileViewRef, LockedTag> _locked;

        private EcsWorld _world;
        
        public void Run()
        {
            if(_signal.IsEmpty() || _locked.IsEmpty())
                return;
            
            int gold = 0;

            foreach (var walletIdx in _wallet)
            {
                gold = _wallet.Get1(walletIdx).CurrentGold;
            }
            
            if(gold < 50)
                return;

            

            _world.NewEntity().Get<TakeGold>().Gold = 50;
            _world.NewEntity().Get<UnlockTileSignal>();

        }
    }

    
}