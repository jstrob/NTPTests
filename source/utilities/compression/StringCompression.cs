using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace NtpTests.utilities
{
    public static class StringCompression
    {
        #region Public Methods

        /// <summary>
        ///     Compress using GZip Stream and convert the result into base64
        /// </summary>
        /// <param name="input">Uncompressed Input</param>
        /// <returns>Compressed Output</returns>
        public static string? CompressToBase64(string input)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            string? result = default;

            using (var outputStream = new MemoryStream())
            {
                using (var gZipStream = new GZipStream(outputStream, CompressionMode.Compress))
                {
                    gZipStream.Write(inputBytes, 0, inputBytes.Length);
                }

                byte[] outputBytes = outputStream.ToArray();

                result = Convert.ToBase64String(outputBytes);
            }

            return result;
        }

        /// <summary>
        ///     Compress using GZip Stream and convert the result into base64
        /// </summary>
        /// <param name="input">Uncompressed Input</param>
        /// <returns>Compressed Output</returns>
        public static byte[] CompressToByteArray(string input)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            byte[] outputBytes;

            using (var outputStream = new MemoryStream())
            {
                using (var gZipStream = new GZipStream(outputStream, CompressionMode.Compress))
                {
                    gZipStream.Write(inputBytes, 0, inputBytes.Length);
                }

                outputBytes = outputStream.ToArray();
            }

            return outputBytes;
        }

        /// <summary>
        ///     Convert from base64 and decompress using GZip Stream
        /// </summary>
        /// <param name="input">Compressed Input</param>
        /// <returns>Decompressed Output</returns>
        public static string DecompressFromBase64(string input)
        {
            byte[] decompressInput = Convert.FromBase64String(input);

            string decompressed;

            using (var inputStream = new MemoryStream(decompressInput))
            {
                using (var gZipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                {
                    using (var streamReader = new StreamReader(gZipStream))
                    {
                        decompressed = streamReader.ReadToEnd();
                    }
                }
            }

            return decompressed;
        }

        /// <summary>
        ///     Convert from base64 and decompress using GZip Stream
        /// </summary>
        /// <param name="input">Compressed Input</param>
        /// <returns>Decompressed Output</returns>
        public static string DecompressFromByteArray(byte[] input)
        {
            string decompressed;

            using (var inputStream = new MemoryStream(input))
            {
                using (var gZipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                {
                    using (var streamReader = new StreamReader(gZipStream))
                    {
                        decompressed = streamReader.ReadToEnd();
                    }
                }
            }

            return decompressed;
        }

        #endregion
    }
}
