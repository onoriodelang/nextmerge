using Leopotam.Ecs;
using Modules.MergeMechanic.Components;
using Modules.ST.EntityTemplates;
using Modules.ViewHub;
using UnityEngine;

namespace Modules.MergeSlash.Interactors
{
    public class VictoryFxSystem : IEcsRunSystem
    {
        private EcsFilter<VictoryFx> _filter;
        private EcsFilter<VictoryFxSignal> _signal;

        private EcsWorld _world;
        
        public void Run()
        {
            foreach (var signalIdx in _signal)
            {
                Vector3 pos = Camera.main.transform.position;
                
                foreach (var idx in _filter)
                {
                    _filter.Get1(idx).Root.position = pos;
                    _filter.Get1(idx).ParticleSystem.Play();
                }
            }
        }
    }
    
    public struct VictoryFxSignal: IEcsIgnoreInFilter
    {
            
    }
}