using UnityEngine;
using System.Collections.Generic;

namespace Modules.LevelSpawner.AdditiveSceneMode
{
    [CreateAssetMenu(menuName = "Modules/LevelSpawner/AdditiveSceneModeSpawnConfig")]
    public class AdditiveSceneModeSpawnConfig : ScriptableObject
    {
        [SerializeField] public SceneSwitchMode SwitchMode;
        [SerializeField] private int _shuffleStartId;
        [SerializeField] public List<string> SceneNames;
        private Data.RandomIDShuffle randomIDShuffle;
        
        public void Init()
        {
            if(SceneNames.Count - _shuffleStartId <= 0)
            {
                _shuffleStartId = 0;
            }

            randomIDShuffle = new Data.RandomIDShuffle("levels_collection_s", SceneNames.Count-_shuffleStartId);
        }

        public string GetCurrentSceneIndex(int currentLevel)
        {
            return GetWithShuffle(currentLevel);
        }

        public string ToConfigString()
        {
            string res = "";
            for (int i = 0; i < SceneNames.Count; i++)
            {
                res += SceneNames[i].ToString() + ";";
            }
            return res;
        }

        public void ApplyConfigFromString(string from)
        {
            string[] res = from.Split(';');
            List<string> newSceneNames = new List<string>();
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = res[i].Trim();
                if(res[i].Length <= 0)
                {
                    continue;
                }
                newSceneNames.Add(res[i]);
            }

            if(newSceneNames.Count > 0)
            {
                SceneNames = newSceneNames;
            }
        }
        
        public string GetWithShuffle(int id)
        {
            if(id < SceneNames.Count)
            {
                return SceneNames[id%SceneNames.Count];
            }else
            {
                if(id >= _shuffleStartId)
                {
                    id -= _shuffleStartId;
                }
                return SceneNames[(randomIDShuffle.Get(id)+_shuffleStartId)%SceneNames.Count];
            }
        }
    }
}
