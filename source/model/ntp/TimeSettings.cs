using System.Runtime.Serialization;

namespace NtpTests.model
{
    /// <summary>
    /// Record class for time query results
    /// </summary>
    [DataContract]
    public class TimeSettings
    {
        /// <summary>
        /// Sucess state enum
        /// </summary>
        [DataMember]
        public TimeSettingsQueryResult QueryResult { get; set; }

        /// <summary>
        /// Leap Indicator
        /// </summary>
        /// <remarks>https://docs.microsoft.com/en-us/troubleshoot/windows-server/identity/time-service-treats-leap-second</remarks>
        [DataMember]
        public string? LeapIndicator { get; set; }

        /// <summary>
        /// Stratum for NTP
        /// </summary>
        /// <remarks>https://en.wikipedia.org/wiki/Network_Time_Protocol#Clock_strata</remarks>
        [DataMember]
        public string? Stratum { get; set; }

        /// <summary>
        /// Precision of time information
        /// </summary>
        /// <remarks>https://superuser.com/a/1328526</remarks>
        [DataMember]
        public string? Precision { get; set; }

        /// <summary>
        /// Round-trip packet delay to stratum 1 time server
        /// </summary>
        /// <remarks>https://blog.meinbergglobal.com/2021/02/25/the-root-of-all-timing-understanding-root-delay-and-root-dispersion-in-ntp/</remarks>
        [DataMember]
        public string? RootDelay { get; set; }

        /// <summary>
        /// Cumulative error from root delay due to fractional frequency error
        /// </summary>
        /// <remarks>https://blog.meinbergglobal.com/2021/02/25/the-root-of-all-timing-understanding-root-delay-and-root-dispersion-in-ntp/</remarks>
        [DataMember]
        public string? RootDispersion { get; set; }

        /// <summary>
        /// Last datetime that w32tm successfully synced
        /// </summary>
        [DataMember]
        public string? LastSuccessfulSyncTime { get; set; }

        /// <summary>
        /// NTP Server Hostname
        /// </summary>
        [DataMember]
        public string? Source { get; set; }

        /// <summary>
        /// Polling interval to compensate large time offsets
        /// </summary>
        /// <remarks>https://docs.microsoft.com/en-US/troubleshoot/windows-server/identity/configure-w32ime-against-huge-time-offset</remarks>
        [DataMember]
        public string? PollInterval { get; set; }
    }
}
