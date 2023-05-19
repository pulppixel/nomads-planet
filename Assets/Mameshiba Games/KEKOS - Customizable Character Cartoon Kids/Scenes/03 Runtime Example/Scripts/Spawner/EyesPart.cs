using System.Linq;
using MameshibaGames.Common.Helpers;
using MameshibaGames.Kekos.CharacterEditorScene.Customization;
using UnityEngine;

namespace MameshibaGames.Kekos.RuntimeExampleScene.Spawner
{
    public class EyesPart : Object
    {
        private static readonly int _Color = Shader.PropertyToID("_Color");
        private static readonly int _URPColor = Shader.PropertyToID("_BaseColor");
        private static readonly int _MainTex = Shader.PropertyToID("_MainTex");
        private static readonly int _URPMainTex = Shader.PropertyToID("_BaseMap");
        private static readonly int _HDRPMainTex = Shader.PropertyToID("_BaseColorMap");
        
        public void ModifiyItem(EyesDatabase eyesDatabase, SaveItemInfo eyesSelected, GameObject parentToFind)
        {
            EyesData eyesData = eyesDatabase.eyesData.FirstOrDefault(x => x.baseColor.name == eyesSelected.itemKey);

            if (eyesData == null) // Fallback to default
            {
                eyesData = eyesDatabase.eyesData[0];
                eyesSelected.SetToDefault();
            }
            
            Renderer eyesRenderer = parentToFind.transform.RecursiveFindChild("HEAD").GetComponent<Renderer>();
            Material[] materials = eyesRenderer.materials;
            Material eyeBaseMaterial = materials[1];
            Material lightMaterial = materials[2];
         
            eyeBaseMaterial.SetTextureIfHasProperty(_MainTex, eyesData.baseColor);
            eyeBaseMaterial.SetTextureIfHasProperty(_URPMainTex, eyesData.baseColor);
            eyeBaseMaterial.SetTextureIfHasProperty(_HDRPMainTex, eyesData.baseColor);
            eyeBaseMaterial.SetColorIfHasProperty(_Color, eyesSelected.colorSelected);
            eyeBaseMaterial.SetColorIfHasProperty(_URPColor, eyesSelected.colorSelected);
            lightMaterial.SetTextureIfHasProperty(_MainTex, eyesData.light);
            lightMaterial.SetTextureIfHasProperty(_URPMainTex, eyesData.light);
            lightMaterial.SetTextureIfHasProperty(_HDRPMainTex, eyesData.light);
        }
    }
}