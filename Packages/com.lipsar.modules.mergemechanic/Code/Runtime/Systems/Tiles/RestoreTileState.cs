using Leopotam.Ecs;

namespace Modules.MergeMechanic
{
    public class RestoreTileState : IEcsRunSystem
    {
        private readonly EcsFilter<LevelSpawner.LevelSpawnedSignal> _init = null;
        private readonly EcsFilter<Components.TileViewRef> _allTiles = null;

        private readonly EcsWorld _world;

        private readonly Data.SaveConfig _saveConfig;

        public void Run()
        {
            if (_init.IsEmpty())
                return;

            int[] unlockedTileID = Utils.PlayerPrefsX.GetIntArray(_saveConfig.UnlockedTileKey);

            if (unlockedTileID.Length > 0)
            {
                foreach (var i in _allTiles)
                {
                    int tileID = _allTiles.Get1(i).ID;
                    foreach (var j in unlockedTileID)
                    {
                        if (j == tileID)
                            _allTiles.GetEntity(i).Del<Components.LockedTag>();
                    }
                }
            }

            _world.NewEntity().Get<Components.UpdateTileViewSignal>();
        }
    }
}