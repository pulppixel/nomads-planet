using System;
using MameshibaGames.Common.Data;
using MameshibaGames.Common.Helpers;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace MameshibaGames.Common.UI
{
    public class ColorSlider : Slider
    {
        [SerializeField]
        private RawImage colorImage;

        [SerializeField]
        private GradientData gradientData;

        [SerializeField]
        private Texture2D textureGradientData;
        
        [SerializeField]
        private Texture2D visibleTextureGradientData;

        [SerializeField] 
        public bool useAlpha = true;

        public ColorEvent onValueChangedColor = new ColorEvent();
        private Color _currentColor;

        [Serializable]
        public class ColorEvent : UnityEvent<Color> {}
        
        protected override void Awake()
        {
            base.Awake();

            colorImage.texture = visibleTextureGradientData != null
                ? visibleTextureGradientData
                : textureGradientData != null
                    ? textureGradientData
                    : TextureHelpers.CreateByGradient(gradientData.gradient);
        }

        protected override void Set(float input, bool sendCallback = true)
        {
            base.Set(input, sendCallback);

            _currentColor = textureGradientData != null
                ? textureGradientData.GetPixel((int)input.Remap(0, 1, 0, textureGradientData.width), 1)
                : gradientData.gradient.Evaluate(input);
            
            onValueChangedColor?.Invoke(_currentColor);
        }

        public Color GetColor() => _currentColor;

        public void Randomize()
        {
            value = Random.Range(0f, 1f);
        }
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(ColorSlider))]
    public class PartObjectDatabaseEditor : SliderEditor
    {
        private SerializedProperty _colorImage;
        private SerializedProperty _gradientData;
        private SerializedProperty _textureGradientData;
        private SerializedProperty _visibleTextureGradientData;
        private SerializedProperty _useAlpha;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            _colorImage = serializedObject.FindProperty("colorImage");
            _gradientData = serializedObject.FindProperty("gradientData");
            _textureGradientData = serializedObject.FindProperty("textureGradientData");
            _visibleTextureGradientData = serializedObject.FindProperty("visibleTextureGradientData");
            _useAlpha = serializedObject.FindProperty("useAlpha");
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            serializedObject.Update();
            EditorGUILayout.PropertyField(_colorImage);
            EditorGUILayout.PropertyField(_gradientData);
            EditorGUILayout.PropertyField(_textureGradientData);
            EditorGUILayout.PropertyField(_visibleTextureGradientData);
            EditorGUILayout.PropertyField(_useAlpha);
            serializedObject.ApplyModifiedProperties();
        }
    }
    #endif
}
