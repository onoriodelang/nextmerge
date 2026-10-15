using Leopotam.Ecs;
using Modules.MergeMechanic.Components;
using Modules.MergeSlash.EntityTemplates;
using Modules.MergeSlash.UI.UnityComponents;
using Modules.ST;
using Modules.ST.EntityTemplates;
using Modules.ViewHub;
using UnityEngine;

namespace Modules.MergeSlash
{
    public class EmoteSystem : IEcsRunSystem
    {
        private EcsFilter<EnemyView, UnityView> _filter;
        private EcsFilter<SawView, UnityView> _saw;
        private EcsFilter<EnemyAngrySignal> _signalAngry;
        private EcsFilter<EnemyScarySignal> _signalScary;
        private EcsFilter<EnemySmileSignal> _signalSmile;

        private EcsWorld _world;
        private PrototypeConfig _prototypeConfig;

        public EmoteSystem(PrototypeConfig prototypeConfig)
        {
            _prototypeConfig = prototypeConfig;
        }
        
        public void Run()
        {
            if(!_signalAngry.IsEmpty())
                EnableEmote(0);
            
            if(!_signalScary.IsEmpty())
                EnableEmote(1);
            
            if(!_signalSmile.IsEmpty())
                EnableEmote(2);

            foreach (var enemyIdx in _filter)
            {
                Vector3 enemyPos = _filter.Get2(enemyIdx).Transform.position;
                enemyPos.y = 0;

                bool inDanger = false;
                foreach (var sawIdx in _saw)
                {
                    if(!_saw.Get2(sawIdx).Transform)
                        continue;
                    
                    Vector3 sawPos = _saw.Get2(sawIdx).Transform.position;
                    sawPos.y = 0;

                    if (Vector3.Distance(enemyPos, sawPos) < _prototypeConfig.ScaryEmoteDistance)
                    {
                        inDanger = true;
                        break;
                    }
                }


                ref var enemyView = ref _filter.Get1(enemyIdx);
                if (enemyView.SkinnedMeshRenderer != null && enemyView.EmotesMeshes != null &&
                    enemyView.EmotesMeshes.Length > 2)
                {
                    enemyView.SkinnedMeshRenderer.sharedMesh = enemyView.EmotesMeshes[inDanger ? 1 : 0];
                    enemyView.SkinnedMeshRenderer.material = enemyView.EmotesMaterials[inDanger ? 1 : 0];
                }
            }
        }

        private void EnableEmote(int emoteIdx)
        {
            foreach (var i in _filter)
            {
                ref var enemyView = ref _filter.Get1(i);
                if (enemyView.SkinnedMeshRenderer != null && enemyView.EmotesMeshes != null && enemyView.EmotesMeshes.Length > 2)
                {
                    enemyView.SkinnedMeshRenderer.sharedMesh = enemyView.EmotesMeshes[emoteIdx];
                    enemyView.SkinnedMeshRenderer.material = enemyView.EmotesMaterials[emoteIdx];
                }
            }
            
        }
    }

    public struct EnemyAngrySignal : IEcsIgnoreInFilter
    {
    }
    
    public struct EnemyScarySignal : IEcsIgnoreInFilter
    {
    }
    
    public struct EnemySmileSignal : IEcsIgnoreInFilter
    {
    }

}