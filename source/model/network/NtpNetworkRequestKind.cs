namespace NtpTests.model.network
{
    /// <summary>
    /// Communicates what operation is expected from the end-point client
    /// </summary>
    public enum NtpNetworkRequestKind
    {
        /// <summary>
        /// In case of parser problems, handle as Unknown
        /// </summary>
        Unknown,

        /// <summary>
        /// Query your current status and reply it
        /// </summary>
        Query,

        /// <summary>
        /// Take the supplied information and synchronize your clock either autonomously or using the supplied information
        /// </summary>
        Push
    }
}
