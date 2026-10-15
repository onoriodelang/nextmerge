using Leopotam.Ecs;
using UnityEngine;

namespace Modules.MergeMechanic
{
    public class TileViewComponent : ATileView
    {
        [SerializeField] private bool _isLocked;
        [SerializeField] private bool _isTileForShooting;

        [SerializeField] private GameObject DefaultView;
        [SerializeField] private GameObject OnMergeAvailableView;
        [SerializeField] private GameObject OnUnitPickedView;
        [SerializeField] private GameObject OnHoverView;
        [SerializeField] private GameObject LockedView;

        public override void EntityInit(EcsEntity ecsEntity, EcsWorld ecsWorld, bool parentOnScene)
        {
            base.EntityInit(ecsEntity, ecsWorld, parentOnScene);

            if (_isLocked)
                ecsEntity.Get<Components.LockedTag>();
            
            if (_isTileForShooting)
                ecsEntity.Get<TileForShootingTag>();
        }

        public override void ResetView()
        {
            SwitchOnHoverView(false);
            SwitchOnMergeAvailableView(false);
            SwitchOnUnitPickedView(false);
        }

        public override void SwitchDefaultView(bool value)
        {
            DefaultView.SetActive(value);
        }

        public override void SwitchLockedView(bool value)
        {
            LockedView.SetActive(value);
        }

        public override void SwitchOnHoverView(bool value)
        {
            OnHoverView.SetActive(value);
            SwitchDefaultView(!value);
        }

        public override void SwitchOnMergeAvailableView(bool value)
        {
            OnMergeAvailableView.SetActive(value);
            SwitchDefaultView(!value);
        }

        public override void SwitchOnUnitPickedView(bool value)
        {
            OnUnitPickedView.SetActive(value);
            SwitchDefaultView(!value);
        }
    }
    
    public struct TileForShootingTag : IEcsIgnoreInFilter { }
}
