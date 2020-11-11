using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Fux.Config
{
    /// <summary>
    /// This class maintains the configuration for Docker
    /// </summary>
    public class Docker
    {
        /// <summary>
        /// This property contains the dictionary of secrets from Docker
        /// </summary>
        private static readonly Dictionary<string, string> _secrets = new Dictionary<string, string>();

        /// <summary>
        /// This property contains the path to the Docker secrets
        /// </summary>
        private static string _secretsDirectory =
            (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "/ProgramData/Docker/secrets" : "/run/secrets");

        /// <summary>
        /// This property contains our JSON serializer settings
        /// </summary>
        private static JsonSerializerSettings _jsonSerializerSettings;

        /// <summary>
        /// This method reads the secrets from the file system
        /// </summary>
        private static void readSecrets()
        {
            // Check the secrets in memory and populate them
            if (!_secrets.Any())
                foreach (string s in Directory.GetFiles(_secretsDirectory))
                    _secrets.Add(s.ToLower(), File.ReadAllText(s));
        }

        /// <summary>
        /// This method asynchronously reads the secrets from the file system
        /// </summary>
        /// <returns></returns>
        private static async Task readSecretsAsync()
        {
            // Check the secrets in memory and populate them
            if (!_secrets.Any())
                foreach (string s in Directory.GetFiles(_secretsDirectory))
                    _secrets.Add(s.ToLower(), await File.ReadAllTextAsync(s));
        }

        /// <summary>
        /// This method resets the secrets directory path into memory
        /// </summary>
        /// <param name="directoryPath"></param>
        public static void SetSecretsDirectory(string directoryPath) =>
            _secretsDirectory = Path.GetFullPath(directoryPath);

        /// <summary>
        /// This method sets the JSON serializer settings into the instance
        /// </summary>
        /// <param name="jsonSerializerSettings"></param>
        /// <returns></returns>
        public static void SetSerializerSettings(JsonSerializerSettings jsonSerializerSettings) =>
            _jsonSerializerSettings = jsonSerializerSettings;

        /// <summary>
        /// This method sets the default JSON serializer settings into the instance
        /// </summary>
        /// <returns></returns>
        public static void SetSerializerSettings() =>
            SetSerializerSettings(new JsonSerializerSettings
        {
            DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFK",
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Include,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize
        });

        /// <summary>
        /// This method returns the string value of a Docker secret named <paramref name="key"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Get(string key)
        {
            // Ensure the secrets are in memory
            readSecrets();
            // Return the secret
            return _secrets
                .Where(s => s.Key.Equals(key.ToLower()))
                .Select(s => s.Value)
                .FirstOrDefault();
        }

        /// <summary>
        /// This method returns the typed value of a Docker secret name <paramref name="key"/>
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static TValue Get<TValue>(string key)
        {
            // Ensure the secrets are in memory
            readSecrets();
            // Localize the value
            string value = Get(key);
            // Try to deserialize the value
            try
            {
                // Return the deserialized object
                return JsonConvert.DeserializeObject<TValue>(value);
            }
            catch (JsonSerializationException)
            {
                // Return the converted value
                return (TValue) Convert.ChangeType(value, typeof(TValue));
            }
        }

        /// <summary>
        /// This method asynchronously returns the string value of a Docker secret name <paramref name="key"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<string> GetAsync(string key)
        {
            // Ensure the secrets are in memory
            await readSecretsAsync();
            // Return the secret
            return _secrets
                .Where(s => s.Key.Equals(key.ToLower()))
                .Select(s => s.Value)
                .FirstOrDefault();
        }

        /// <summary>
        /// This method asynchronously returns the typed value of a Docker secret named <paramref name="key"/>
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static async Task<TValue> GetAsync<TValue>(string key)
        {
            // Ensure the secrets are in memory
            await readSecretsAsync();
            // Localize the value
            string value = await GetAsync(key);
            // Try to deserialize the value
            try
            {
                // Return the deserialized object
                return JsonConvert.DeserializeObject<TValue>(value);
            }
            catch (JsonSerializationException)
            {
                // Return the converted value
                return (TValue) Convert.ChangeType(value, typeof(TValue));
            }
        }
    }
}