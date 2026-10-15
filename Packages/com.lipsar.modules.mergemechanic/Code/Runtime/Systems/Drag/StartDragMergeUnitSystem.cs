using Leopotam.Ecs;
using UnityEngine;

namespace Modules.MergeMechanic
{
    public class StartDragMergeUnitSystem : IEcsRunSystem
    {
        private readonly EcsFilter<Components.MergeUnit, ViewHub.UnityView, UserInput.PointerDown> _startDrag = null;
        
        private readonly EcsWorld _world = null;

        public void Run()
        {
            if (_startDrag.IsEmpty())
                return;

            foreach (var i in _startDrag)
            {
                _startDrag.GetEntity(i).Get<Components.DragTargetTag>();
                _startDrag.Get1(i).Collider.enabled = false;
            }

            _world.NewEntity().Get<Components.DragStartSignal>();
        }
    }
}