using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;

namespace NtpTests.utilities.serialization
{
    /// <summary>
    /// Utility class to read/write JSON from DataContract-annotated objects
    /// </summary>
    public static class JsonDataContractHandler
    {
        public static object? Deserialize(string jsonString, Type type)
        {
            return Deserialize(jsonString, type, new List<Type> { type });
        }

        public static object? Deserialize(string jsonString, Type type, List<Type> supportedTypes)
        {
            if (jsonString == null)
            {
                return default;
            }

            try
            {
                var deserializer = new DataContractJsonSerializer(type, supportedTypes);

                byte[] byteArray = Encoding.UTF8.GetBytes(jsonString);

                using (var stream = new MemoryStream(byteArray))
                {
                    return deserializer.ReadObject(stream);
                }
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static T? Deserialize<T>(string jsonString)
        {
            return Deserialize<T>(jsonString, new List<Type> { typeof(T) });
        }

        public static T? Deserialize<T>(string jsonString, List<Type> supportedTypes)
        {
            if (jsonString == null)
            {
                return default(T);
            }

            try
            {
                var deserializer = new DataContractJsonSerializer(typeof(T), supportedTypes);

                byte[] byteArray = Encoding.UTF8.GetBytes(jsonString);

                using (var stream = new MemoryStream(byteArray))
                {
                    var obj = deserializer.ReadObject(stream);
                    return obj != null ? (T)obj : default(T);
                }
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public static object? DeserializeFile(string filePath, Type type)
        {
            return DeserializeFile(filePath, type, new List<Type> { type });
        }

        public static object? DeserializeFile(string filePath, Type type, List<Type> supportedTypes)
        {
            if (filePath == null || !File.Exists(filePath))
            {
                return default;
            }

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    var deserializer = new DataContractJsonSerializer(type, supportedTypes);

                    return deserializer.ReadObject(stream);
                }
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static T? DeserializeFile<T>(string filePath)
        {
            return DeserializeFile<T>(filePath, new List<Type> { typeof(T) });
        }

        public static T? DeserializeFile<T>(string filePath, List<Type> supportedTypes)
        {
            if (filePath == null || !File.Exists(filePath))
            {
                return default(T);
            }

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    var deserializer = new DataContractJsonSerializer(typeof(T), supportedTypes);

                    var obj = deserializer.ReadObject(stream);
                    return obj != null ? (T)obj : default(T);
                }
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public static T? DeserializeFromResource<T>(string resourceKey, List<Type> supportedTypes, Assembly? containingAssembly = default)
        {
            try
            {
                Assembly assembly = containingAssembly == null ? Assembly.GetCallingAssembly() : containingAssembly;

                using (Stream? stream = assembly.GetManifestResourceStream(resourceKey))
                {
                    if (stream == null)
                    {
                        return default(T);
                    }

                    using (var streamReader = new StreamReader(stream))
                    {
                        var result = streamReader.ReadToEnd();

                        return Deserialize<T>(result);
                    }
                }
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public static string GetUniqueFilename(string filepath, string entity, string seedName)
        {
            string filePath = Path.Combine(filepath, $"{entity}_{seedName}.xml");
            var iterator = 0;

            while (File.Exists(filePath))
            {
                ++iterator;
                filePath = Path.Combine(filepath, $"{entity}_{seedName}_{iterator}.xml");
            }

            return filePath;
        }

        public static string? Serialize<T>(T obj)
        {
            return Serialize(obj, new List<Type> { typeof(T) });
        }

        public static string? Serialize<T>(T obj, List<Type> supportedTypes)
        {
            try
            {
                if (obj == null)
                {
                    return default;
                }

                using (var memStm = new MemoryStream())
                {
                    var mySerializer = new DataContractJsonSerializer(obj.GetType(), supportedTypes);
                    mySerializer.WriteObject(memStm, obj);

                    memStm.Seek(0, SeekOrigin.Begin);

                    using (var streamReader = new StreamReader(memStm))
                    {
                        string result = streamReader.ReadToEnd();

                        return result;
                    }
                }
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static bool SerializeFile<T>(string filePath, T obj)
        {
            return SerializeFile(filePath, obj, new List<Type> { typeof(T) });
        }

        public static bool SerializeFile<T>(string filePath, T obj, List<Type> supportedTypes)
        {
            if (obj == null)
            {
                return false;
            }

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    var mySerializer = new DataContractJsonSerializer(obj.GetType(), supportedTypes);
                    mySerializer.WriteObject(stream, obj);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
