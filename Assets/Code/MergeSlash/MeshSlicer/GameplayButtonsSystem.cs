using Leopotam.Ecs;
using Modules.MergeMechanic;
using Modules.MergeMechanic.Components;
using Modules.MergeSlash.Components;
using Modules.MergeSlash.EntityTemplates;
using Modules.MergeSlash.UI;
using Modules.ST;
using Modules.Utils;
using Modules.ViewHub;
using UnityEngine;

namespace Modules.MergeSlash.Interactors
{
    public class GameplayButtonsSystem : IEcsRunSystem
    {
        private EcsFilter<GameplayButtonsPanelView> _filter;
        private EcsFilter<GoldWallet> _wallet;
        private EcsFilter<TileViewRef> _tiles;
        private EcsFilter<MergeUnit, OnTile>.Exclude<ThrowTag> _sawView;
        private EcsFilter<TileViewRef, LockedTag> _locked;
        private EcsFilter<TileViewRef>.Exclude<LockedTag, TileForShootingTag> _unlocked;

        private readonly EcsWorld _world;
        
        private readonly PrototypeConfig _prototypeConfig;
        private int[] _weaponsLevels;
        private Vector3[] _weaponsPoses;
        
        public GameplayButtonsSystem(PrototypeConfig prototypeConfig)
        {
            _prototypeConfig = prototypeConfig;
        }
        
        public void Run()
        {
            if (_weaponsLevels == null && !_tiles.IsEmpty())
            {
                _weaponsLevels = new int[_tiles.GetEntitiesCount()];
            }
            
            if (_weaponsPoses == null && !_tiles.IsEmpty())
            {
                _weaponsPoses = new Vector3[_tiles.GetEntitiesCount()];
            }
            
            int gold = 0;

            if (!_wallet.IsEmpty())
                gold = _wallet.Get1(0).CurrentGold;

            bool readyToSlash = false;
            int labelIdx = 0;

            foreach (var idx in _sawView)
            {
                if (_weaponsLevels != null && _weaponsPoses != null)
                {
                    _weaponsLevels[labelIdx] = _sawView.Get1(idx).CurrentLevel+1;
                    _weaponsPoses[labelIdx] = _sawView.Get1(idx).ViewRoot.position;

                    labelIdx++;
                }

                if(_sawView.Get2(idx).TileID > 2)
                    continue;

                readyToSlash = true;
            }
            
            if (_weaponsLevels != null && _weaponsPoses != null)
            {
                for (int i = labelIdx; i < _weaponsLevels.Length; i++)
                {
                    _weaponsLevels[i] = 0;
                }
            }
            
            bool enoughGoldForSlot = gold >= _prototypeConfig.SlotCost;
            bool enoughGoldForWeapon = gold >= _prototypeConfig.WeaponCost;
            bool lockedSlotsExist = !_locked.IsEmpty();
            
            int weaponsInTiles = 0;

            foreach (var i in _sawView)
            {
                if(_sawView.Get2(i).TileID <= 2)
                    continue;

                weaponsInTiles++;
            }
            
            bool emptySlots = _unlocked.GetEntitiesCount() > weaponsInTiles;
            
            foreach (var i in _filter)
            {
                _filter.Get1(i).View.SetBuySlotButtonActive(enoughGoldForSlot && lockedSlotsExist);
                _filter.Get1(i).View.SetBuyWeaponButtonActive(enoughGoldForWeapon && emptySlots);
                _filter.Get1(i).View.SetSlashButtonActive(readyToSlash);

                if (readyToSlash)
                    _world.NewEntity().Get<EnemyAngrySignal>();
                
                if (_weaponsLevels != null && _weaponsPoses != null)
                {
                    _filter.Get1(i).View.SetLevels(_weaponsLevels, _weaponsPoses);
                }
                
                if(_filter.Get1(i).Inited)
                    continue;

                _filter.Get1(i).View.SetCosts(_prototypeConfig.WeaponCost, _prototypeConfig.SlotCost);
                _filter.Get1(i).Inited = true;
            }
        }
    }

}