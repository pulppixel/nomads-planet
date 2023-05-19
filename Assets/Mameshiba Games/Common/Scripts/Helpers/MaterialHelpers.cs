using UnityEngine;

namespace MameshibaGames.Common.Helpers
{
    public static class MaterialHelpers
    {
        public static void GetTextureIfHasProperty(this Material material, int nameID, ref Texture saveTexture)
        {
            if (material.HasProperty(nameID))
                saveTexture = material.GetTexture(nameID);
        }

        public static void SetTextureIfHasProperty(this Material material, int nameID, Texture value)
        {
            if (material.HasProperty(nameID))
                material.SetTexture(nameID, value);
        }
        
        public static void GetColorIfHasProperty(this Material material, int nameID, ref Color saveColor)
        {
            if (material.HasProperty(nameID))
                saveColor = material.GetColor(nameID);
        }

        public static void SetColorIfHasProperty(this Material material, int nameID, Color value)
        {
            if (material.HasProperty(nameID))
                material.SetColor(nameID, value);
        }
    }
}