using System.Collections.Generic;
using UnityEngine;

namespace NomadsPlanet.Utils
{
    public static class CustomFunc
    {
        /// <summary>
        /// Fisher-Yates Algorithm (콜렉션의 내부 멤버들의 위치를 섞어준다.)
        /// </summary>
        /// <param name="list">셔플하고 싶은 콜렉션</param>
        /// <typeparam name="T">변수 타입 설정</typeparam>
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

        public static void ConsoleLog(object obj, bool isError = false)
        {
#if UNITY_EDITOR
            if (isError)
            {
                Debug.LogError(obj);
            }
            else
            {
                Debug.Log(obj);
            }
#endif
        }

        /// <summary>
        /// 이름으로 자식 객체를 찾아주는 메소드 (재귀 호출)
        /// </summary>
        /// <param name="parent">타겟 부모 객체</param>
        /// <param name="name">찾고 싶은 이름</param>
        /// <typeparam name="T">변수 타입</typeparam>
        /// <returns>찾았을 때는 객체를, 못찾았을 때는 null을 반환</returns>
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

        /// <summary>
        /// 갖고 있는 트래픽 리스트들 중 하나를 얻어올 수 있다.
        /// </summary>
        /// <param name="type">알아보고 싶은 Traffic Type</param>
        /// <returns>동일한 확률로 Traffic Type을 얻어올 수 있다.</returns>
        public static TrafficType GetRandomTrafficType(this TrafficType type)
        {
            List<TrafficType> result = new();

            if (type.HasFlag(TrafficType.Left))
            {
                result.Add(TrafficType.Left);
            }

            if (type.HasFlag(TrafficType.Right))
            {
                result.Add(TrafficType.Right);
            }

            if (type.HasFlag(TrafficType.Forward))
            {
                result.Add(TrafficType.Forward);
            }

            return result[Random.Range(0, result.Count)];
        }

        /// <summary>
        /// 너무 글자가 길어지면 뒤는 자른다. (예: 환기는 아주 긴...)
        /// </summary>
        /// <param name="original">체크할 텍스트</param>
        /// <param name="maxLength">최종 길이</param>
        /// <returns>최종 결과물</returns>
        public static string TruncateString(this string original, int maxLength)
        {
            if (string.IsNullOrEmpty(original)) return original;
            if (original.Length <= maxLength) return original;

            return original[..(maxLength - 3)] + "...";
        }
    }
}