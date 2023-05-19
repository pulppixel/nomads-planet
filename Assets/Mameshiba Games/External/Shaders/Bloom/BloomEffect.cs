using System;
using UnityEngine;

namespace MameshibaGames.External.Shaders
{
    [ExecuteInEditMode]
    [ImageEffectAllowedInSceneView]
    public class BloomEffect : MonoBehaviour
    {
        private const int _BoxDownPrefilterPass = 0;
        private const int _BoxDownPass = 1;
        private const int _BoxUpPass = 2;
        private const int _ApplyBloomPass = 3;
        private const int _DebugBloomPass = 4;

        public Shader bloomShader;

        [Range(0, 10)]
        public float intensity = 1;

        [Range(1, 16)]
        public int iterations = 4;

        [Range(0, 10)]
        public float threshold = 1;

        [Range(0, 1)]
        public float softThreshold = 0.5f;

        public bool debug;

        private readonly RenderTexture[] _textures = new RenderTexture[16];

        [NonSerialized]
        private Material _bloom;

        private static readonly int _Filter = Shader.PropertyToID("_Filter");
        private static readonly int _Intensity = Shader.PropertyToID("_Intensity");
        private static readonly int _SourceTex = Shader.PropertyToID("_SourceTex");

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (_bloom == null)
            {
                _bloom = new Material(bloomShader)
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
            }

            float knee = threshold * softThreshold;
            Vector4 filter;
            filter.x = threshold;
            filter.y = filter.x - knee;
            filter.z = 2f * knee;
            filter.w = 0.25f / (knee + 0.00001f);
            _bloom.SetVector(_Filter, filter);
            _bloom.SetFloat(_Intensity, Mathf.GammaToLinearSpace(intensity));

            int width = source.width / 2;
            int height = source.height / 2;
            RenderTextureFormat format = source.format;

            RenderTexture currentDestination = _textures[0] = RenderTexture.GetTemporary(width, height, 0, format);
            Graphics.Blit(source, currentDestination, _bloom, _BoxDownPrefilterPass);
            RenderTexture currentSource = currentDestination;

            int i = 1;
            for (; i < iterations; i++)
            {
                width /= 2;
                height /= 2;
                if (height < 2) break;

                currentDestination = _textures[i] = RenderTexture.GetTemporary(width, height, 0, format);
                Graphics.Blit(currentSource, currentDestination, _bloom, _BoxDownPass);
                currentSource = currentDestination;
            }

            for (i -= 2; i >= 0; i--)
            {
                currentDestination = _textures[i];
                _textures[i] = null;
                Graphics.Blit(currentSource, currentDestination, _bloom, _BoxUpPass);
                RenderTexture.ReleaseTemporary(currentSource);
                currentSource = currentDestination;
            }

            if (debug)
            {
                Graphics.Blit(currentSource, destination, _bloom, _DebugBloomPass);
            }
            else
            {
                _bloom.SetTexture(_SourceTex, source);
                Graphics.Blit(currentSource, destination, _bloom, _ApplyBloomPass);
            }

            RenderTexture.ReleaseTemporary(currentSource);
        }
    }
}