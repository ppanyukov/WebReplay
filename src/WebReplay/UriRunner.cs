namespace WebReplay
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;

    #endregion

    /// <summary>
    /// Runs specified URIs in parallel.
    /// </summary>
    internal static class UriRunner
    {
        #region Public Methods

        /// <summary>
        /// Runs the specified uris concurrently and reports on the results for them.
        /// </summary>
        /// <param name="client">The HTTP client.</param>
        /// <param name="uris">The uris to run.</param>
        /// <param name="uriRunnerOptions">The options for the runner.</param>
        /// <returns>The results for the uri.</returns>
        public static Task<UriRunnerResult[]> RunUrisConcurrentlyAsync(HttpClient client, IReadOnlyList<string> uris, UriRunnerOptions uriRunnerOptions)
        {
            var tasks = new Task<UriRunnerResult>[uris.Count];

            Parallel.For(
                fromInclusive: 0, 
                toExclusive: uris.Count, 
                body: (i, state) =>
                {
                    var uri = uris[i];
                    tasks[i] = RunUriAsync(
                        client: client, 
                        uri: uri,
                        uriRunnerOptions: uriRunnerOptions);
                });

            return Task.WhenAll(tasks);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts byte array into a hex string. Useful for MD5 hash.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>The hex string.</returns>
        private static string ByteArrayToHexString(IReadOnlyList<byte> bytes)
        {
            var buffer = new StringBuilder(capacity: bytes.Count * 2);
            for (var i = 0; i < bytes.Count; i++)
            {
                var b = bytes[i];
                var s = b.ToString("X").ToLowerInvariant();
                buffer.Append(s);
            }

            return buffer.ToString();
        }

        /// <summary>
        /// Runs the specified URI asynchronously and reports timing results.
        /// </summary>
        /// <param name="client">The HTTP client.</param>
        /// <param name="uri">The URI to run.</param>
        /// <param name="uriRunnerOptions">The options.</param>
        /// <returns>The timing result.</returns>
        private static async Task<UriRunnerResult> RunUriAsync(HttpClient client, string uri, UriRunnerOptions uriRunnerOptions)
        {
            var sw = new Stopwatch();
            sw.Start();

            using (var response = await client.GetAsync(uri, completionOption: HttpCompletionOption.ResponseHeadersRead))
            {
                var responseTime = sw.Elapsed;
                var statusCode = response.StatusCode;

                // Do not need to get the body if we don't need to calculate body size or checksum.
                if (!uriRunnerOptions.CalculateBodyChecksum && !uriRunnerOptions.CalculateBodySize)
                {
                    // Optimistically get the body size from the headers if it's there.
                    var contentLength = response.Content.Headers.ContentLength;

                    return new UriRunnerResult(
                        uri: uri,
                        responseTime: responseTime,
                        statusCode: statusCode,
                        bodySize: contentLength.GetValueOrDefault(),
                        bodyChecksum: "0");
                }

                using (var httpContent = response.Content)
                {
                    var body = await httpContent.ReadAsByteArrayAsync();
                    var bodySize = body.Length;
                    var md5 = MD5.Create();
                    var hash = md5.ComputeHash(body);
                    var bodyChecksum = ByteArrayToHexString(hash);

                    return new UriRunnerResult(
                        uri: uri,
                        responseTime: responseTime,
                        statusCode: statusCode,
                        bodySize: bodySize,
                        bodyChecksum: bodyChecksum);
                }
            }
        }

        #endregion
    }
}