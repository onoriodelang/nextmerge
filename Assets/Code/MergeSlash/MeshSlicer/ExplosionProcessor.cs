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
    public class ExplosionProcessor : IEcsRunSystem
    {
        private EcsFilter<ExplodableView, Explode> _filter;

        private EcsWorld _world;

        public void Run()
        {
            foreach (var i in _filter)
            {
                ref var explodableView = ref _filter.Get1(i);
                explodableView.Collider.enabled = false;
                explodableView.ExplosionFx.Play();

                foreach (var view in explodableView.View)
                {
                    view.enabled = false;
                }
                
                if(explodableView.CanvasAnimator)
                    explodableView.CanvasAnimator.SetTrigger("Dead");
                _filter.GetEntity(i).Del<ExplodableView>();
                _filter.GetEntity(i).Del<Explode>();
            }
        }
    }
}