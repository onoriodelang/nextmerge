using System.Collections.Generic;
using EzySlice;
using Leopotam.Ecs;
using Modules.MergeMechanic.Components;
using Modules.ST.EntityTemplates;
using Modules.ST.MeshSlicer.Components;
using Modules.Utils;
using Modules.ViewHub;
using UnityEngine;

namespace Modules.ST.MeshSlicer
{
    public class MergeHelpSystem : IEcsRunSystem
    {
        private EcsFilter<MergeUnit, DragTargetTag> _dragUnits;
        private EcsFilter<MergeUnit>.Exclude<DragTargetTag> _calmUnits;

        private EcsWorld _world;
        private TimeService _timeService;

        public void Run()
        {
            if (_dragUnits.IsEmpty())
                AllCollidersNormalSize();
            else
                IncreaseColliders();
        }

        private void AllCollidersNormalSize()
        {
            foreach (var i in _calmUnits)
            {
                _calmUnits.Get1(i).Collider.size = _calmUnits.Get1(i).NormalColliderSize;
            }
        }

        private void IncreaseColliders()
        {
            foreach (var dragIdx in _dragUnits)
            {
                int unitType = _dragUnits.Get1(dragIdx).TypeID;
                int unitLevel = _dragUnits.Get1(dragIdx).CurrentLevel;

                foreach (var calmIdx in _calmUnits)
                {
                    ref var calmUnit = ref _calmUnits.Get1(calmIdx);
                    if(calmUnit.TypeID == unitType && calmUnit.CurrentLevel == unitLevel)
                        calmUnit.Collider.size = calmUnit.HelpColliderSize;
                }
                
            }
            
        }
    }

    
}