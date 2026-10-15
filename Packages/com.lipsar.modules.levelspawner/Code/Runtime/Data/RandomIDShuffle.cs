using UnityEngine;

namespace Modules.LevelSpawner.Data
{
    public class RandomIDShuffle
    {
        public const string LAST_ID_VALUE_SAVE_KEY = "M_LS_SHUFFLE_last_id_value_";
        public const string LAST_VALUE_SAVE_KEY = "M_LS_SHUFFLE_last_value_";

        private int Current;
        private int[] IDS;
        private string _saveKey;

        public RandomIDShuffle(string saveKey, int amount)
        {
            _saveKey = saveKey;
            if(HasSave())
            {
                IDS = new int[amount];
                for (int i = 0; i < IDS.Length; i++)
                {
                    IDS[i] = i;
                }
                Load();
            }else
            {
                IDS = new int[amount];
                for (int i = 0; i < IDS.Length; i++)
                {
                    IDS[i] = i;
                }
                Shuffle();
            }
        }

        public int Get(int id)
        {
            if(Current != id && id%IDS.Length  == 0)
            {
                Shuffle();
                Current = id;
                Save();
                return IDS[Current%IDS.Length];
            }

            Current = id;
            Save();
            return IDS[Current%IDS.Length];
        }

        /// <summary>
        /// simple random swap shuffle
        /// </summary>
        public void Shuffle() 
        {
            int prev;
            int shufflePos;
            int prevShuffledValue = IDS[IDS.Length-1];
            for (int i = 0; i < IDS.Length; i++)
            {
                shufflePos = UnityEngine.Random.Range(0, IDS.Length);
                prev = IDS[shufflePos];
                IDS[shufflePos] = IDS[i];
                IDS[i] = prev;
            }

            if(prevShuffledValue == IDS[0])
            {
                IDS[0] = IDS[IDS.Length-1];
                IDS[IDS.Length-1] = prevShuffledValue;
            }
        }
        
        public void Load()
        {
            int lastPos = PlayerPrefs.GetInt(LAST_ID_VALUE_SAVE_KEY+_saveKey, 0);
            int lastValue = PlayerPrefs.GetInt(LAST_VALUE_SAVE_KEY+_saveKey, 0);
            Shuffle();

            for (int i = 0; i < IDS.Length; i++)
            {
                if(IDS[i] == lastValue)
                {
                    IDS[i] = IDS[lastPos%IDS.Length];
                    IDS[lastPos%IDS.Length] = lastValue;
                    Current = lastPos;
                    return;
                }
            }
        }

        public bool HasSave()
        {
            return PlayerPrefs.HasKey(LAST_ID_VALUE_SAVE_KEY+_saveKey) && PlayerPrefs.HasKey(LAST_VALUE_SAVE_KEY+_saveKey);
        }

        public void Save()
        {
            PlayerPrefs.SetInt(LAST_ID_VALUE_SAVE_KEY+_saveKey, Current);
            PlayerPrefs.SetInt(LAST_VALUE_SAVE_KEY+_saveKey, IDS[Current%IDS.Length]);
            PlayerPrefs.Save();
        }
    }
}