namespace NtpTests.model.network
{
    /// <summary>
    /// Just for future-proofing, we should version requests
    /// </summary>
    public enum NtpNetworkMessageVersion
    {
        /// <summary>
        /// No version info was provided
        /// </summary>
        UnknownVersion = 0,

        /// <summary>
        /// The first version we try 
        /// </summary>
        Prototype = 1
    }
}
