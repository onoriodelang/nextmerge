using Leopotam.Ecs;
using Modules.MergeMechanic.Components;
using Modules.MergeSlash.UI.UnityComponents;
using Modules.ST.EntityTemplates;
using Modules.Utils;
using UnityEngine;

namespace Modules.MergeSlash.Interactors
{
    public class SwitchPlayerViewSystem : IEcsRunSystem
    {
        private EcsFilter<Player, SwitchPlayerView> _filter;

        private EcsWorld _world;
        private TimeService _timeService;
        
        private static readonly int BlendKeyHash = Animator.StringToHash("Blend");
        
        public void Run()
        {
            foreach (var i in _filter)
            {
                _filter.Get2(i).Time -= _timeService.DeltaTime;
                if(_filter.Get2(i).Time > 0)
                    continue;
                
                ref var player = ref _filter.Get1(i);
                ref var playerSwitch = ref _filter.Get2(i);
                 
                
                player.HandsGO.SetActive(playerSwitch.SwitchToHandsView);

                foreach (var renderer in player.HandsRenderers)
                {
                    renderer.enabled = true;
                }
                
                player.PersonGO.SetActive(!playerSwitch.SwitchToHandsView);
                player.HandsAnimator.SetFloat(BlendKeyHash, playerSwitch.AnimationBlend);
                _filter.GetEntity(i).Del<SwitchPlayerView>();
            }
        }
    }

    public struct SwitchPlayerView
    {
        public float Time;
        public bool SwitchToHandsView;
        public float AnimationBlend;
    }
    
}