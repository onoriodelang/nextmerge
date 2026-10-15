using Leopotam.Ecs;
using Modules.MergeSlash.Components;
using Modules.MergeSlash.EntityTemplates;
using Modules.MergeSlash.UI;
using Modules.Utils;
using Modules.ViewHub;
using UnityEngine;

namespace Modules.MergeSlash.Interactors
{
    public class GoldSystem : IEcsRunSystem, IEcsInitSystem
    {
        private const string GoldKey = "MS_Gold";
        
        private EcsFilter<GoldWallet> _wallet;
        private EcsFilter<CurrentGoldPanelView> _panels;
        private EcsFilter<AddGold> _adds;
        private EcsFilter<TakeGold> _takes;

        private EcsWorld _world;

        public void Init()
        {
            if (_wallet.IsEmpty())
                _world.NewEntity().Get<GoldWallet>().CurrentGold = LoadGold();
        }

        public void Run()
        {
            int gold = 0;

            foreach (var walletIdx in _wallet)
            {
                gold = _wallet.Get1(walletIdx).CurrentGold;
            }

            int goldChange = 0;

            foreach (var addIdx in _adds)
            {
                goldChange += _adds.Get1(addIdx).Gold;
                _adds.GetEntity(addIdx).Del<AddGold>();
            }
            
            foreach (var takeIdx in _takes)
            {
                goldChange -= _takes.Get1(takeIdx).Gold;
                _takes.GetEntity(takeIdx).Del<TakeGold>();
            }

            if (goldChange != 0)
            {
                gold += goldChange;
                SaveGold(gold);
            }
            
            foreach (var walletIdx in _wallet)
            {
                _wallet.Get1(walletIdx).CurrentGold = gold;
            }

            foreach (var panelIdx in _panels)
            {
                _panels.Get1(panelIdx).View.SetGold(gold);
            }
        }

        private int LoadGold()
        {
            return PlayerPrefs.GetInt(GoldKey, 0);
        }

        private void SaveGold(int gold)
        {
            PlayerPrefs.SetInt(GoldKey, gold);
            PlayerPrefs.Save();
        }
    }

    public struct GoldWallet
    {
        public int CurrentGold;
    }
    
    public struct AddGold
    {
        public int Gold;
    }
    
    public struct TakeGold
    {
        public int Gold;
    }
}