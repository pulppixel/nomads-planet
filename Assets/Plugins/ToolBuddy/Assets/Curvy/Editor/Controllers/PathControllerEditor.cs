// =====================================================================
// Copyright 2013-2023 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using FluffyUnderware.Curvy.Controllers;
using UnityEditor;

namespace FluffyUnderware.CurvyEditor.Controllers
{
    [CanEditMultipleObjects]
    [CustomEditor(
        typeof(PathController),
        true
    )]
    public class PathControllerEditor : CurvyControllerEditor<PathController> { }
}