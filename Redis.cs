using System;
using Fux.Config.RedisHelper;
using Fux.Core;
using Fux.Config.RedisHelper.Abstract;

namespace Fux.Config
{
    /// <summary>
    /// This class maintains our fluid interface for Redis connections
    /// </summary>
    public static class Redis
    {
        /// <summary>
        /// This method sets a new connection structure into memory
        /// </summary>
        /// <param name="connection"></param>
        public static IConnection Connect(IConnection connection) =>
            Singleton<IConnection>.Instance(connection);

        /// <summary>
        /// This method sets a new typed connection structure into memory
        /// </summary>
        /// <param name="connection"></param>
        /// <typeparam name="TConnection"></typeparam>
        /// <returns></returns>
        public static TConnection Connect<TConnection>(TConnection connection) where TConnection : IConnection =>
            Singleton<TConnection>.Instance(connection);

        /// <summary>
        /// This method sets up a Redis connection from Docker Secrets using only the configuration POCO
        /// </summary>
        /// <typeparam name="TDockerSettings"></typeparam>
        /// <returns></returns>
        public static DockerConnection ConnectFromDocker<TDockerSettings>() where TDockerSettings : DockerConnectionSettings =>
            new DockerConnection<TDockerSettings>().ToConnection<DockerConnection>();

        /// <summary>
        /// This method sets up a Redis connection from Environment Variables using only the configuration POCO
        /// </summary>
        /// <typeparam name="TEnvironmentSettings"></typeparam>
        /// <returns></returns>
        public static EnvironmentConnection ConnectFromEnvironment<TEnvironmentSettings>() where TEnvironmentSettings : EnvironmentConnectionSettings =>
            new EnvironmentConnection<TEnvironmentSettings>().ToConnection<EnvironmentConnection>();

        /// <summary>
        /// This method sets up a Redis connection from Docker Secrets
        /// </summary>
        /// <returns></returns>
        public static DockerConnection ConnectWithDockerSecrets() =>
            Connect(Reflection.Instance<DockerConnection>());

        /// <summary>
        /// This method sets up a Redis connection from Docker Secrets
        /// </summary>
        /// <typeparam name="TDockerConnection"></typeparam>
        /// <returns></returns>
        public static TDockerConnection ConnectWithDockerSecrets<TDockerConnection>() where TDockerConnection : DockerConnection =>
            Connect<TDockerConnection>(Reflection.Instance<TDockerConnection>());

        /// <summary>
        /// This method sets up a Redis connection from Environment Variables
        /// </summary>
        /// <returns></returns>
        public static EnvironmentConnection ConnectWithEnvironmentVariables() =>
            Connect(new EnvironmentConnection());

        /// <summary>
        /// This method sets up a Redis connection from Environment Variables
        /// </summary>
        /// <typeparam name="TEnvironmentConnection"></typeparam>
        /// <returns></returns>
        public static EnvironmentConnection ConnectWithEnvironmentVariables<TEnvironmentConnection>() where TEnvironmentConnection : EnvironmentConnection =>
            Connect<TEnvironmentConnection>(Reflection.Instance<TEnvironmentConnection>());

        /// <summary>
        /// This method returns a connection structure from memory
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static dynamic Connection(Type type) =>
            Singleton.Instance(type);

        /// <summary>
        /// This method returns a typed connection structure from memory
        /// </summary>
        /// <typeparam name="TConnection"></typeparam>
        /// /// <returns></returns>
        public static TConnection Connection<TConnection>() =>
            Singleton<TConnection>.Instance();
    }
}
