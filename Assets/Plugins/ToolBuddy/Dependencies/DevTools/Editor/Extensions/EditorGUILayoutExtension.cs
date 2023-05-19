// =====================================================================
// Copyright 2013-2023 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using System;
using UnityEditor;
using UnityEngine;

namespace FluffyUnderware.DevToolsEditor.Extensions
{
    /// <summary>
    /// EditorGUILayout extension methods that mirror some of its methods, but handles properly exceptions thrown in between the Begin and End calls
    /// </summary>
    public static class EditorGUILayoutExtension
    {
        public static void Horizontal(Action action)
        {
            EditorGUILayout.BeginHorizontal();
            try
            {
                action();
            }
            finally
            {
                EditorGUILayout.EndHorizontal();
            }
        }

        public static void Vertical(Action action, GUIStyle style)
        {
            EditorGUILayout.BeginVertical(style);
            try
            {
                action();
            }
            finally
            {
                EditorGUILayout.EndVertical();
            }
        }

        public static Vector2 ScrollView(Action action, Vector2 scrollPosition, params GUILayoutOption[] layoutOptions)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(
                scrollPosition,
                layoutOptions
            );
            try
            {
                action();
            }
            finally
            {
                EditorGUILayout.EndScrollView();
            }

            return scrollPosition;
        }

        public static void FadeGroup(Action<bool> action, float value)
        {
            bool visible = EditorGUILayout.BeginFadeGroup(value);
            try
            {
                action(visible);
            }
            finally
            {
                EditorGUILayout.EndFadeGroup();
            }
        }
    }
}