using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MameshibaGames.Kekos.CharacterEditorScene.Data
{
    [CreateAssetMenu(fileName = "Face Expression Database", menuName = "Mameshiba Games/Kekos/Face Expression Database",
        order = 21)]
    public class FaceExpressionDatabase : ScriptableObject
    {
        public List<FaceExpressionInfo> faceExpressionInfo;

        [Serializable]
        public class FaceExpressionInfo
        {
            public string blendshapeName;
            public Sprite blendShapeIcon;
            public List<BlendshapeValues> blendshapeValues;
        }

        [Serializable]
        public class BlendshapeValues
        {
            public string blendshapeName;

            [Range(0, 100)]
            public float value;
        }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(FaceExpressionDatabase))]
    public class FaceExpressionDatabaseEditor : Editor
    {
        private string _newGestureName;
        private const string _FaceCharacteristicPrefix = "FACE_";

        public override void OnInspectorGUI()
        {
            FaceExpressionDatabase targetPlayer = (FaceExpressionDatabase)target;

            DrawDefaultInspector();

            GUILayout.Space(5);

            _newGestureName = GUILayout.TextField(_newGestureName);
            if (GUILayout.Button("Capture New Gesture"))
            {
                GameObject headObject = GameObject.Find("HEAD");
                SkinnedMeshRenderer skinnedMeshRenderer = headObject.GetComponent<SkinnedMeshRenderer>();

                FaceExpressionDatabase.FaceExpressionInfo newFaceExpressionInfo = new FaceExpressionDatabase.FaceExpressionInfo
                {
                    blendshapeName = _newGestureName,
                };

                List<FaceExpressionDatabase.BlendshapeValues> newBlendshapeValues =
                    new List<FaceExpressionDatabase.BlendshapeValues>();

                for (int i = 0; i < skinnedMeshRenderer.sharedMesh.blendShapeCount; i++)
                {
                    string blendshapeName = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(i);
                    float blendShapeWeight = skinnedMeshRenderer.GetBlendShapeWeight(i);
                    if (blendshapeName.StartsWith(_FaceCharacteristicPrefix) && blendShapeWeight > 0)
                    {
                        newBlendshapeValues.Add(new FaceExpressionDatabase.BlendshapeValues()
                        {
                            blendshapeName = blendshapeName,
                            value = blendShapeWeight
                        });
                    }
                }

                newFaceExpressionInfo.blendshapeValues = newBlendshapeValues;

                targetPlayer.faceExpressionInfo.Add(newFaceExpressionInfo);
            }
        }
    }
    #endif
}