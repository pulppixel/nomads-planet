using System;
using System.Collections.Generic;
using System.Linq;
using MameshibaGames.Common.Helpers;
using MameshibaGames.Kekos.CharacterEditorScene.Customization;
using MameshibaGames.Kekos.CharacterEditorScene.Data;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MameshibaGames.Kekos.RuntimeExampleScene.Spawner
{
    public class FilterPart : Object
    {
        private const string _MaterialPrefix = "MAT_";

        public void FilterItem(PartDatabase database, SaveItemInfo selectedItem, GameObject parentObjectToFilter,
            bool applyColor = false, Action extraLogicIfFound = null)
        {
            string partPrefix = selectedItem.itemKey.Split('_')[0] + "_";

            List<Transform> filteredItemPieces =
                parentObjectToFilter.transform.All(x => x.gameObject.name.StartsWith(partPrefix));

            List<Transform> notMatchingItems = filteredItemPieces.Where(x => x.name != selectedItem.itemKey).ToList();

            bool itemNotFound = notMatchingItems.Count == filteredItemPieces.Count;
            if (itemNotFound) // Try to fallback to default
            {
                notMatchingItems = filteredItemPieces.Where(x => !x.name.StartsWith($"{partPrefix}00_")).ToList();
                selectedItem.SetToDefault();
            }

            notMatchingItems.ForEach(x => Destroy(x.gameObject));
            filteredItemPieces.RemoveAll(x => notMatchingItems.Contains(x));
            
            if (filteredItemPieces.Count == 0) return;
            
            foreach (Transform filteredItemPiece in filteredItemPieces)
            {
                string spriteName = NameSolver.GetSpriteName(filteredItemPiece.gameObject, _MaterialPrefix, partPrefix);
                
                foreach (ItemDatabase databaseItemsDatabase in database.itemsDatabases)
                {
                    List<ItemDatabase.ItemInfo> materialsList = databaseItemsDatabase.GetSpriteNameStartsWith(spriteName);
                    if (materialsList.Count > 0)
                    {
                        Renderer currentRenderer = filteredItemPiece.gameObject.GetComponent<Renderer>();
                        Material material = materialsList[selectedItem.colorIndexSelected].primaryMaterial;

                        if (currentRenderer == null) break;
                        
                        Material[] materials = currentRenderer.materials;
                        if (materials.Length == 1)
                        {
                            materials[0] = material;
                        }
                        else if (materials.Length > 1)
                        {
                            materials[1] = material;
                        }
                        currentRenderer.materials = materials;
                        
                        if (applyColor)
                            currentRenderer.SetColor(selectedItem.colorSelected);

                        extraLogicIfFound?.Invoke();
                        
                        break;
                    }
                }
                
                filteredItemPiece.gameObject.SetActive(true);
            }
        }
    }
}