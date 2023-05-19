using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using MameshibaGames.Common.Helpers;
using MameshibaGames.Common.UI;
using MameshibaGames.Common.Utils;
using MameshibaGames.Kekos.CharacterEditorScene.Data;
using TMPro;
using UnityEngine.Events;

namespace MameshibaGames.Kekos.CharacterEditorScene.Customization
{
    public class PartCustomization : MonoBehaviour
    {
        protected class ItemData
        {
            public GameObject gameObject;
            public Renderer renderer;
            public Material material;
            public ItemButton itemButton;
            public int colorIndex;
            public int itemCode;
        }
        
        [SerializeField]
        protected string partPrefix = "XXX_";

        [SerializeField]
        protected PartDatabase partDatabase;
        
        [SerializeField]
        protected ItemButton itemButton;

        [SerializeField]
        protected Transform newItemButtonsParent;

        [SerializeField]
        protected Button randomPartButton;

        [SerializeField]
        protected UnityEvent onItemChange = new UnityEvent();
        
        [SerializeField]
        protected UnityEvent onChangeToDefault = new UnityEvent();
        
        [SerializeField]
        protected UnityEventInt onChangeToNonDefault = new UnityEventInt();

        [SerializeField]
        protected ColorSlider colorSlider;

        private const string _MaterialPrefix = "MAT_";

        private List<ItemButton> _itemButtons = new List<ItemButton>();
        protected readonly List<ItemData> itemsList = new List<ItemData>();
        protected ItemData currentItem;
        protected ItemData defaultItem;
        protected Transform characterHead;
        
        private Color _partColor;
        private GameObject _character;

        private static readonly int _Color = Shader.PropertyToID("_Color");
        private static readonly int _URPColor = Shader.PropertyToID("_BaseColor");
        private const string _HeadModelName = "Bip001 Head";

        public void Init(GameObject character)
        {
            InitCharacter(character);

            InitColorSlider();
            
            foreach (GameObject itemObject in GetItemsList(character))
            {
                if (itemObject != null)
                    itemObject.SetActive(false);

                string spriteName = NameSolver.GetSpriteName(itemObject, _MaterialPrefix, partPrefix);

                for (int itemDatabaseIndex = 0; itemDatabaseIndex < partDatabase.itemsDatabases.Length; itemDatabaseIndex++)
                {
                    ItemDatabase itemDatabase = partDatabase.itemsDatabases[itemDatabaseIndex];
                    List<ItemDatabase.ItemInfo> itemsWithColors = itemDatabase.GetSpriteNameStartsWith(spriteName);

                    if (itemsWithColors.Count == 0) continue;

                    ItemButton newItemButton = Instantiate(itemButton, newItemButtonsParent);
                    newItemButton.Init(itemDatabaseIndex);
                    _itemButtons.Add(newItemButton);

                    for (int i = 0; i < itemsWithColors.Count; i++)
                    {
                        ItemDatabase.ItemInfo itemInfo = itemsWithColors[i];
                        Renderer itemRenderer = itemObject != null ? itemObject.GetComponent<Renderer>() : null;
                        ItemData newItemData = new ItemData
                        {
                            gameObject = itemObject,
                            renderer = itemRenderer,
                            material = itemInfo.primaryMaterial,
                            itemButton = newItemButton,
                            colorIndex = i,
                            itemCode = NameSolver.GetItemCode(itemObject)
                        };

                        itemsList.Add(newItemData);
                        ExtraPassWithItemDatabase(newItemData, partDatabase, itemDatabaseIndex);

                        newItemButton.AddItem(itemInfo, () =>
                        {
                            DisableAllButtonsButThis(newItemButton);
                            ChangeItemModel(newItemData);
                        });
                    }
                }
            }

            OrderItemButtonsByDatabasePosition();

            randomPartButton.onClick.AddListener(Randomize);
            defaultItem = itemsList.First();
            ChangeToDefault();

            void OrderItemButtonsByDatabasePosition()
            {
                _itemButtons = _itemButtons.OrderBy(x => x.DatabasePosition).ToList();
                foreach (ItemButton button in _itemButtons)
                    button.transform.SetAsLastSibling();
            }
        }

        private void InitCharacter(GameObject character)
        {
            _character = character;
            characterHead = _character.transform.RecursiveFindChild(_HeadModelName);
        }

        private void InitColorSlider()
        {
            if (colorSlider == null) return;

            colorSlider.onValueChangedColor.AddListener(newColor =>
                ChangeColorToCurrentItem(newColor, GetCurrentRenderer()));
            colorSlider.value = 0;
        }

        protected virtual IEnumerable<GameObject> GetItemsList(GameObject kid)
        {
            List<Transform> filteredItemPieces = kid.transform.All(x => x.gameObject.name.StartsWith(partPrefix));
            return filteredItemPieces.Select(itemPiece => itemPiece.gameObject);
        }

        protected virtual void ExtraPassWithItemDatabase(ItemData newItemData, PartDatabase itemDatabase, int itemDatabaseIndex)
        {
            
        }

        protected virtual void ChangeItemModel(ItemData itemData, bool needToSendEvents = true)
        {
            DisablePreviousItem();
            InitNewItem(itemData);

            ChangeMaterialToCurrentItem(GetCurrentRenderer());
            ChangeColorToCurrentItem(_partColor, GetCurrentRenderer());

            ActivateNewItem();

            SendEvents(needToSendEvents);
        }

        protected virtual void DisablePreviousItem()
        {
            currentItem?.gameObject.SetActive(false);
        }

        protected virtual void InitNewItem(ItemData itemData)
        {
            currentItem = itemData;
        }

        protected virtual void ActivateNewItem()
        {
            currentItem.gameObject.SetActive(true);
        }

        protected virtual Renderer GetCurrentRenderer() => currentItem?.renderer;

        protected virtual void ChangeMaterialToCurrentItem(Renderer currentRenderer)
        {
            if (currentRenderer == null) return;
            
            Material[] sharedMaterials = currentRenderer.sharedMaterials;
            if (sharedMaterials.Length == 1)
                sharedMaterials[0] = currentItem.material;
            else if (sharedMaterials.Length > 1)
                sharedMaterials[1] = currentItem.material;
            currentRenderer.sharedMaterials = sharedMaterials;
        }

        private void ChangeColorToCurrentItem(Color newColor, Renderer currentRenderer)
        {
            _partColor = newColor;
            Color normalColor = _partColor;
            Color urpColor = _partColor;
            
            if (colorSlider == null || currentItem == null || currentItem.renderer == null || currentRenderer == null) return;

            if (currentRenderer.materials.Length == 1)
            {
                if (currentRenderer.materials[0].HasProperty(_Color))
                {
                    if (!colorSlider.useAlpha)
                        normalColor.a = currentRenderer.materials[0].GetColor(_Color).a;
                    currentRenderer.materials[0].SetColor(_Color, normalColor);
                }

                if (currentRenderer.materials[0].HasProperty(_URPColor))
                {
                    if (!colorSlider.useAlpha)
                        urpColor.a = currentRenderer.materials[0].GetColor(_URPColor).a;
                    currentRenderer.materials[0].SetColor(_URPColor, urpColor);
                }
            }
            else if (currentRenderer.materials.Length > 1)
            {
                if (currentRenderer.materials[1].HasProperty(_Color))
                {
                    if (!colorSlider.useAlpha)
                        normalColor.a = currentRenderer.materials[1].GetColor(_Color).a;
                    currentRenderer.materials[1].SetColor(_Color, normalColor);
                }

                if (currentRenderer.materials[1].HasProperty(_URPColor))
                {
                    if (!colorSlider.useAlpha)
                        urpColor.a = currentRenderer.materials[1].GetColor(_URPColor).a;
                    currentRenderer.materials[1].SetColor(_URPColor, urpColor);
                }
            }

            TMP_Text itemText = currentRenderer.gameObject.GetComponentInChildren<TMP_Text>();
            if (itemText != null)
            {
                Color textColor = _partColor;
                textColor.a = 1;
                itemText.color = textColor;
            }
        }

        private void SendEvents(bool sendEvents)
        {
            if (!sendEvents) return;
            
            if (defaultItem == currentItem)
                onChangeToDefault?.Invoke();
            else
                onChangeToNonDefault?.Invoke(currentItem.itemCode);
            onItemChange?.Invoke();
        }

        public void ChangeToDefault()
        {
            if (currentItem == defaultItem) return;
            
            DisableAllButtonsButThis(defaultItem.itemButton);
            ChangeItemModel(defaultItem, false);
        }

        public void Randomize()
        {
            ItemData randomItem = itemsList[Random.Range(0, itemsList.Count)];
            DisableAllButtonsButThis(randomItem.itemButton);
            randomItem.itemButton.ActivateWithColorIndex(randomItem.colorIndex);
            
            if (colorSlider != null)
                colorSlider.Randomize();
        }

        public void DisableAllElements()
        {
            foreach (ItemData item in itemsList)
            {
                item.itemButton.ChangeToggleState(false);
                item.gameObject.SetActive(false);
            }
        }

        public void RecoverLastItem()
        {
            ChangeItemModel(currentItem, false);
            currentItem.itemButton.ChangeToggleState(true);
        }

        private void DisableAllButtonsButThis(ItemButton itemFilter)
        {
            foreach (ItemButton button in _itemButtons)
                button.ChangeToggleState(button == itemFilter);
        }

        public SaveItemInfo GetSaveItemInfo()
        {
            string key = currentItem.gameObject != null ? currentItem.gameObject.name : "null";
            int selectedColorIndex = currentItem.colorIndex;
            float sliderColorSelected = colorSlider != null ? colorSlider.value : 0;
            Color colorSelected = colorSlider != null ? colorSlider.GetColor() : Color.black;
            SaveItemInfo saveItemInfo = new SaveItemInfo(key, colorSelected, selectedColorIndex, sliderColorSelected);
            return saveItemInfo;
        }
        
        public void ChangeWithSaveItemInfo(SaveItemInfo itemData)
        {
            ItemData savedItem = itemsList.FirstOrDefault(item =>
                ((item.gameObject == null && itemData.itemKey == "null") ||
                 (item.gameObject != null && item.gameObject.name == itemData.itemKey)) &&
                item.colorIndex == itemData.colorIndexSelected);
            
            if (savedItem == null) return;
            
            DisableAllButtonsButThis(savedItem.itemButton);
            savedItem.itemButton.ActivateWithColorIndex(savedItem.colorIndex);
            if (colorSlider != null)
                colorSlider.value = itemData.sliderValueSelected;
        }
    }
}