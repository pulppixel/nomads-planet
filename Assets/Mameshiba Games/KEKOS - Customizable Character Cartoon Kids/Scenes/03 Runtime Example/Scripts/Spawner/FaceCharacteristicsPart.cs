using System.Collections.Generic;
using MameshibaGames.Common.Helpers;
using MameshibaGames.Kekos.CharacterEditorScene.Customization;
using UnityEngine;

namespace MameshibaGames.Kekos.RuntimeExampleScene.Spawner
{
    public class FaceCharacteristicsPart : Object
    {
        public void ModifyItem(List<SaveItemInfo> faceCharacteristicsSelected, GameObject parentToFind)
        {
            SkinnedMeshRenderer renderer = parentToFind.transform.RecursiveFindChild("HEAD").GetComponent<SkinnedMeshRenderer>();
            
            foreach (SaveItemInfo saveItemInfo in faceCharacteristicsSelected)
            {
                ChangeCharacteristic(renderer, saveItemInfo.itemKey, saveItemInfo.sliderValueSelected);
            }
        }
        
        private void ChangeCharacteristic(SkinnedMeshRenderer renderer, string characteristicName, float newValue)
        {
            for (int i = 0; i < renderer.sharedMesh.blendShapeCount; i++)
            {
                if (renderer.sharedMesh.GetBlendShapeName(i) == characteristicName)
                {
                    renderer.SetBlendShapeWeight(i, newValue * 100);
                    break;
                }
            }
        }
    }
}