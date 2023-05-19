using System.Collections.Generic;
using System.Linq;
using MameshibaGames.Common.Helpers;
using UnityEngine;

namespace MameshibaGames.Kekos.CharacterEditorScene.Customization
{
    public class PartCustomizationObject : PartCustomization
    {
        protected GameObject currentObject;
        protected Renderer currentItemRenderer;

        protected override IEnumerable<GameObject> GetItemsList(GameObject kid)
        {
            PartObjectDatabase partDatabaseObject = (PartObjectDatabase)partDatabase;
            return partDatabaseObject.itemObjects.Select(x => x.itemSubobjects[0]);
        }

        protected override void DisablePreviousItem()
        {
            if (currentObject != null)
                Destroy(currentObject);
        }

        protected override void InitNewItem(ItemData itemData)
        {
            base.InitNewItem(itemData);

            if (currentItem.gameObject != null)
            {
                currentObject = Instantiate(currentItem.gameObject, characterHead);
                currentObject.transform.ResetTransform();
                currentItemRenderer = currentObject.GetComponent<Renderer>();
            }
        }

        protected override void ActivateNewItem()
        {
            if (currentObject != null)
                currentObject.SetActive(true);
        }
        
        protected override Renderer GetCurrentRenderer() => currentItemRenderer;

        protected override void ChangeMaterialToCurrentItem(Renderer currentRenderer)
        {
            if (currentRenderer == null) return;
            
            Material[] sharedMaterials = currentRenderer.sharedMaterials;
            sharedMaterials[0] = currentItem.material;
            currentRenderer.sharedMaterials = sharedMaterials;
        }
    }
}
