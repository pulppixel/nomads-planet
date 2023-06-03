using UnityEngine;

public static class CustomFunc
{
    public static void WriteLine(System.Object message, bool isError = false)
    {
#if UNITY_EDITOR
        if (isError)
        {
            Debug.LogError(message);
        }
        else
        {
            Debug.Log(message);
        }
#endif
    }
}