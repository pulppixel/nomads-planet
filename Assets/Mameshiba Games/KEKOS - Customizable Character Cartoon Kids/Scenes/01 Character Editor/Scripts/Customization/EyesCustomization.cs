using System.Collections.Generic;
using System.Linq;
using MameshibaGames.Common.Helpers;
using MameshibaGames.Common.UI;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace MameshibaGames.Kekos.CharacterEditorScene.Customization
{
   public class EyesCustomization : MonoBehaviour
   {
      [SerializeField]
      private EyesDatabase eyesDatabase;
      
      [SerializeField]
      private ColorSlider colorSlider;

      [SerializeField]
      private ItemButton itemButton;
      
      [SerializeField]
      private Transform newItemButtonsParent;
      
      [SerializeField]
      private Button randomizeButton;

      private Material _eyeBaseMaterial;
      private Material _lightMaterial;
      private readonly List<ItemButton> _itemButtons = new List<ItemButton>();

      private EyesData _currentEyes;
      
      private Texture _defaultBaseColor;
      private Texture _defaultURPBaseColor;
      private Texture _defaultHDRPBaseColor;
      private Texture _defaultLight;
      private Texture _defaultURPLight;
      private Texture _defaultHDRPLight;
      private Color _defaultColor;
      private Color _defaultURPColor;
      
      private static readonly int _Color = Shader.PropertyToID("_Color");
      private static readonly int _URPColor = Shader.PropertyToID("_BaseColor");
      private static readonly int _MainTex = Shader.PropertyToID("_MainTex");
      private static readonly int _URPMainTex = Shader.PropertyToID("_BaseMap");
      private static readonly int _HDRPMainTex = Shader.PropertyToID("_BaseColorMap");

      public void Init(GameObject kekosCharacter)
      {
         Renderer eyesRenderer = kekosCharacter.transform.RecursiveFindChild("HEAD").GetComponent<Renderer>();
         Material[] sharedMaterials = eyesRenderer.sharedMaterials;
         _eyeBaseMaterial = sharedMaterials[1];
         _lightMaterial = sharedMaterials[2];
         
         _eyeBaseMaterial.GetTextureIfHasProperty(_MainTex, ref _defaultBaseColor);
         _eyeBaseMaterial.GetTextureIfHasProperty(_URPMainTex, ref _defaultURPBaseColor);
         _eyeBaseMaterial.GetTextureIfHasProperty(_HDRPMainTex, ref _defaultHDRPBaseColor);
         _eyeBaseMaterial.GetColorIfHasProperty(_Color, ref _defaultColor);
         _eyeBaseMaterial.GetColorIfHasProperty(_URPColor, ref _defaultURPColor);
         _lightMaterial.GetTextureIfHasProperty(_MainTex, ref _defaultLight);
         _lightMaterial.GetTextureIfHasProperty(_URPMainTex, ref _defaultURPLight);
         _lightMaterial.GetTextureIfHasProperty(_HDRPMainTex, ref _defaultHDRPLight);

         colorSlider.onValueChangedColor.AddListener(ChangeEyeColor);
         colorSlider.value = 0;

         for (int itemDatabaseIndex = 0; itemDatabaseIndex < eyesDatabase.eyesData.Length; itemDatabaseIndex++)
         {
            EyesData eyesData = eyesDatabase.eyesData[itemDatabaseIndex];
            ItemButton newItemButton = Instantiate(itemButton, newItemButtonsParent);
            newItemButton.Init(itemDatabaseIndex, eyesData.sprite, () =>
            {
               DisableAllButtonsButThis(newItemButton);
               ChangeEyeTextures(eyesData);
            });
            
            _itemButtons.Add(newItemButton);
         }

         randomizeButton.onClick.AddListener(Randomize);
         
         _itemButtons[0].ChangeToggleState(true, true);

         ChangeToDefault();
      }

      public void Clean()
      {
         _eyeBaseMaterial.SetTextureIfHasProperty(_MainTex, _defaultBaseColor);
         _eyeBaseMaterial.SetTextureIfHasProperty(_URPMainTex, _defaultURPBaseColor);
         _eyeBaseMaterial.SetTextureIfHasProperty(_HDRPMainTex, _defaultHDRPBaseColor);
         _eyeBaseMaterial.SetColorIfHasProperty(_Color, _defaultColor);
         _eyeBaseMaterial.SetColorIfHasProperty(_URPColor, _defaultURPColor);
         _lightMaterial.SetTextureIfHasProperty(_MainTex, _defaultLight);
         _lightMaterial.SetTextureIfHasProperty(_URPMainTex, _defaultURPLight);
         _lightMaterial.SetTextureIfHasProperty(_HDRPMainTex, _defaultHDRPLight);
      }

      private void ChangeEyeTextures(EyesData eyesData)
      {
         _currentEyes = eyesData;
         
         _eyeBaseMaterial.SetTextureIfHasProperty(_MainTex, _currentEyes.baseColor);
         _eyeBaseMaterial.SetTextureIfHasProperty(_URPMainTex, _currentEyes.baseColor);
         _eyeBaseMaterial.SetTextureIfHasProperty(_HDRPMainTex, _currentEyes.baseColor);
         _lightMaterial.SetTextureIfHasProperty(_MainTex, _currentEyes.light);
         _lightMaterial.SetTextureIfHasProperty(_URPMainTex, _currentEyes.light);
         _lightMaterial.SetTextureIfHasProperty(_HDRPMainTex, _currentEyes.light);
      }

      private void ChangeEyeColor(Color newColor)
      {
         _eyeBaseMaterial.SetColorIfHasProperty(_Color, newColor);
         _eyeBaseMaterial.SetColorIfHasProperty(_URPColor, newColor);
      }

      public void Randomize()
      {
         ItemButton randomButton = _itemButtons[Random.Range(0, _itemButtons.Count)];
         randomButton.ChangeToggleState(true, true);
         
         colorSlider.Randomize();
      }

      public void ChangeToDefault()
      {
         _itemButtons.First().ChangeToggleState(true, true);
         colorSlider.value = 0;
      }
      
      private void DisableAllButtonsButThis(ItemButton itemFilter)
      {
         foreach (ItemButton button in _itemButtons)
            button.ChangeToggleState(button == itemFilter);
      }
      
      public SaveItemInfo GetSaveItemInfo()
      {
         string key = _currentEyes.baseColor.name;
         float sliderColorSelected = colorSlider != null ? colorSlider.value : 0;
         Color colorSelected = colorSlider != null ? colorSlider.GetColor() : Color.black;
         SaveItemInfo saveItemInfo = new SaveItemInfo(key, colorSelected, sliderValueSelected: sliderColorSelected);
         return saveItemInfo;
      }
        
      public void ChangeWithSaveItemInfo(SaveItemInfo itemData)
      {
         EyesData savedItem = eyesDatabase.eyesData.FirstOrDefault(item =>
            item.baseColor.name == itemData.itemKey);
            
         if (savedItem == null) return;
            
         ItemButton button = _itemButtons[eyesDatabase.eyesData.IndexOf( x => x == savedItem)];
         button.ChangeToggleState(true, true);
         colorSlider.value = itemData.sliderValueSelected;
      }
   }
}