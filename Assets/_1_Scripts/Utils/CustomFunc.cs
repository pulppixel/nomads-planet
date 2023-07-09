using System.Collections.Generic;
using UnityEngine;

namespace NomadsPlanet.Utils
{
    public static class CustomFunc
    {
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