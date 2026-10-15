using Leopotam.Ecs;
using Modules.MergeSlash.Components;
using Modules.MergeSlash.EntityTemplates;
using Modules.Utils;
using Modules.ViewHub;
using UnityEngine;

namespace Modules.MergeSlash.Interactors
{
    public class CameraSystem : IEcsRunSystem
    {
        private EcsFilter<SawView, SawThrownTag> _thrown;
        private EcsFilter<SawView, ThrowTag> _throwSignal;

        private EcsWorld _world;
        
        public void Run()
        {
            if (_thrown.IsEmpty() && _throwSignal.IsEmpty())
            {
                
            }
        }
    }

}