using Leopotam.Ecs;
using Modules.EventGroup;
using Modules.LevelSpawner;
using Modules.MergeSlash.Components;
using Modules.MergeSlash.EntityTemplates;
using Modules.MergeSlash.UI;
using Modules.PlayerLevel;
using Modules.Utils;
using Modules.ViewHub;
using UnityEngine;

namespace Modules.MergeSlash.Interactors
{
    public class ProgressBarSystem : IEcsRunSystem
    {
        private EcsFilter<ProgressBarView> _bars;
        private EcsFilter<GamePlayState> _gameplay;

        private EcsWorld _world;
        private LevelsCollection _levelsCollection;

        public ProgressBarSystem(LevelsCollection levelsCollection)
        {
            _levelsCollection = levelsCollection;
        }
        
        public void Run()
        {
            if(_gameplay.IsEmpty())
                return;

            int level = ProgressionInfo.CurrentLevel + 1;
             int numberOfCurrentLevel = level < 3 ? level : 3;
            int[] levelNumbers = new int[5];

            if (level < 3)
            {
                for (int i = 0; i < levelNumbers.Length; i++)
                {
                    levelNumbers[i] = i + 1;
                    if (_levelsCollection.LevelsBetweenBossLevels > 0 && levelNumbers[i] % (_levelsCollection.LevelsBetweenBossLevels + 1) == 0)
                        levelNumbers[i] = 0;
                }
            }
            else
            {
                for (int i = 0; i < levelNumbers.Length; i++)
                {
                    levelNumbers[i] = level - 2 + i;
                    if (_levelsCollection.LevelsBetweenBossLevels > 0 && levelNumbers[i] % (_levelsCollection.LevelsBetweenBossLevels + 1) == 0)
                        levelNumbers[i] = 0;
                }
            }

            foreach (var i in _bars)
            {
                if(_bars.Get1(i).LastLevel == level)
                    continue;

                _bars.Get1(i).LastLevel = level;
                _bars.Get1(i).View.SetData(numberOfCurrentLevel - 1, levelNumbers);
            }
        }
        
        
    }

}