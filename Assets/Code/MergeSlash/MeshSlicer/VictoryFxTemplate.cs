using Leopotam.Ecs;
using Modules.ViewHub;
using UnityEngine;

namespace Modules.ST.EntityTemplates
{
    public class VictoryFxTemplate : ViewElement
    {
        public ParticleSystem ParticleSystem; 
        public Transform Root; 
        
        public override void OnSpawn(EcsEntity entity, EcsWorld world)
        {
            base.OnSpawn(entity, world);
            ref var view = ref entity.Get<VictoryFx>();
            view.ParticleSystem = ParticleSystem;
            view.Root = Root;
        }
    }

    public struct VictoryFx
    {
        public ParticleSystem ParticleSystem; 
        public Transform Root; 
    }
}