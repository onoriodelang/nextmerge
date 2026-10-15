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
    public class SliceProcessor : IEcsRunSystem
    {
        private EcsFilter<Slice>.Exclude<SlicedTag, LevelSpawner.CleanEntityTag> _filter;
        private EcsFilter<EventGroup.GamePlayState> _gameplay;

        private EcsWorld _world;

        public void Run()
        {
            if (_filter.IsEmpty() || _gameplay.IsEmpty())
                return;

            foreach (var idx in _filter)
            {
                ref var slice = ref _filter.Get1(idx);
                ref var sliceableView = ref slice.ToSliceEntity.Get<SliceableView>();
                Material sliceMaterial = sliceableView.SliceMaterial;
                float partMass = sliceableView.MassOfParts;
                float forceMult = sliceableView.ForceMult;
                ParticleSystem deathFx = sliceableView.DeathFx;
                Material[] newMaterials = sliceableView.NewMaterials;
                
                if(sliceableView.DisableOnSlice != null)
                    foreach (var go in sliceableView.DisableOnSlice)
                    {
                        go.SetActive(false);
                    }
                
                if(deathFx)
                    deathFx.Play();
                
                if (slice.ToSliceEntity.Has<EnemyView>())
                {
                    ref EnemyView enemyView = ref slice.ToSliceEntity.Get<EnemyView>();
                        enemyView.View.Bake();

                        if (enemyView.WeaponRigidbody)
                        {
                            enemyView.WeaponRigidbody.transform.parent = slice.ToSlice.transform.parent;
                            enemyView.WeaponRigidbody.useGravity = true;
                            enemyView.WeaponCollider.enabled = true;
                        }
                }

                var slicedObjects = slice.ToSlice.gameObject.SliceInstantiate(slice.SlicerWorldPosition,
                    slice.SlicerWorldDirection, newMaterials, sliceMaterial);

                if (slicedObjects != null)
                {
                    if (slice.EnemyCanvasAnimator != null)
                    {
                        slice.EnemyCanvasAnimator.transform.parent = slice.EnemyCanvasAnimator.transform.parent.parent;
                        slice.EnemyCanvasAnimator.transform.rotation = Quaternion.identity;
                        slice.EnemyCanvasAnimator.SetTrigger("Dead");
                    }
                    
                    slice.ToSlice.gameObject.SetActive(false);

                    bool isUp = true;
                    foreach (var slicedObject in slicedObjects)
                    {
                        // init unity gameobject
                        slicedObject.transform.position = slice.ToSlice.transform.position;
                        slicedObject.transform.localScale = slice.ToSlice.transform.localScale;

                        var collider = slicedObject.AddComponent<MeshCollider>();
                        collider.convex = true;

                        var rigidbody = slicedObject.AddComponent<Rigidbody>();
                        rigidbody.mass = partMass;
                        rigidbody.AddForceAtPosition(
                            (slice.SlicerWorldDirection + (isUp ? Vector3.zero : Random.insideUnitSphere)) *
                            (10f * (isUp ? 1 : -1.5f)) * forceMult,
                            slice.SlicerWorldPosition, ForceMode.Impulse);
                        isUp = !isUp;

                        // init ecs entity
                        var entity = _world.NewEntity();
                        var template = slicedObject.AddComponent<SliceableTemplate>();
                        template.sliceMaterial = sliceMaterial;
                        template._components = new List<ViewComponent>();
                        template.Spawn(entity, _world);
                        entity.Get<WaitTimer>().RemainingTime = 0.2f; // it wouldn't slice for which time
                    }
                }

                _filter.GetEntity(idx).Get<SlicedTag>();
            }
        }
    }
}