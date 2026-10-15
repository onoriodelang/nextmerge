using System;
using Leopotam.Ecs;
using UnityEngine;
using TMPro;
using UICoreECS;
using UnityEngine.UI;

namespace Modules.MergeSlash.UI
{
    public class TutorialPanel : AUIEntity
    {
        public TutorialPointer TutorialPointer;
        public TextMeshProUGUI TutorialText;
        public Vector3 TargetsOffset;
        public CanvasGroup CanvasGroup;
        public float FadeInTime;
        public float FadeOutTime;
        
        public string TutorialMergeText;
        public string TutorialReplaceText;

        private EcsWorld _world;
        private float _canvasGroupAlphaTarget = 1;

        public override void Init(EcsWorld world, EcsEntity screen)
        {
            _world = world;
            screen.Get<TutorialPanelView>().View = this;
        }

        private void Update()
        {
            if (CanvasGroup.alpha < _canvasGroupAlphaTarget)
            {
                float diff = Time.unscaledDeltaTime * (1 / FadeInTime);

                if (diff > _canvasGroupAlphaTarget - CanvasGroup.alpha)
                {
                    CanvasGroup.alpha = _canvasGroupAlphaTarget;
                }
                else
                {
                    CanvasGroup.alpha += diff;
                }
            }
            else if(CanvasGroup.alpha > _canvasGroupAlphaTarget)
            {
                float diff = Time.unscaledDeltaTime * (1 / FadeOutTime);

                if (diff > CanvasGroup.alpha - _canvasGroupAlphaTarget)
                {
                    CanvasGroup.alpha = _canvasGroupAlphaTarget;
                }
                else
                {
                    CanvasGroup.alpha -= diff;
                }
            }
        }

        public void EnableMergeTutorial()
        {
            TutorialText.text = TutorialMergeText;
            TutorialPointer.gameObject.SetActive(true);
        }
        
        public void EnableReplaceTutorial()
        {
            TutorialText.text = TutorialReplaceText;
            TutorialPointer.gameObject.SetActive(true);
        }

        public void DisableTutorial()
        {
            TutorialPointer.gameObject.SetActive(false);
        }

        public void SetTutorialTargets(Vector3 target1, Vector3 target2)
        {
            TutorialPointer.SetTargets(target1 + TargetsOffset, target2 + TargetsOffset);
        }

        public void PauseTutorial()
        {
            _canvasGroupAlphaTarget = 0;
        }
        
        public void ResumeTutorial()
        {
            _canvasGroupAlphaTarget = 1;
        }
    }

    public struct TutorialPanelView
    {
        public TutorialPanel View;
        public bool Inited;
    }
}