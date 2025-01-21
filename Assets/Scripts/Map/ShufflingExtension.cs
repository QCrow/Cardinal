using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Map
{
    public static class ShufflingExtension
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                // Use UnityEngine.Random instead of System.Random
                int k = UnityEngine.Random.Range(0, n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static T Random<T>(this IList<T> list)
        {
            int index = UnityEngine.Random.Range(0, list.Count);
            return list[index];
        }

        public static T Last<T>(this IList<T> list)
        {
            return list[list.Count - 1];
        }

        public static List<T> GetRandomElements<T>(this List<T> list, int elementsCount)
        {
            return list
                .OrderBy(arg => System.Guid.NewGuid())
                .Take(list.Count < elementsCount ? list.Count : elementsCount)
                .ToList();
        }
    }
}
