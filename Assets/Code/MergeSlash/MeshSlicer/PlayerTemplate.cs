using Leopotam.Ecs;
using Modules.ViewHub;
using UnityEngine;

namespace Modules.ST.EntityTemplates
{
    public class PlayerTemplate : ViewElement
    {
        public Animator PersonAnimator;
        public Animator HandsAnimator;
        public Transform[] WeaponPlaceholders;
        public GameObject PersonGO;
        public GameObject HandsGO;
        public Renderer[] HandsRenderers;
        public Transform CameraPoint;
        public Transform CameraFocus;
        
        public override void OnSpawn(EcsEntity entity, EcsWorld world)
        {
            base.OnSpawn(entity, world);
            ref var view = ref entity.Get<Player>();
           
            view.PersonAnimator = PersonAnimator;
            view.HandsAnimator = HandsAnimator;
            view.WeaponPlaceholders = WeaponPlaceholders;
            view.PersonGO = PersonGO;
            view.HandsGO = HandsGO;
            view.HandsRenderers = HandsRenderers;
            view.CameraPoint = CameraPoint;
            view.CameraFocus = CameraFocus;
            
            foreach (var renderer in HandsRenderers)
            {
                renderer.enabled = false;
            }
        }
    }

    public struct Player
    {
        public Animator PersonAnimator;
        public Animator HandsAnimator;
        public Transform[] WeaponPlaceholders;
        public GameObject PersonGO;
        public GameObject HandsGO;
        public Renderer[] HandsRenderers;
        public Transform CameraPoint;
        public Transform CameraFocus;
    }
    
}