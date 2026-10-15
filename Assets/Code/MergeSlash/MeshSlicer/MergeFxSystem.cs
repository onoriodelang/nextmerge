using Leopotam.Ecs;
using Modules.MergeMechanic.Components;
using Modules.ST.EntityTemplates;
using Modules.ViewHub;
using UnityEngine;

namespace Modules.MergeSlash.Interactors
{
    public class MergeFxSystem : IEcsRunSystem
    {
        private EcsFilter<MergeFx> _filter;
        private EcsFilter<MergeTargetSignal, UnityView> _signal;

        private EcsWorld _world;
        
        public void Run()
        {
            if(_signal.IsEmpty())
                return;
            
            foreach (var signalIdx in _signal)
            {
                Vector3 pos = _signal.Get2(signalIdx).Transform.position;
                
                foreach (var idx in _filter)
                {
                    _filter.Get1(idx).Root.position = pos;
                    _filter.Get1(idx).ParticleSystem.Play();
                }
            }
            
            if (VibroService.VibroSettings.VibroEnabled)
            {
                MoreMountains.NiceVibrations.MMVibrationManager.Haptic(MoreMountains.NiceVibrations.HapticTypes.SoftImpact);
            }
        }
    }
}