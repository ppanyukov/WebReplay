namespace WebReplay
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;

    #endregion

    /// <summary>
    /// The statistics helper functions.
    /// </summary>
    internal static class Statistics
    {
        #region Public Methods

        /// <summary>
        /// Calculates the specified percentile for the specified records.
        /// If several percentiles required, it's much more efficient to call the bulk version.
        /// </summary>
        /// <typeparam name="T">The type of the item.</typeparam>
        /// <param name="records">The records.</param>
        /// <param name="percentile">The percentile.</param>
        /// <returns>
        /// The item at the specified percentile.
        /// </returns>
        public static T Percentile<T>(IEnumerable<T> records, double percentile)
        {
            return Percentiles(records, percentile).First().Item2;
        }

        /// <summary>
        /// Calculates the percentiles.
        /// </summary>
        /// <typeparam name="T">The type of the item.</typeparam>
        /// <param name="records">The records.</param>
        /// <param name="percentiles">The percentiles.</param>
        /// <returns>
        /// The list of items at the requested percentiles. 
        /// This is a list of tuples. First item is the percentile,
        /// and the second item is the item at that percentile.
        /// </returns>
        public static Tuple<double, T>[] Percentiles<T>(IEnumerable<T> records, params double[] percentiles)
        {
            var ordered = records.ToArray();
            Array.Sort(ordered);

            var length = ordered.Length - 1;
            var result = new Tuple<double, T>[percentiles.Length];
            for (var i = 0; i < percentiles.Length; i++)
            {
                var perc = percentiles[i];
                var index = (int)(length * perc);
                index = Math.Min(index, length);
                index = Math.Max(index, 0);
                var value = ordered[index];
                result[i] = Tuple.Create(perc, value);
            }

            return result;
        }

        #endregion
    }
}