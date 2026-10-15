using Leopotam.Ecs;
using UnityEngine;

namespace Modules.MergeMechanic
{
    public class MergeUnitViewUpdateSystem : IEcsRunSystem
    {
        private readonly EcsFilter<Components.MergeUnit, Components.UpdateMergeUnitViewSignal> _initView = null;

        private readonly EcsWorld _world;
        private readonly Data.UnitViewConfig _unitViewConfig;

        public void Run()
        {
            if (_initView.IsEmpty())
                return;

            foreach (var i in _initView)
            {
                ref var unit = ref _initView.Get1(i);
                var view = _unitViewConfig.GetView(unit.TypeID, unit.CurrentLevel);

                if (view != null)
                {
                    EcsEntity entity = _world.NewEntity();
                    entity.Get<LevelSpawner.LevelEntityTag>();
                    
                    if (_initView.GetEntity(i).Has<Components.MergeUnitView>())
                        _initView.GetEntity(i).Get<Components.MergeUnitView>().Entity.Get<Modules.Utils.DestroyTag>();

                    _initView.GetEntity(i).Get<Components.MergeUnitView>().Entity = entity;
                    GameObject.Instantiate(view, unit.ViewRoot).Spawn(entity, _world);
                }

                //TODO : REMOVE ME!
                if (_initView.GetEntity(i).Has<Components.UpgradeLimitTag>() == false)
                    unit.MaxLevel = _unitViewConfig.GetMaxLevel(unit.TypeID);
            }
        }
    }
}