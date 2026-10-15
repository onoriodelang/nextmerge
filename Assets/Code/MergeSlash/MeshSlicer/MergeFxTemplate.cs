using Leopotam.Ecs;
using Modules.ViewHub;
using UnityEngine;

namespace Modules.ST.EntityTemplates
{
    public class MergeFxTemplate : ViewElement
    {
        public ParticleSystem ParticleSystem; 
        public Transform Root; 
        
        public override void OnSpawn(EcsEntity entity, EcsWorld world)
        {
            base.OnSpawn(entity, world);
            ref var view = ref entity.Get<MergeFx>();
            view.ParticleSystem = ParticleSystem;
            view.Root = Root;
        }
    }

    public struct MergeFx
    {
        public ParticleSystem ParticleSystem; 
        public Transform Root; 
    }
}