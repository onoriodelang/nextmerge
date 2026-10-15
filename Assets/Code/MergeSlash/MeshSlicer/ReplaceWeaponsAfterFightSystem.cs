using System.Collections.Generic;
using Leopotam.Ecs;
using Modules.MergeMechanic.Components;
using Modules.MergeSlash.Components;
using UnityEngine;

namespace Modules.MergeSlash.Interactors
{
    public class ReplaceWeaponsAfterFightSystem : IEcsRunSystem
    {
        private EcsFilter<ReplaceWeaponsAfterFightTag> _signal;
        private EcsFilter<MergeUnit, PlaceInHands> _weaponsForReplace;
        private EcsFilter<MergeUnit, OnTile> _mergeUnits;
        private EcsFilter<TileViewRef>.Exclude<LockedTag> _unlockedTiles;
    
        private EcsWorld _world;


        public void Run()
        {
            if (_signal.IsEmpty() || _weaponsForReplace.IsEmpty())
                return;

            
            int mergeUnitsNumber = _mergeUnits.GetEntitiesCount();
            int tilesNumber = _unlockedTiles.GetEntitiesCount();
            int emptyTilesNumber = tilesNumber - mergeUnitsNumber;

            int unitsInUpperLine = 0;

            foreach (var i in _mergeUnits)
            {
                if (_mergeUnits.Get2(i).TileID <= 2)
                    unitsInUpperLine++;
            }
            
            if(unitsInUpperLine > 0 && emptyTilesNumber > 0)
                ReplaceFromUpperLine();

            foreach (var i in _signal)
            {
                _signal.GetEntity(i).Del<ReplaceWeaponsAfterFightTag>();
            }
        }

        private void ReplaceFromUpperLine()
        {
            int[] emptyTilesId = new int[3];

            for (int i = 0; i < emptyTilesId.Length; i++)
            {
                emptyTilesId[i] = -1;
            }

            int arrayIndex = 0;
            
            for (int i = 3; i < _unlockedTiles.GetEntitiesCount(); i++)
            {

                bool isEmpty = true;
                foreach (var idx in _mergeUnits)
                {
                    if (_mergeUnits.Get2(idx).TileID != i)
                        continue;

                    isEmpty = false;
                    break;
                }

                if (isEmpty)
                {
                    emptyTilesId[arrayIndex] = i;
                    arrayIndex++;
                }
                
                if(arrayIndex >= emptyTilesId.Length)
                    break;
            }

            arrayIndex = 0;
            bool reposition = false;

            foreach (var i in _mergeUnits)
            {
                if (_mergeUnits.Get2(i).TileID > 2)
                    continue;
                
                if(arrayIndex >= emptyTilesId.Length || emptyTilesId[arrayIndex] < 0)
                    break;

                _mergeUnits.Get2(i).TileID = emptyTilesId[arrayIndex];
                arrayIndex++;
                reposition = true;
            }

            if (reposition)
            {
                _world.NewEntity().Get<RepositionSignal>();
                _world.NewEntity().Get<SaveTilesTag>();
            }
        }
    }

    public struct ReplaceWeaponsAfterFightTag : IEcsIgnoreInFilter
    {
    }
}
