using Fux.Config.DockerHelper.Attribute;
using Fux.Core;

namespace Fux.Config.RedisHelper
{
    /// <summary>
    /// This class bootstraps a Redis connection from Docker secrets
    /// </summary>
    public class DockerConnection : Abstract.Connection
    {
        /// <summary>
        /// This method instantiates the connection from the environment
        /// </summary>
        public DockerConnection()
        {
            // Localize the docker settings construct
            DockerConnectionSettings connectionSettings =
                Convert.MapWithValueGetter<DockerConnectionSettings, string, DockerSecretNameAttribute>(attribute =>
                    Docker.Get(attribute.Name));
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
    /// This class bootstraps a Redis connection from Docker secrets with a generic connection settings provider
    /// </summary>
    /// <typeparam name="TSettings"></typeparam>
    public class DockerConnection<TSettings> : DockerConnection where TSettings : DockerConnectionSettings
    {
        /// <summary>
        /// This method instantiates the connection from the environment
        /// </summary>
        public DockerConnection()
        {
            // Localize the docker settings construct
            TSettings connectionSettings =
                Convert.MapWithValueGetter<TSettings, string, DockerSecretNameAttribute>(attribute =>
                    Docker.Get(attribute.Name));
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
