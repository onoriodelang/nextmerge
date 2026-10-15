using System;
using Leopotam.Ecs;
using Modules.ViewHub;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.ST.EntityTemplates
{
    public class EnemyViewSwitcher : MonoBehaviour
    {
        public EnemyType EnemyType;
        public EnemyViewData[] Datas;
    }

    public enum EnemyType
    {
        RocketEnemy = 0,
        PistolEnemy = 1
    }

    [Serializable]
    public class EnemyViewData
    {
        public EnemyType Type;
        public SkinnedMeshRenderer _skinnedMeshRenderer;
        public GameObject _character;
        public Animator _animator;
        public GameObject MainGO;
        public Rigidbody WeaponRigidbody;
        public Collider WeaponCollider;
        public Mesh[] EmotesMeshes;
        public Material[] EmotesMaterials;
    }
}