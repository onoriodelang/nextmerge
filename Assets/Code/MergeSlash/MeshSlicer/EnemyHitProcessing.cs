using Leopotam.Ecs;
using Modules.MergeSlash.Components;
using Modules.ST.EntityTemplates;
using UnityEngine;

namespace Modules.MergeSlash.Interactors
{
    public class EnemyHitProcessing : IEcsRunSystem
    {
        private EcsFilter<EnemyHitSignal, EnemyView> _filter;
        
        public void Run()
        {
            if(_filter.IsEmpty())
                return;

            foreach (var filter in _filter)
            {
                var enemyEntity = _filter.GetEntity(filter);
                ref var hit = ref _filter.Get1(filter);

                // hit particles
                ref var enemy = ref _filter.Get2(filter);
                enemy.BloodParticle.transform.parent = hit.EnemyObject.transform.parent;
                var pos = enemy.BloodParticle.transform.position;
                pos.y = hit.SawView.Transform.position.y;
                enemy.BloodParticle.transform.position = pos;
                enemy.BloodParticle.transform.LookAt(hit.SawView.Transform.position);
                enemy.BloodParticle.transform.eulerAngles = enemy.BloodParticle.transform.eulerAngles + new Vector3(0f, -90f, 0f);
                enemy.BloodParticle.Play();
                
                enemyEntity.Get<EnemyKilledTag>();
                
            }
        }
    }
}