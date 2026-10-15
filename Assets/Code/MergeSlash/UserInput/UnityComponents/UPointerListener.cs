using Leopotam.Ecs;
using UnityEngine.EventSystems;

namespace Modules.MergeSlash.UserInput
{
    public class UPointerListener : ViewHub.ViewComponent, IPointerDownHandler
    {
        private EcsEntity _entity;

        public override void EntityInit(EcsEntity ecsEntity, EcsWorld ecsWorld, bool parentOnScene)
        {
            _entity = ecsEntity;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _entity.Get<PointerDown>();
        }
    }
}
