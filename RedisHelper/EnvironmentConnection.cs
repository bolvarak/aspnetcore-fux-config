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
                    attribute => Docker.Get(attribute.Name));
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