using Leopotam.Ecs;
using Modules.ViewHub;
using UnityEngine;

namespace Modules.ST.EntityTemplates
{
    public class ExplodableTemplate : ViewElement
    {
        public ParticleSystem ExplosionFx;
        public Collider Collider;
        public Renderer[] View;
        public Animator CanvasAnimator;
        
        public override void OnSpawn(EcsEntity entity, EcsWorld world)
        {
            base.OnSpawn(entity, world);
            ref var view = ref entity.Get<ExplodableView>();
            view.Transform = transform;
            view.ExplosionFx = ExplosionFx;
            view.Collider = Collider;
            view.View = View;
            view.CanvasAnimator = CanvasAnimator;
        }
    }

    public struct ExplodableView
    {
        public Transform Transform;
        public ParticleSystem ExplosionFx;
        public Collider Collider;
        public Animator CanvasAnimator;
        public Renderer[] View;
    }
}