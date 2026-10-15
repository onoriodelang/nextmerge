using Leopotam.Ecs;
using Modules.CameraUtils;
using Modules.EventGroup;
using Modules.MergeMechanic.Components;
using Modules.MergeSlash.Components;
using Modules.MergeSlash.EntityTemplates;
using Modules.MergeSlash.UI;
using Modules.ST;
using Modules.ST.EntityTemplates;
using Modules.ST.MeshSlicer;
using Modules.Utils;
using Modules.ViewHub;
using UnityEngine;

namespace Modules.MergeSlash.Interactors
{
    public class WeaponPreparationSystem : IEcsRunSystem
    {
        private EcsFilter<PlaceInHands, UnityView, MergeUnit> _unitForPlace;
        private EcsFilter<InHandTag, UnityView> _weaponsInHands;
        private EcsFilter<SawView, ThrowTag> _sawForThrow;
        private EcsFilter<SawView, SawThrownTag> _sawThrown;
        private EcsFilter<GameplayButtonsPanelView> _panelView;
        private EcsFilter<CameraFocus, UnityView> _cameraFocus;
        private EcsFilter<OnTile>.Exclude<ThrowTag, SawThrownTag> _sawView;
        private EcsFilter<TargetWithHealth, Enemy> _enemies;
        private EcsFilter<Player> _player;

        private EcsWorld _world;
        private TimeService _timeService;
        private float _secondWeaponTimer;
        private PrototypeConfig _prototypeConfig;
        private float _buttonsTime = 0;

        private static readonly int BlendKeyHash = Animator.StringToHash("Blend");

        public WeaponPreparationSystem(PrototypeConfig prototypeConfig)
        {
            _prototypeConfig = prototypeConfig;
        }
        
        public void Run()
        {
            PlaceFirstWeaponInHands();
            PlaceSecondWeapon();
        }

        private void PlaceFirstWeaponInHands()
        {
            if(_unitForPlace.IsEmpty() || _player.IsEmpty() || !_weaponsInHands.IsEmpty() || _enemies.IsEmpty())
                return;

            ref Player player = ref _player.Get1(0);

            _world.NewEntity().Get<OpenDoorSignal>();
            _world.NewEntity().Get<EnemyWalkSignal>();
            
            _world.NewEntity().Get<CameraUtils.SwitchCameraSignal>() = new CameraUtils.SwitchCameraSignal
            {
                CameraId = 2,
                Follow = player.CameraFocus,
                LookAt = player.CameraFocus
            };

            foreach (var i in _panelView)
            {
                _panelView.Get1(i).View.DisableAllButtons();
                _panelView.Get1(i).View.EnableThrowButtons(_prototypeConfig.ThrowButtonsEnable);
            }

            _buttonsTime = 0;

            float blend = 0f;

            foreach (var i in _unitForPlace)
            {
                if(_unitForPlace.Get1(i).Number > 0)
                    continue;

                int type = _unitForPlace.Get3(i).TypeID;
                
                _unitForPlace.Get2(i).Transform.parent = player.WeaponPlaceholders[type];
                _unitForPlace.Get2(i).Transform.localPosition = Vector3.zero;
                _unitForPlace.Get2(i).Transform.localRotation = Quaternion.identity;
                
                EntityRef[] sawRefs = _unitForPlace.Get2(i).Transform.GetComponentsInChildren<EntityRef>();

                foreach (var sawRef in sawRefs)
                {
                    if (sawRef.Entity.Has<SawView>())
                    {
                        sawRef.Entity.Get<ThrowTag>();

                        blend = sawRef.Entity.Get<SawView>().AnimationBlend;
                        
                        player.HandsAnimator.SetFloat(BlendKeyHash, blend);
                    }
                }

                _unitForPlace.GetEntity(i).Get<InHandTag>().Number = _unitForPlace.Get1(i).Number;
                _unitForPlace.GetEntity(i).Del<PlaceInHands>();

                _unitForPlace.Get3(i).Collider.enabled = false;
            }
            
            foreach (var i in _cameraFocus)
            {
                _cameraFocus.GetEntity(i).Get<CameraFocusMoveTag>();
                _cameraFocus.Get1(i).StartPos = _cameraFocus.Get2(i).Transform.position;
                _cameraFocus.Get1(i).EndPos = player.CameraPoint.position;

                ref var switchPlayer = ref _player.GetEntity(0).Get<SwitchPlayerView>();
                switchPlayer.Time = 0.75f * _cameraFocus.Get1(i).MoveTime;
                switchPlayer.SwitchToHandsView = true;
                switchPlayer.AnimationBlend = blend;
            }
        }

        private void PlaceSecondWeapon()
        {
            if (_weaponsInHands.IsEmpty())
                return;
            if (!_sawForThrow.IsEmpty())
                return;
            if (!_sawThrown.IsEmpty())
                return;
            
            _buttonsTime = _prototypeConfig.ThrowButtonsEnable;

            foreach (var idx in _unitForPlace)
            {
                _unitForPlace.Get1(idx).Number -= 1;
            }

            foreach (var i in _player)
            {
                foreach (var renderer in _player.Get1(i).HandsRenderers)
                {
                    renderer.enabled = true;
                }
            }
            
        }


    }

    
}