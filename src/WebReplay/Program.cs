namespace WebReplay
{
    #region Using Directives

    using System;
    using System.IO;

    #endregion

    // ReSharper disable RedundantArgumentDefaultValue
    // ReSharper disable RedundantArgumentName
    // ReSharper disable ForCanBeConvertedToForeach

    /// <summary>
    /// The main benchmarking replay program.
    /// </summary>
    internal static class Program
    {
        #region Public Methods

        /// <summary>
        /// Mains entry-point.
        /// </summary>
        /// <param name="args">The command-line arguments. We expect none at the moment.</param>
        public static void Main(string[] args)
        {
            // Just to make sure there isn't anything fuffing around with the number of concurrent requests.
            System.Net.ServicePointManager.DefaultConnectionLimit = int.MaxValue;

            var consoleWriteLine = StringWriterFuns.ConsoleWriter();

            var options = new CommandLineOptions();
            if (!CommandLine.Parser.Default.ParseArgumentsStrict(args, options))
            {
                consoleWriteLine("Something is wrong with command-line arguments.");
                Environment.Exit(-1);
            }

            var replayFileNames = options.ExpandedReplayFiles();
            var replayFiles = new ReplayFile[replayFileNames.Length];
            var replayFileErrors = 0;
            for (var i = 0; i < replayFileNames.Length; i++)
            {
                var replayFileName = replayFileNames[i];

                try
                {
                    var content = File.ReadAllText(replayFileName);
                    content = Environment.ExpandEnvironmentVariables(content);
                    var replay = Newtonsoft.Json.JsonConvert.DeserializeObject<ReplayFile>(content);
                    replayFiles[i] = replay;

                    consoleWriteLine("Yay!: {0}", replayFileName);
                    consoleWriteLine("    - {0}", replay.Name);
                    consoleWriteLine("    - {0}", replay.Description);
                    consoleWriteLine("    - {0}", replay.BaseUri);
                    consoleWriteLine("    - {0}", replay.Headers);
                    consoleWriteLine("    - {0} uris", replay.Uris.Length.ToString("G"));
                }
                catch (Exception e)
                {
                    consoleWriteLine("**** ERROR reading replay file: {0}", replayFileName);
                    consoleWriteLine(e.ToString());
                    replayFileErrors++;
                }
            }

            if (replayFileErrors > 0)
            {
                consoleWriteLine("+++++ Could not read all replay files. Exiting.");
                Environment.Exit(-1);
            }

            if (replayFiles.Length == 0)
            {
                consoleWriteLine("+++++ No suitable replay files.");
                Environment.Exit(-1);
            }

            if (options.OutFile != null && File.Exists(options.OutFile))
            {
                File.Delete(options.OutFile);
            }

            StringWriterFuns.WriteLineFun writeReportLine;
            if (options.OutFile != null)
            {
                writeReportLine = StringWriterFuns.AggregateWriter(
                    new[]
                    {
                        StringWriterFuns.ConsoleWriter(), 
                        StringWriterFuns.FileWriter(options.OutFile)
                    });
            }
            else
            {
                writeReportLine = StringWriterFuns.ConsoleWriter();
            }

            // The CSV header with field names.
            const string Header = "Sequence, Iterations, Concurrent Requests, TestStartTimeUtc, TestEndTimeUtc, TestDurationMs, Status Code, Response Size, Response Checksum, Req/Sec, min, perc50, perc75, perc90, perc95, max, description, baseUri, uri";
            writeReportLine(Header);

            // The callback for completed HTTP request. We use it here to
            // write the CSV result into the file. Keep the sequence number
            // so that we can print that out too.
            var sequenceNumber = 0;
            Action<ReplayFilerRunnerResult> callback = (r) =>
            {
                sequenceNumber++;

                var line = string.Format(
                    "{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}", 
                    sequenceNumber.ToString("G"), 
                    r.Iterations.ToString("G"), 
                    r.ConcurrentRequests.ToString("G"),
                    r.TestStartTime.ToUniversalTime().ToString("yyy-mm-dd HH:MM:ss.fff"),
                    r.TestEndTime.ToUniversalTime().ToString("yyy-mm-dd HH:MM:ss.fff"),
                    r.TestDuration.TotalMilliseconds.ToString("F"),
                    r.LastStatusCode.ToString("G"), 
                    r.LastResponseBodySize.ToString("G"), 
                    r.LastResponseBodyChecksum, 
                    r.RequestsPerSec.ToString("F"),
                    r.ResponseTimeMin.TotalMilliseconds.ToString("G"), 
                    r.ResponseTimePerc50.TotalMilliseconds.ToString("G"), 
                    r.ResponseTimePerc75.TotalMilliseconds.ToString("G"), 
                    r.ResponseTimePerc90.TotalMilliseconds.ToString("G"), 
                    r.ResponseTimePerc95.TotalMilliseconds.ToString("G"), 
                    r.ResponseTimeMax.TotalMilliseconds.ToString("G"), 
                    r.Description, 
                    r.BaseUri, 
                    r.Uri);

                writeReportLine(line);
            };

            foreach (var replayFile in replayFiles)
            {
                var uriRunnerOptions = new UriRunnerOptions(
                    calculateBodySize: options.CalculateBodySize,
                    calculateBodyChecksum: options.CalculateBodyChecksum);

                ReplayFileRunner.RunReplayFileAsync(
                    replayFile: replayFile, 
                    callback: callback, 
                    uriRunnerOptions: uriRunnerOptions, 
                    iterations: options.Iterations, 
                    concurrentRequests: options.Concurrency).Wait();
            }
        }

        #endregion
    }

    // ReSharper restore RedundantArgumentDefaultValue
    // ReSharper restore RedundantArgumentName
    // ReSharper restore ForCanBeConvertedToForeach
}