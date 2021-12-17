namespace NtpTests.model
{
    /// <summary>
    /// Possible results for time settings query
    /// </summary>
    public enum TimeSettingsQueryResult
    {
        /// <summary>
        /// Sucecssful query & parsing
        /// </summary>
        Success,

        /// <summary>
        /// Time service is not running
        /// </summary>
        TimeServiceNotRunning,

        /// <summary>
        /// Platform is not supported
        /// </summary>
        UnsupportedPlatform,

        /// <summary>
        /// Query Result is in unexpected format
        /// </summary>
        QueryParserException
    }
}
