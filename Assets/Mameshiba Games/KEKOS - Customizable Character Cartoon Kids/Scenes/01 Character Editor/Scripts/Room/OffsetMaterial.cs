using UnityEngine;

namespace MameshibaGames.Kekos.CharacterEditorScene.Room
{
    public class OffsetMaterial : MonoBehaviour
    {
        public Vector2 offsetSpeed;
        public Vector2 tiling = Vector2.one;

        private MaterialPropertyBlock _propertyBlock;
        private Renderer _renderer;
        private Vector4 _tilingAndOffset;
        
        private static readonly int _MainTexSt = Shader.PropertyToID("_MainTex_ST");
        private static readonly int _URPMainTexSt = Shader.PropertyToID("_BaseMap_ST");
        private static readonly int _HDRPMainTexSt = Shader.PropertyToID("_EmissiveColorMap_ST");

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _propertyBlock = new MaterialPropertyBlock();
            _renderer.GetPropertyBlock(_propertyBlock);
            _tilingAndOffset = new Vector4(tiling.x, tiling.y, 0.1f, 0);
        }

        private void Update()
        {
            _tilingAndOffset.z += offsetSpeed.x * Time.deltaTime;
            _tilingAndOffset.w += offsetSpeed.y * Time.deltaTime;
            _propertyBlock.SetVector(_MainTexSt, _tilingAndOffset);
            _propertyBlock.SetVector(_URPMainTexSt, _tilingAndOffset);
            _propertyBlock.SetVector(_HDRPMainTexSt, _tilingAndOffset);
            _renderer.SetPropertyBlock(_propertyBlock);
        }
    }
}