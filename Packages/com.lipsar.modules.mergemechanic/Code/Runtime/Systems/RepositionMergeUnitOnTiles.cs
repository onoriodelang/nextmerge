using Leopotam.Ecs;
using System;

namespace Modules.MergeMechanic
{
    public class RepositionMergeUnitOnTiles : IEcsRunSystem
    {
        private readonly EcsFilter<Components.RepositionSignal> _initSignal = null;
        private readonly EcsFilter<Components.TileViewRef, ViewHub.UnityView> _tiles = null;
        private readonly EcsFilter<Components.OnTile, ViewHub.UnityView> _units = null;

        private readonly Data.TileConfig _tileConfig;

        public void Run()
        {
            if (_initSignal.IsEmpty())
                return;

            foreach (var i in _tiles)
            {
                int ID = _tiles.Get1(i).ID;

                foreach (var j in _units)
                {
                    if (_units.Get1(j).TileID == ID)
                    {
                        var position = _tiles.Get2(i).Transform.position;

                        position.x += _tileConfig.UnitOffsetX;
                        position.y += _tileConfig.UnitOffsetY;
                        position.z += _tileConfig.UnitOffsetZ;

                        _units.Get2(j).Transform.position = position;
                    }
                }
            }
        }
    }
}