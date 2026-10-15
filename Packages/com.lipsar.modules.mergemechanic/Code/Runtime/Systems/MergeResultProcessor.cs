using Leopotam.Ecs;

namespace Modules.MergeMechanic
{
    public class MergeResultProcessor : IEcsRunSystem
    {
        private readonly EcsFilter<Components.MergeUnit, Components.MergeTargetSignal> _target = null;
        private readonly EcsFilter<Components.MergeVictimSignal> _victim = null;

        public void Run()
        {
            if (_target.IsEmpty())
                return;

            foreach (var i in _target)
            {
                if (_target.Get1(i).CurrentLevel < _target.Get1(i).MaxLevel)
                {
                    _target.Get1(i).CurrentLevel += 1;
                    _target.GetEntity(i).Get<Components.UpdateMergeUnitViewSignal>();
                }
            }

            foreach (var i in _victim)
            {
                _victim.GetEntity(i).Get<Modules.Utils.DestroyTag>();
            }
        }
    }
}