using Leopotam.Ecs;
using Modules.ViewHub;
using UnityEngine;

namespace Modules.ST.EntityTemplates
{
    public class CameraFocusTemplate : ViewElement
    {
        public float MoveTime;
        public AnimationCurve PathCurve;
        
        public override void OnSpawn(EcsEntity entity, EcsWorld world)
        {
            base.OnSpawn(entity, world);
            ref var view = ref entity.Get<CameraFocus>();
            // view.MoveTime = MoveTime;
            view.PathCurve = PathCurve;
            view.MoveTime = MoveTime;
        }
    }

    public struct CameraFocus
    {
        public float MoveTime;
        public float Timer;
        public AnimationCurve PathCurve;
        public Vector3 StartPos;
        public Vector3 EndPos;
    }
}