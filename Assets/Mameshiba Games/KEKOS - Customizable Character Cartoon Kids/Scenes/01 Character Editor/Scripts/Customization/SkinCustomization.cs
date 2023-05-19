using System.Collections.Generic;
using System.Linq;
using MameshibaGames.Common.Helpers;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace MameshibaGames.Kekos.CharacterEditorScene.Customization
{
    public class SkinCustomization : MonoBehaviour
    {
        [SerializeField]
        private SkinDatabase skinDatabase;
        
        [SerializeField]
        private Button randomizeButton;

        [SerializeField]
        private ItemButton itemButton;

        [SerializeField]
        private Transform newBaseItemButtonsParent;
        
        [SerializeField]
        private Transform newDetailItemButtonsParent;

        private SkinBaseData _currentSkinBase;
        private SkinDetailData _currentSkinDetail;
        
        private Texture _defaultSkinDetail;
        private Texture _defaultSkinDetailMask;
        private Texture _defaultMainSkin;
        private Texture _defaultURPMainSkin;
        private Texture _defaultEmissive;
        private Color _defaultEmissionColor;
        private Color _defaultHDRPEmissionColor;
        
        private Material _skinMaterial;
        private Renderer _skinRenderer;
        private readonly List<ItemButton> _baseItemButtons = new List<ItemButton>();
        private readonly List<ItemButton> _detailItemButtons = new List<ItemButton>();

        private static readonly int _DetailAlbedoMap = Shader.PropertyToID("_DetailAlbedoMap");
        private static readonly int _Emission = Shader.PropertyToID("_EmissionMap");
        private static readonly int _DetailMask = Shader.PropertyToID("_DetailMask");
        private static readonly int _MainTex = Shader.PropertyToID("_MainTex");
        private static readonly int _URPMainTex = Shader.PropertyToID("_BaseMap");
        private static readonly int _EmissionColor = Shader.PropertyToID("_EmissionColor");
        private static readonly int _HDRPEmissionColor = Shader.PropertyToID("_EmissionColorHDRP");

        public void Init(GameObject character)
        {
            _skinRenderer = character.transform.RecursiveFindChild("HEAD").GetComponent<Renderer>();
            _skinMaterial = _skinRenderer.sharedMaterials[0];

            _skinMaterial.GetTextureIfHasProperty(_DetailAlbedoMap, ref _defaultSkinDetail);
            _skinMaterial.GetTextureIfHasProperty(_DetailMask, ref _defaultSkinDetailMask);
            _skinMaterial.GetTextureIfHasProperty(_MainTex, ref _defaultMainSkin);
            _skinMaterial.GetTextureIfHasProperty(_URPMainTex, ref _defaultURPMainSkin);
            _skinMaterial.GetTextureIfHasProperty(_Emission, ref _defaultEmissive);
            _skinMaterial.GetColorIfHasProperty(_EmissionColor, ref _defaultEmissionColor);
            _skinMaterial.GetColorIfHasProperty(_HDRPEmissionColor, ref _defaultHDRPEmissionColor);
            
            for (int itemDatabaseIndex = 0; itemDatabaseIndex < skinDatabase.skinBaseData.Length; itemDatabaseIndex++)
            {
                SkinBaseData skinData = skinDatabase.skinBaseData[itemDatabaseIndex];
                ItemButton newItemButton = Instantiate(itemButton, newBaseItemButtonsParent);
                newItemButton.Init(itemDatabaseIndex, skinData.sprite, () =>
                {
                    DisableAllButtonsButThis(_baseItemButtons, newItemButton);
                    ChangeSkinBase(skinData);
                });
            
                _baseItemButtons.Add(newItemButton);
            }
            
            for (int itemDatabaseIndex = 0; itemDatabaseIndex < skinDatabase.skinDetailData.Length; itemDatabaseIndex++)
            {
                SkinDetailData skinDetailData = skinDatabase.skinDetailData[itemDatabaseIndex];
                ItemButton newItemButton = Instantiate(itemButton, newDetailItemButtonsParent);
                newItemButton.Init(itemDatabaseIndex, skinDetailData.sprite, () =>
                {
                    DisableAllButtonsButThis(_detailItemButtons, newItemButton);
                    ChangeSkinDetail(skinDetailData);
                });
            
                _detailItemButtons.Add(newItemButton);
            }
            
            _baseItemButtons[0].ChangeToggleState(true, true);
            _detailItemButtons[0].ChangeToggleState(true, true);

            randomizeButton.onClick.AddListener(Randomize);
        }

        public void Clean()
        {
            _skinMaterial.SetTextureIfHasProperty(_DetailMask, _defaultSkinDetailMask);
            _skinMaterial.SetTextureIfHasProperty(_DetailAlbedoMap, _defaultSkinDetail);
            _skinMaterial.SetTextureIfHasProperty(_MainTex, _defaultMainSkin);
            _skinMaterial.SetTextureIfHasProperty(_URPMainTex, _defaultURPMainSkin);
            _skinMaterial.SetTextureIfHasProperty(_Emission, _defaultEmissive);
            _skinMaterial.SetColorIfHasProperty(_EmissionColor, _defaultEmissionColor);
            _skinMaterial.SetColorIfHasProperty(_HDRPEmissionColor, _defaultHDRPEmissionColor);
        }

        private void ChangeSkinDetail(SkinDetailData skinDetailData)
        {
            _currentSkinDetail = skinDetailData;
            
            _skinMaterial.SetTextureIfHasProperty(_DetailMask, _currentSkinDetail.detailMask);
            _skinMaterial.SetTextureIfHasProperty(_DetailAlbedoMap, _currentSkinDetail.detailBase);
            _skinMaterial.EnableKeyword("_Emission");
            _skinMaterial.EnableKeyword("_DETAIL_MULX2");
            _skinMaterial.SetTextureIfHasProperty(_Emission, _currentSkinDetail.emissive);
            _skinMaterial.SetColorIfHasProperty(_EmissionColor, _currentSkinDetail.emissiveColor);
            _skinMaterial.SetColorIfHasProperty(_HDRPEmissionColor, _currentSkinDetail.emissiveColor);
        }

        private void ChangeSkinBase(SkinBaseData skinBaseData)
        {
            _currentSkinBase = skinBaseData;
            
            _skinMaterial.SetTextureIfHasProperty(_MainTex, _currentSkinBase.baseColor);
            _skinMaterial.SetTextureIfHasProperty(_URPMainTex, _currentSkinBase.baseColor);
        }

        public void Randomize()
        {
            ItemButton randomBaseButton = _baseItemButtons[Random.Range(0, _baseItemButtons.Count)];
            randomBaseButton.ChangeToggleState(true, true);
            ItemButton randomDetailButton = _detailItemButtons[Random.Range(0, _detailItemButtons.Count)];
            randomDetailButton.ChangeToggleState(true, true);
        }
        
        private void DisableAllButtonsButThis(List<ItemButton> detailItemButtons, ItemButton itemFilter)
        {
            foreach (ItemButton button in detailItemButtons)
                button.ChangeToggleState(button == itemFilter);
        }
        
        public (SaveItemInfo baseSkinInfo, SaveItemInfo detailSkinInfo) GetSaveItemInfo()
        {
            string baseKey = _currentSkinBase.baseColor.name;
            SaveItemInfo baseSaveItemInfo = new SaveItemInfo(baseKey);
            
            string detailKey = _currentSkinDetail.detailMask != null ? _currentSkinDetail.detailMask.name : "null";
            SaveItemInfo detailSaveItemInfo = new SaveItemInfo(detailKey);
            return (baseSaveItemInfo, detailSaveItemInfo);
        }
        
        public void ChangeWithSaveItemInfo(SaveItemInfo baseSkinInfo, SaveItemInfo detailSkinInfo)
        {
            SkinBaseData savedBaseItem = skinDatabase.skinBaseData.FirstOrDefault(item =>
                item.baseColor.name == baseSkinInfo.itemKey);

            SkinDetailData savedDetailItem = skinDatabase.skinDetailData.FirstOrDefault(item =>
                (item.detailMask == null && detailSkinInfo.itemKey == "null") ||
                (item.detailMask != null && item.detailMask.name == detailSkinInfo.itemKey));

            if (savedBaseItem != null)
            {
                ItemButton button = _baseItemButtons[skinDatabase.skinBaseData.IndexOf( x => x == savedBaseItem)];
                button.ChangeToggleState(true, true);
            }
            
            if (savedDetailItem != null)
            {
                ItemButton button = _detailItemButtons[skinDatabase.skinDetailData.IndexOf( x => x == savedDetailItem)];
                button.ChangeToggleState(true, true);
            }
        }
    }
}