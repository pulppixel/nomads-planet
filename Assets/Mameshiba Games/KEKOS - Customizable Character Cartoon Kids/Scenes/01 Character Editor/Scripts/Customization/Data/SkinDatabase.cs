using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MameshibaGames.Kekos.CharacterEditorScene.Customization
{
    [CreateAssetMenu(fileName = "Skin Database", menuName = "Mameshiba Games/Kekos/Skin Database", order = 1)]
    public class SkinDatabase : ScriptableObject
    {
        public SkinBaseData[] skinBaseData = Array.Empty<SkinBaseData>();
        public SkinDetailData[] skinDetailData = Array.Empty<SkinDetailData>();
    }
    
    [Serializable]
    public class SkinBaseData
    {
        public Texture baseColor;
        public Sprite sprite;
    }
    
    [Serializable]
    public class SkinDetailData
    {
        public Texture detailMask;
        public Texture detailBase;
        public Texture emissive;
        [ColorUsage(false, true)]
        public Color emissiveColor;
        public Sprite sprite;
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(SkinDatabase))]
    public class SkinDatabaseEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            SkinDatabase targetPlayer = (SkinDatabase)target;
    
            DrawDefaultInspector();
    
            GUILayout.Space(30);
              
            GUILayout.BeginVertical();
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            for (int i = 0; i < targetPlayer.skinBaseData.Length; i++)
            {
                SkinBaseData skinData = targetPlayer.skinBaseData[i];
                if (skinData == null) continue;
                if (skinData.sprite == null) continue;

                EditorGUILayout.ObjectField(skinData.sprite, typeof(Texture2D), false, GUILayout.Width(100),
                    GUILayout.Height(100));

                if ((i + 1) % 3 == 0)
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                }
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(30);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            for (int i = 0; i < targetPlayer.skinDetailData.Length; i++)
            {
                SkinDetailData skinDetailData = targetPlayer.skinDetailData[i];
                if (skinDetailData == null) continue;
                if (skinDetailData.sprite == null) continue;

                EditorGUILayout.ObjectField(skinDetailData.sprite, typeof(Texture2D), false, GUILayout.Width(100),
                    GUILayout.Height(100));

                if ((i + 1) % 3 == 0)
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                }
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
              
            GUILayout.EndVertical();
    
        }
    }
    #endif
}