using Leopotam.Ecs;
using UICoreECS;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Modules.MergeSlash.UI.UnityComponents
{
    // activate via pointer down to not require click for transition
    public class ThrowButton : AUIEntity, IPointerDownHandler
    {
        private EcsWorld _world;

        public override void Init(EcsWorld world, EcsEntity screen)
        {
            _world = world;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _world.NewEntity().Get<ThrowButtonSignal>();
        }
    }

    public struct ThrowButtonSignal : IEcsIgnoreInFilter
    {
        
    }
}
