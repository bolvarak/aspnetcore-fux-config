using System.Threading.Tasks;
using Newtonsoft.Json;
using Fux.Core.Extension.String;
using StackExchange.Redis;

namespace Fux.Config.RedisHelper.Abstract
{
    /// <summary>
    /// This interface provides a structure for configuring a Redis connection
    /// </summary>
    public interface IConnection
    {
        /// <summary>
        /// This property contains the connection admin flag
        /// </summary>
        [JsonProperty("allowAdmin")]
        public bool AllowAdmin { get; set; }

        /// <summary>
        /// This property contains the hostname on which the Redis service is listening
        /// </summary>
        [JsonProperty("host")]
        public string Host { get; set; }

        /// <summary>
        /// This property contains the password to use for authentication with the Redis service
        /// </summary>
        [JsonIgnore]
        public string Password { get; set; }

        /// <summary>
        /// This property masks the password when serializing
        /// </summary>
        [JsonProperty("password")]
        public string PasswordSafe => Password.Mask();

        /// <summary>
        /// This property contains the port on which the redis service is listening
        /// </summary>
        [JsonProperty("port")]
        public int? Port { get; set; }

        /// <summary>
        /// This property contains
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; set; }

        /// <summary>
        /// This property contains the SSL connection flag
        /// </summary>
        [JsonProperty("useSsl")]
        public bool UseSsl { get; set; }

        /// <summary>
        /// This method turns the AllowAdmin flag on for the connection
        /// </summary>
        /// <returns></returns>
        public IConnection AsAdmin() =>
            WithAllowAdminFlag(true);

        /// <summary>
        /// This method turns the AllowAdmin flag off for the connection
        /// </summary>
        /// <returns></returns>
        public IConnection AsNonAdmin() =>
            WithAllowAdminFlag(false);

        /// <summary>
        /// This method retrieves a database interface from the Redis service
        /// </summary>
        /// <param name="databaseNumber"></param>
        /// <returns></returns>
        public IDatabase Database(int databaseNumber);

        /// <summary>
        /// This method retrieves a database interface from the Redis service using the last used database number
        /// </summary>
        /// <returns></returns>
        public IDatabase Database();

        /// <summary>
        /// This method asynchronously retrieves a database interface from the Redis service
        /// </summary>
        /// <param name="databaseNumber"></param>
        /// <returns></returns>
        public Task<IDatabase> DatabaseAsync(int databaseNumber);

        /// <summary>
        /// This method asynchronously retrieves a database interface from the Redis service using the last used database number
        /// </summary>
        /// <returns></returns>
        public Task<IDatabase> DatabaseAsync();

        /// <summary>
        /// This method returns a value from Redis and will throw an exception
        /// if the value is empty and <paramref name="allowEmpty"/> is <code>false</code>
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="allowEmpty"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public string Get(RedisKey redisKey, bool allowEmpty = true);

        /// <summary>
        /// This method returns a typed value from Redis as <typeparamref name="TValue"/> and will throw
        /// an exception if the value is empty and <paramref name="allowEmpty"/> is <code>false</code>
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="allowEmpty"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public TValue Get<TValue>(RedisKey redisKey, bool allowEmpty = true);

        /// <summary>
        /// This method returns a value from Redis and will throw an exception
        /// if the value is empty and <paramref name="allowEmpty"/> is <code>false</code>
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="allowEmpty"></param>
        /// <returns></returns>
        public string Get(string redisKey, bool allowEmpty = true) =>
            Get(new RedisKey(redisKey), allowEmpty);

        /// <summary>
        /// This method returns a value from Redis
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public string Get(RedisKey redisKey) =>
            Get(redisKey, true);

        /// <summary>
        /// This method returns a value from Redis
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public string Get(string redisKey) =>
            Get(new RedisKey(redisKey), true);

        /// <summary>
        /// This method returns a typed value from Redis as <typeparamref name="TValue"/>
        /// </summary>
        /// <param name="redisKey"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public TValue Get<TValue>(RedisKey redisKey) =>
            Get<TValue>(redisKey, true);

        /// <summary>
        /// This method returns a typed value from Redis as <typeparamref name="TValue"/>
        /// </summary>
        /// <param name="redisKey"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public TValue Get<TValue>(string redisKey) =>
            Get<TValue>(new RedisKey(redisKey));

        /// <summary>
        /// This method loads a POCO from Redis stored as a JSON string using a
        /// <code>RedisKey</code> attribute at the class level
        /// </summary>
        /// <param name="allowEmpty"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public TValue Get<TValue>(bool allowEmpty = true);

        /// <summary>
        /// This method asynchronously returns a value from Redis and will throw an exception
        /// if the value is empty and <paramref name="allowEmpty"/> is <code>false</code>
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="allowEmpty"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public Task<string> GetAsync(RedisKey redisKey, bool allowEmpty = true);

        /// <summary>
        /// This method asynchronously returns a typed value from Redis as <typeparamref name="TValue"/> and will throw
        /// an exception if the value is empty and <paramref name="allowEmpty"/> is <code>false</code>
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="allowEmpty"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public Task<TValue> GetAsync<TValue>(RedisKey redisKey, bool allowEmpty = true);

        /// <summary>
        /// This method asynchronously returns a value from Redis and will throw an exception
        /// if the value is empty and <paramref name="allowEmpty"/> is <code>false</code>
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="allowEmpty"></param>
        /// <returns></returns>
        public Task<string> GetAsync(string redisKey, bool allowEmpty = true) =>
            GetAsync(new RedisKey(redisKey), allowEmpty);

        /// <summary>
        /// This method asynchronously returns a value from Redis
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public Task<string> GetAsync(RedisKey redisKey) =>
            GetAsync(redisKey, true);

        /// <summary>
        /// This method asynchronously returns a value from Redis
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public Task<string> GetAsync(string redisKey) =>
            GetAsync(new RedisKey(redisKey), true);

        /// <summary>
        /// This method asynchronously returns a typed value from Redis as <typeparamref name="TValue"/>
        /// </summary>
        /// <param name="redisKey"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public Task<TValue> GetAsync<TValue>(RedisKey redisKey) =>
            GetAsync<TValue>(redisKey, true);

        /// <summary>
        /// This method asynchronously returns a typed value from Redis as <typeparamref name="TValue"/>
        /// </summary>
        /// <param name="redisKey"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public Task<TValue> GetAsync<TValue>(string redisKey) =>
            GetAsync<TValue>(new RedisKey(redisKey), true);

        /// <summary>
        /// This method asynchronously loads a POCO from Redis stored as a JSON string using a
        /// <code>RedisKey</code> attribute at the class level
        /// </summary>
        /// <param name="allowEmpty"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public Task<TValue> GetAsync<TValue>(bool allowEmpty = true);

        /// <summary>
        /// This method sets a value into Redis
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <returns></returns>
        public IConnection Set(RedisKey redisKey, RedisValue redisValue);

        /// <summary>
        /// This method sets a value into Redis
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <returns></returns>
        public IConnection Set(string redisKey, string redisValue) =>
            Set(new RedisKey(redisKey), new RedisValue(redisValue));

        /// <summary>
        /// This method sets a typed value into Redis
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public IConnection Set<TValue>(RedisKey redisKey, TValue redisValue);

        /// <summary>
        /// This method sets a typed value into Redis
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public IConnection Set<TValue>(string redisKey, TValue redisValue) =>
            Set<TValue>(new RedisKey(redisKey), redisValue);

        /// <summary>
        /// This method asynchronously saves a POCO to Redis stored as a JSON string using a
        /// <code>RedisKey</code> attribute at the class level
        /// </summary>
        /// <param name="redisValue"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public IConnection Set<TValue>(TValue redisValue);

        /// <summary>
        /// This method asynchronously sets a value into Redis
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <returns></returns>
        public Task<IConnection> SetAsync(RedisKey redisKey, RedisValue redisValue);

        /// <summary>
        /// This method asynchronously sets a value into Redis
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <returns></returns>
        public Task<IConnection> SetAsync(string redisKey, string redisValue) =>
            SetAsync(new RedisKey(redisKey), new RedisValue(redisValue));

        /// <summary>
        /// This method asynchronously sets a typed value into Redis
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public Task<IConnection> SetAsync<TValue>(RedisKey redisKey, TValue redisValue);

        /// <summary>
        /// This method asynchronously sets a typed value into Redis
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public Task<IConnection> SetAsync<TValue>(string redisKey, TValue redisValue) =>
            SetAsync(new RedisKey(redisKey), redisValue);

        /// <summary>
        /// This method asynchronously saves a POCO to Redis stored as a JSON string using a
        /// <code>RedisKey</code> attribute at the class level
        /// </summary>
        /// <param name="redisValue"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public Task<IConnection> SetAsync<TValue>(TValue redisValue);

        /// <summary>
        /// This method forces any generic inheritors to a generic Connection type
        /// </summary>
        /// <returns></returns>
        public IConnection ToConnection();

        /// <summary>
        /// This method forces any generic inheritors to a hard Connection type
        /// </summary>
        /// <typeparam name="TConnection"></typeparam>
        /// <returns></returns>
        public TConnection ToConnection<TConnection>() where TConnection : Connection;

        /// <summary>
        /// This method resets the allow admin flag into the instance
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public IConnection WithAllowAdminFlag(bool flag);

        /// <summary>
        /// This method resets the database index into the instance
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IConnection WithDatabaseAtIndex(int index);

        /// <summary>
        /// This method fluidly resets the host into the instance
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public IConnection WithHost(string host);

        /// <summary>
        /// This method fluidly turns off SSL for the connection
        /// </summary>
        /// <returns></returns>
        public IConnection WithoutSsl() =>
            WithSslFlag(false);

        /// <summary>
        /// This method fluidly resets the authentication password into the instance
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public IConnection WithPassword(string password);

        /// <summary>
        /// This method fluidly resets the port on which the Redis service is listening
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public IConnection WithPort(int? port);

        /// <summary>
        /// This method fluidly resets the port on which the Redis service is listening
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public IConnection WithPort(string port);

        /// <summary>
        /// This method sets the JSON serializer settings into the instance
        /// </summary>
        /// <param name="jsonSerializerSettings"></param>
        /// <returns></returns>
        public IConnection WithSerializerSettings(JsonSerializerSettings jsonSerializerSettings);

        /// <summary>
        /// This method sets the default JSON serializer settings into the instance
        /// </summary>
        /// <returns></returns>
        public IConnection WithSerializerSettings();

        /// <summary>
        /// This method fluidly sets a socket path as the host into the instance
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public IConnection WithSocket(string filePath) =>
            WithHost(filePath).WithSocketFlag(true);

        /// <summary>
        /// This method resets the socket flag into the instance
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public IConnection WithSocketFlag(bool flag);

        /// <summary>
        /// This method fluidly turns on SSL for the connection
        /// </summary>
        /// <returns></returns>
        public IConnection WithSsl() =>
            WithSslFlag(true);

        /// <summary>
        /// This method resets the use SSL flag into the instance
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public IConnection WithSslFlag(bool flag);

        /// <summary>
        /// This method fluidly resets the username into the instance
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public IConnection WithUsername(string username);
    }
}
