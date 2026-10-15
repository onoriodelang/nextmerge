using Leopotam.Ecs;
using UnityEngine;

namespace Modules.MergeMechanic
{
    public class MergeUnitPurchaseSystem : IEcsRunSystem
    {
        private readonly EcsFilter<Components.PurhaceMergeUnitSignal> _signal = null;

        private readonly EcsWorld _world;
        private readonly Data.UnitViewConfig _unitViewConfig;

        public void Run()
        {
            if (_signal.IsEmpty())
                return;

            foreach (var i in _signal)
            {
                EcsEntity entity = _world.NewEntity();
                entity.Get<LevelSpawner.LevelEntityTag>();

                var unit = _signal.Get1(i);

                GameObject.Instantiate(_unitViewConfig.GetRootObject(unit.TypeID)).Spawn(entity, _world);
                entity.Get<Components.OnTile>().TileID = unit.TileID;
                entity.Get<Components.MergeUnit>().CurrentLevel = unit.Level;
            }

            _world.NewEntity().Get<Components.RepositionSignal>();
        }
    }
}