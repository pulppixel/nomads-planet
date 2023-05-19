using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MameshibaGames.Kekos.Editor.Tools
{
    public class FixPrefabBonesTool : EditorWindow
    {
        private const int _MessageTime = 2;
    
        private GameObject _originalFBX;
        private GameObject _test;
        private double _lastTime;
        private GUIStyle _textStyle;
        private GUIStyle _boxStyle;

        [MenuItem("Tools/Mameshiba Games/Kekos/Fix Prefabs Bones Tool")]
        private static void Init()
        {
            FixPrefabBonesTool window =
                (FixPrefabBonesTool)GetWindow(typeof(FixPrefabBonesTool), false, "Fix Prefabs Bones Tool");
            window.Show();
        }

        private void OnGUI()
        {
            FindOriginalFBX();
        
            EditorGUILayout.BeginVertical(new GUIStyle { padding = new RectOffset(30,30,30,30) });
            DrawDropAreaGUI();
            _originalFBX = EditorGUILayout.ObjectField("Original FBX", _originalFBX, typeof(GameObject), true) as
                GameObject;
            if (_lastTime + _MessageTime > EditorApplication.timeSinceStartup)
            {
                if (_textStyle == null)
                {
                    _textStyle = new GUIStyle("Label")
                    {
                        alignment = TextAnchor.MiddleCenter,
                        fontStyle = FontStyle.Bold,
                        fontSize = 20,
                        normal = new GUIStyleState
                        {
                            textColor = new Color(0.54f, 1f, 0.35f)
                        }
                    };
                }
            
                EditorGUILayout.Space(25);
                EditorGUILayout.LabelField("Done! Prefabs Fixed", _textStyle);
            }

            EditorGUILayout.EndVertical();
        }

        private void FindOriginalFBX()
        {
            if (_originalFBX != null) return;
        
            string[] guids = AssetDatabase.FindAssets("t:Model KekosCharacter");
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (asset != null)
                    _originalFBX = asset;
            }
        }

        private void DrawDropAreaGUI()
        {
            Event evt = Event.current;
       
            Rect dropArea = GUILayoutUtility.GetRect(0, 200, GUILayout.ExpandWidth(true));
            if (_boxStyle == null)
            {
                _boxStyle = new GUIStyle("Box")
                {
                    fontStyle = FontStyle.Bold,
                    normal = new GUIStyleState
                    {
                        textColor = new Color(1f, 0.58f, 0.9f)
                    },
                    hover = new GUIStyleState
                    {
                        textColor = new Color(1f, 0.58f, 0.9f)
                    }
                };
            }
            GUI.Box(dropArea, "Drop Prefabs to Fix Bones", _boxStyle);
       
            switch(evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
           
                    if(!dropArea.Contains (evt.mousePosition)){
                        break;
                    }
           
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
           
                    if(evt.type == EventType.DragPerform){
                        DragAndDrop.AcceptDrag();
               
                        foreach(Object o in DragAndDrop.objectReferences)
                        {
                            GameObject go = (GameObject)o;
                            if(!go)
                                continue;
                   
                            FixGameObject(go);
                        }
                    }
           
                    Event.current.Use ();
                    break;
            }
        }

        private void FixGameObject(GameObject gameObject)
        {
            if (_originalFBX == null) return;

            SkinnedMeshRenderer[] originalSkinnedMeshRenderers =
                _originalFBX.GetComponentsInChildren<SkinnedMeshRenderer>();
            Transform[] allObjectChilds = gameObject.transform.GetComponentsInChildren<Transform>();
        
            foreach (SkinnedMeshRenderer skinnedMeshRenderer in gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                SkinnedMeshRenderer originalSkinnedMeshRenderer =
                    originalSkinnedMeshRenderers.FirstOrDefault(x => x.name == skinnedMeshRenderer.name);
            
                if (originalSkinnedMeshRenderer == null) continue;
            
                Transform[] bones = new Transform[originalSkinnedMeshRenderer.bones.Length];
                for (int index = 0; index < originalSkinnedMeshRenderer.bones.Length; index++)
                {
                    Transform otherMeshBone = originalSkinnedMeshRenderer.bones[index];
                    bones[index] = allObjectChilds.First(x => x.name == otherMeshBone.name);
                }

                skinnedMeshRenderer.bones = bones;
            }

            _lastTime = EditorApplication.timeSinceStartup;
        }
    }
}
