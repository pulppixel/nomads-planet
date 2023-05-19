using UnityEngine;

namespace MameshibaGames.Common.Helpers
{
    public static class TextureHelpers
    {
        public static Texture2D CreateByGradient(Gradient grad, int width = 32, int height = 1, bool withoutAlpha = true)
        {
            Texture2D gradTex = new Texture2D(width, height, TextureFormat.ARGB32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };
            float inv = 1f / (width - 1);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float t = x * inv;
                    Color col = grad.Evaluate(t);
                    if (withoutAlpha)
                        col.a = 1;
                    gradTex.SetPixel(x, y, col);
                }
            }

            gradTex.Apply();
            return gradTex;
        }
        
        public static Texture2D CreateByColor(Color color, int width = 32, int height = 1, bool withoutAlpha = true)
        {
            Texture2D gradTex = new Texture2D(width, height, TextureFormat.ARGB32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };
            
            Color col = color;
            if (withoutAlpha)
                col.a = 1;
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    gradTex.SetPixel(x, y, col);
                }
            }

            gradTex.Apply();
            return gradTex;
        }
    }
}