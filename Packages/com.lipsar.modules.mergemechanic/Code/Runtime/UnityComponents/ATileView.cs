using Leopotam.Ecs;
using UnityEngine;

namespace Modules.MergeMechanic
{
    public abstract class ATileView : ViewHub.ViewComponent
    {
        [SerializeField] private int _tileID;

        public override void EntityInit(EcsEntity ecsEntity, EcsWorld ecsWorld, bool parentOnScene)
        {
            ref var tile = ref ecsEntity.Get<Components.TileViewRef>();
            tile.ID = _tileID;
            tile.View = this;
        }

        public abstract void ResetView();
        public abstract void SwitchOnMergeAvailableView(bool value);
        public abstract void SwitchDefaultView(bool value);
        public abstract void SwitchOnUnitPickedView(bool value);
        public abstract void SwitchOnHoverView(bool value);
        public abstract void SwitchLockedView(bool value);
    }
}