using Leopotam.Ecs;
using Modules.MergeMechanic.Components;
using Modules.MergeSlash.Components;
using Modules.MergeSlash.EntityTemplates;
using Modules.MergeSlash.Interactors;
using Modules.ST.EntityTemplates;
using Modules.ST.MeshSlicer.Components;
using Modules.Utils;
using Modules.ViewHub;
using UnityEngine;

namespace Modules.MergeSlash.Trackers
{
    public class SawCollisionTracker : IEcsRunSystem
    {
        private EcsFilter<SawView, UPhysics.Triggered> _filter;

        private EcsWorld _world;

        public void Run()
        {
            if(_filter.IsEmpty())
                return;

            foreach (var idx in _filter)
            {
                ref var saw = ref _filter.Get1(idx);
                ref var collided = ref _filter.Get2(idx);

                bool inWall = collided.Collider.CompareTag("SawStop");
                bool inPillar = collided.Collider.CompareTag("SawStopPillar");
                
                if (inWall || inPillar)
                {
                    var sawEntity = _filter.GetEntity(idx);
                    
                    SawStop(saw, null, inWall, inPillar);

                    int gold = (int)(saw.Durability * sawEntity.Get<SawView>().GoldForHealthRatio);

                    _world.NewEntity().Get<AddGold>().Gold = gold;
                    
                    sawEntity.Get<WaitTimer>().RemainingTime = 0.5f;
                    var mergeUnitEntity = sawEntity.Get<SawLink>().MergeUnitEntity.Entity;
                    mergeUnitEntity.Get<DestroyTag>();
                    _world.NewEntity().Get<SaveTilesTag>();
                    sawEntity.Del<SawView>();
                    
                    var entity = _world.NewEntity();
                    entity.Get<WaitTimer>().RemainingTime = 0.5f;
                    // entity.Get<SawDestroyedTag>();
                }
                else if(collided.Other != EcsEntity.Null && collided.Other.Has<SliceableView>())
                {
                    if (collided.Other.Has<WaitTimer>()) // wait for slicing
                    {
                        return;
                    }
                    
                    // the sliceable object that needed to track
                    if (collided.Other.Has<EnemyView>())
                    {
                        ref var enemyHit = ref collided.Other.Get<EnemyHitSignal>();
                        enemyHit.EnemyObject = collided.Collider.gameObject;
                        enemyHit.SawView = saw;
                    }

                    bool sawDestroyed = false;
                    Animator enemyCanvasAnimator = null;

                    if (collided.Other.Has<TargetWithHealth>())
                    {
                        ref var target = ref collided.Other.Get<TargetWithHealth>();
                        if (saw.Durability > target.CurrentHealth)
                        {
                            saw.Durability -= target.CurrentHealth;
                            if(target.HealthSlider)
                                target.HealthSlider.gameObject.SetActive(false);
                            _world.NewEntity().Get<AddGold>().Gold = target.Reward;
                            enemyCanvasAnimator = target.EnemyCanvasAnimator;
                            collided.Other.Del<TargetWithHealth>();
                        }
                        else
                        {
                            target.CurrentHealth -= saw.Durability;
                            if(target.HealthSlider)
                                target.HealthSlider.value = (float)target.CurrentHealth / target.FullHealth;
                            saw.Durability = 0;
                            sawDestroyed = true;
                        }
                    }

                    if (sawDestroyed)
                    {
                        SawStop(saw, collided.Collider, false, false);
                        var sawEntity = _filter.GetEntity(idx);
                        sawEntity.Get<WaitTimer>().RemainingTime = 0.5f;


                        var mergeUnitEntity = sawEntity.Get<SawLink>().MergeUnitEntity.Entity;
                        mergeUnitEntity.Get<DestroyTag>();
                        _world.NewEntity().Get<SaveTilesTag>();
                        sawEntity.Del<SawView>();
                    }
                    else
                    {
                        // slice sliceable object
                        ref var slice = ref collided.Other.Get<Slice>();
                        slice.ToSlice = collided.Collider.gameObject;
                        slice.ToSliceEntity = collided.Other;
                        slice.SlicerWorldPosition = saw.Transform.position;
                        slice.SlicerWorldDirection = saw.Transform.up;
                        slice.EnemyCanvasAnimator = enemyCanvasAnimator;
                        
                        if (VibroService.VibroSettings.VibroEnabled)
                        {
                            MoreMountains.NiceVibrations.MMVibrationManager.Haptic(MoreMountains.NiceVibrations.HapticTypes.MediumImpact);
                        }

                        if (saw.ParticlesOnCollision != null)
                            foreach (var fx in saw.ParticlesOnCollision)
                            {
                                fx.Play();
                            }
                    }
                }
                else if (collided.Other != EcsEntity.Null && collided.Other.Has<ExplodableView>())
                {
                    if (collided.Other.Has<WaitTimer>()) // wait for explode
                    {
                        return;
                    }
                    
                    // the Explodable object that needed to track
                    if (collided.Other.Has<EnemyView>())
                    {
                        ref var enemyHit = ref collided.Other.Get<EnemyHitSignal>();
                        enemyHit.EnemyObject = collided.Collider.gameObject;
                        enemyHit.SawView = saw;
                    }

                    bool sawDestroyed = false;

                    if (collided.Other.Has<TargetWithHealth>())
                    {
                        ref var target = ref collided.Other.Get<TargetWithHealth>();
                        if (saw.Durability > target.CurrentHealth)
                        {
                            saw.Durability -= target.CurrentHealth;
                            if(target.HealthSlider)
                                target.HealthSlider.gameObject.SetActive(false);
                            _world.NewEntity().Get<AddGold>().Gold = target.Reward;
                            collided.Other.Del<TargetWithHealth>();
                        }
                        else
                        {
                            target.CurrentHealth -= saw.Durability;
                            if(target.HealthSlider)
                                target.HealthSlider.value = (float)target.CurrentHealth / target.FullHealth;
                            saw.Durability = 0;
                            sawDestroyed = true;
                        }
                    }

                    if (sawDestroyed)
                    {
                        SawStop(saw, collided.Collider, false, false);

                        var sawEntity = _filter.GetEntity(idx);
                        sawEntity.Get<WaitTimer>().RemainingTime = 0.5f;

                        var mergeUnitEntity = sawEntity.Get<SawLink>().MergeUnitEntity.Entity;
                        mergeUnitEntity.Get<DestroyTag>();
                        _world.NewEntity().Get<SaveTilesTag>();
                        sawEntity.Del<SawView>();
                    }
                    else
                    {
                        // explode Explodable object
                        ref var explode = ref collided.Other.Get<Explode>();
                        
                        if (VibroService.VibroSettings.VibroEnabled)
                        {
                            MoreMountains.NiceVibrations.MMVibrationManager.Haptic(MoreMountains.NiceVibrations.HapticTypes.MediumImpact);
                        }

                        if (saw.ParticlesOnCollision != null)
                            foreach (var fx in saw.ParticlesOnCollision)
                            {
                                fx.Play();
                            }
                    }
                }
            }
        }

        private void SawStop(SawView saw, Collider other, bool inWall, bool inPillar)
        {
            saw.Collider.enabled = false;
            if(other)
                saw.SawVisible.transform.parent = other.transform;

            if (inWall  && saw.ParticlesOnStop != null)
            {
                foreach (var fx in saw.ParticlesOnStop)
                {
                    fx.Play();
                }
            }
            
            if (inPillar && saw.ParticlesOnStopPillar != null)
            {
                foreach (var fx in saw.ParticlesOnStopPillar)
                {
                    fx.Play();
                }
            }

            if (saw.ParticlesOnStart != null)
                foreach (var fx in saw.ParticlesOnStart)
                {
                    fx.Stop();
                }
            
            if (VibroService.VibroSettings.VibroEnabled)
            {
                MoreMountains.NiceVibrations.MMVibrationManager.Haptic(MoreMountains.NiceVibrations.HapticTypes.SoftImpact);
            }
        }
    }
}