using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MameshibaGames.Kekos.CharacterEditorScene.Customization
{
    [CreateAssetMenu(fileName = "Eyes Database", menuName = "Mameshiba Games/Kekos/Eyes Database", order = 1)]
    public class EyesDatabase : ScriptableObject
    {
        public EyesData[] eyesData = Array.Empty<EyesData>();
    }
    
    [Serializable]
    public class EyesData
    {
        public Texture baseColor;
        public Texture light;
        public Sprite sprite;
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(EyesDatabase))]
    public class EyeDatabaseEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EyesDatabase targetPlayer = (EyesDatabase)target;
    
            DrawDefaultInspector();
    
            GUILayout.Space(30);
              
            GUILayout.BeginVertical();
            foreach (EyesData eyeData in targetPlayer.eyesData)
            {
                if (eyeData == null) continue;
                if (eyeData.sprite == null) continue;
                  
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                EditorGUILayout.ObjectField(eyeData.sprite, typeof(Texture2D), false, GUILayout.Width(150), GUILayout.Height(150));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
              
            GUILayout.EndVertical();
    
        }
    }
    #endif
}