using System;
using UnityEngine;

namespace MameshibaGames.Kekos.CharacterEditorScene.Data
{
    public static class NameSolver
    {
        public static string GetSpriteName(GameObject itemObject, string materialPrefix, string partPrefix)
        {
            if (itemObject == null) return $"{materialPrefix}{partPrefix}Empty";
                
            string itemName = itemObject.name;
            string[] itemNameParts = itemName.Split('_');

            string spriteName = $"{materialPrefix}{partPrefix}{itemNameParts[2]}";
            if (itemNameParts.Length > 3)
                spriteName += $"_{itemNameParts[3]}";
            return spriteName;
        }
        
        public static int GetItemCode(GameObject itemObject)
        {
            if (itemObject == null) return 0;
                
            string itemName = itemObject.name;
            string[] itemNameParts = itemName.Split('_');

            return Convert.ToInt32(itemNameParts[1]);
        }
    }
}