using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace MameshibaGames.Kekos.DemoScene
{
    [CreateAssetMenu(fileName = "ClonePose", menuName = "Mameshiba Games/Kekos/ClonePose", order = 100)]
    public class ClonePose : ScriptableObject
    {
        public List<ObjectData> objectsData = new List<ObjectData>();
        public List<BlendShapeData> blendShapeData = new List<BlendShapeData>();

        [Serializable]
        public class ObjectData
        {
            public string name;
            public Vector3 localPosition;
            public Quaternion localRotation;
            public Vector3 localScale;
        }

        [Serializable]
        public class BlendShapeData
        {
            public string blendShapeName;
            public float blendShapeValue;
        }

        public void SavePose(GameObject objectToSave)
        {
            objectsData.Clear();

            foreach (Transform componentsInChild in objectToSave.GetComponentsInChildren<Transform>())
            {
                objectsData.Add(new ObjectData
                {
                    name = componentsInChild.name,
                    localPosition = componentsInChild.localPosition,
                    localRotation = componentsInChild.localRotation,
                    localScale = componentsInChild.localScale
                });
            }
        }

        public void SaveFaceExpression(GameObject objectToSave)
        {
            blendShapeData.Clear();
            
            foreach (SkinnedMeshRenderer skinnedMeshRenderer in objectToSave.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                if (skinnedMeshRenderer.gameObject.name != "HEAD") continue;
                
                for (int i = 0; i < skinnedMeshRenderer.sharedMesh.blendShapeCount; i++)
                {
                    string blendShapeName = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(i);

                    if (blendShapeName.Contains("FACE_"))
                    {
                        if (blendShapeData.Exists(x => x.blendShapeName == blendShapeName)) continue;
                        
                        blendShapeData.Add(new BlendShapeData
                        {
                            blendShapeName = blendShapeName,
                            blendShapeValue = skinnedMeshRenderer.GetBlendShapeWeight(i)
                        });
                    }
                }
            }
        }

        public void ApplyPose(GameObject objectToApply)
        {
            foreach (Transform componentsInChild in objectToApply.GetComponentsInChildren<Transform>())
            {
                ObjectData data = objectsData.FirstOrDefault(x => x.name == componentsInChild.name);
                if (data == null) continue;

                componentsInChild.localPosition = data.localPosition;
                componentsInChild.localRotation = data.localRotation;
                componentsInChild.localScale = data.localScale;

                #if UNITY_EDITOR
                EditorUtility.SetDirty(componentsInChild);
                #endif
            }
        }

        public void ApplyFaceExpression(GameObject objectToApply)
        {
            foreach (SkinnedMeshRenderer skinnedMeshRenderer in objectToApply.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                for (int i = 0; i < skinnedMeshRenderer.sharedMesh.blendShapeCount; i++)
                {
                    string blendShapeName = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(i);

                    if (blendShapeName.Contains("FACE_"))
                    {
                        BlendShapeData blendShape = blendShapeData.FirstOrDefault(x => x.blendShapeName == blendShapeName);
                        if (blendShape == null) continue;

                        skinnedMeshRenderer.SetBlendShapeWeight(i, blendShape.blendShapeValue);
                    }
                }
                
                #if UNITY_EDITOR
                EditorUtility.SetDirty(skinnedMeshRenderer);
                #endif
            }
        }

        #if UNITY_EDITOR

        [CustomEditor(typeof(ClonePose))]
        public class ClonePoseEditor : Editor
        {

            public override void OnInspectorGUI()
            {
                ClonePose clonePose = (ClonePose)target;

                DrawDefaultInspector();

                if (GUILayout.Button("Save Pose"))
                    clonePose.SavePose(Selection.activeGameObject);

                if (GUILayout.Button("Apply Pose"))
                    clonePose.ApplyPose(Selection.activeGameObject);
                
                if (GUILayout.Button("Save Face Expression"))
                    clonePose.SaveFaceExpression(Selection.activeGameObject);

                if (GUILayout.Button("Apply Face Expression"))
                    clonePose.ApplyFaceExpression(Selection.activeGameObject);
            }
        }

        #endif
    }
}