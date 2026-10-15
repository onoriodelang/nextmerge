using Leopotam.Ecs;
using UnityEngine;

namespace Modules.MergeMechanic
{
    public class EndDragMergeUnitSystem : IEcsRunSystem
    {
        private readonly EcsFilter<Components.MergeUnit, Components.DragTargetTag> _drag = null;
        private readonly EcsFilter<UserInput.OnScreenTapUp> _onTapUp = null;
        private readonly EcsFilter<Components.OnTile>.Exclude<Components.DragTargetTag> _occupiedTiles = null;

        private readonly EcsWorld _world = null;

        private EcsEntity currentEntity;
        private EcsEntity targetEntity;

        public void Run()
        {
            if (_drag.IsEmpty() || _onTapUp.IsEmpty())
                return;

            foreach (var i in _drag)
            {

                currentEntity = _drag.GetEntity(i);

                RaycastCheck();

                _drag.GetEntity(i).Del<Components.DragTargetTag>();
                _world.NewEntity().Get<Components.DragEndSignal>();
                _drag.Get1(i).Collider.enabled = true;
            }
        }

        private void RaycastCheck()
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, float.PositiveInfinity))
            {
                var entityRef = hit.transform.GetComponent<ViewHub.EntityRef>();
                if (entityRef != null)
                {
                    targetEntity = entityRef.Entity;

                    if (targetEntity.Has<Components.MergeUnit>() && !targetEntity.Has<Components.DragTargetTag>())
                    {
                        MergeOrSwapPosition();
                    }
                    else if (targetEntity.Has<Components.TileViewRef>() && !targetEntity.Has<Components.LockedTag>())
                    {
                        OnTilePlacement();
                    }
                    else if (targetEntity.Has<Components.DumpTag>())
                    {
                        Sell();
                    }
                    else
                    {
                        ResetPosition();
                    }
                }
                else
                {
                    ResetPosition();
                }
            }
            else
            {
                ResetPosition();
            }
        }

        private void MergeOrSwapPosition()
        {
            var current = currentEntity.Get<Components.MergeUnit>();
            var target = targetEntity.Get<Components.MergeUnit>();

            if (target.TypeID == current.TypeID && target.CurrentLevel == current.CurrentLevel && target.CurrentLevel < target.MaxLevel)
            {
                MergeUnits();
            }
            else
            {
                SwapUnitsPosition();
            }
        }

        private void OnTilePlacement()
        {
            int tileID = targetEntity.Get<Components.TileViewRef>().ID;

            if (TileIsOccupied(tileID))
            {
                SwapUnitsPosition();
                return;
            }

            currentEntity.Get<Components.OnTile>().TileID = tileID;
            _world.NewEntity().Get<Components.RepositionSignal>();
        }

        private void Sell()
        {
            currentEntity.Get<Components.SellMergeUnitSignal>();
            currentEntity.Get<Modules.Utils.DestroyTag>();
        }

        private void ResetPosition()
        {
            _world.NewEntity().Get<Components.RepositionSignal>();
        }

        private bool TileIsOccupied(int tileID)
        {
            foreach (var i in _occupiedTiles)
            {
                if (_occupiedTiles.Get1(i).TileID == tileID)
                {
                    targetEntity = _occupiedTiles.GetEntity(i);
                    return true;
                }
            }
            return false;
        }

        private void MergeUnits()
        {
            targetEntity.Get<Components.MergeTargetSignal>();
            currentEntity.Get<Components.MergeVictimSignal>();
        }

        private void SwapUnitsPosition()
        {
            targetEntity.Get<Components.SwapPositionTargetSignal>();
            currentEntity.Get<Components.SwapPositionVictimSignal>();
        }
    }
}