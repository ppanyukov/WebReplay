namespace WebReplay
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    #endregion

    /// <summary>
    /// Replays URIs in the given replay file.
    /// </summary>
    internal static class ReplayFileRunner
    {
        #region Public Methods

        /// <summary>
        /// Asynchronously runs the specified replay file.
        /// </summary>
        /// <param name="replayFile">The replay file.</param>
        /// <param name="callback">The callback to call at the completion of each of the request in the replay file..</param>
        /// <param name="uriRunnerOptions">The options for the uri runner.</param>
        /// <param name="iterations">The iterations.</param>
        /// <param name="concurrentRequests">The number of concurrent requests to execute.</param>
        /// <returns>The list of the results.</returns>
        public static async Task RunReplayFileAsync(
            ReplayFile replayFile, 
            Action<ReplayFilerRunnerResult> callback, 
            UriRunnerOptions uriRunnerOptions, 
            int iterations = 1, 
            int concurrentRequests = 1)
        {
            var consoleWriteLine = StringWriterFuns.ConsoleWriter();

            using (var handler = new HttpClientHandler())
            using (var client = new HttpClient(handler))
            {
                handler.AllowAutoRedirect = true;
                handler.AutomaticDecompression = DecompressionMethods.None;
                handler.UseProxy = true;

                // Need to set this to false to make sure the http client does not
                // mess around with our own cookies.
                handler.UseCookies = false;

                client.BaseAddress = new Uri(replayFile.BaseUri);
                foreach (var header in replayFile.Headers)
                {
                    if (!client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value))
                    {
                        consoleWriteLine("Failed to add header: {0} = {1}", header.Key, header.Value);
                    }
                }

                foreach (var uri in replayFile.Uris)
                {
                    var sw = new Stopwatch();

                    var numberOfRequests = iterations * concurrentRequests;
                    var allResults = new List<UriRunnerResult>(capacity: numberOfRequests);
                    var startTimeUtc = DateTimeOffset.UtcNow;

                    // Exec same URI with the specified concurrency level
                    var uris = Enumerable.Repeat(uri, concurrentRequests).ToArray();
                    for (var i = 0; i < iterations; i++)
                    {
                        sw.Start();
                        var runUrisResults = await UriRunner.RunUrisConcurrentlyAsync(
                            client: client, 
                            uris: uris,
                            uriRunnerOptions: uriRunnerOptions);
                        sw.Stop();
                        allResults.AddRange(runUrisResults);
                    }

                    var endTimeUtc = DateTimeOffset.UtcNow;
                    var requestsPerSec = (double)numberOfRequests / sw.Elapsed.TotalSeconds;
                    var responseTimes = allResults.Select(r => r.ResponseTime);
                    var percentiles = new PercentileStats<TimeSpan>(responseTimes);

                    var lastResult = allResults.Last();

                    var reportLine = new ReplayFilerRunnerResult(
                        iterations: iterations, 
                        concurrentRequests: concurrentRequests, 
                        lastStatusCode: (int)lastResult.StatusCode, 
                        lastResponseBodySize: lastResult.BodySize, 
                        lastResponseBodyChecksum: lastResult.BodyChecksum, 
                        responseTimeStats: percentiles, 
                        description: replayFile.Description, 
                        baseUri: replayFile.BaseUri, 
                        uri: uri,
                        requestsPerSec: requestsPerSec,
                        testStartTime: startTimeUtc,
                        testEndTime: endTimeUtc);

                    if (callback != null)
                    {
                        callback(reportLine);
                    }
                }
            }
        }

        #endregion
    }
}