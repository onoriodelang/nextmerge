using Leopotam.Ecs;
using UnityEngine;

namespace Modules.MergeMechanic
{
    public class SaveTileState : IEcsRunSystem
    {
        private readonly EcsFilter<Components.UnlockTileSignal> _unlock = null;
        private readonly EcsFilter<Components.TileViewRef>.Exclude<Components.LockedTag> _availableTiles = null;

        private readonly Data.SaveConfig _saveConfig;
        public void Run()
        {
            if (_unlock.IsEmpty() || _availableTiles.IsEmpty())
                return;

            int[] unlockedTileID = new int[_availableTiles.GetEntitiesCount()];

            foreach (var i in _availableTiles)
            {
                unlockedTileID[i] = _availableTiles.Get1(i).ID;
            }

            Utils.PlayerPrefsX.SetIntArray(_saveConfig.UnlockedTileKey, unlockedTileID);
            PlayerPrefs.Save();
        }
    }
}