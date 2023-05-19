using System;
using System.Collections.Generic;
using UnityEngine;

namespace MameshibaGames.Kekos.CharacterEditorScene.Customization
{
    [Serializable]
    public class SaveCustomizationModel
    {
        public SaveItemInfo torsoSelected;
        public SaveItemInfo legsSelected;
        public SaveItemInfo feetSelected;
        public SaveItemInfo handsSelected;
        public SaveItemInfo fullTorsoSelected;
        public SaveItemInfo torsoPropSelected;
        public SaveItemInfo hairSelected;
        public SaveItemInfo capSelected;
        public SaveItemInfo glassesSelected;
        public SaveItemInfo eyesSelected;
        public SaveItemInfo skinBaseSelected;
        public SaveItemInfo skinDetailSelected;
        public SaveItemInfo eyebrowsSelected;
        public List<SaveItemInfo> faceCharacteristicsSelected;
    }

    [Serializable]
    public class SaveItemInfo
    {
        public string itemKey;
        public int colorIndexSelected;
        public Color colorSelected;
        public float sliderValueSelected;

        public SaveItemInfo(string itemKey, int colorIndexSelected = 0, float sliderValueSelected = 0) : this(itemKey, Color.black,
            colorIndexSelected, sliderValueSelected)
        { }
        
        public SaveItemInfo(string itemKey, Color colorSelected, int colorIndexSelected = 0, float sliderValueSelected = 0)
        {
            this.itemKey = itemKey;
            this.colorIndexSelected = colorIndexSelected;
            this.colorSelected = colorSelected;
            this.sliderValueSelected = sliderValueSelected;
        }

        public void SetToDefault()
        {
            colorIndexSelected = 0;
            sliderValueSelected = 0;
            colorSelected = Color.black;
        }
    }
}
