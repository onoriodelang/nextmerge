using System;
using Leopotam.Ecs;
using Modules.ViewHub;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.ST.EntityTemplates
{
    public class BallLauncherComponent : ViewComponent
    {
        public ParticleSystem ShootFx;
        public ParticleSystem MissileFx;
        public Transform Missile;
        public Transform LaunchedMissileParent;
        public Vector3 StartPosition;
        public float Speed;
        public float Delay;


        public override void EntityInit(EcsEntity ecsEntity, EcsWorld ecsWorld, bool parentOnScene)
        {
            ref var component = ref ecsEntity.Get<BallLauncher>();
            component.ShootFx = ShootFx;
            component.MissileFx = MissileFx;
            component.Missile = Missile;
            component.LaunchedMissileParent = LaunchedMissileParent;
            component.Speed = Speed;
            component.StartPosition = StartPosition;
            component.Delay = Delay;
        }

    }
    
    public struct BallLauncher
    {
        public ParticleSystem ShootFx;
        public ParticleSystem MissileFx;
        public Transform Missile;
        public Transform LaunchedMissileParent;
        public float Speed;
        public float TargetDistance;
        public Vector3 FlyDirection;
        public Vector3 StartPosition;
        public float Delay;
        public bool Inited;
    }
    
    public struct BallLaunchedTag
    {
        
    }
   
}