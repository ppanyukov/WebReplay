namespace WebReplay
{
    /// <summary>
    /// The options for running and timing the URIs.
    /// </summary>
    internal sealed class UriRunnerOptions
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UriRunnerOptions"/> class.
        /// </summary>
        /// <param name="calculateBodySize">
        ///     If set to <c>true</c> will calculate the body size. This may require to wait for the entire body to download
        ///     if the body size is not specified in the Content-Length response header and thus may affect the 
        ///     metrics such as requests/sec.
        /// </param>
        /// <param name="calculateBodyChecksum">
        ///     If set to <c>true</c> will calculate checksum for the response body. 
        ///     This requires the entire body to be available and may affect metrics such as
        ///     requests/sec.
        /// </param>
        public UriRunnerOptions(bool calculateBodySize, bool calculateBodyChecksum)
        {
            this.CalculateBodySize = calculateBodySize;
            this.CalculateBodyChecksum = calculateBodyChecksum;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether the checksum of the response body needs to be calculated.
        /// </summary>
        public bool CalculateBodyChecksum { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the body size should be recorded.
        /// </summary>
        public bool CalculateBodySize { get; private set; }

        #endregion
    }
}