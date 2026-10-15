using Leopotam.Ecs;
using UnityEngine;

namespace Modules.MergeMechanic
{
    public class DragMergeUnitSystem : IEcsRunSystem
    {
        private readonly EcsFilter<ViewHub.UnityView, Components.DragTargetTag> _drag = null;
        private readonly EcsFilter<Components.DragStartSignal> _start = null;

        private readonly Data.DragConfig _dragConfig;

        private float prevX;
        private float prevY;
        private float deltaX;
        private float deltaY;

        Transform dragableTransform;

        public void Run()
        {
            if (_start.IsEmpty() == false)
            {
                prevX = Input.mousePosition.x / Screen.width;
                prevY = Input.mousePosition.y / Screen.height;

                foreach (var i in _drag)
                {
                    dragableTransform = _drag.Get1(i).Transform;
                }
            }

            if (_drag.IsEmpty())
                return;

            deltaX = Input.mousePosition.x / Screen.width - prevX;
            deltaY = Input.mousePosition.y / Screen.height - prevY;

            Vector3 currentPos = dragableTransform.position;
            currentPos.x = Mathf.Clamp(currentPos.x + (_dragConfig.DragSpeedX * (deltaX)), _dragConfig.XMinPos, _dragConfig.XMaxPos);
            currentPos.z = Mathf.Clamp(currentPos.z - (_dragConfig.DragSpeedZ * -(deltaY)), _dragConfig.ZMinPos, _dragConfig.ZMaxPos);
            currentPos.y = _dragConfig.YOffset;

            dragableTransform.position = currentPos;

            prevX = Input.mousePosition.x / Screen.width;
            prevY = Input.mousePosition.y / Screen.height;
        }
    }
}