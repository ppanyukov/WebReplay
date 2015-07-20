namespace WebReplay
{
    #region Using Directives

    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// Calculates the common percentiles out of a sequence of records.
    /// </summary>
    /// <typeparam name="T">The type of items.</typeparam>
    internal sealed class PercentileStats<T>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PercentileStats{T}"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        public PercentileStats(IEnumerable<T> items)
        {
            var requiredPercentiles = new[]
            {
                0.00, 
                0.50, 
                0.75, 
                0.90, 
                0.95, 
                1.00
            };

            var percentiles = Statistics.Percentiles(items, requiredPercentiles);
            this.Min = percentiles[0].Item2;
            this.Perc50 = percentiles[1].Item2;
            this.Perc75 = percentiles[2].Item2;
            this.Perc90 = percentiles[3].Item2;
            this.Perc95 = percentiles[4].Item2;
            this.Max = percentiles[5].Item2;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the maximum.
        /// </summary>
        public T Max { get; private set; }

        /// <summary>
        /// Gets the minimum.
        /// </summary>
        public T Min { get; private set; }

        /// <summary>
        /// Gets the 50-th percentile, aka median.
        /// </summary>
        public T Perc50 { get; private set; }

        /// <summary>
        /// Gets the 75-th percentile.
        /// </summary>
        public T Perc75 { get; private set; }

        /// <summary>
        /// Gets the 90-th percentile.
        /// </summary>
        public T Perc90 { get; private set; }

        /// <summary>
        /// Gets the 95-th percentile.
        /// </summary>
        public T Perc95 { get; private set; }

        #endregion
    }
}