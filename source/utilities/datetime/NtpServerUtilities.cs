using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtpTests.utilities.datetime
{
    internal static class NtpServerUtilities
    {
        /// <summary>
        /// Get accurate time information from external source
        /// </summary>
        internal static bool tryGetInfoFromYortNTP(out Yort.Ntp.RequestTimeResult result)
        {
            result = default;
            try
            {
                var client = new Yort.Ntp.NtpClient("pool.ntp.org");
                result = client.RequestTimeAsync().Result;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void PrintNtpServerTime()
        {
            if (tryGetInfoFromYortNTP(out var data))
            {
                Console.WriteLine($"Current time according to pool.ntp.org: {data.NtpTime}");
                Console.WriteLine($"Current time according to system: {DateTime.UtcNow}");
            }            
        }
    }
}
