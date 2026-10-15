using System;
using Leopotam.Ecs;
using UnityEngine;
using TMPro;
using UICoreECS;
using UnityEngine.UI;

namespace Modules.MergeSlash.UI
{
    public class TutorialPointer : MonoBehaviour
    {
        public float Speed;
        public float MinMoveTime;
        public float MaxMoveTime;
        public float FadeInTime;
        public float FadeOutTime;
        public Image FingerImage;
        public Transform Finger;
        public Transform Text;
        public Vector3 TextOffset;
        public Vector3 FingerOffset;
        public float FingerPressedScale;
        
        private Vector3 _target1;
        private Vector3 _target2;
        private Camera _mainCamera;
        private float _timer;
        private float _iterationTime;
        private float _moveStateTime;
        private Vector3 _fingerOffset;

        private void Start()
        {
            _fingerOffset = FingerOffset * Screen.width;
        }

        private void Update()
        {
            _timer -= Time.unscaledDeltaTime;

            if (_timer < 0)
            {
                _timer = _iterationTime;
            }

            if (_iterationTime - _timer < FadeInTime)
            {
                // fade in state
                Finger.transform.position = _target1 + _fingerOffset;
                float t = (_iterationTime - _timer) / FadeInTime;
                SetFingerPressed(t);
            }
            else if (_timer <= FadeOutTime)
            {
                // fade out state
                Finger.transform.position = _target2 + _fingerOffset;
                float t = _timer / FadeOutTime;
                SetFingerPressed(t);
                
            }
            else
            {
                // move state
                
               float t = 1 - (_timer - FadeOutTime) / _moveStateTime;

                Finger.transform.position = Vector3.Lerp(_target1, _target2, t) +  _fingerOffset;
                SetFingerPressed(1);
            }
        }

        private void SetFingerPressed(float t)
        {
            Color color = FingerImage.color;
            color.a = t;
            FingerImage.color = color;

            float scale = FingerPressedScale + (1 - t) * (1 - FingerPressedScale);
            FingerImage.transform.localScale = Vector3.one * scale;
        }
        
        

        public void SetTargets(Vector3 target1, Vector3 target2)
        {
            if (!_mainCamera)
            {
                _mainCamera = Camera.main;
            }
            
            _target1 = _mainCamera.WorldToScreenPoint(target1);
            _target2 = _mainCamera.WorldToScreenPoint(target2);

            float distance = Vector3.Distance(_target1, _target2) / Screen.width * 100f;

            _moveStateTime =  Mathf.Clamp(distance / Speed, MinMoveTime, MaxMoveTime);
            _iterationTime = _moveStateTime + FadeInTime + FadeOutTime;
            _timer = _iterationTime;

            float xPos = (_target1.x + _target2.x) / 2;
            float yPos = _target1.y < _target2.y ? _target1.y : _target2.y;
            Vector3 textPos = Text.transform.position;
            textPos.x = xPos;
            textPos.y = yPos;
            Text.transform.position = textPos + TextOffset * Screen.height;
        }
    }
}