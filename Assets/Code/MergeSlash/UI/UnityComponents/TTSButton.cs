using Leopotam.Ecs;
using UICoreECS;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Modules.MergeSlash.UI.UnityComponents
{
    // activate via pointer down to not require click for transition
    public class TTSButton : AUIEntity, IPointerDownHandler
    {
        private EcsWorld _world;

        public override void Init(EcsWorld world, EcsEntity screen)
        {
            _world = world;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            EventGroup.StateFactory.CreateState<EventGroup.GamePlayState>(_world);
        }
    }
}
