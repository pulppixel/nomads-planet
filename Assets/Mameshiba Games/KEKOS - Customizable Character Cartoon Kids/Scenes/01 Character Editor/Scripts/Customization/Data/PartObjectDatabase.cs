using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MameshibaGames.Kekos.CharacterEditorScene.Customization
{
    [CreateAssetMenu(fileName = "Part Object Database", menuName = "Mameshiba Games/Kekos/Part Object Database", order = 2)]
    public class PartObjectDatabase : PartDatabase
    {
        public List<ObjectList> itemObjects = new List<ObjectList>();

        [Serializable]
        public class ObjectList
        {
            public List<GameObject> itemSubobjects = new List<GameObject>();
        }
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(PartObjectDatabase))]
    public class PartObjectDatabaseEditor : PartDatabaseEditor
    {
        // ReSharper disable once RedundantOverriddenMember
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
    #endif
}