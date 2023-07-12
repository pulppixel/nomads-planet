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

        public static T GetChildFromName<T>(this Transform parent, string name) where T : Component
        {
            foreach (Transform child in parent.transform)
            {
                if (child.name.Equals(name) && child.TryGetComponent<T>(out var component))
                {
                    return component;
                }

                var result = child.GetChildFromName<T>(name);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}