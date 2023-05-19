using MameshibaGames.Common.Editor.Utils;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace MameshibaGames.Common.Editor.Window
{
    public abstract class CustomEditorWindow : EditorWindow
    {
        protected VisualElement root;
        protected SerializedObject bindObject;

        private static bool _refreshing;
        
        private bool _isOpen;
        private bool _isFocused;
        private string[] _visualTreeAssetPaths;
        private string[] _stylesheetsPaths;

        protected static void Open<T>(string windowTitle, string[] visualTreeAssetPaths = null, string[] stylesheetsPaths = null)
            where T : CustomEditorWindow
        {
            CustomEditorWindow mainWindow = GetWindow<T>();
            
            if (mainWindow != null)
                mainWindow.CheckErrorOnOpen(mainWindow);
            
            mainWindow = GetWindow<T>();
            
            if (mainWindow != null)
            {
                mainWindow.titleContent = new GUIContent(windowTitle);
                mainWindow.Show(true);
                mainWindow._visualTreeAssetPaths = visualTreeAssetPaths;
                mainWindow._stylesheetsPaths = stylesheetsPaths;
                mainWindow.Paint();
                mainWindow._isOpen = true;
            }
        }

        private void OnEnable()
        {
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        }

        private void OnDisable()
        {
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
        }

        private void OnAfterAssemblyReload()
        {
            Paint();
        }

        [Shortcut("Refresh Mameshiba Windows", typeof(CustomEditorWindow) , KeyCode.F5)]
        private static void Refresh()
        {
            if (focusedWindow != null)
            {
                ((CustomEditorWindow)focusedWindow).Paint();
            }
        }

        private void CheckErrorOnOpen(CustomEditorWindow mainWindow)
        {
            // Check if there are any error opening the window and closing in that case
            try
            {
                if (!mainWindow._isOpen)
                {
                    mainWindow.Close();
                }
            }
            catch
            {
                // ignored
            }
        }

        private void OnDestroy() => CleanUp();

        private void Paint()
        {
            bindObject = new SerializedObject(this);

            string rootPath = this.GetScriptPath();
            
            root = rootVisualElement;
            root.Clear();

            if (_visualTreeAssetPaths != null)
                foreach (string visualTreeAssetPath in _visualTreeAssetPaths)
                {
                    VisualTreeAsset visualTree =
                        AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{rootPath}/{visualTreeAssetPath}");
                    root.Add(visualTree.CloneTree());
                }

            if (_visualTreeAssetPaths != null)
                foreach (string stylesheetsPath in _stylesheetsPaths)
                {
                    StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{rootPath}/{stylesheetsPath}");
                    root.styleSheets.Add(styleSheet);
                }

            Init(rootPath);
        }

        protected abstract void Init(string rootPath);
        
        protected virtual void CleanUp() { }
    }
}