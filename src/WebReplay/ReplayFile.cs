namespace WebReplay
{
    #region Using Directives

    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// Represents the replay file -- the URIs and settings to use for replaying.
    /// </summary>
    internal sealed class ReplayFile
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the base URI for the HTTP requests.
        /// </summary>
        /// <value>
        /// The base URI.
        /// </value>
        public string BaseUri { get; set; }

        /// <summary>
        /// Gets or sets the description of the replay file, e.g. "www - all owned movies".
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the HTTP headers for the requests.
        /// </summary>
        /// <value>
        /// The headers.
        /// </value>
        public Dictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Gets or sets the short name of the replay file.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the list of URIs to replay. These are relative the the <see cref="BaseUri"/>.
        /// </summary>
        /// <value>
        /// The uris to replay.
        /// </value>
        public string[] Uris { get; set; }

        #endregion
    }
}