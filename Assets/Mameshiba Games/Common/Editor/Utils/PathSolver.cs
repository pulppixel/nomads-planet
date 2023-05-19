using UnityEditor;
using UnityEngine;

namespace MameshibaGames.Common.Editor.Utils
{
    public static class PathSolver
    {
        public static string GetScriptPath(this ScriptableObject scriptableObject)
        {
            MonoScript monoScript = MonoScript.FromScriptableObject( scriptableObject );
            return GetPathByMonoScript(monoScript);
        }

        public static string GetScriptPath(this MonoBehaviour monoBehaviour)
        {
            MonoScript monoScript = MonoScript.FromMonoBehaviour( monoBehaviour );
            return GetPathByMonoScript(monoScript);
        }

        private static string GetPathByMonoScript(MonoScript monoScript)
        {
            string path = AssetDatabase.GetAssetPath(monoScript);
            path = path.Replace($"/{monoScript.name}.cs", "");
            return path;
        }
    }
}