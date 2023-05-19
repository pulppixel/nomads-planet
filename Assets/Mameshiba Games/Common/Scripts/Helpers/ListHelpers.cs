using System;
using System.Collections.Generic;

namespace MameshibaGames.Common.Helpers
{
    public static class ListHelpers
    {
        private static Random rng = new Random();  
        
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
    }
}
