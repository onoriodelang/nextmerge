using Leopotam.Ecs;
using Modules.ViewHub;
using UnityEngine;

namespace Modules.ST.EntityTemplates
{
    public class SliceableTemplate : ViewElement
    {
        public Material sliceMaterial;
        public float massOfParts = 1f;
        public float forceMult = 1f;
        public ParticleSystem deathFx;
        public Material[] NewMaterials;
        public GameObject[] DisableOnSlice;
        
        public override void OnSpawn(EcsEntity entity, EcsWorld world)
        {
            base.OnSpawn(entity, world);
            entity.Get<LevelSpawner.LevelEntityTag>();
            ref var view = ref entity.Get<SliceableView>();
            view.Transform = transform;
            view.SliceMaterial = sliceMaterial;
            view.Rigidbody = GetComponent<Rigidbody>();
            view.MassOfParts = massOfParts;
            view.ForceMult = forceMult;
            view.DeathFx = deathFx;
            view.NewMaterials = NewMaterials;
            view.DisableOnSlice = DisableOnSlice;
        }
    }

    public struct SliceableView
    {
        public Transform Transform;
        public Material SliceMaterial;
        public Rigidbody Rigidbody;
        public float MassOfParts;
        public float ForceMult;
        public ParticleSystem DeathFx;
        public Material[] NewMaterials;
        public GameObject[] DisableOnSlice;
    }
}