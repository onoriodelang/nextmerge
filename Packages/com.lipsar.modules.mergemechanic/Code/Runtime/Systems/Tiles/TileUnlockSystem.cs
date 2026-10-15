using Leopotam.Ecs;

namespace Modules.MergeMechanic
{
    public class TileUnlockSystem : IEcsRunSystem
    {
        private readonly EcsFilter<Components.UnlockTileSignal> _unlock = null;
        private readonly EcsFilter<Components.TileViewRef>.Exclude<Components.LockedTag> _availableTiles = null;
        private readonly EcsFilter<Components.TileViewRef> _allTiles = null;

        private readonly EcsWorld _world;

        public void Run()
        {
            if (_unlock.IsEmpty())
                return;

            int count = _availableTiles.GetEntitiesCount();

            foreach (var i in _allTiles)
            {
                if (_allTiles.Get1(i).ID == count)
                {
                    _allTiles.GetEntity(i).Del<Components.LockedTag>();
                    _allTiles.GetEntity(i).Get<Components.UnlockedTileSignal>();
                }
            }

            _world.NewEntity().Get<Components.UpdateTileViewSignal>();
        }
    }
}