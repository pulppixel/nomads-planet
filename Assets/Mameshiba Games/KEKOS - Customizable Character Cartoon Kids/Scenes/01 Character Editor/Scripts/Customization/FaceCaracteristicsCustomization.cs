using System.Collections.Generic;
using System.Linq;
using MameshibaGames.Common.Helpers;
using MameshibaGames.Kekos.CharacterEditorScene.Customization;
using UnityEngine;
using UnityEngine.UI;

public class FaceCaracteristicsCustomization : MonoBehaviour
{
    [SerializeField]
    private Slider[] sliders;

    [SerializeField]
    private string[] blendName;

    [SerializeField]
    private Button randomizeButton;

    private SkinnedMeshRenderer _skinRenderer;
    private Dictionary<string, float> _currentValues;

    public void Init(GameObject character)
    {
        _currentValues = new Dictionary<string, float>();

        _skinRenderer = character.transform.RecursiveFindChild("HEAD").GetComponent<SkinnedMeshRenderer>();
        
        for (int i = 0; i < sliders.Length; i++)
        {
            Slider slider = sliders[i];
            int i1 = i;
            _currentValues.Add(blendName[i1], 0);
            slider.onValueChanged.AddListener(x => ChangeCharacteristic(blendName[i1], x));
        }
        
        randomizeButton.onClick.AddListener(Randomize);
    }

    private void ChangeCharacteristic(string characteristicName, float newValue)
    {
        for (int i = 0; i < _skinRenderer.sharedMesh.blendShapeCount; i++)
        {
            if (_skinRenderer.sharedMesh.GetBlendShapeName(i) == characteristicName)
            {
                _skinRenderer.SetBlendShapeWeight(i, newValue * 100);
                _currentValues[characteristicName] = newValue;
                break;
            }
        }
    }

    public void Randomize()
    {
        foreach (Slider slider in sliders)
        {
            slider.value = Random.Range(0f, 1f);
        }
    }
    
    public List<SaveItemInfo> GetSaveItemInfo()
    {
        return _currentValues.Select(faceCharacteristic =>
                new SaveItemInfo(faceCharacteristic.Key, sliderValueSelected: faceCharacteristic.Value))
            .ToList();
    }
        
    public void ChangeWithSaveItemInfo(List<SaveItemInfo> faceInfo)
    {
        foreach (SaveItemInfo saveItemInfo in faceInfo)
        {
            int index = blendName.IndexOf(x => x == saveItemInfo.itemKey);
            sliders[index].value = saveItemInfo.sliderValueSelected;
        }
    }
}