using System;
using Leopotam.Ecs;
using UnityEngine;
using TMPro;
using UICoreECS;
using UnityEngine.UI;

namespace Modules.MergeSlash.UI
{
    public class BloodScreen : AUIEntity
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private string ShowKey;

        private int _showKeyHash;

        public override void Init(EcsWorld world, EcsEntity screen)
        {
            screen.Get<BloodScreenView>().View = this;
            _showKeyHash = Animator.StringToHash(ShowKey);
        }

        public void ShowBlood()
        {
            _animator.SetBool(_showKeyHash, true);
        }
        
        public void HideBlood()
        {
            _animator.SetBool(_showKeyHash, false);
        }
    }

    public struct BloodScreenView
    {
        public BloodScreen View;
    }
}