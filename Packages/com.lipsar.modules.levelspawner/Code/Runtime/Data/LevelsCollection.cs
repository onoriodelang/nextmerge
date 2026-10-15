using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
#if UNITY_EDITOR
using System.Linq;
using System.IO;
using UnityEditor;
#endif

namespace Modules.LevelSpawner
{
    public enum OverfillStrategy
    {
        Loop,
        PureRandom,
        Shuffled
    }

    [CreateAssetMenu(menuName = "Modules/LevelSpawner/LevelsCollection")]
    public class LevelsCollection : ScriptableObject
    {
        [SerializeField] private OverfillStrategy _overfillStrategy;
        [SerializeField] private int _shuffleStartId;
        [SerializeField] public string[] Levels;
        [SerializeField] public string[] BossLevels;
        [SerializeField] public int LevelsBetweenBossLevels;
        private Data.RandomIDShuffle randomIDShuffle;


        public void Init()
        {
            if(Levels.Length - _shuffleStartId <= 0)
            {
                _shuffleStartId = 0;
            }

            randomIDShuffle = new Data.RandomIDShuffle("levels_collection_a", Levels.Length-_shuffleStartId);
            
        }

        public string Get(int id) 
        {
            switch (_overfillStrategy)
            {
                case OverfillStrategy.Shuffled:
                    return GetWithShuffle(id);
                case OverfillStrategy.PureRandom:
                    return GetWithRandomOverFill(id);
                case OverfillStrategy.Loop:
                    return Levels[id % Levels.Length];
                default:
                    return Levels[id % Levels.Length];
            }
        }

        // if id > levels return with id else return random
        public string GetWithRandomOverFill(int id)
        {
            if(id < Levels.Length)
            {
                return Levels[id % Levels.Length];
            }
            else
            {
                return Levels[Random.Range(0, Levels.Length)];
            }
        }

        public string GetWithShuffle(int id)
        {
            int levelsLengthWithBoss = LevelsBetweenBossLevels > 0 ? Levels.Length + Levels.Length / (LevelsBetweenBossLevels) : Levels.Length;
            if(id < levelsLengthWithBoss)
            {
                if (LevelsBetweenBossLevels == 0 || (id + 1) % (LevelsBetweenBossLevels + 1) != 0)
                {
                    // regular level
                    if(LevelsBetweenBossLevels > 0)
                        id -= (id + 1) / (LevelsBetweenBossLevels + 1);
                    return Levels[id % levelsLengthWithBoss];
                }
                else
                {
                    // boss level
                    return BossLevels[id / (LevelsBetweenBossLevels + 1) % BossLevels.Length];
                }
            }

            if (LevelsBetweenBossLevels == 0 || (id + 1) % (LevelsBetweenBossLevels + 1) != 0)
            {
                // regular level
                if (id >= _shuffleStartId)
                {
                    id -= _shuffleStartId;
                }

                return Levels[(randomIDShuffle.Get(id) + _shuffleStartId) % Levels.Length];
            }
            else
            {
                // boss level
                return BossLevels[id / (LevelsBetweenBossLevels + 1) % BossLevels.Length];
            }
        }

        public string GetRandom() 
        {
            return Levels[Random.Range(0, Levels.Length)];
        }

#if UNITY_EDITOR
        
        [ContextMenu("Collect")]
        public void Collect() 
        {
            // get directory with config
            string targetDir = Path.GetDirectoryName(AssetDatabase.GetAssetPath(this));

            // collect assets at directory
            List<GameObject> levels = new List<GameObject>();
            foreach (var guid in AssetDatabase.FindAssets("", new[] { targetDir }))
            {
                GameObject go = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(GameObject)) as GameObject;
                if (go != null)
                {
                    levels.Add(go);
                }

            }

            // sort by name
            // add length at start of name to correctly sort strings with different lengths
            levels.OrderBy(level => level.name.Length.ToString() + level.name);
            Levels = new string[levels.Count];
            for (int i = 0; i < levels.Count; i++)
            {
                Levels[i] = levels[i].name;
            }
            UnityEditor.EditorUtility.DisplayDialog("Levels collection", $"Collected and sorted by name {levels.Count} levels", "OK");
        }

#endif
    }
}
