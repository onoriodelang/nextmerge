using Leopotam.Ecs;
using System;

namespace Modules.MergeMechanic
{
    public class RestoreMergeUnitOnTiles : IEcsRunSystem
    {
        private readonly EcsFilter<LevelSpawner.LevelSpawnedSignal> _initSignal = null;

        private readonly EcsWorld _world;

        private readonly Data.SaveConfig _saveConfig;
        private readonly Data.UnitViewConfig _unitViewConfig;
        private readonly Data.InititialUnitsConfig _inititialUnitsConfig;

        public void Run()
        {
            if (_initSignal.IsEmpty())
                return;

            int[] TypeID = Utils.PlayerPrefsX.GetIntArray(_saveConfig.TypeIDSaveKey);
            int[] CurrentLevel = Utils.PlayerPrefsX.GetIntArray(_saveConfig.CurrentLevelSaveKey);
            int[] TileID = Utils.PlayerPrefsX.GetIntArray(_saveConfig.TileIDSaveKey);

            if (TypeID.Length > 0 && CurrentLevel.Length > 0 && TileID.Length > 0)
                RestoreState(TypeID, CurrentLevel, TileID);
            else
                LoadFromConfig();

            _world.NewEntity().Get<Components.RepositionSignal>();
        }

        private void LoadFromConfig()
        {
            SpawnUnit(_inititialUnitsConfig.UnitViews);
        }

        private void RestoreState(int[] typeID, int[] currentLevel, int[] tileID)
        {
            Data.EmptyMergeUnit[] unitViews = new Data.EmptyMergeUnit[typeID.Length];

            for (int i = 0; i < typeID.Length; i++)
            {
                unitViews[i] = new Data.EmptyMergeUnit
                {
                    TypeID = typeID[i],
                    Level = currentLevel[i],
                    TileID = tileID[i]
                };
            }

            SpawnUnit(unitViews);
        }

        private void SpawnUnit(Data.EmptyMergeUnit[] unitViews)
        {
            for (int i = 0; i < unitViews.Length; i++)
            {
                EcsEntity entity = _world.NewEntity();
                entity.Get<LevelSpawner.LevelEntityTag>();

                var unit = unitViews[i];
                UnityEngine.GameObject.Instantiate(_unitViewConfig.GetRootObject(unit.TypeID)).Spawn(entity, _world);
                entity.Get<Components.MergeUnit>().CurrentLevel = unit.Level;
                entity.Get<Components.OnTile>().TileID = unit.TileID;
            }
        }
    }
}