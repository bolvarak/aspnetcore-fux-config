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
        public DockerConnection() : base()
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
}