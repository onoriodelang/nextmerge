using Leopotam.Ecs;
using Modules.MergeSlash.Components;
using Modules.MergeSlash.EntityTemplates;
using Modules.ST.EntityTemplates;
using Modules.ViewHub;
using UnityEngine;

namespace Modules.MergeSlash.Interactors
{
    public class SawFlyProcessing : IEcsRunSystem
    {
        private EcsFilter<SawView, SawThrownTag> _sawView;
        private EcsFilter<TargetWithHealth, UnityView> _targets;
        
        private Utils.TimeService _time;
        
        public void Run()
        {
            if(_sawView.IsEmpty())
                return;
            
            foreach (var sawView in _sawView)
            {
                ref var saw = ref _sawView.Get1(sawView);
                saw.Transform.Translate(Vector3.forward * (_time.DeltaTime * saw.Speed), Space.World);
                saw.SawVisible.transform.Rotate(0f, 5f * saw.RotationSpeed, 0f, Space.Self);
                saw.Collider.enabled = true;
            }
        }
    }
}