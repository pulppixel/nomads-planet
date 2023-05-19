using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MameshibaGames.Kekos.CharacterEditorScene.Customization
{
    [CreateAssetMenu(fileName = "Part Database", menuName = "Mameshiba Games/Kekos/Part Database", order = 1)]
    public class PartDatabase : ScriptableObject
    {
        public ItemDatabase[] itemsDatabases = Array.Empty<ItemDatabase>();
    }
    
     #if UNITY_EDITOR
      [CustomEditor(typeof(PartDatabase))]
      public class PartDatabaseEditor : Editor
      {
          public override void OnInspectorGUI()
          {
              PartDatabase targetPlayer = (PartDatabase)target;

              DrawDefaultInspector();

              GUILayout.Space(30);
              
              GUILayout.BeginVertical();
              foreach (ItemDatabase targetPlayerItemsDatabase in targetPlayer.itemsDatabases)
              {
                  if (targetPlayerItemsDatabase == null) continue;
                  if (targetPlayerItemsDatabase.items.Count == 0) continue;
                  
                  GUILayout.BeginHorizontal();
                  GUILayout.FlexibleSpace();
                  EditorGUILayout.ObjectField(targetPlayerItemsDatabase.items[0].sprite, typeof(Texture2D), false, GUILayout.Width(150), GUILayout.Height(150));
                  GUILayout.FlexibleSpace();
                  GUILayout.EndHorizontal();
              }
              
              GUILayout.EndVertical();

          }
      }
      #endif
}