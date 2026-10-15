using Leopotam.Ecs;
using Modules.MergeMechanic.Components;
using Modules.MergeSlash.UI.UnityComponents;
using Modules.ST.EntityTemplates;
using UnityEngine;

namespace Modules.MergeSlash.Interactors
{
    public class BuyWeaponSystem : IEcsRunSystem
    {
        private EcsFilter<BuyWeaponButtonSignal> _signal;
        private EcsFilter<GoldWallet> _wallet;
        private EcsFilter<LevelData> _levelData;
        private EcsFilter<TileViewRef>.Exclude<LockedTag> _tiles;
        private EcsFilter<OnTile> _onTiles;

        private EcsWorld _world;
        
        public void Run()
        {
            if(_signal.IsEmpty())
                return;
            
            int gold = 0;

            foreach (var walletIdx in _wallet)
            {
                gold = _wallet.Get1(walletIdx).CurrentGold;
            }
            
            if(gold < 10)
                return;

            int freeTileID = -1;

            foreach (var tileIdx in _tiles)
            {
                int tileID = _tiles.Get1(tileIdx).ID;
                
                if(tileID <= 2)
                    continue;
                
                bool free = true;

                foreach (var onTileIdx in _onTiles)
                {
                    if (_onTiles.Get1(onTileIdx).TileID == tileID)
                    {
                        free = false;
                        break;
                    }
                }

                if (free)
                {
                    freeTileID = tileID;
                    break;
                }
            }
            
            if(freeTileID < 0)
                return;

            int weaponId = 0;

            if (!_levelData.IsEmpty())
            {
                ref var levelData = ref _levelData.Get1(0);

                if (levelData.WeaponsBuySequence != null && levelData.WeaponsBuySequence.Length >= 1 &&
                    levelData.WeaponsBuySequenceIdx < levelData.WeaponsBuySequence.Length)
                {
                    weaponId = levelData.WeaponsBuySequence[levelData.WeaponsBuySequenceIdx];
                    levelData.WeaponsBuySequenceIdx++;
                }
                else if (levelData.WeaponsForBuy != null && levelData.WeaponsForBuy.Length >= 1)
                {
                    int random = Random.Range(0, levelData.WeaponsForBuy.Length);
                    weaponId = levelData.WeaponsForBuy[random];
                }
            }

            _world.NewEntity().Get<TakeGold>().Gold = 10;
            ref var purchace = ref _world.NewEntity().Get<PurhaceMergeUnitSignal>();
            purchace.Level = 0;
            purchace.TypeID  = weaponId;
            purchace.TileID  = freeTileID;
        }
    }

    
}