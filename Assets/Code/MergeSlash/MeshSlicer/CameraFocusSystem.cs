using System.Collections.Generic;
using EzySlice;
using Leopotam.Ecs;
using Modules.ST.EntityTemplates;
using Modules.ST.MeshSlicer.Components;
using Modules.Utils;
using Modules.ViewHub;
using UnityEngine;

namespace Modules.ST.MeshSlicer
{
    public class CameraFocusSystem : IEcsRunSystem
    {
        private EcsFilter<CameraFocus, CameraFocusMoveTag, UnityView> _filter;

        private EcsWorld _world;
        private TimeService _timeService;

        public void Run()
        {
            foreach (var i in _filter)
            {
                ref var focus = ref _filter.Get1(i);
                Transform transform = _filter.Get3(i).Transform;

                 focus.Timer += _timeService.DeltaTime;

                 if (focus.Timer < focus.MoveTime)
                 {
                     float t = focus.Timer / focus.MoveTime;
                     float y = focus.PathCurve.Evaluate(t);
                     float z = Vector3.Distance(focus.StartPos, focus.EndPos) * t;
                     Vector3 dir = (focus.EndPos - focus.StartPos).normalized;

                     Vector3 pos = focus.StartPos + dir * z + Vector3.up * y;
                     transform.position = pos;
                 }
                 else
                 {
                     transform.position = focus.EndPos;
                     _filter.GetEntity(i).Del<CameraFocusMoveTag>();
                 }
                 
            }
        }
    }

    public struct CameraFocusMoveTag
    {
        
    }
}