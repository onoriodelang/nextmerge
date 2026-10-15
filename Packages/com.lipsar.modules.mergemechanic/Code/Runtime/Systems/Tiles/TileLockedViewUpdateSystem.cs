using Leopotam.Ecs;

namespace Modules.MergeMechanic
{
    public class TileLockedViewUpdateSystem : IEcsRunSystem
    {
        private readonly EcsFilter<Components.UpdateTileViewSignal> _signal = null;
        private readonly EcsFilter<Components.TileViewRef> _tiles = null;

        public void Run()
        {
            if (_signal.IsEmpty())
                return;

            foreach (var i in _tiles)
            {
                _tiles.Get1(i).View.SwitchLockedView(_tiles.GetEntity(i).Has<Components.LockedTag>());
            }
        }
    }
}