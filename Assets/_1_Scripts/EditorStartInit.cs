#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using NomadsPlanet.Utils;

[InitializeOnLoad]
public class EditorPlayModeStartScene
{
    static EditorPlayModeStartScene()
    {
        EditorApplication.playModeStateChanged += PlayModeStateChanged;
    }

    private static void PlayModeStateChanged(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.ExitingEditMode)
        {
            return;
        }

        EditorSceneManager.OpenScene(SceneName.NetBootStrap);
    }
}
#endif