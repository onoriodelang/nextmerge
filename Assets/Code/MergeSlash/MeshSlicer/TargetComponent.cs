using Leopotam.Ecs;
using Modules.ViewHub;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.ST.EntityTemplates
{
    public class TargetComponent : ViewComponent
    {
        public int Health;
        public int Gold;
        public Slider HealthSlider;
        public bool IsEnemy;
        public Animator EnemyCanvasAnimator;

        public override void EntityInit(EcsEntity ecsEntity, EcsWorld ecsWorld, bool parentOnScene)
        {
            ref var target = ref ecsEntity.Get<TargetWithHealth>();
            target.CurrentHealth = Health;
            target.FullHealth = Health;
            target.HealthSlider = HealthSlider;
            target.Reward = Gold;
            target.EnemyCanvasAnimator = EnemyCanvasAnimator;

            if (IsEnemy)
            {
                ecsEntity.Get<Enemy>();
            }

            if (HealthSlider)
                HealthSlider.value = 1f;
        }
    }

    public struct TargetWithHealth
    {
        public int CurrentHealth;
        public int FullHealth;
        public int Reward;
        public Slider HealthSlider;
        public Animator EnemyCanvasAnimator;
    }

    public struct Enemy : IEcsIgnoreInFilter
    {
        
    }
}