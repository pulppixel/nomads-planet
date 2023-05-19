using System;
using System.Collections.Generic;

namespace MameshibaGames.Common.Helpers
{
    public static class LinqExtensions
    {
        public static int IndexOf<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
     
            int index = 0;
            foreach (TSource item in source) {
                if (predicate.Invoke(item)) {
                    return index;
                }
                index++;
            }

            return -1;
        }
    }
}