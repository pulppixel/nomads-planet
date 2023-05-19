using TMPro;
using UnityEngine;

namespace MameshibaGames.Kekos.RuntimeExampleScene.Spawner
{
    public static class ColorSetter
    {
        private static readonly int _Color = Shader.PropertyToID("_Color");
        private static readonly int _URPColor = Shader.PropertyToID("_BaseColor");

        public static void SetColor(this Renderer renderer, Color newColor)
        {
            if (renderer.materials.Length == 1)
            {
                if (renderer.materials[0].HasProperty(_Color))
                {
                    renderer.materials[0].SetColor(_Color, newColor);
                }

                if (renderer.materials[0].HasProperty(_URPColor))
                {
                    renderer.materials[0].SetColor(_URPColor, newColor);
                }
            }
            else if (renderer.materials.Length > 1)
            {
                if (renderer.materials[1].HasProperty(_Color))
                {
                    renderer.materials[1].SetColor(_Color, newColor);
                }

                if (renderer.materials[1].HasProperty(_URPColor))
                {
                    renderer.materials[1].SetColor(_URPColor, newColor);
                }
            }

            TMP_Text itemText = renderer.gameObject.GetComponentInChildren<TMP_Text>();
            if (itemText != null)
            {
                newColor.a = 1;
                itemText.color = newColor;
            }
        }
    }
}