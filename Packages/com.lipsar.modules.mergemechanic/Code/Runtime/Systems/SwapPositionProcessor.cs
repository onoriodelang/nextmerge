using Leopotam.Ecs;

namespace Modules.MergeMechanic
{
    public class SwapPositionProcessor : IEcsRunSystem
    {
        private readonly EcsFilter<Components.OnTile, Components.SwapPositionTargetSignal> _target = null;
        private readonly EcsFilter<Components.OnTile, Components.SwapPositionVictimSignal> _drag = null;

        private readonly EcsWorld _world = null;
        public void Run()
        {
            if (_target.IsEmpty())
                return;

            foreach (var i in _drag)
            {
                int tileID = _drag.Get1(i).TileID;

                foreach (var j in _target)
                {
                    _drag.Get1(i).TileID = _target.Get1(j).TileID;
                    _target.Get1(j).TileID = tileID;
                }

                _world.NewEntity().Get<Components.RepositionSignal>();
            }
        }
    }
}