using System;
using Leopotam.Ecs;
using UnityEngine;
using TMPro;
using UICoreECS;
using UnityEngine.UI;

namespace Modules.MergeSlash.UI
{
    public class ProgressBar : AUIEntity
    {
        public LevelLabel[] LevelLabels;
        public Slider Slider;

        private EcsWorld _world;

        public override void Init(EcsWorld world, EcsEntity screen)
        {
            _world = world;
            ref var view = ref screen.Get<ProgressBarView>();
            view.View = this;
            view.LastLevel = -1;
        }

        public void SetData(int numberOfCurrentLevel, int[] levelNumbers)
        {
            
            for (int i = 0; i < LevelLabels.Length; i++)
            {
                if(levelNumbers[i] <= 0)
                    LevelLabels[i].SetBoss();
                else
                {
                    LevelLabels[i].SetRegular(levelNumbers[i]);
                }
                
                if (i < numberOfCurrentLevel)
                    LevelLabels[i].SetPassed();
                else if (i == numberOfCurrentLevel)
                    LevelLabels[i].SetCurrent();
                else
                    LevelLabels[i].SetFuture();
            }

            Slider.value = numberOfCurrentLevel * (1f / (LevelLabels.Length - 1));
        }
    }

    public struct ProgressBarView
    {
        public ProgressBar View;
        public int LastLevel;
    }
}