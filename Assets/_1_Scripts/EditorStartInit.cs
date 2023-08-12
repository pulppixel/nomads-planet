using UnityEditor;
using NomadsPlanet.Utils;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class EditorPlayModeStartScene
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

        EditorSceneManager.OpenScene(SceneName.BootStrap);
    }
}