using System.Collections.Generic;
using MameshibaGames.Common.Helpers;
using UnityEngine;

namespace MameshibaGames.Kekos.CharacterEditorScene.Customization
{
    public class HairCustomization : PartCustomizationObject
    {
        private class HairExtraData
        {
            public GameObject capGameObject;
        }
        
        private readonly Dictionary<ItemData, HairExtraData> _itemToExtraDataDictionary = new Dictionary<ItemData, HairExtraData>();

        private enum HairStyle
        {
            Full,
            Cap,
            Bald
        }
        
        private HairStyle _hairStyle;

        public void ChangeToFull()
        {
            _hairStyle = HairStyle.Full;
            ChangeItemModel(currentItem);
        }
        
        public void ChangeToCap(int capVersion)
        {
            _hairStyle = capVersion == 52 ? HairStyle.Bald : HairStyle.Cap;
            ChangeItemModel(_hairStyle == HairStyle.Bald? null : currentItem);
        }

        protected override void ExtraPassWithItemDatabase(ItemData newItemData, PartDatabase itemDatabase, int itemDatabaseIndex)
        {
            base.ExtraPassWithItemDatabase(newItemData, itemDatabase, itemDatabaseIndex);
            
            _itemToExtraDataDictionary.Add(newItemData, new HairExtraData
            {
                capGameObject = ((PartObjectDatabase)itemDatabase).itemObjects[itemDatabaseIndex].itemSubobjects[1]
            });
        }

        protected override void InitNewItem(ItemData itemData)
        {
            if (itemData == null) return;

            currentItem = itemData;
            
            if (_hairStyle == HairStyle.Bald) return;

            if (currentItem.gameObject != null)
            {
                currentObject = Instantiate(_hairStyle == HairStyle.Cap
                            ? _itemToExtraDataDictionary[currentItem].capGameObject
                            : currentItem.gameObject, characterHead);
                currentObject.transform.ResetTransform();
                currentItemRenderer = currentObject.GetComponent<Renderer>();
            }
        }
    }
}
