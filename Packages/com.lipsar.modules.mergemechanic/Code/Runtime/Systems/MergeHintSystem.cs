using Leopotam.Ecs;

namespace Modules.MergeMechanic
{
    public class MergeHintSystem : IEcsRunSystem
    {
        private readonly EcsFilter<LevelSpawner.LevelSpawnedSignal> _levelSpawned = null;
        private readonly EcsFilter<Components.MergeHintInitTag>.Exclude<Modules.Utils.WaitTimer> _init = null;
        private readonly EcsFilter<Components.MergeHintTag> _hinted = null;
        private readonly EcsFilter<Components.MergeUnit> _units = null;

        private readonly EcsFilter<Components.DragStartSignal> _dragStart = null;
        private readonly EcsFilter<Components.DragEndSignal> _dragEnd = null;

        private readonly EcsWorld _world;
        private readonly Data.HintConfig _hintConfig;

        public void Run()
        {
            if (_dragStart.IsEmpty() == false)
            {
                foreach (var i in _hinted)
                {
                    _hinted.GetEntity(i).Del<Components.MergeHintTag>();
                    _hinted.GetEntity(i).Get<Components.MergeHintClearSignal>();
                }
            }

            if (_dragEnd.IsEmpty() == false || _levelSpawned.IsEmpty() == false)
            {
                EcsEntity entity = _world.NewEntity();
                entity.Get<LevelSpawner.LevelEntityTag>();
                entity.Get<Components.MergeHintInitTag>();
                entity.Get<Modules.Utils.WaitTimer>().RemainingTime = _hintConfig.HintDelay;
            }

            if (_init.IsEmpty())
                return;

            foreach (var i in _init)
            {
                _init.GetEntity(i).Destroy();
                FindMergePair();
            }
        }

        private void FindMergePair()
        {
            foreach (var i in _units)
            {
                var currentUnit = _units.Get1(i);
                var currentEntity = _units.GetEntity(i);
                foreach (var j in _units)
                {
                    var targetEntity = _units.GetEntity(j);

                    if (currentEntity.Equals(targetEntity))
                        continue;

                    var targetUnit = _units.Get1(j);

                    if (currentUnit.TypeID == targetUnit.TypeID && currentUnit.CurrentLevel == targetUnit.CurrentLevel && targetUnit.CurrentLevel < targetUnit.MaxLevel)
                    {
                        currentEntity.Get<Components.MergeHintTag>();
                        currentEntity.Get<Components.MergeHintAddedSignal>();

                        targetEntity.Get<Components.MergeHintTag>();
                        targetEntity.Get<Components.MergeHintAddedSignal>();

                        return;
                    }
                }
            }
        }
    }
}