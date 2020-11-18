using Fux.Core;
using Fux.Config.EnvironmentHelper.Attribute;

namespace Fux.Config.RedisHelper
{
    /// <summary>
    /// This class bootstraps a Redis connection from Environment variables
    /// </summary>
    public class EnvironmentConnection : Abstract.Connection
    {
        /// <summary>
        /// This method instantiates the connection from the environment
        /// </summary>
        public EnvironmentConnection() : base()
        {
            // Localize the docker settings construct
            EnvironmentConnectionSettings connectionSettings =
                Convert.MapWithValueGetter<EnvironmentConnectionSettings, string, EnvironmentVariableAttribute>(
                    attribute => System.Environment.GetEnvironmentVariable(attribute.Name).ToString());
            // Setup the instance
            WithAllowAdminFlag(connectionSettings.AllowAdmin)
                .WithHost(connectionSettings.Host)
                .WithPassword(connectionSettings.Password)
                .WithPort(connectionSettings.Port)
                .WithSerializerSettings()
                .WithSocketFlag(connectionSettings.IsUnixSocket)
                .WithSslFlag(connectionSettings.UseSsl)
                .WithUsername(connectionSettings.Username);
        }
    }

    /// <summary>
    /// This class provides a generic for connections using Environment Variables
    /// </summary>
    /// <typeparam name="TSettings"></typeparam>
    public class EnvironmentConnection<TSettings> : Abstract.Connection where TSettings : EnvironmentConnectionSettings
    {
        /// <summary>
        /// This method instantiates the environment connection from a custom settings class
        /// </summary>
        /// <returns></returns>
        public EnvironmentConnection() : base()
        {
            // Localize the docker settings construct
            TSettings connectionSettings =
                Convert.MapWithValueGetter<TSettings, string, EnvironmentVariableAttribute>(
                    attribute => System.Environment.GetEnvironmentVariable(attribute.Name).ToString());
            // Setup the instance
            WithAllowAdminFlag(connectionSettings.AllowAdmin)
                .WithHost(connectionSettings.Host)
                .WithPassword(connectionSettings.Password)
                .WithPort(connectionSettings.Port)
                .WithSerializerSettings()
                .WithSocketFlag(connectionSettings.IsUnixSocket)
                .WithSslFlag(connectionSettings.UseSsl)
                .WithUsername(connectionSettings.Username);
        }
    }
}
