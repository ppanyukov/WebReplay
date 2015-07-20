namespace WebReplay
{
    #region Using Directives

    using System;

    #endregion

    /// <summary>
    /// The reporting item for an individual URI with aggregate statistics.
    /// To be used directly to present the final results.
    /// </summary>
    internal sealed class ReplayFilerRunnerResult
    {
        #region Constants and Fields

        /// <summary>
        /// The response time statistics.
        /// </summary>
        private readonly PercentileStats<TimeSpan> responseTimeStats;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplayFilerRunnerResult"/> class.
        /// </summary>
        /// <param name="description">The description, from the replay file.</param>
        /// <param name="baseUri">The base URI.</param>
        /// <param name="uri">The relative URI.</param>
        /// <param name="iterations">The number of iterations performed.</param>
        /// <param name="concurrentRequests">The number of concurrent requests performed.</param>
        /// <param name="lastStatusCode">The last status code observed.</param>
        /// <param name="lastResponseBodySize">The last size of the response body observed.</param>
        /// <param name="lastResponseBodyChecksum">The last response body checksum observed.</param>
        /// <param name="responseTimeStats">The response time statistics</param>
        /// <param name="requestsPerSec">The number of requests per second.</param>
        /// <param name="testStartTime">The approximate timestamp when the first iteration started.</param>
        /// <param name="testEndTime">The approximate timestamp when all iterations finished.</param>
        public ReplayFilerRunnerResult(
            string description,
            string baseUri,
            string uri,
            int iterations,
            int concurrentRequests,
            int lastStatusCode,
            long lastResponseBodySize,
            string lastResponseBodyChecksum,
            PercentileStats<TimeSpan> responseTimeStats,
            double requestsPerSec,
            DateTimeOffset testStartTime,
            DateTimeOffset testEndTime)
        {
            this.responseTimeStats = responseTimeStats;
            this.Description = description;
            this.BaseUri = baseUri;
            this.Uri = uri;
            this.Iterations = iterations;
            this.ConcurrentRequests = concurrentRequests;
            this.LastStatusCode = lastStatusCode;
            this.LastResponseBodySize = lastResponseBodySize;
            this.LastResponseBodyChecksum = lastResponseBodyChecksum;
            this.RequestsPerSec = requestsPerSec;
            this.TestStartTime = testStartTime;
            this.TestEndTime = testEndTime;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the base uri of the request.
        /// </summary>
        public string BaseUri { get; private set; }

        /// <summary>
        /// Gets the number of concurrent requests performed.
        /// </summary>
        public int ConcurrentRequests { get; private set; }

        /// <summary>
        /// Gets the description of the uri, from the replay file.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the number of iterations performed.
        /// </summary>
        public int Iterations { get; private set; }

        /// <summary>
        /// Gets the checksum of the last response body observed.
        /// </summary>
        public string LastResponseBodyChecksum { get; private set; }

        /// <summary>
        /// Gets the number of requests per second.
        /// </summary>
        public double RequestsPerSec { get; private set; }

        /// <summary>
        /// Gets the last observed response body size in bytes.
        /// </summary>
        public long LastResponseBodySize { get; private set; }

        /// <summary>
        /// Gets the last observed response status code.
        /// </summary>
        public int LastStatusCode { get; private set; }

        /// <summary>
        /// Gets the maximum observed response time.
        /// </summary>
        public TimeSpan ResponseTimeMax
        {
            get
            {
                return this.responseTimeStats.Max;
            }
        }

        /// <summary>
        /// Gets the minimum observed response time.
        /// </summary>
        public TimeSpan ResponseTimeMin
        {
            get
            {
                return this.responseTimeStats.Min;
            }
        }

        /// <summary>
        /// Gets the median (50 percentile) response time.
        /// </summary>
        public TimeSpan ResponseTimePerc50
        {
            get
            {
                return this.responseTimeStats.Perc50;
            }
        }

        /// <summary>
        /// Gets the 75-th percentile response time.
        /// </summary>
        public TimeSpan ResponseTimePerc75
        {
            get
            {
                return this.responseTimeStats.Perc75;
            }
        }

        /// <summary>
        /// Gets the 90-th percentile response time.
        /// </summary>
        public TimeSpan ResponseTimePerc90
        {
            get
            {
                return this.responseTimeStats.Perc90;
            }
        }

        /// <summary>
        /// Gets the 95-th percentile response time.
        /// </summary>
        public TimeSpan ResponseTimePerc95
        {
            get
            {
                return this.responseTimeStats.Perc95;
            }
        }

        /// <summary>
        /// Gets the approximate timestamp when the first HTTP request to the URI went out.
        /// For ordering and historical purposes.
        /// </summary>
        public DateTimeOffset TestStartTime { get; private set; }

        /// <summary>
        /// Gets the approximate timestamp when the all HTTP request to the URI completed.
        /// For ordering and historical purposes.
        /// </summary>
        public DateTimeOffset TestEndTime { get; private set; }

        /// <summary>
        /// Gets the approximate duration of the test run for the URI.
        /// Might be a better indicator of how fast or slow things are than
        /// requests per second and the absolute response times.
        /// </summary>
        public TimeSpan TestDuration
        {
            get
            {
                return this.TestEndTime - this.TestStartTime;
            }
        }

        /// <summary>
        /// Gets the relative uri of this replay item.
        /// </summary>
        public string Uri { get; private set; }

        #endregion
    }
}