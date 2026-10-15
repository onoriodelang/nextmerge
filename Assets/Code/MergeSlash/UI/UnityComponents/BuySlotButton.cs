using Leopotam.Ecs;
using UICoreECS;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Modules.MergeSlash.UI.UnityComponents
{
    // activate via pointer down to not require click for transition
    public class BuySlotButton : AUIEntity, IPointerDownHandler
    {
        private EcsWorld _world;

        public override void Init(EcsWorld world, EcsEntity screen)
        {
            _world = world;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _world.NewEntity().Get<BuySlotButtonSignal>(); 
            
            if (VibroService.VibroSettings.VibroEnabled)
            {
                MoreMountains.NiceVibrations.MMVibrationManager.Haptic(MoreMountains.NiceVibrations.HapticTypes.MediumImpact);
            }
        }
    }

    public struct BuySlotButtonSignal : IEcsIgnoreInFilter
    {
        
    }
}
