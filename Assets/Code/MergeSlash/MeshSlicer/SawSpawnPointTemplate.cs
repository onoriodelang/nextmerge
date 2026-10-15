using Leopotam.Ecs;
using Modules.MergeSlash.Interactors;
using Modules.ViewHub;
using UnityEngine;

namespace Modules.MergeSlash.EntityTemplates
{
    public class SawSpawnPointTemplate : ViewComponent
    {
        public SawTemplate Saw;

        public override void EntityInit(EcsEntity ecsEntity, EcsWorld ecsWorld, bool parentOnScene)
        {
            
            ref var point = ref ecsEntity.Get<SpawnSawPoint>();
            point.Saw = Saw;

            // world.NewEntity().Get<SpawnSawSignal>();
        }
    }
   
}