using System;
using MameshibaGames.External.Shaders;
using MameshibaGames.Kekos.CharacterEditorScene.UI;
using UnityEngine;
using UnityEngine.Rendering;

namespace MameshibaGames.Kekos.CharacterEditorScene.Room
{
    public class LightsSwitcher : MonoBehaviour
    {
        [SerializeField]
        private BloomEffect bloomEffect;

        [Serializable]
        public class LightData
        {
            public MenuButton menuButton;
            public bool enableBloom;
            public Color environmentColor;
            public GameObject objectToActivate;
        }

        [SerializeField]
        private LightData[] lightsData;

        private void Awake()
        {
            RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;
        
            foreach (LightData lightData in lightsData)
                lightData.menuButton.button.onClick.AddListener(() => ChangeToLight(lightData));
        }

        private void ChangeToLight(LightData lightData)
        {
            if (bloomEffect != null) 
                bloomEffect.enabled = lightData.enableBloom;
        
            DisableAllObjects();
            lightData.objectToActivate.SetActive(true);

            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = lightData.environmentColor;
        }
        
        private void DisableAllObjects()
        {
            foreach (LightData lightData in lightsData)
                lightData.objectToActivate.SetActive(false);
        }
    }
}