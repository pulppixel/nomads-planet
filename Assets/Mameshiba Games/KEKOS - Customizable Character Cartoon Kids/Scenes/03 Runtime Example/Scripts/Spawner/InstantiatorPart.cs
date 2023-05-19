using System;
using System.Linq;
using MameshibaGames.Common.Helpers;
using MameshibaGames.Kekos.CharacterEditorScene.Customization;
using MameshibaGames.Kekos.CharacterEditorScene.Data;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MameshibaGames.Kekos.RuntimeExampleScene.Spawner
{
    public class InstantiatorPart : Object
    {
       public int CreateItem(PartObjectDatabase database, SaveItemInfo selectedItem, string attachObjectName,
            GameObject objectToAttach, bool applyColor = false, int indexToCheck = 0, Action extraLogicIfFound = null)
        {
            PartObjectDatabase.ObjectList objectToInstantiate = database.itemObjects.FirstOrDefault(x =>
                x.itemSubobjects.Count > 0 && x.itemSubobjects[indexToCheck] != null &&
                x.itemSubobjects[0].gameObject.name == selectedItem.itemKey);

            if (objectToInstantiate == null) // Try to find default item fallback
            {
                string partPrefix = selectedItem.itemKey.Split('_')[0] + "_";
                
                objectToInstantiate = database.itemObjects.FirstOrDefault(x =>
                    x.itemSubobjects.Count > 0 && x.itemSubobjects[indexToCheck] != null &&
                    x.itemSubobjects[0].gameObject.name.StartsWith($"{partPrefix}00_"));
                
                selectedItem.SetToDefault();
            }
            
            if (objectToInstantiate == null) return -1;

            int itemIndex = database.itemObjects.IndexOf(x => x == objectToInstantiate);
            
            Transform transformToAttach = objectToAttach.transform.RecursiveFindChild(attachObjectName);

            GameObject newGameObject = Instantiate(objectToInstantiate.itemSubobjects[indexToCheck].gameObject, transformToAttach,
                true);
            newGameObject.transform.ResetTransform();

            Renderer renderer = newGameObject.GetComponent<Renderer>();
            renderer.material = database.itemsDatabases[itemIndex].items[selectedItem.colorIndexSelected].primaryMaterial;

            if (applyColor)
                renderer.SetColor(selectedItem.colorSelected);
            
            extraLogicIfFound?.Invoke();
            
            newGameObject.SetActive(true);

            return NameSolver.GetItemCode(newGameObject);
        }
    }
}