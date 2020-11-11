using Microsoft.Extensions.Configuration;
using Fux.Config.RedisHelper.Abstract;

namespace Fux.Config.RedisHelper
{
    /// <summary>
    /// This class maintains the 
    /// </summary>
    public class AppSettingsConnection : Connection
    {
        /// <summary>
        /// This property contains the configuration section name for the Redis configuration details
        /// </summary>
        private static string _configurationSectionName = "Tux:Systems:Configuration:Redis";

        /// <summary>
        /// This method resets the configuration section name into memory
        /// </summary>
        /// <param name="configurationSectionName"></param>
        /// <returns></returns>
        private static string SetConfigurationSectionName(string configurationSectionName) =>
            _configurationSectionName = configurationSectionName;

        /// <summary>
        /// This method instantiates the connection settings from the appSettings.json file section
        /// </summary>
        /// <param name="configuration"></param>
        public AppSettingsConnection(IConfiguration configuration) : base()
        {
            // Localize the configuration section
            ConnectionSettings appSettings =
                configuration.GetSection(_configurationSectionName).Get<ConnectionSettings>();
            // Setup the instance
            WithAllowAdminFlag(appSettings.AllowAdmin)
                .WithHost(appSettings.Host)
                .WithPort(appSettings.Port)
                .WithPassword(appSettings.Password)
                .WithSerializerSettings()
                .WithSocketFlag(appSettings.IsUnixSocket)
                .WithSslFlag(appSettings.UseSsl);
        }

        /// <summary>
        /// This method instantiates the connection from the parsed appSettings.json file section
        /// </summary>
        /// <param name="appSettings"></param>
        private AppSettingsConnection(ConnectionSettings appSettings) : base(appSettings.Host, appSettings.Port,
            appSettings.Username, appSettings.Password) =>
            WithAllowAdminFlag(appSettings.AllowAdmin)
                .WithSocketFlag(appSettings.IsUnixSocket)
                .WithSslFlag(appSettings.UseSsl);
    }
}