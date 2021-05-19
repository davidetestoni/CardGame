using System;
using System.Collections.Generic;

namespace CardGame.Server.Extensions
{
    public static class IListExtensions
    {
        /// <summary>
        /// Shuffles a <paramref name="list"/> using a random number generator <paramref name="rng"/>
        /// and returns the shuffled list. Modifies the original list, does not create a copy.
        /// </summary>
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

        /// <summary>
        /// Gets a random item from a <paramref name="list"/> given a
        /// random number generator <paramref name="rng"/>.
        /// </summary>
        public static T GetRandom<T>(this IList<T> list, Random rng)
            => list[rng.Next(0, list.Count)];
    }
}
