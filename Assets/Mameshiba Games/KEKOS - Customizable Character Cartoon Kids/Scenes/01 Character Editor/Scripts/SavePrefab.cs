using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MameshibaGames.Common.Helpers;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MameshibaGames.Kekos.CharacterEditorScene
{
    public class SavePrefab : MonoBehaviour
    {
        [SerializeField]
        private GameObject kid;

        [SerializeField]
        private Button saveButton;

        [SerializeField]
        private Toggle generatePackageToggle;

        private void Awake()
        {
            #if UNITY_EDITOR
            saveButton.onClick.AddListener(SaveSelectionPanel);
            generatePackageToggle.isOn = EditorPrefs.GetBool("KEKOS_GeneratePackageToggle", true);
            #else
            gameObject.SetActive(false);
            #endif
        }

        private void OnDestroy()
        {
            #if UNITY_EDITOR
            EditorPrefs.SetBool("KEKOS_GeneratePackageToggle", generatePackageToggle.isOn);
            #endif
        }

        private void SaveSelectionPanel()
        {
            #if UNITY_EDITOR
            string path =
                EditorUtility.SaveFilePanelInProject("Save Prefab", "Name", "prefab", "Please enter a file name to save");
            if (path.Length != 0)
            {
                StartCoroutine(Save(path));
            }
            #endif
        }

        private IEnumerator Save(string fullPath)
        {
            #if UNITY_EDITOR
            EditorUtility.DisplayProgressBar("Saving Character", "Saving Prefab and materials...", 0.33f);

            string folderPath = fullPath.Substring(0, fullPath.LastIndexOf("/"));
            string fileName = fullPath.Remove(0, fullPath.LastIndexOf("/") + 1).Replace(".prefab", "");

            GameObject parentKidCopy = Instantiate(kid);
            GameObject kidCopy = parentKidCopy.transform.GetChild(0).gameObject;

            kidCopy.transform.position = new Vector3(0, -1000, 0);

            // Set neutral Pose
            Animator copyAnimator = kidCopy.GetComponent<Animator>();
            copyAnimator.PlayInFixedTime("SavePose");

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            // Remove Animator and add a clean one
            Avatar savedAvatar = copyAnimator.avatar;
            DestroyImmediate(copyAnimator);
            Animator newAnimator = kidCopy.AddComponent<Animator>();
            newAnimator.avatar = savedAvatar;

            Renderer[] kidRenderers = kidCopy.GetComponentsInChildren<Renderer>();
            Dictionary<Material, Material> materialsDictionary = new Dictionary<Material, Material>();

            foreach (Renderer renderers in kidRenderers)
            {
                if (renderers.GetComponent<TMP_Text>() != null) continue;
            
                List<Material> sharedMaterials = new List<Material>();
                renderers.GetSharedMaterials(sharedMaterials);

                for (int index = 0; index < sharedMaterials.Count; index++)
                {
                    Material material = sharedMaterials[index];

                    materialsDictionary.TryGetValue(material, out Material newMaterial);

                    if (newMaterial == null)
                    {
                        newMaterial = new Material(material);
                        AssetDatabase.CreateAsset(newMaterial,
                            $"{folderPath}/{fileName}_{material.name.ToLower().Replace(" (instance)", "")}.mat");
                        AssetDatabase.SaveAssets();
                        materialsDictionary.Add(material, newMaterial);
                    }

                    sharedMaterials[index] = newMaterial;
                }

                renderers.sharedMaterials = sharedMaterials.ToArray();
            }

            // Remove unused objects
            IEnumerable<Transform> disabledObjects = kidCopy.GetComponentsInChildren<Transform>(true).Where(x => !x.gameObject.activeSelf);
            foreach (Transform disabledObject in disabledObjects)
            {
                DestroyImmediate(disabledObject.gameObject);
            }

            // Remove unused bones
            HashSet<Transform> usedBoneNames = new HashSet<Transform>();
            foreach (Renderer renderer1 in kidRenderers.Where(x => x is SkinnedMeshRenderer))
            {
                SkinnedMeshRenderer skinnedRenderer = (SkinnedMeshRenderer)renderer1;
                usedBoneNames.UnionWith(skinnedRenderer.bones);
            }

            IEnumerable<Transform> disabledBones = kidCopy.transform.Find("Skeleton").GetComponentsInChildren<Transform>(true)
                .Where(x => !usedBoneNames.Contains(x));
            foreach (Transform disabledBone in disabledBones)
            {
                if (disabledBone == null) continue;
                if (disabledBone.gameObject.name == "Skeleton" || disabledBone.gameObject.name == "Bip001") continue;
                if (disabledBone.gameObject.name.StartsWith("HAI_") 
                    || disabledBone.gameObject.name.StartsWith("CAP_") 
                    || disabledBone.gameObject.name.StartsWith("FACP_")) continue;
                DestroyImmediate(disabledBone.gameObject);
            }

            kidCopy.transform.localPosition = Vector3.zero;
            kidCopy.transform.localRotation = Quaternion.identity;
            kidCopy.transform.localScale = Vector3.one;
            
            parentKidCopy.transform.localPosition = Vector3.zero;
            parentKidCopy.transform.localRotation = Quaternion.identity;
            parentKidCopy.transform.localScale = Vector3.one;
            
            // Remove face animation
            DestroyImmediate(kidCopy.transform.RecursiveFindChild("HEAD").GetComponent<Animation>());
            
            // RESET BLEND SHAPE
            foreach (Renderer renderer1 in kidRenderers.Where(x => x is SkinnedMeshRenderer))
            {
                SkinnedMeshRenderer skinnedRenderer = (SkinnedMeshRenderer)renderer1;
                for (int i = 0; i < skinnedRenderer.sharedMesh.blendShapeCount; i++)
                {
                    string blendshapeName = skinnedRenderer.sharedMesh.GetBlendShapeName(i);
                    if (blendshapeName.StartsWith("FACE_"))
                    {
                        skinnedRenderer.SetBlendShapeWeight(i, 0);
                    }
                }
            }
            
            PrefabUtility.SaveAsPrefabAsset(parentKidCopy, fullPath);

            Destroy(parentKidCopy);

            EditorUtility.DisplayProgressBar("Saving Character", "Saving package...", 0.66f);

            // Save package
            if (generatePackageToggle.isOn)
            {
                AssetDatabase.ExportPackage(fullPath, $"{folderPath}/{fileName}.unitypackage",
                    ExportPackageOptions.IncludeDependencies);
                AssetDatabase.ImportAsset($"{folderPath}/{fileName}.unitypackage");
            }

            EditorUtility.ClearProgressBar();
            #endif
            
            yield return null;

        }
    }
}