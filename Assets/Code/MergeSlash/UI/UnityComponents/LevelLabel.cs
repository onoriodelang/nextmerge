using System;
using Leopotam.Ecs;
using UnityEngine;
using TMPro;
using UICoreECS;
using UnityEngine.UI;

namespace Modules.MergeSlash.UI
{
    public class LevelLabel : MonoBehaviour
    {
        public GameObject RegularLevelLabel;
        public Image RegularLevelImage;
        public TextMeshProUGUI RegularLevelText;
        [Space]
        public GameObject BossLevelLabel;
        public Image BossLevelImage;
        public Image SkullImage;
        public TextMeshProUGUI BossLevelText;
        [Space]
        public Sprite PassedLevelsSprite;
        public Sprite CurrentLevelSprite;
        public Sprite FutureLevelsSprite;
        [Space]
        public Color PassedLevelsTextColor;
        public Color CurrentLevelTextColor;
        public Color FutureLevelsTextColor;

        private bool _bossActive;
        
        public void SetPassed()
        {
            if (_bossActive)
            {
                BossLevelImage.sprite = PassedLevelsSprite;
                SkullImage.color = PassedLevelsTextColor;
                BossLevelText.color = PassedLevelsTextColor;
            }
            else
            {
                RegularLevelImage.sprite = PassedLevelsSprite;
                RegularLevelText.color = PassedLevelsTextColor;
            }
        }

        public void SetCurrent()
        {
            if (_bossActive)
            {
                BossLevelImage.sprite = CurrentLevelSprite;
                SkullImage.color = CurrentLevelTextColor;
                BossLevelText.color = CurrentLevelTextColor;
            }
            else
            {
                RegularLevelImage.sprite = CurrentLevelSprite;
                RegularLevelText.color = CurrentLevelTextColor;
            }
        }
        
        public void SetFuture()
        {
            if (_bossActive)
            {
                BossLevelImage.sprite = FutureLevelsSprite;
                SkullImage.color = FutureLevelsTextColor;
                BossLevelText.color = FutureLevelsTextColor;
            }
            else
            {
                RegularLevelImage.sprite = FutureLevelsSprite;
                RegularLevelText.color = FutureLevelsTextColor;
            }
        }

        public void SetBoss()
        {
            _bossActive = true;
            RegularLevelLabel.SetActive(false);
            BossLevelLabel.SetActive(true);
        }
        
        public void SetRegular(int level)
        {
            _bossActive = false;
            RegularLevelLabel.SetActive(true);
            BossLevelLabel.SetActive(false);

            RegularLevelText.text = level.ToString();
        }
    }
}