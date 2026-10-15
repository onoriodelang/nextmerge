using Leopotam.Ecs;
using Modules.MergeSlash.Components;
using Modules.MergeSlash.EntityTemplates;
using Modules.ST.EntityTemplates;
using Modules.Utils;
using Modules.ViewHub;
using UnityEngine;

namespace Modules.MergeSlash
{
    public class OpenDoorSystem : IEcsRunSystem
    {
        private EcsFilter<Door> _filter;
        private EcsFilter<OpenDoorSignal> _signal;

        private EcsWorld _world;
        
        public void Run()
        {
            if(_signal.IsEmpty())
                return;

            foreach (var i in _filter)
            {
                _filter.Get1(i).Animator.SetTrigger("Open");
                _filter.GetEntity(i).Del<Door>();
            }
        }
    }

    

    public struct OpenDoorSignal : IEcsIgnoreInFilter
    {
        
    }
}