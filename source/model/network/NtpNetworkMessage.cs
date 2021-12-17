using System.Runtime.Serialization;

namespace NtpTests.model.network
{
    /// <summary>
    /// Example message implementation to send across network
    /// </summary>
    [DataContract]
    public class NtpNetworkMessage
    {
        /// <summary>
        /// Request kind
        /// </summary>
        [DataMember]
        public NtpNetworkRequestKind RequestKind { get; set; }

        /// <summary>
        /// Protocol Version
        /// </summary>
        [DataMember]
        public NtpNetworkMessageVersion Version { get; set; }

        /// <summary>
        /// Encrypted host information
        /// </summary>
        [DataMember]
        public string? HostInformation { get; set; }

        /// <summary>
        /// Encrypted query information
        /// </summary>
        [DataMember]
        public string? NtpQueryInformation { get; set; }

    }
}
