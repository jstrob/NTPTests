using NtpTests.model;
using NtpTests.model.network;
using NtpTests.service;
using NtpTests.utilities.datetime;
using System;

namespace NtpTests
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Example on how to access NTP Server programatically
            NtpServerUtilities.PrintNtpServerTime();

            // Example on how to query local time service
            var ntpInfo = tryQueryLocalNTP();

            // Wrapper for return address info, may be useful for configuration purposes when customers use reverse proxies and such
            var recipient = new NetworkRecipient { Hostname = "localhost", PrimaryPort = 54790, BackupPort = 58248 };

            // JSON string of a message to be sent across network, with data contents compressed and encrypted
            var preparedMessage = SecureNtpMessageExchange.PrepareMessageForSending(ntpInfo, recipient, NtpNetworkRequestKind.Query, NtpNetworkMessageVersion.Prototype);

            // Actually sending this message to a client and receiving a reply is left as an exercise to the reader
            var receivedMessage = preparedMessage;

            // How to unpack such a received message
            if (!SecureNtpMessageExchange.TryParseMessage(receivedMessage, out var requestKind, out var requestVersion, out var sender, out var data))
            {
                Console.WriteLine("Could not parse received message");
                return;
            }

            Console.WriteLine($"Received {requestKind} request via protocol version {requestVersion} from sender {sender?.Hostname}:{sender?.PrimaryPort}");
        }

        /// <summary>
        /// Get information about NTP status from local service
        /// </summary>
        private static TimeSettings tryQueryLocalNTP()
        {
            var result = TimeServiceUtilities.GetNtpSettingInfo();

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

            return result;
        }
    }
}
