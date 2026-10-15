using Leopotam.Ecs;
using Modules.MergeSlash.Components;
using Modules.MergeSlash.EntityTemplates;
using Modules.MergeSlash.UI;
using Modules.MergeSlash.UI.UnityComponents;
using Modules.ST.EntityTemplates;
using Modules.Utils;
using Modules.ViewHub;
using UnityEngine;

namespace Modules.MergeSlash.Interactors
{
    public class SawThrower: IEcsRunSystem
    {
        private EcsFilter<SawView, ThrowTag>.Exclude<SawThrownTag, WaitTimer> _readyForAnim;
        private EcsFilter<SawView, ThrowTag, SawThrowAnimation>.Exclude<SawThrownTag, WaitTimer> _readyForThrow;
        private EcsFilter<Player>.Exclude<SawThrownTag> _player;
        private EcsFilter<ThrowButtonSignal> _signal;
        private EcsFilter<TargetWithHealth, Enemy> _enemies;
        private EcsFilter<GameplayButtonsPanelView> _panelView;

        private EcsWorld _world;
        private static readonly int ThrowKeyHash = Animator.StringToHash("throw");
        private static readonly int BlendKeyHash = Animator.StringToHash("Blend");

        public void Run()
        {
            if (!_readyForAnim.IsEmpty() && !_signal.IsEmpty() && !_enemies.IsEmpty())
            {
                AnimationStart(_readyForAnim.Get1(0).AnimationBlend);
            }

            if (!_readyForThrow.IsEmpty())
            {
                ThrowStart();
            }
        }
        
        private void AnimationStart(float animationBlend)
        {
            foreach (var i in _player)
            {
                _player.Get1(i).HandsAnimator.SetBool(ThrowKeyHash, true);
                // _player.Get1(i).HandsAnimator.SetFloat(BlendKeyHash, animationBlend);
                foreach (var viewIdx in _panelView)
                {
                    _panelView.Get1(viewIdx).View.DisableAllButtons();
                }
            }

            foreach (var sawIdx in _readyForAnim)
            {
                _readyForAnim.GetEntity(sawIdx).Get<WaitTimer>().RemainingTime = 0.5f;
                _readyForAnim.GetEntity(sawIdx).Get<SawThrowAnimation>();
            }

            // _world.NewEntity().Get<EnemyScarySignal>();
        }

        private void ThrowStart()
        {
            foreach (var sawView in _readyForThrow)
            {
                if (!_readyForThrow.Get1(sawView).SawVisible)
                {
                    _readyForThrow.GetEntity(sawView).Del<SawView>();
                    _readyForThrow.GetEntity(sawView).Del<ThrowTag>();
                    
                    continue;
                }
                _readyForThrow.Get1(sawView).SawVisible.SetActive(true);
                _readyForThrow.GetEntity(sawView).Get<SawLink>().MergeUnitEntity = _readyForThrow.Get1(sawView).Transform.parent
                    .parent.parent.GetComponent<EntityRef>();
                _readyForThrow.Get1(sawView).Transform.parent = null;
                _readyForThrow.Get1(sawView).Transform.rotation = Quaternion.identity;
                _readyForThrow.GetEntity(sawView).Get<SawThrownTag>();
                _readyForThrow.GetEntity(sawView).Del<SawThrowAnimation>();
                
                ref var delayedScale = ref _readyForThrow.GetEntity(sawView).Get<DelayedScale>();
                delayedScale.ScaleTime = 1f;
                delayedScale.EndScale = new Vector3(1.25f, 1.25f, 1.25f);
                
                _world.NewEntity().Get<CameraUtils.SwitchCameraSignal>() = new CameraUtils.SwitchCameraSignal
                {
                    CameraId = 1,
                    Follow = _readyForThrow.Get1(sawView).Transform,
                    LookAt = null
                };

                if (_readyForThrow.Get1(sawView).ParticlesOnStart != null)
                {
                    foreach (var fx in _readyForThrow.Get1(sawView).ParticlesOnStart)
                    {
                        fx.Play();
                    }
                }
            }
            
            foreach (var i in _player)
            {
                _player.Get1(i).HandsAnimator.SetBool(ThrowKeyHash, false);

                foreach (var renderer in _player.Get1(i).HandsRenderers)
                {
                    renderer.enabled = false;
                }
            }
            
            if (VibroService.VibroSettings.VibroEnabled)
            {
                MoreMountains.NiceVibrations.MMVibrationManager.Haptic(MoreMountains.NiceVibrations.HapticTypes.MediumImpact);
            }
        }
    }

    public struct SawLink
    {
        public EntityRef MergeUnitEntity;
    }
    
    public struct SawThrowAnimation : IEcsIgnoreInFilter
    {
        
    }
    
}