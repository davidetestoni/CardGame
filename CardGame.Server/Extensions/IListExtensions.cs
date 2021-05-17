using System;
using System.Collections.Generic;

namespace CardGame.Server.Extensions
{
    public static class IListExtensions
    {
        public static IList<T> Shuffle<T>(this IList<T> list, Random rng)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }

        public static T GetRandom<T>(this IList<T> list, Random rng)
            => list[rng.Next(0, list.Count)];
    }
}
