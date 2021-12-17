using NtpTests.model;
using System;
using System.Diagnostics;
using System.Linq;

namespace NtpTests
{
    public class Program
    {
        static void Main(string[] args)
        {
            tryGetInfoFromYortNTP();

            tryQueryLocalNTP();
        }

        /// <summary>
        /// Get information about NTP status from local service
        /// </summary>
        private static void tryQueryLocalNTP()
        {
            var result = GetNtpSettingInfo();

            switch (result.QueryResult)
            {
                case TimeSettingsQueryResult.TimeServiceNotRunning:
                    Console.WriteLine("Time service not running!");
                    break;
                case TimeSettingsQueryResult.UnsupportedPlatform:
                    Console.WriteLine($"Unsupported platform: {Environment.OSVersion.ToString}");
                    break;
                case TimeSettingsQueryResult.QueryParserException:
                    Console.WriteLine("Query result could not be parsed.");
                    break;
                default:
                    Console.WriteLine($"Source: {result.Source}");
                    Console.WriteLine($"Stratum: {result.Stratum}");
                    Console.WriteLine($"Poll Interval: {result.PollInterval}");
                    Console.WriteLine($"Leap indicator: {result.LeapIndicator}");
                    break;
            }
        }

        /// <summary>
        /// Get accurate time information from external source
        /// </summary>
        private static void tryGetInfoFromYortNTP()
        {
            var client = new Yort.Ntp.NtpClient("pool.ntp.org");
            var currentTime = client.RequestTimeAsync().Result;
            Console.WriteLine($"Current time according to pool.ntp.org: {currentTime.NtpTime}");
            Console.WriteLine($"Current time according to system: {DateTime.UtcNow}");
        }

        /// <summary>
        /// Wrapper for OS-dependant datetime service
        /// </summary>
        /// <returns>TimeSettings record</returns>
        static TimeSettings GetNtpSettingInfo()
        {
            if (OperatingSystem.IsWindows())
            {
                return GetNtpSettingInfoWindows();
            }

            if (OperatingSystem.IsLinux())
            {
                return GetNtpSettingInfoLinux();
            }

            return new TimeSettings {  QueryResult = TimeSettingsQueryResult.UnsupportedPlatform };
        }

        private static TimeSettings GetNtpSettingInfoLinux()
        {
            try
            {
                var startInfo = new ProcessStartInfo("timedatectl", "show-timesync") {CreateNoWindow = true, RedirectStandardOutput = true};
                var proc = Process.Start(startInfo);
                var rawOutput = proc?.StandardOutput.ReadToEnd();

                proc?.WaitForExit();

                var result = new TimeSettings();

                if (rawOutput == null)
                {
                    result.QueryResult = TimeSettingsQueryResult.TimeServiceNotRunning;

                    return result;
                }

                var output = (from inp in rawOutput?.Split("\n", StringSplitOptions.RemoveEmptyEntries) select inp.Split("=") into split where split.Length > 1 select split[1]).ToList();

                var lineCount = output.Count;

                if (lineCount > 1)
                {
                    result.Source = output[1];
                }

                if (lineCount > 6)
                {
                    result.PollInterval = output[6];
                }

                if (lineCount > 7)
                {
                    var ntpMessage = output[7];

                    var partials = (from ntp in ntpMessage.Split(",", StringSplitOptions.RemoveEmptyEntries) select ntp.Split("=", StringSplitOptions.TrimEntries) into split where split.Length > 1 select split[1]).ToList();

                    var partialCount = partials.Count;

                    if (partialCount > 0)
                    {
                        result.LeapIndicator = partials[0];
                    }

                    if (partialCount > 3)
                    {
                        result.Stratum = partials[3];
                    }

                    if (partialCount > 4)
                    {
                        result.Precision = partials[4];
                    }

                    if (partialCount > 5)
                    {
                        result.RootDelay = partials[5];
                    }

                    if (partialCount > 6)
                    {
                        result.RootDispersion = partials[6];
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");

                return new TimeSettings { QueryResult = TimeSettingsQueryResult.QueryParserException };
            }
        }

        /// <summary>
        /// Windows w32tm query wrapper
        /// </summary>
        /// <returns>TimeSettings info</returns>
        /// <remarks>Does not require UAC elevation</remarks>
        private static TimeSettings GetNtpSettingInfoWindows()
        {
            try
            {
                var startInfo = new ProcessStartInfo("w32tm.exe", "/query /status /verbose") {CreateNoWindow = true, UseShellExecute = false, RedirectStandardOutput = true};
                var proc = Process.Start(startInfo);
                var rawOutput = proc?.StandardOutput.ReadToEnd();
                var output = (from inp in rawOutput?.Split("\n", StringSplitOptions.RemoveEmptyEntries) select inp.Split(": ") into split where split.Length > 1 select split[1]).ToList();

                proc?.WaitForExit();

                var result = new TimeSettings();
                var lineCount = output.Count;

                if (lineCount == 1 && rawOutput != null && rawOutput.Contains("80070426"))
                {
                    result.QueryResult = TimeSettingsQueryResult.TimeServiceNotRunning;

                    return result;
                }

                if (lineCount == 0)
                {
                    return result;
                }

                result.LeapIndicator = output[0];

                if (lineCount < 2)
                {
                    return result;
                }

                result.Stratum = output[1];

                if (lineCount < 3)
                {
                    return result;
                }

                result.Precision = output[2];

                if (lineCount < 4)
                {
                    return result;
                }

                result.RootDelay = output[3];

                if (lineCount < 5)
                {
                    return result;
                }

                result.RootDispersion = output[4];

                if (lineCount < 7)
                {
                    return result;
                }

                result.LastSuccessfulSyncTime = output[6];

                if (lineCount < 8)
                {
                    return result;
                }

                result.Source = output[7];

                if (lineCount < 9)
                {
                    return result;
                }

                result.PollInterval = output[8];

                result.QueryResult = TimeSettingsQueryResult.Success;

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");

                return new TimeSettings { QueryResult = TimeSettingsQueryResult.QueryParserException };
            }
        }
    }
}
