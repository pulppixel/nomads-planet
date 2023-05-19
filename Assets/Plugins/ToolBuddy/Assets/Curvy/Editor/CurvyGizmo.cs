// =====================================================================
// Copyright 2013-2023 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using System;
using FluffyUnderware.Curvy;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace FluffyUnderware.CurvyEditor
{
    public static class CurvyGizmo
    {
        /// <summary>
        /// Displays a label next to a point. The relative position of the label compared to the point is defined by <paramref name="direction"/>
        /// </summary>
#if CURVY_SANITY_CHECKS_PRIVATE
        [UsedImplicitly]
        [Obsolete(
            "Do not call this method from a method not having the DrawGizmo attribute until the issue with Unity 2021.2 is fixed"
        )]
#endif
        public static void PointLabel(Vector3 pointPosition, string label, OrientationAxisEnum direction,
            float? handleSize = null, [CanBeNull] GUIStyle style = null)
        {
#if UNITY_2021_2_0 || UNITY_2021_2_1 || UNITY_2021_2_2 || UNITY_2021_2_3 || UNITY_2021_2_4 || UNITY_2021_2_5 || UNITY_2021_2_6 || UNITY_2021_2_7 || UNITY_2021_2_8 || UNITY_2021_2_9 || UNITY_2021_2_10 || UNITY_2021_2_11
            //workaround to this issue: https://issuetracker.unity3d.com/issues/handles-dot-label-does-not-appear-in-the-supposed-place
            //the issue seems to not happen when this method is called from a OnGui method.
            pointPosition = DTHandles.TranslateByPixel(
                pointPosition,
                -53,
                23
            );
#endif
            //ugly shit to bypass the joke that is style.alignment. Tried to bypass the issue by using style.CalcSize(new GUIContent(label)) to manually place the labels. No luck with that
            while (label.Length <= 5)
                label = $" {label} ";

            if (handleSize.HasValue == false)
                handleSize = HandleUtility.GetHandleSize(pointPosition);

            style = style ?? CurvyStyles.GizmoText;

            pointPosition -= Camera.current.transform.right * handleSize.Value * 0.1f;
            pointPosition += Camera.current.transform.up * handleSize.Value * 0.1f;
            Vector3 labelPosition;
            switch (direction)
            {
                case OrientationAxisEnum.Up:
                    //style.alignment = TextAnchor.LowerCenter;
                    labelPosition = pointPosition;
                    labelPosition += Camera.current.transform.up * handleSize.Value * 0.3f;
                    break;
                case OrientationAxisEnum.Down:
                    //style.alignment = TextAnchor.UpperCenter;
                    labelPosition = pointPosition;
                    labelPosition -= Camera.current.transform.up * handleSize.Value * 0.3f;
                    break;
                case OrientationAxisEnum.Right:
                    //style.alignment = TextAnchor.MiddleLeft;
                    labelPosition = pointPosition;
                    labelPosition += Camera.current.transform.right * handleSize.Value * 0.4f;
                    break;
                case OrientationAxisEnum.Left:
                    //style.alignment = TextAnchor.MiddleRight;
                    labelPosition = pointPosition;
                    labelPosition -= Camera.current.transform.right * handleSize.Value * 0.45f;
                    break;
                case OrientationAxisEnum.Forward:
                case OrientationAxisEnum.Backward:
                    //style.alignment = TextAnchor.MiddleCenter;
                    labelPosition = pointPosition;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(direction),
                        direction,
                        null
                    );
            }

            Handles.Label(
                labelPosition,
                label,
                style
            );
        }
    }
}