using System.Runtime.Serialization;

namespace NtpTests.model.network
{
    [DataContract]
    public class NetworkRecipient
    {
        /// <summary>
        ///     Hostname for return address
        /// </summary>
        /// <remarks>If NullOrEmpty use IPv4 information, but customer might have reverse proxy or such</remarks>
        [DataMember]
        public string? Hostname { get; set; }

        /// <summary>
        ///     Port for return address
        /// </summary>
        [DataMember]
        public int? PrimaryPort { get; set; }

        /// <summary>
        ///     Backup Port for return address
        /// </summary>
        [DataMember]
        public int? BackupPort { get; set; }
    }
}
