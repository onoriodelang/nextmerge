using Leopotam.Ecs;
using UnityEngine;

namespace Modules.MergeMechanic
{
    public class MergeUnitViewComponent : ViewHub.ViewComponent
    {
        [SerializeField] private Components.MergeUnit _mergeUnit;
        [SerializeField] private bool _limitMaxLevel;

        public override void EntityInit(EcsEntity ecsEntity, EcsWorld ecsWorld, bool parentOnScene)
        {
            ecsEntity.Get<Components.MergeUnit>() = _mergeUnit;
            ecsEntity.Get<Components.UpdateMergeUnitViewSignal>();

            if (_limitMaxLevel)
                ecsEntity.Get<Components.UpgradeLimitTag>();
        }
    }
}
