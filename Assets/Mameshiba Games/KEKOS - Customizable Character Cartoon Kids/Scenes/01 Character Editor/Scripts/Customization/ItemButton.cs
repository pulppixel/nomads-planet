using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MameshibaGames.Kekos.CharacterEditorScene.Customization
{
    public class ItemButton : MonoBehaviour
    {
        [SerializeField]
        private Toggle toggle;
        
        [SerializeField]
        private Image itemImage;

        [SerializeField]
        private TMP_Text itemText;

        [SerializeField]
        private GameObject colorButtonsGroup;

        [SerializeField]
        private ColorButton colorButtonPrefab;

        private readonly List<ColorButton> _colorButtons = new List<ColorButton>();

        public int DatabasePosition { get; private set; }
        private int _pointer;
        private Action _lastChange;

        public void Init(int position, Sprite iconSprite = null, Action toggleOnAction = null, string iconText = null)
        {
            if (iconSprite != null)
                itemImage.sprite = iconSprite;

            if (!string.IsNullOrEmpty(iconText) && itemText != null)
            {
                itemText.text = iconText;
                itemText.gameObject.SetActive(true);
            }
            
            DatabasePosition = position;
            
            toggle.onValueChanged.AddListener(value =>
            {
                if (value)
                {
                    _lastChange?.Invoke();
                    toggleOnAction?.Invoke();
                }

                toggle.interactable = !value;

                ChangeColorButtonsVisibility(value);
            });
        }
        
        public void AddItem(ItemDatabase.ItemInfo itemInfo, Action actionWhenClick)
        {
            ColorButton newColorButton = Instantiate(colorButtonPrefab, colorButtonsGroup.transform);
            _colorButtons.Add(newColorButton);
            
            if (_pointer == 0)
            {
                _lastChange = actionWhenClick;
                itemImage.sprite = itemInfo.sprite;
                newColorButton.outline.enabled = true;
            }

            newColorButton.SetColors(itemInfo.iconColors);

            newColorButton.button.onClick.AddListener(() =>
            {
                DisableAllOutlines();
                newColorButton.outline.enabled = true;
                itemImage.sprite = itemInfo.sprite;
                _lastChange = actionWhenClick;
                toggle.isOn = true;
                actionWhenClick?.Invoke();
            });
            
            _pointer++;
        }

        private void ChangeColorButtonsVisibility(bool selected)
        {
            colorButtonsGroup.SetActive(_pointer == 1 ? false : selected);
        }

        public void ChangeToggleState(bool newState, bool notifyOnTrueState = false)
        {
            if (!newState)
            {
                toggle.isOn = false;
            }
            else
            {
                toggle.interactable = false;
                if (!notifyOnTrueState)
                    toggle.SetIsOnWithoutNotify(true);
                else
                    toggle.isOn = true;
                ChangeColorButtonsVisibility(true);
            }
        }

        private void DisableAllOutlines()
        {
            foreach (ColorButton colorButton in _colorButtons)
                colorButton.outline.enabled = false;
        }
        
        public void ActivateWithColorIndex(int index)
        {
            _colorButtons[index].button.onClick?.Invoke();
        }
    }
}