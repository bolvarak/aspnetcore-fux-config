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
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Connect<T>(T connection) where T : IConnection =>
            Singleton<T>.Instance(connection);

        /// <summary>
        /// This method sets up a Docker typed connection structure into memory
        /// </summary>
        /// <returns></returns>
        public static IConnection ConnectFromDocker() =>
            Connect(new DockerConnection());

        /// <summary>
        /// This method sets up a Docker typed connection structure into memory
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static DockerConnection ConnectFromDocket<T>() =>
            Connect<DockerConnection>(new DockerConnection());

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
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Connection<T>() =>
            Singleton<T>.Instance();
    }
}