using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MameshibaGames.Common.Helpers
{
    public static class TransformHelpers
    {
        public static void ResetTransform(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }
        
        public static List<Transform> All(this Transform transform, Func<Transform, bool> query)
        {
            List<Transform> transforms = new List<Transform>();

            if (query(transform))
            {
                transforms.Add(transform);
                return transforms;
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                List<Transform> result = All(transform.GetChild(i), query);
                if (result.Count > 0)
                {
                    transforms = transforms.Concat(result).ToList();
                }
            }

            return transforms;
        }
        
        public static Transform RecursiveFindChild(this Transform parent, string childName)
        {
            Transform child = null;
            for (int i = 0; i < parent.childCount; i++)
            {
                child = parent.GetChild(i);
                if (child.name == childName)
                {
                    break;
                }
                else
                {
                    child = RecursiveFindChild(child, childName);
                    if (child != null)
                    {
                        break;
                    }
                }
            }

            return child;
        }
    }
}
