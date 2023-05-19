using System.Linq;
using MameshibaGames.Common.Helpers;
using MameshibaGames.Kekos.CharacterEditorScene.Customization;
using UnityEngine;

namespace MameshibaGames.Kekos.RuntimeExampleScene.Spawner
{
    public class SkinPart : Object
    {
        private static readonly int _DetailAlbedoMap = Shader.PropertyToID("_DetailAlbedoMap");
        private static readonly int _Emission = Shader.PropertyToID("_EmissionMap");
        private static readonly int _DetailMask = Shader.PropertyToID("_DetailMask");
        private static readonly int _MainTex = Shader.PropertyToID("_MainTex");
        private static readonly int _URPMainTex = Shader.PropertyToID("_BaseMap");
        private static readonly int _EmissionColor = Shader.PropertyToID("_EmissionColor");
        private static readonly int _HDRPEmissionColor = Shader.PropertyToID("_EmissionColorHDRP");
        
        public void ModifyItem(SkinDatabase skinDatabase, SaveItemInfo skinBaseSelected, SaveItemInfo skinDetailSelected,
            GameObject parentToFind)
        {
            SkinBaseData skinBaseData =
                skinDatabase.skinBaseData.FirstOrDefault(x => x.baseColor.name == skinBaseSelected.itemKey);

            if (skinBaseData == null) // Fallback to default
            {
                skinBaseData = skinDatabase.skinBaseData[0];
                skinBaseSelected.SetToDefault();
            }
            
            SkinDetailData skinDetailData =
                skinDatabase.skinDetailData.FirstOrDefault(x => (x.detailMask == null && skinDetailSelected.itemKey == "null") ||
                                                                (x.detailMask != null &&
                                                                 x.detailMask.name == skinDetailSelected.itemKey));

            if (skinDetailData == null) // Fallback to default
            {
                skinDetailData = skinDatabase.skinDetailData[0];
                skinDetailSelected.SetToDefault();
            }
            
            Renderer headRenderer = parentToFind.transform.RecursiveFindChild("HEAD").GetComponent<Renderer>();
            Material[] materials = headRenderer.materials;
            Material skinMaterial = new Material(materials[0]);
            string skinMaterialName = skinMaterial.name.Replace("(Instance)", "").Trim();

            ChangeSkinBase(skinMaterial, skinBaseData);
            ChangeSkinDetail(skinMaterial, skinDetailData);
            
            foreach (Transform child in parentToFind.transform)
            {
                Renderer childRenderer = child.GetComponent<Renderer>();
                
                if (childRenderer == null) continue;

                Material[] childRendererMaterials = childRenderer.materials;
                for (int i = 0; i < childRendererMaterials.Length; i++)
                {
                    Material childRendererMaterial = childRendererMaterials[i];
                    if (childRendererMaterial.name.StartsWith(skinMaterialName))
                        childRendererMaterials[i] = skinMaterial;
                }
                childRenderer.materials = childRendererMaterials;
            }
        }
        
        private void ChangeSkinDetail(Material skinMaterial, SkinDetailData skinDetailData)
        {
            skinMaterial.SetTextureIfHasProperty(_DetailMask, skinDetailData.detailMask);
            skinMaterial.SetTextureIfHasProperty(_DetailAlbedoMap, skinDetailData.detailBase);
            skinMaterial.EnableKeyword("_Emission");
            skinMaterial.EnableKeyword("_DETAIL_MULX2");
            skinMaterial.SetTextureIfHasProperty(_Emission, skinDetailData.emissive);
            skinMaterial.SetColorIfHasProperty(_EmissionColor, skinDetailData.emissiveColor);
            skinMaterial.SetColorIfHasProperty(_HDRPEmissionColor, skinDetailData.emissiveColor);
        }
        
        private void ChangeSkinBase(Material skinMaterial, SkinBaseData skinBaseData)
        {
            skinMaterial.SetTextureIfHasProperty(_MainTex, skinBaseData.baseColor);
            skinMaterial.SetTextureIfHasProperty(_URPMainTex, skinBaseData.baseColor);
        }
    }
}