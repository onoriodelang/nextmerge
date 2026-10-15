using Leopotam.Ecs;
using Modules.MergeSlash.Components;
using Modules.MergeSlash.EntityTemplates;
using Modules.ST.EntityTemplates;
using Modules.Utils;
using Modules.ViewHub;
using UnityEngine;

namespace Modules.MergeSlash
{
    public class LevelDataInitSystem : IEcsRunSystem
    {
        private EcsFilter<TargetWithHealth, Enemy> _enemies;
        private EcsFilter<TargetWithHealth> _targets;
        private EcsFilter<LevelData> _data;
        
        public void Run()
        {
            foreach (var dataIdx in _data)
            {
                if(_data.Get1(dataIdx).Inited)
                    continue;
                
                LevelData levelData = _data.Get1(dataIdx);
                int targetsNumber = _targets.GetEntitiesCount();
                int enemiesNumber = _enemies.GetEntitiesCount();
                int health = levelData.AllTargetsHealth / enemiesNumber;
                int reward = levelData.AllTargetsGold / targetsNumber;
                
                foreach (var enemyIdx in _enemies)
                {
                    _enemies.Get1(enemyIdx).CurrentHealth = health;
                    _enemies.Get1(enemyIdx).FullHealth = health;
                }
                
                foreach (var targetIdx in _targets)
                {
                    _targets.Get1(targetIdx).Reward = reward;
                }

                _data.Get1(dataIdx).Inited = true;
            }
        }
    }

}