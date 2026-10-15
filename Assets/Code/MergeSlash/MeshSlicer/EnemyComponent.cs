using System;
using Leopotam.Ecs;
using Modules.ViewHub;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.ST.EntityTemplates
{
    public class EnemyComponent : ViewComponent
    {
        [SerializeField] private ParticleSystem _bloodParticle;
        [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;
        [SerializeField] private GameObject _character;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Animator _animator;
        [SerializeField] private EnemyViewSwitcher _switcher;
        [SerializeField] private EnemyType _lastType;
        [SerializeField] private Rigidbody _weaponRigidbody;
        [SerializeField] private Collider _weaponCollider;
        [SerializeField] private Mesh[] _emotesMeshes;
        [SerializeField] private Material[] _emotesMaterials;
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private MeshCollider _meshCollider;
        
        [Space, Header("For GD")]
        [SerializeField] private bool _walking;
        [SerializeField] private float _walkingDistance;
        [SerializeField] private float _walkingSpeed;
        
        
        private Camera _mainCamera;

        private void Awake()
        {
            SwitchView();
        }

        public override void EntityInit(EcsEntity ecsEntity, EcsWorld ecsWorld, bool parentOnScene)
        {
            if(!_meshFilter)
                _meshFilter = GetComponent<MeshFilter>();
            if (!_meshRenderer)
                _meshRenderer = GetComponent<MeshRenderer>();
            if (!_meshCollider)
                _meshCollider = GetComponent<MeshCollider>();

            ref var enemy = ref ecsEntity.Get<EnemyView>();
            enemy.View = this;
            enemy.Animator = _animator;
            enemy.BloodParticle = _bloodParticle;
            enemy.WeaponRigidbody = _weaponRigidbody;
            enemy.WeaponCollider = _weaponCollider;
            enemy.EmotesMeshes = _emotesMeshes;
            enemy.SkinnedMeshRenderer = _skinnedMeshRenderer;
            enemy.EmotesMaterials = _emotesMaterials;
            enemy.Walking = _walking;
            enemy.WalkingDistance = _walkingDistance;
            enemy.WalkingSpeed = _walkingSpeed;
            _mainCamera = Camera.main;
        }

        public void Bake()
        {
            var mesh = new Mesh();
            _skinnedMeshRenderer.BakeMesh(mesh);
            _meshFilter.sharedMesh = mesh;
            
            _meshCollider.sharedMesh = mesh;
            _meshCollider.enabled = true;
            _meshCollider.convex = true;
            
            _meshRenderer.materials = _skinnedMeshRenderer.materials;
            _meshRenderer.enabled = true;
            _skinnedMeshRenderer.enabled = false;
            
            gameObject.transform.rotation = _character.transform.rotation;
        }

        private void Update()
        {
            Vector3 canvasDir = transform.position - _mainCamera.transform.position;
            canvasDir.y = 0;
            _canvas.transform.forward = canvasDir.normalized;
        }

        private void OnDrawGizmos()
        {
            if (_switcher != null && _lastType != _switcher.EnemyType)
            {
                SwitchView();
            }
        }

        private void SwitchView()
        {
            if(_switcher == null)
                return;

            _lastType = _switcher.EnemyType;

            int type = (int) _switcher.EnemyType;

            EnemyViewData data = _switcher.Datas[type];
            _skinnedMeshRenderer = data._skinnedMeshRenderer;
            _character = data._character;
            _animator = data._animator;
            _weaponCollider = data.WeaponCollider;
            _weaponRigidbody = data.WeaponRigidbody;
            _emotesMeshes = data.EmotesMeshes;
            _emotesMaterials = data.EmotesMaterials;
            
            for (int i = 0; i < _switcher.Datas.Length; i++)
            {
                _switcher.Datas[i].MainGO.SetActive(i == type);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if(!_walking)
                return;

            Vector3 start = transform.position;
            Vector3 end = transform.position + transform.forward * -_walkingDistance;
            
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(start, end);
            Gizmos.DrawSphere(end, 0.5f);
        }
    }
    
    public struct EnemyView
    {
        public EnemyComponent View;
        public ParticleSystem BloodParticle;
        public Animator Animator;
        public Rigidbody WeaponRigidbody;
        public Collider WeaponCollider;
        public Mesh[] EmotesMeshes;
        public Material[] EmotesMaterials;
        public SkinnedMeshRenderer SkinnedMeshRenderer;
        public bool Walking;
        public float WalkingDistance;
        public float WalkingSpeed;
    }
}