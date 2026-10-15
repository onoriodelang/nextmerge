using Leopotam.Ecs;
using Modules.ViewHub;
using UnityEngine;

namespace Modules.MergeSlash.EntityTemplates
{
    public class SawTemplate : ViewElement
    {
        public int Health;
        public float GoldForHealthRatio = 1f;
        
        [Header("Forward move and rotate settings")]
        [SerializeField] private float _speed;
        [SerializeField] private float _rotationSpeed;
        
        [Header("Horizontal move and rotate settings")]
        [SerializeField] private float _horizontalSpeed;
        [SerializeField] private float _horizontalChangeTolerance;
        [SerializeField] private float _horizontalMaxChange;
        [SerializeField] private float _horizontalRotationAngle;

        [Header("Rotated object")] 
        [SerializeField] private GameObject _sawVisiblePart;
        [SerializeField] private Collider _sawCollider;

        [Header("Fx setting")] 
        [SerializeField] private ParticleSystem[] _playOnStop;
        [SerializeField] private ParticleSystem[] _playOnStopPillar;
        [SerializeField] private ParticleSystem[] _playOnStart;
        [SerializeField] private ParticleSystem[] _playOnCollision;
        
        [Header("Other")] 
        [SerializeField] private float _animationBlend;


        public override void OnSpawn(EcsEntity entity, EcsWorld world)
        {
            base.OnSpawn(entity, world);
            entity.Get<LevelSpawner.LevelEntityTag>();

            ref var view = ref entity.Get<SawView>();
            view.Speed = _speed / 2;
            view.Durability = Health;
            view.GoldForHealthRatio = GoldForHealthRatio;
            view.Collider = _sawCollider;
            view.RotationSpeed = _rotationSpeed;
            
            view.HorizontalSpeed = _horizontalSpeed;
            view.HorizontalChangeTolerance = _horizontalChangeTolerance;
            view.HorizontalMaxChange = _horizontalMaxChange;
            view.HorizontalRotationAngle = _horizontalRotationAngle;

            view.SawVisible = _sawVisiblePart;
            // view.SawVisible.SetActive(false);
            view.Transform = transform;

            view.ParticlesOnStop = _playOnStop;
            view.ParticlesOnStopPillar = _playOnStopPillar;
            view.ParticlesOnStart = _playOnStart;
            view.ParticlesOnCollision = _playOnCollision;
            view.AnimationBlend = _animationBlend;
        }
    }
    
    public struct SawView
    {
        public int Durability;
        public float GoldForHealthRatio;
        public Collider Collider;
        
        public float Speed;
        public float RotationSpeed;
        
        public float HorizontalSpeed;
        public float HorizontalChangeTolerance;
        public float HorizontalMaxChange;
        public float HorizontalRotationAngle;

        public GameObject SawVisible;
        public Transform Transform;

        public ParticleSystem[] ParticlesOnStop;
        public ParticleSystem[] ParticlesOnStopPillar;
        public ParticleSystem[] ParticlesOnStart;
        public ParticleSystem[] ParticlesOnCollision;

        public float AnimationBlend;
    }

    [System.Serializable]
    public class ComboEffect
    {
        public int Combo;
        public ParticleSystem[] Particles;
        public float AddToSawScale;
        [HideInInspector] public bool Played;
    }
}