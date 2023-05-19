using System;
using GameCreator.Runtime.Variables;
using UnityEditor;
using UnityEngine;

namespace GameCreator.Editor.Variables
{
    public class GlobalVariablesPostProcessor : AssetPostprocessor
    {
        public static event Action EventRefresh;
        
        // PROCESSORS: ----------------------------------------------------------------------------

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            ScheduleRefresh();
        }
        
        private static void OnPostprocessAllAssets(
            string[] importedAssets, 
            string[] deletedAssets, 
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (importedAssets.Length == 0 && deletedAssets.Length == 0) return;
            ScheduleRefresh();
        }

        public static void ScheduleRefresh()
        {
            EditorApplication.delayCall -= RefreshVariables;
            EditorApplication.delayCall += RefreshVariables;
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static void RefreshVariables()
        {
            string[] varSettingsGuids = AssetDatabase.FindAssets($"t:{nameof(VariablesSettings)}");
            if (varSettingsGuids.Length == 0) return;

            string varSettingsPath = AssetDatabase.GUIDToAssetPath(varSettingsGuids[0]);
            
            VariablesSettings varSettings = AssetDatabase.LoadAssetAtPath<VariablesSettings>(varSettingsPath);
            if (varSettings == null) return;

            string[] nameVariablesGuids = AssetDatabase.FindAssets($"t:{nameof(GlobalNameVariables)}");
            GlobalNameVariables[] nameVariables = new GlobalNameVariables[nameVariablesGuids.Length];
            
            for (int i = 0; i < nameVariablesGuids.Length; i++)
            {
                string nameVariablesGuid = nameVariablesGuids[i];
                string nameVariablesPath = AssetDatabase.GUIDToAssetPath(nameVariablesGuid);
                nameVariables[i] = AssetDatabase.LoadAssetAtPath<GlobalNameVariables>(nameVariablesPath);
            }
            
            string[] listVariablesGuids = AssetDatabase.FindAssets($"t:{nameof(GlobalListVariables)}");
            GlobalListVariables[] listVariables = new GlobalListVariables[listVariablesGuids.Length];

            for (int i = 0; i < listVariablesGuids.Length; i++)
            {
                string listVariablesGuid = listVariablesGuids[i];
                string listVariablesPath = AssetDatabase.GUIDToAssetPath(listVariablesGuid);
                listVariables[i] = AssetDatabase.LoadAssetAtPath<GlobalListVariables>(listVariablesPath);
            }

            varSettings.Get().Variables.Editor_Set(nameVariables, listVariables);
            EventRefresh?.Invoke();
        }
    }
}
