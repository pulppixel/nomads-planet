using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MameshibaGames.Common.Helpers;
using MameshibaGames.Kekos.CharacterEditorScene.Customization;
using MameshibaGames.Kekos.CharacterEditorScene.Data;
using UnityEngine;

namespace MameshibaGames.Kekos.CharacterEditorScene.UI
{
    public class ExpressionsController : MonoBehaviour
    {
        private const string _FaceCharacteristicPrefix = "FACE_";

        [SerializeField]
        private GameObject kid;

        [SerializeField]
        private ItemButton buttonPrefab;

        [SerializeField]
        private Transform buttonsParent;

        [SerializeField]
        private FaceExpressionDatabase faceExpressionDatabase;

        [SerializeField]
        private GameObject contentObject;

        [SerializeField]
        private AnimationClip blinkAnimationClip;

        private readonly Dictionary<string, HashSet<SkinnedMeshRenderer>> _blendshapeNameToObjects =
            new Dictionary<string, HashSet<SkinnedMeshRenderer>>();

        private List<ItemButton> _itemButtons = new List<ItemButton>();
        
        private FaceExpressionDatabase.FaceExpressionInfo _lastFaceExpressionInfo;

        private Animation _headAnimation;

        public void Show()
        {
            _headAnimation.Stop();
            contentObject.SetActive(true);

            if (_lastFaceExpressionInfo == null) return;
            ChangeExpressionValue(_lastFaceExpressionInfo);
        }

        public void Hide()
        {
            _headAnimation.Play(blinkAnimationClip.name);
            
            RestartAllBlendshapes();
            contentObject.SetActive(false);
        }

        private void Awake()
        {
            _headAnimation = kid.transform.RecursiveFindChild("HEAD").gameObject.AddComponent<Animation>();
            _headAnimation.AddClip(blinkAnimationClip, blinkAnimationClip.name);
            _headAnimation.Play(blinkAnimationClip.name);

            Transform meshes = kid.transform.Find("Meshes");

            foreach (Transform child in meshes.transform)
            {
                if (child.TryGetComponent(out SkinnedMeshRenderer skinnedMeshRenderer))
                {
                    for (int i = 0; i < skinnedMeshRenderer.sharedMesh.blendShapeCount; i++)
                    {
                        string blendshapeName = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(i);
                        if (blendshapeName.StartsWith(_FaceCharacteristicPrefix))
                        {
                            if (!_blendshapeNameToObjects.ContainsKey(blendshapeName))
                            {
                                _blendshapeNameToObjects.Add(blendshapeName, new HashSet<SkinnedMeshRenderer>
                                {
                                    skinnedMeshRenderer
                                });
                            }
                            else
                            {
                                _blendshapeNameToObjects[blendshapeName].Add(skinnedMeshRenderer);
                            }
                        }
                    }
                }
            }

            RestartAllBlendshapes();

            foreach (FaceExpressionDatabase.FaceExpressionInfo faceExpressionInfo in faceExpressionDatabase.faceExpressionInfo)
            {
                if (_lastFaceExpressionInfo == null)
                    _lastFaceExpressionInfo = faceExpressionInfo;
                
                ItemButton newButton = Instantiate(buttonPrefab, buttonsParent);
               _itemButtons.Add(newButton);
               
                newButton.Init(0, faceExpressionInfo.blendShapeIcon, () =>
                    {
                        DisableAllButtonsButThis(newButton);
                    _lastFaceExpressionInfo = faceExpressionInfo;
                    ChangeExpressionValue(faceExpressionInfo);
                },
                    faceExpressionInfo.blendshapeName);
            }
            
            _itemButtons[0].ChangeToggleState(true, true);
        }
        
        private void DisableAllButtonsButThis(ItemButton itemFilter)
        {
            foreach (ItemButton button in _itemButtons)
                button.ChangeToggleState(button == itemFilter);
        }

        private void ChangeExpressionValue(FaceExpressionDatabase.FaceExpressionInfo faceExpressionInfo)
        {
            StopAllCoroutines();

            foreach (FaceExpressionDatabase.BlendshapeValues blendshapeValues in faceExpressionInfo.blendshapeValues)
            {
                ChangeMorpher(blendshapeValues.blendshapeName, blendshapeValues.value);
            }

            List<string> usedBlendshapeNames = faceExpressionInfo.blendshapeValues.Select(y => y.blendshapeName).ToList();

            foreach (KeyValuePair<string, HashSet<SkinnedMeshRenderer>> blendshapeNameToObject in _blendshapeNameToObjects)
            {
                string blendShapeName = blendshapeNameToObject.Key;
                if (usedBlendshapeNames.Contains(blendshapeNameToObject.Key)) continue;

                foreach (SkinnedMeshRenderer skinnedMeshRenderer in blendshapeNameToObject.Value)
                {
                    StartCoroutine(LerpBlendhape(skinnedMeshRenderer,
                        skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(blendShapeName), 0, 2));
                }
            }
        }

        private void ChangeMorpher(string blendShapeName, float newValue)
        {
            foreach (SkinnedMeshRenderer skinnedMeshRenderer in _blendshapeNameToObjects[blendShapeName])
            {
                StartCoroutine(LerpBlendhape(skinnedMeshRenderer,
                    skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(blendShapeName), newValue, 2));
            }
        }

        private void RestartAllBlendshapes()
        {
            StopAllCoroutines();

            foreach (KeyValuePair<string, HashSet<SkinnedMeshRenderer>> blendshapeNameToObject in _blendshapeNameToObjects)
            {
                string blendShapeName = blendshapeNameToObject.Key;
                foreach (SkinnedMeshRenderer skinnedMeshRenderer in blendshapeNameToObject.Value)
                {
                    StartCoroutine(LerpBlendhape(skinnedMeshRenderer,
                        skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(blendShapeName), 0, 2));
                }
            }
        }

        private IEnumerator LerpBlendhape(SkinnedMeshRenderer skinnedMeshRenderer, int index, float newValue, float duration)
        {
            float t = 0;

            while (t < duration)
            {
                t += Time.deltaTime;
                skinnedMeshRenderer.SetBlendShapeWeight(index,
                    Mathf.Lerp(skinnedMeshRenderer.GetBlendShapeWeight(index), newValue, (t / duration)));
                yield return null;
            }
        }
    }
}