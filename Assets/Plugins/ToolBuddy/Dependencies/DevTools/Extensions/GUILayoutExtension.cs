// =====================================================================
// Copyright 2013-2023 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using System;
using UnityEngine;

namespace FluffyUnderware.DevTools.Extensions
{
    /// <summary>
    /// GUILayout extension methods that mirror some of its methods, but handles properly exceptions thrown in between the Begin and End calls
    /// </summary>
    public static class GUILayoutExtension
    {
        public static void Area(Action action, Rect screenRectangle, GUIStyle skinBox)
        {
            GUILayout.BeginArea(
                screenRectangle,
                skinBox
            );
            try
            {
                action();
            }
            finally
            {
                GUILayout.EndArea();
            }
        }

        public static void Area(Action action, Rect screenRectangle)
        {
            GUILayout.BeginArea(screenRectangle);
            try
            {
                action();
            }
            finally
            {
                GUILayout.EndArea();
            }
        }

        public static void Horizontal(Action action, GUIStyle style)
        {
            GUILayout.BeginHorizontal(style);
            try
            {
                action();
            }
            finally
            {
                GUILayout.EndHorizontal();
            }
        }

        public static void Horizontal(Action action, params GUILayoutOption[] layoutOptions)
        {
            GUILayout.BeginHorizontal(layoutOptions);
            try
            {
                action();
            }
            finally
            {
                GUILayout.EndHorizontal();
            }
        }

        public static Vector2 ScrollView(Action action, Vector2 scrollPosition, params GUILayoutOption[] layoutOptions)
        {
            scrollPosition = GUILayout.BeginScrollView(
                scrollPosition,
                layoutOptions
            );
            try
            {
                action();
            }
            finally
            {
                GUILayout.EndScrollView();
            }

            return scrollPosition;
            ;
        }
    }
}