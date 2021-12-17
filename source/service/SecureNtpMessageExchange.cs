using NtpTests.model;
using NtpTests.model.network;
using NtpTests.utilities;
using NtpTests.utilities.serialization;
using System;

namespace NtpTests.service
{
    public class SecureNtpMessageExchange
    {
        /// <summary>
        /// We need to store the key for encryption somewhere
        /// </summary>
        /// <remarks>
        /// The first guess for every C# developer is to use a SecureString, but there is no point.
        /// See https://github.com/dotnet/platform-compat/blob/master/docs/DE0001.md for details
        /// There is simply no OS-supported way to secure the memory contents.
        /// If someone can access the physical RAM, they have access to the key.
        /// 
        /// This implementation generates a new random key on every start.
        /// Obviously, that's not a solid implementation for an actual network protocol.
        /// </remarks>
        public static Lazy<string> SecureString = new Lazy<string>(() => Convert.ToBase64String(AESGCM.NewKey()));

        internal static string? PrepareMessageForSending( TimeSettings data, NetworkRecipient recipient, NtpNetworkRequestKind requestKind = NtpNetworkRequestKind.Query, NtpNetworkMessageVersion version = NtpNetworkMessageVersion.Prototype )
        {
            var prepared = new NtpNetworkMessage();
            prepared.Version = version;
            prepared.RequestKind = requestKind;
            prepared.NtpQueryInformation = WriteTimeSettings(data);
            prepared.HostInformation = WriteRecipient(recipient);

            // Create unencrypted, uncompressed json string for sending
            return JsonDataContractHandler.Serialize(prepared);
        }

        public static bool TryParseMessage(string? receivedMessage, out NtpNetworkRequestKind requestKind, out NtpNetworkMessageVersion requestVersion, out NetworkRecipient? recipient, out TimeSettings? data)
        {
            requestKind = NtpNetworkRequestKind.Unknown;
            requestVersion = NtpNetworkMessageVersion.UnknownVersion;
            recipient = default;
            data = default;

            if (receivedMessage == null)
            {
                return false;
            }

            var parsed = JsonDataContractHandler.Deserialize<NtpNetworkMessage>(receivedMessage);

            if (parsed == null)
            {
                return false;
            }

            requestKind = parsed.RequestKind;
            requestVersion = parsed.Version;
            recipient = ReadRecipient(parsed.HostInformation);
            data = ReadTimeSettings(parsed.NtpQueryInformation);
            return true;
        }

        internal static NetworkRecipient? ReadRecipient(string? data)
        {
            return ReadArbitraryData<NetworkRecipient>(data);
        }

        internal static TimeSettings? ReadTimeSettings(string? data)
        {
            return ReadArbitraryData<TimeSettings>(data);
        }

        internal static string? WriteRecipient( NetworkRecipient data )
        {
            return WriteArbitraryData(data);
        }

        internal static string? WriteTimeSettings( TimeSettings data )
        {
            return WriteArbitraryData(data);
        }

        protected static string? WriteArbitraryData<T>( T data)
        {
            var serialized = JsonDataContractHandler.Serialize(data);

            if (serialized == null)
            {
                return default;
            }

            var compressed = StringCompression.CompressToBase64(serialized);

            if (compressed == null)
            {
                return default;
            }

            string? encrypted;

            try
            {
                encrypted = AESGCM.SimpleEncrypt(compressed, Convert.FromBase64String(SecureString.Value));
            }
            catch (Exception)
            {
                // Something went wrong with AESGCM
                return default;
            }

            return encrypted;
        }

        protected static T? ReadArbitraryData<T>(string? data)
        {
            if (data == null)
            {
                return default;
            }

            var decrypted = AESGCM.SimpleDecrypt(data, Convert.FromBase64String(SecureString.Value));

            if (decrypted == null)
            {
                return default;
            }

            var decompressed = StringCompression.DecompressFromBase64(decrypted);

            if (decompressed == null)
            {
                return default;
            }

            return JsonDataContractHandler.Deserialize<T>(decompressed);
        }
    }
}
