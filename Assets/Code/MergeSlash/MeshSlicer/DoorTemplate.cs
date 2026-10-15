using Leopotam.Ecs;
using Modules.ViewHub;
using UnityEngine;

namespace Modules.ST.EntityTemplates
{
    public class DoorTemplate : ViewElement
    {
        public Animator Animator;
        
        public override void OnSpawn(EcsEntity entity, EcsWorld world)
        {
            base.OnSpawn(entity, world);
            ref var view = ref entity.Get<Door>();
            view.Animator = Animator;
        }
    }

    public struct Door
    {
        public Animator Animator;
    }
}