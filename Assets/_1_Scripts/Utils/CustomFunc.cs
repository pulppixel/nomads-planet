using System.Collections.Generic;
using UnityEngine;

namespace NomadsPlanet.Utils
{
    public static class CustomFunc
    {
        public static void WriteLine(object message, bool isError = false)
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

        // Fisher-Yates Algorithm
        public static void ShuffleList<T>(this IList<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                T temp = list[i];
                int randomIndex = Random.Range(i, list.Count);
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }
    }
}