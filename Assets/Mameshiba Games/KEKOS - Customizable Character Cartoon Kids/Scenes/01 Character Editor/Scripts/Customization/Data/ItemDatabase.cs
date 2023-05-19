using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using Object = UnityEngine.Object;
using System.IO;
using UnityEditor;
#endif

namespace MameshibaGames.Kekos.CharacterEditorScene.Customization
{
      [CreateAssetMenu(fileName = "Item Database", menuName = "Mameshiba Games/Kekos/Item Database", order = 5)]
      public class ItemDatabase : ScriptableObject
      {
            public List<ItemInfo> items = new List<ItemInfo>();

            public IEnumerable<ItemInfo> GetMaterialsStartsWith(string prefix)
            {
                  return items.Where(x => x.primaryMaterial.name.StartsWith(prefix));
            }
            
            public List<ItemInfo> GetSpriteNameStartsWith(string prefix)
            {
                  return items.Where(x => x.sprite.name.StartsWith(prefix)).ToList();
            }

            [Serializable]
            public class ItemInfo
            {
                  public Material primaryMaterial;
                  public Sprite sprite;
                  public List<Color> iconColors;
            }
      }
      
      #if UNITY_EDITOR
      [CustomEditor(typeof(ItemDatabase))]
      public class ItemDatabaseEditor : Editor
      {
            public override void OnInspectorGUI()
            {
                  ItemDatabase targetPlayer = (ItemDatabase)target;
                  
                  GUILayout.Space(5);
                  
                  if (GUILayout.Button("Clean"))
                  {
                        targetPlayer.items.Clear();
                        EditorUtility.SetDirty(targetPlayer);
                  }

                  if (GUILayout.Button("Get Files"))
                  {
                        targetPlayer.items.Clear();
                        string assetPath = AssetDatabase.GetAssetPath(target);
                        assetPath = assetPath.Replace(targetPlayer.name+".asset", "");
                        assetPath = assetPath.Replace("Assets", "");
                        Material[] materials = GetAtPath<Material>(assetPath);
                        Sprite[] sprite = GetAtPath<Sprite>(assetPath);
                        for (int index = 0; index < materials.Length; index++)
                        {
                              Sprite newSprite = sprite.First(x => x.name == materials[index].name);
                              SetTextureImporterFormat(newSprite.texture, true);
                              Texture2D newTexture = TextureFromSprite(newSprite);
                              SetTextureImporterFormat(newSprite.texture, false);
                              ColorThief.ColorThief palette = new ColorThief.ColorThief();
                              List<ColorThief.QuantizedColor> colors = palette.GetPalette(newTexture, 3, 5);
                              colors = colors.OrderByDescending(x => x.Population).ToList();
                              
                              targetPlayer.items.Add(new ItemDatabase.ItemInfo()
                              {
                                    primaryMaterial = materials[index],
                                    iconColors = new List<Color> { colors[0].UnityColor, colors[1].UnityColor, colors[2].UnityColor },
                                    sprite = sprite.FirstOrDefault(x => x.name == materials[index].name),
                              });
                        }

                        EditorUtility.SetDirty(targetPlayer);
                  }

                  foreach (ItemDatabase.ItemInfo targetPlayerItem in targetPlayer.items)
                  {
                        GUILayout.Space(40);
                        GUILayout.BeginVertical();
                        if (targetPlayerItem.primaryMaterial != null)
                        {
                              string[] parts = targetPlayerItem.primaryMaterial.name.Split('_');
                              GUIStyle bold = new GUIStyle(EditorStyles.boldLabel)
                              {
                                    alignment = TextAnchor.MiddleCenter
                              };
                              bold.fontSize = (int)(bold.fontSize * 1.2f);
                              if (parts.Length > 2)
                                    GUILayout.Label(parts[2] + " " + parts[3]+ " " + (parts.Length > 4 ? parts[4] : ""), bold);
                              else
                                    GUILayout.Label(targetPlayerItem.primaryMaterial.name, bold);
                        }
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        Texture2D materialPreview = AssetPreview.GetAssetPreview(targetPlayerItem.primaryMaterial);
                        
                        Texture2D spritePreview =AssetPreview.GetAssetPreview(targetPlayerItem.sprite);
                        
                        EditorGUILayout.ObjectField(materialPreview, typeof(Texture2D), false, GUILayout.Width(70), GUILayout.Height(70));
                        GUILayout.Space(30);
                        EditorGUILayout.ObjectField(spritePreview, typeof(Texture2D), false, GUILayout.Width(70), GUILayout.Height(70));
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(10);
                        if (targetPlayerItem.iconColors.Count == 0)
                              targetPlayerItem.iconColors.Add(Color.white);

                        GUI.enabled = targetPlayerItem.iconColors.Count > 1;
                        if (GUILayout.Button("-",  GUILayout.Width(40)))
                        {
                              if (targetPlayerItem.iconColors.Count == 1)
                                    return;
                              targetPlayerItem.iconColors.RemoveAt(targetPlayerItem.iconColors.Count - 1);
                              EditorUtility.SetDirty(target);
                        }
                        GUI.enabled = true;
                        
                        EditorGUI.BeginChangeCheck();
                        
                        targetPlayerItem.iconColors[0] = EditorGUILayout.ColorField(targetPlayerItem.iconColors[0]);
                        if (targetPlayerItem.iconColors.Count >= 2)
                              targetPlayerItem.iconColors[1] = EditorGUILayout.ColorField(targetPlayerItem.iconColors[1]);
                        if (targetPlayerItem.iconColors.Count >= 3)
                              targetPlayerItem.iconColors[2] = EditorGUILayout.ColorField(targetPlayerItem.iconColors[2]);
                        
                        if (EditorGUI.EndChangeCheck())
                              EditorUtility.SetDirty(target);
                        
                        GUI.enabled = targetPlayerItem.iconColors.Count < 3;
                        if (GUILayout.Button("+",  GUILayout.Width(40)))
                        {
                              if (targetPlayerItem.iconColors.Count == 3)
                                    return;
                              targetPlayerItem.iconColors.Add(Color.white);
                              EditorUtility.SetDirty(target);
                        }
                        GUI.enabled = true;

                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                  }
                  
                  //DrawDefaultInspector ();
            }
            
            public static void SetTextureImporterFormat( Texture2D texture, bool isReadable)
            {
                  if ( null == texture ) return;

                  string assetPath = AssetDatabase.GetAssetPath( texture );
                  TextureImporter tImporter = AssetImporter.GetAtPath( assetPath ) as TextureImporter;
                  if ( tImporter != null )
                  {
                        //tImporter.textureType = TextureImporterType.Sprite;

                        tImporter.isReadable = isReadable;

                        AssetDatabase.ImportAsset( assetPath );
                        AssetDatabase.Refresh();
                  }
            }

            private static Texture2D TextureFromSprite(Sprite sprite)
            {
                  Texture2D newText = new Texture2D(sprite.texture.width,sprite.texture.height);
                  Color[] newColors = sprite.texture.GetPixels();
                  newText.SetPixels(newColors);
                  newText.Apply();
                  return newText;
            }

            private static T[] GetAtPath<T> (string path) {
       
                  ArrayList al = new ArrayList();
                  string [] fileEntries = Directory.GetFiles(Application.dataPath+"/"+path);
                  foreach(string fileName in fileEntries)
                  {
                        int index = fileName.LastIndexOf("/", StringComparison.Ordinal);
                        string localPath = "Assets/" + path;
           
                        if (index > 0)
                              localPath += fileName.Substring(index);
               
                        Object t = AssetDatabase.LoadAssetAtPath(localPath, typeof(T));
 
                        if(t != null)
                              al.Add(t);
                  }
                  T[] result = new T[al.Count];
                  for(int i=0;i<al.Count;i++)
                        result[i] = (T)al[i];
           
                  return result;
            }
      }
      #endif
}