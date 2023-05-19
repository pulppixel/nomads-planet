// =====================================================================
// Copyright 2013-2023 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using FluffyUnderware.Curvy.Generator.Modules;
using UnityEditor;
using UnityEngine;

namespace FluffyUnderware.CurvyEditor.Generator.Modules
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CreatePathLineRenderer))]
    public class CreatePathLineRendererEditor : CGModuleEditor<CreatePathLineRenderer>
    {
        protected override void OnCustomInspectorGUIBefore()
        {
            base.OnCustomInspectorGUIBefore();
            EditorGUILayout.HelpBox(
                "Please edit parameters in inspector!",
                MessageType.Info
            );
            if (GUILayout.Button("Select Inspector"))
                Selection.activeGameObject = Target.gameObject;
        }
    }
}