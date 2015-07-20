namespace WebReplay
{
    #region Using Directives

    using System.IO;
    using System.Linq;

    using CommandLine;
    using CommandLine.Text;

    using WebReplay.Annotations;

    #endregion

    /// <summary>
    /// Command line arguments.
    /// </summary>
    internal sealed class CommandLineOptions
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the concurrency -- the number of concurrent users for each URI.
        /// </summary>
        [Option('c', "concurrency", DefaultValue = 1, HelpText = "The number of concurrent requests for each URI.", Required = false)]
        public int Concurrency { get; [UsedImplicitly("Used by command line args parser")] set; }

        /// <summary>
        /// Gets or sets the number of iterations for each URI.
        /// </summary>
        [Option('i', "iterations", DefaultValue = 1, HelpText = "The number of iterations for each for each URI.", Required = false)]
        public int Iterations { get; [UsedImplicitly("Used by command line args parser")] set; }

        /// <summary>
        /// Gets or sets the path to the output CSV file.
        /// </summary>
        [Option('o', "out", HelpText = "The path to the output file. Optional.", Required = false)]
        public string OutFile { get; [UsedImplicitly("Used by command line args parser")] set; }

        /// <summary>
        /// Gets or sets a value indicating whether to calculate the MD5 checksum of the response bodies.
        /// </summary>
        [Option("checksum", HelpText = "An indicator whether to caluclate MD5 checksum of the reponse bodies. Will affect metrics like requests/sec.", Required = false, DefaultValue = false)]
        public bool CalculateBodyChecksum { get; [UsedImplicitly("Used by command line args parser")] set; }

        /// <summary>
        /// Gets or sets a value indicating whether to calculate the MD5 checksum of the response bodies.
        /// </summary>
        [Option("bodysize", HelpText = "An indicator whether to caluclate reponse body sizes. May affect metrics like requests/sec.", Required = false, DefaultValue = false)]
        public bool CalculateBodySize { get; [UsedImplicitly("Used by command line args parser")] set; }

        /// <summary>
        /// Gets or sets the replay files names.
        /// </summary>
        [OptionArray('r', "replayfile", HelpText = "The path to the replay file. Can contain wildcard in the name of the file e.g. c:\\temp\\*.json. Required.", Required = true)]
        public string[] ReplayFiles { get; [UsedImplicitly("Used by command line args parser")] set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Expands the input replay file names into proper file paths.
        /// </summary>
        /// <returns>Ordered list of file names. The order is alphabetic.</returns>
        public string[] ExpandedReplayFiles()
        {
            // This takes care of parameters like c:\temp\*.json
            // and will get all .json files in c:\temp.
            var result = this.ReplayFiles.SelectMany(r =>
            {
                var filePart = Path.GetFileName(r);
                if (filePart.Contains("*"))
                {
                    var dirPart = Path.GetDirectoryName(r);
                    return Directory.GetFiles(dirPart, searchPattern: filePart);
                }
                else
                {
                    return new[] { r };
                }
            });

            return result
                .OrderBy(r => r)
                .ToArray();
        }

        /// <summary>
        /// Prints the usage message. For internal use by Command line package.
        /// </summary>
        /// <returns>Usage string.</returns>
        [HelpOption]
        [UsedImplicitly("Used by command line args parser")]
        public string GetUsage()
        {
            var help = new HelpText("Web Replay - Replays the URIs from the replay files and calculates response time statistics in CSV format. \nThink of this as ab on steroids. \nSee SampleReplayFile.json for an example.");
            help.AddDashesToOption = true;
            help.AddOptions(this);
            return help;
        }

        #endregion
    }
}