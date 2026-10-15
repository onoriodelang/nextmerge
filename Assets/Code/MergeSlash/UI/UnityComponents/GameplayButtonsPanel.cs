using System;
using Leopotam.Ecs;
using UnityEngine;
using TMPro;
using UICoreECS;
using UnityEngine.UI;

namespace Modules.MergeSlash.UI
{
    public class GameplayButtonsPanel : AUIEntity
    {
        public GameObject TilesButtons;
        public GameObject ThrowButtons;

        public TextMeshProUGUI SlotButtonText;
        public TextMeshProUGUI WeaponButtonText;
        public Color EnabledTextColor;
        public Color DisabledTextColor;

        public Image SlotDollarImage;
        public Image WeaponDollarImage;
        public Sprite EnabledDollarSprite;
        public Sprite DisabledDollarSprite;
        
        public Image SlotBuyButtonImage;
        public Image WeaponBuyButtonImage;
        public Sprite EnabledBuyButtonSprite;
        public Sprite DisabledBuyButtonSprite;

        public Image SlashButtonImage;
        public Sprite EnabledSlashButtonSprite;
        public Sprite DisabledSlashButtonSprite;

        public TextMeshProUGUI[] WeaponLevelLabels;
        public GameObject WeaponLevelLabelsFolder;
        public Vector3 LabelOffset;
        public float _timerEnableThrowButtons;

        private EcsWorld _world;

        public override void Init(EcsWorld world, EcsEntity screen)
        {
            _world = world;
            screen.Get<GameplayButtonsPanelView>().View = this;
        }

        public void EnableTilesButtons()
        {
            TilesButtons.SetActive(true);
            WeaponLevelLabelsFolder.SetActive(true);
            ThrowButtons.SetActive(false);
        }
        
        public void EnableThrowButtons(float time = -1)
        {
            if (time > 0)
            {
                _timerEnableThrowButtons = time;
            }
            else
            {
                TilesButtons.SetActive(false);
                WeaponLevelLabelsFolder.SetActive(false);
                ThrowButtons.SetActive(true);
            }
        }

        private void Update()
        {
            if(_timerEnableThrowButtons <= 0)
                return;

            _timerEnableThrowButtons -= Time.unscaledDeltaTime;

            if (_timerEnableThrowButtons <= 0)
            {
                TilesButtons.SetActive(false);
                WeaponLevelLabelsFolder.SetActive(false);
                ThrowButtons.SetActive(true);
            }
        }

        public void DisableAllButtons()
        {
            TilesButtons.SetActive(false);
            WeaponLevelLabelsFolder.SetActive(false);
            ThrowButtons.SetActive(false);
        }

        public void SetCosts(int weaponCost, int slotCost)
        {
            WeaponButtonText.text = $"{weaponCost}";
            SlotButtonText.text = $"{slotCost}";
        }

        public void SetBuyWeaponButtonActive(bool value)
        {
            WeaponButtonText.color = value ? EnabledTextColor : DisabledTextColor;
            WeaponDollarImage.sprite = value ? EnabledDollarSprite : DisabledDollarSprite;
            WeaponBuyButtonImage.sprite = value ? EnabledBuyButtonSprite : DisabledBuyButtonSprite;
            WeaponBuyButtonImage.raycastTarget = value;
        }
        
        public void SetBuySlotButtonActive(bool value)
        {
            SlotButtonText.color = value ? EnabledTextColor : DisabledTextColor;
            SlotDollarImage.sprite = value ? EnabledDollarSprite : DisabledDollarSprite;
            SlotBuyButtonImage.sprite = value ? EnabledBuyButtonSprite : DisabledBuyButtonSprite;
            SlotBuyButtonImage.raycastTarget = value;
        }
        
        public void SetSlashButtonActive(bool value)
        {
            SlashButtonImage.sprite = value ? EnabledSlashButtonSprite : DisabledSlashButtonSprite;
            SlashButtonImage.raycastTarget = value;
        }

        public void SetLevels(int[] levels, Vector3[] poses)
        {
            if(!WeaponLevelLabelsFolder.activeInHierarchy)
                return;
            
            int idx = 0;
            foreach (var label in WeaponLevelLabels)
            {
                Transform parent = label.transform.parent;

                if (levels[idx] > 0)
                {
                    parent.gameObject.SetActive(true);
                    label.text = levels[idx].ToString();
                    parent.position = Camera.main.WorldToScreenPoint(poses[idx] + LabelOffset);
                }
                else
                {
                    parent.gameObject.SetActive(false);
                }

                idx++;
            }
        }
    }

    public struct GameplayButtonsPanelView
    {
        public GameplayButtonsPanel View;
        public bool Inited;
    }
}