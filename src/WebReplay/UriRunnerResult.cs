namespace WebReplay
{
    #region Using Directives

    using System;
    using System.Net;

    #endregion

    /// <summary>
    /// Support structure for reporting the results of the URI execution.
    /// </summary>
    internal sealed class UriRunnerResult
    {
        #region Constants and Fields

        /// <summary>
        /// The MD5 checksum of the response.
        /// </summary>
        public readonly string BodyChecksum;

        /// <summary>
        /// The size of the response in bytes.
        /// </summary>
        public readonly long BodySize;

        /// <summary>
        /// The response time.
        /// </summary>
        public readonly TimeSpan ResponseTime;

        /// <summary>
        /// The status code of the response.
        /// </summary>
        public readonly HttpStatusCode StatusCode;

        /// <summary>
        /// The uri which was executed.
        /// </summary>
        public readonly string Uri;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UriRunnerResult"/> class.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="responseTime">The response time (time to first byte).</param>
        /// <param name="statusCode">The status code.</param>
        /// <param name="bodySize">Size of the body in bytes.</param>
        /// <param name="bodyChecksum">The body checksum.</param>
        public UriRunnerResult(string uri, TimeSpan responseTime, HttpStatusCode statusCode, long bodySize, string bodyChecksum)
        {
            this.Uri = uri;
            this.ResponseTime = responseTime;
            this.StatusCode = statusCode;
            this.BodySize = bodySize;
            this.BodyChecksum = bodyChecksum;
        }

        #endregion
    }
}