using Leopotam.Ecs;
using Modules.MergeMechanic.Components;
using Modules.MergeSlash.UI.UnityComponents;
using Modules.ST.EntityTemplates;
using Modules.Utils;
using Modules.ViewHub;
using UnityEngine;

namespace Modules.MergeSlash.Interactors
{
    public class WalkingSystem : IEcsRunSystem
    {
        
        private EcsFilter<EnemyView> _enemies;
        private EcsFilter<EnemyView, UnityView, WalkingTag> _walkingEnemies;
        private EcsFilter<EnemyWalkSignal> _signal;

        private EcsWorld _world;
        private TimeService _timeService;
        
        public void Run()
        {
            foreach (var i in _walkingEnemies)
            {
                ref var view = ref _walkingEnemies.Get1(i);
                float diff = _timeService.DeltaTime * view.WalkingSpeed;

                if (diff < view.WalkingDistance)
                {
                    _walkingEnemies.Get2(i).Transform.position +=
                        _walkingEnemies.Get2(i).Transform.forward * -diff;

                    view.WalkingDistance -= diff;
                }
                else
                {
                    _walkingEnemies.Get2(i).Transform.position +=
                        _walkingEnemies.Get2(i).Transform.forward * -view.WalkingDistance;
                    
                    view.WalkingDistance = 0;
                    
                    view.Animator.SetBool("Walk", false);
                    _walkingEnemies.GetEntity(i).Del<WalkingTag>();
                }
            }
            
            if(_signal.IsEmpty())
                return;

            foreach (var i in _enemies)
            {
                if(!_enemies.Get1(i).Walking)
                    continue;
                
                _enemies.GetEntity(i).Get<WalkingTag>();
                _enemies.Get1(i).Animator.SetBool("Walk", true);
            }
        }
    }

    public struct EnemyWalkSignal : IEcsIgnoreInFilter
    {
        
    }
    
    public struct WalkingTag : IEcsIgnoreInFilter
    {
        
    }
}