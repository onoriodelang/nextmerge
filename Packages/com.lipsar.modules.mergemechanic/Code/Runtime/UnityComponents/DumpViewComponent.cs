using Leopotam.Ecs;
using UnityEngine;

namespace Modules.MergeMechanic
{
    public class DumpViewComponent : ViewHub.ViewComponent
    {
        public Animator Animator;
        public GameObject DefaultView;
        public GameObject HoverView;
        
        public override void EntityInit(EcsEntity ecsEntity, EcsWorld ecsWorld, bool parentOnScene)
        {
            ref var dump = ref ecsEntity.Get<Components.DumpTag>();
                dump.Animator = Animator;
                dump.DefaultView = DefaultView;
                dump.HoverView = HoverView;
        }
    }
}
