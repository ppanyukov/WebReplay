namespace WebReplay
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.IO;

    using WebReplay.Annotations;

    #endregion

    /// <summary>
    /// An bunch of functions for writing lines of text to places: files, console etc.
    /// </summary>
    internal static class StringWriterFuns
    {
        #region Delegates

        /// <summary>
        /// The signature of the function to write lines of text.
        /// </summary>
        /// <param name="line">The line of text to write.</param>
        /// <param name="args">The optional arguments.</param>
        [StringFormatMethod("line")]
        public delegate void WriteLineFun(string line, params object[] args);

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a writer function which aggregates other writers. It calls all
        /// other writers one by one.
        /// </summary>
        /// <param name="writerFuns">The list of writer functions to delegate to.</param>
        /// <returns>The aggregate writer.</returns>
        public static WriteLineFun AggregateWriter(IReadOnlyList<WriteLineFun> writerFuns)
        {
            return (line, args) =>
            {
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < writerFuns.Count; i++)
                {
                    var writerFun = writerFuns[i];
                    writerFun(line, args);
                }
            };
        }

        /// <summary>
        /// Creates a function which writes text to console.
        /// </summary>
        /// <returns>The writer which writes to console.</returns>
        public static WriteLineFun ConsoleWriter()
        {
            return Console.WriteLine;
        }

        /// <summary>
        /// Creates a writer function which writes lines of text to the specified file.
        /// </summary>
        /// <param name="fileName">The file name to write to.</param>
        /// <returns>The writer function which writes lines of text to file.</returns>
        public static WriteLineFun FileWriter(string fileName)
        {
            return (line, args) =>
            {
                var formattedLine = args == null || args.Length == 0 ? line : string.Format(line, args);
                File.AppendAllLines(path: fileName, contents: new[] { formattedLine });
            };
        }

        #endregion
    }
}