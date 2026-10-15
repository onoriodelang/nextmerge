using System.Collections.Generic;
using EzySlice;
using Leopotam.Ecs;
using Modules.MergeSlash.UI;
using Modules.ST.EntityTemplates;
using Modules.ST.MeshSlicer.Components;
using Modules.Utils;
using Modules.ViewHub;
using UnityEngine;

namespace Modules.ST.MeshSlicer
{
    public class EnemyShootSystem : IEcsRunSystem
    {
        private EcsFilter<TargetWithHealth, EnemyView, UnityView> _filter;
        private EcsFilter<Pistol, PistolShotTag> _pistol;
        private EcsFilter<RocketLauncher, MissileLaunchedTag> _launcher;
        private EcsFilter<BallLauncher, BallLaunchedTag>.Exclude<WaitTimer> _ballLauncher;
        private EcsFilter<EnemyShootTag> _signal;
        private EcsFilter<Player, UnityView> _player;
        private EcsFilter<BloodScreenView> _bloodScreens;

        private EcsWorld _world;
        private TimeService _timeService;
        
        private static readonly int AttackKeyHash = Animator.StringToHash("Attack");

        public void Run()
        {
            LaunchedRun();
            
            if(_signal.IsEmpty() || _player.IsEmpty())
                return;

            Vector3 playerPos = _player.Get2(0).Transform.position;
            playerPos.y = 0;

            foreach (var i in _filter)
            {
                _filter.Get2(i).Animator.SetTrigger(AttackKeyHash);
                
                Vector3 enemyPos = _filter.Get3(i).Transform.position;
                enemyPos.y = 0;
                Vector3 dirToPlayer = (playerPos - enemyPos).normalized;

                _filter.Get3(i).Transform.forward = -dirToPlayer;
                
                if (_filter.GetEntity(i).Has<Pistol>() && _filter.GetEntity(i).Get<Pistol>().Missile.gameObject.activeSelf)
                {
                    ref var pistol = ref _filter.GetEntity(i).Get<Pistol>();
                    if(pistol.ShootFx)
                        pistol.ShootFx.Play();
                    pistol.Missile.gameObject.SetActive(true);
                    if(pistol.MissileFx)
                        pistol.MissileFx.gameObject.SetActive(true);
                    pistol.Missile.parent = pistol.LaunchedMissileParent;
                    pistol.Missile.localPosition = pistol.StartPosition;
                    pistol.TargetDistance = Vector3.Distance(playerPos, enemyPos) - 1;
                    pistol.FlyDirection = dirToPlayer;
                    _filter.GetEntity(i).Get<PistolShotTag>();
                }
                
                if (_filter.GetEntity(i).Has<RocketLauncher>() && _filter.GetEntity(i).Get<RocketLauncher>().Missile.gameObject.activeSelf)
                {
                    ref var launcher = ref _filter.GetEntity(i).Get<RocketLauncher>();
                    if(launcher.ShootFx)
                        launcher.ShootFx.Play();
                    launcher.Missile.gameObject.SetActive(true);
                    if(launcher.MissileFx)
                        launcher.MissileFx.gameObject.SetActive(true);
                    launcher.Missile.parent = launcher.LaunchedMissileParent;
                    launcher.Missile.localPosition = launcher.StartPosition;
                    launcher.Missile.right = dirToPlayer;
                    launcher.TargetDistance = Vector3.Distance(playerPos, enemyPos) - 1;
                    launcher.FlyDirection = dirToPlayer;
                    _filter.GetEntity(i).Get<MissileLaunchedTag>();
                }
                
                if (_filter.GetEntity(i).Has<BallLauncher>() && _filter.GetEntity(i).Get<BallLauncher>().Missile.gameObject.activeSelf)
                {
                    _filter.GetEntity(i).Get<BallLaunchedTag>();
                    _filter.GetEntity(i).Get<WaitTimer>().RemainingTime = _filter.GetEntity(i).Get<BallLauncher>().Delay;
                }
            }
        }

        private void LaunchedRun()
        {
            bool playerHitted = false;
            
            foreach (var i in _launcher)
            {
                ref var launcher = ref _launcher.GetEntity(i).Get<RocketLauncher>();

                float dist = launcher.Speed * _timeService.DeltaTime * 30;
                launcher.Missile.transform.right = launcher.FlyDirection;

                if (dist < launcher.TargetDistance)
                {
                    launcher.Missile.transform.position += launcher.FlyDirection * dist;
                    launcher.TargetDistance -= dist;
                }
                else
                {
                    launcher.Missile.transform.position += launcher.FlyDirection * launcher.TargetDistance;
                    
                    launcher.Missile.gameObject.SetActive(false);
                    
                    _launcher.GetEntity(i).Del<MissileLaunchedTag>();

                    playerHitted = true;
                }
            }
            
            foreach (var i in _pistol)
            {
                ref var pistol = ref _pistol.GetEntity(i).Get<Pistol>();

                float dist = pistol.Speed * _timeService.DeltaTime * 30;
                pistol.Missile.transform.right = pistol.FlyDirection;

                if (dist < pistol.TargetDistance)
                {
                    pistol.Missile.transform.position += pistol.FlyDirection * dist;
                    pistol.TargetDistance -= dist;
                }
                else
                {
                    pistol.Missile.transform.position += pistol.FlyDirection * pistol.TargetDistance;
                    
                    pistol.Missile.gameObject.SetActive(false);
                    
                    _pistol.GetEntity(i).Del<PistolShotTag>();
                    
                    playerHitted = true;
                }
            }
            
            foreach (var i in _ballLauncher)
            {
                ref var launcher = ref _ballLauncher.GetEntity(i).Get<BallLauncher>();

                if (!launcher.Inited)
                {
                    Vector3 playerPos = _player.Get2(0).Transform.position;
                    Vector3 enemyPos = _filter.Get3(i).Transform.position;
                    enemyPos.y = 0;
                    Vector3 dirToPlayer = (playerPos - enemyPos).normalized;
                    
                    if(launcher.ShootFx)
                        launcher.ShootFx.Play();
                    launcher.Missile.gameObject.SetActive(true);
                    if(launcher.MissileFx)
                        launcher.MissileFx.gameObject.SetActive(true);
                    launcher.Missile.parent = launcher.LaunchedMissileParent;
                    launcher.Missile.localPosition = launcher.StartPosition;
                    launcher.TargetDistance = Vector3.Distance(playerPos, enemyPos) - 1;
                    launcher.FlyDirection = dirToPlayer;
                    launcher.Inited = true;
                }

                float dist = launcher.Speed * _timeService.DeltaTime * 30;
                launcher.Missile.transform.up = launcher.FlyDirection;

                if (dist < launcher.TargetDistance)
                {
                    launcher.Missile.transform.position += launcher.FlyDirection * dist;
                    launcher.TargetDistance -= dist;
                }
                else
                {
                    launcher.Missile.transform.position += launcher.FlyDirection * launcher.TargetDistance;
                    
                    launcher.Missile.gameObject.SetActive(false);
                    
                    _ballLauncher.GetEntity(i).Del<BallLaunchedTag>();
                    
                    playerHitted = true;
                }
            }
            
            if(playerHitted)
                PlayerHit();
        }

        private void PlayerHit()
        {
            foreach (var i in _bloodScreens)
            {
                _bloodScreens.Get1(i).View.ShowBlood();
            }
        }
    }

    public struct EnemyShootTag : IEcsIgnoreInFilter
    {
        
    }
}