using Leopotam.Ecs;
using Modules.MergeSlash.Components;
using Modules.MergeSlash.EntityTemplates;
using Modules.Utils;
using Modules.ViewHub;
using UnityEngine;

namespace Modules.MergeSlash.Interactors
{
    public class DelayedScaleSystem : IEcsRunSystem
    {
        private EcsFilter<DelayedScale, UnityView> _filter; 

        private EcsWorld _world;
        private TimeService _timeService;
        
        public void Run()
        {
            foreach (var i in _filter)
            {
                ref var delayedScale = ref _filter.Get1(i);

                if (!delayedScale.Inited)
                {
                    delayedScale.Inited = true;
                    delayedScale.StartScale = _filter.Get2(i).Transform.localScale;
                }

                delayedScale.Timer += _timeService.DeltaTime;
                

                if (delayedScale.Timer >= delayedScale.ScaleTime)
                {
                    _filter.Get2(i).Transform.localScale = delayedScale.EndScale;
                    _filter.GetEntity(i).Del<DelayedScale>();
                }
                else
                {
                    float t = delayedScale.Timer / delayedScale.ScaleTime;
                    _filter.Get2(i).Transform.localScale = Vector3.Lerp(delayedScale.StartScale, delayedScale.EndScale, t);
                }

                
            }
        }
    }

    public struct DelayedScale
    {
        public float Timer;
        public float ScaleTime;
        public Vector3 StartScale;
        public Vector3 EndScale;
        public bool Inited;
    }

}