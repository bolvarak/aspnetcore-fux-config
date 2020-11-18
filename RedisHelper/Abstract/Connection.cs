using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Fux.Config.RedisHelper.Attribute;
using Fux.Core.Extension.String;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Fux.Config.RedisHelper.Abstract
{
    /// <summary>
    /// This class maintains the structure of a connection for Redis
    /// </summary>
    public abstract class Connection : IConnection
    {
        /// <summary>
        /// This property contains the Redis connection handle for the provider
        /// </summary>
        private ConnectionMultiplexer _connection = null;

        /// <summary>
        /// This property contains the UNIX domain socket flag for the host
        /// </summary>
        private bool _isUnixDomainSocket = false;

        /// <summary>
        /// This property contains our serializer settings for deserializing Redis values
        /// </summary>
        private JsonSerializerSettings _jsonSerializerSettings;

        /// <summary>
        /// This property contains the connection admin flag
        /// </summary>
        [JsonProperty("allowAdmin")]
        public bool AllowAdmin { get; set; } = false;

        /// <summary>
        /// This property contains the Redis database that will be used
        /// </summary>
        [JsonProperty("database")]
        public int DatabaseIndex { get; set; } = 0;

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
        public int? Port { get; set; } = 6379;

        /// <summary>
        /// This property contains
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; set; } = Environment.GetEnvironmentVariable("TUX_REDIS_USERNAME");

        /// <summary>
        /// This property contains the SSL connection flag
        /// </summary>
        [JsonProperty("useSsl")]
        public bool UseSsl { get; set; } = false;

        /// <summary>
        /// This method instantiates the connection from the environment
        /// </summary>
        public Connection() =>
            WithSerializerSettings().WithHost("localhost").WithPort(6379);

        /// <summary>
        /// This method instantiates the class with Redis authentication details
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public Connection(string username, string password) =>
            WithSerializerSettings().WithHost("").WithUsername(username).WithPassword(password);

        /// <summary>
        /// This method instantiates the class with connection information and uses defaults for missing information
        /// </summary>
        /// <param name="host"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public Connection(string host, string username, string password) =>
            WithSerializerSettings().WithHost(host).WithUsername(username).WithPassword(password);

        /// <summary>
        /// This method instantiates a connection with all of the values necessary to connect to Redis
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public Connection(string host, int? port, string username, string password) =>
            WithSerializerSettings().WithHost(host).WithPort(port).WithUsername(username).WithPassword(password);

        /// <summary>
        /// This method instantiates a connection with all of the values necessary to connect to Redis
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public Connection(string host, string port, string username, string password) =>
            WithSerializerSettings().WithHost(host).WithPort(port).WithUsername(username).WithPassword(password);

        /// <summary>
        /// This method ensures that we have a valid connection in the instance before trying to execute any operations
        /// </summary>
        private void ensureConnection()
        {
            // Check for a connection
            if (_connection != null) return;
            // Generate our configuration options
            ConfigurationOptions configurationOptions = generateConfigurationOptions();
            // Instantiate our connection
            _connection = ConnectionMultiplexer.Connect(configurationOptions);
        }

        /// <summary>
        /// This method asynchronously ensures that we have a valid connection in the instance before trying to execute any operations
        /// </summary>
        /// <returns></returns>
        public async Task ensureConnectionAsync()
        {
            // Check for a connection
            if (_connection != null) return;
            // Generate our configuration options
            ConfigurationOptions configurationOptions = generateConfigurationOptions();
            // Instantiate our connection
            _connection = await ConnectionMultiplexer.ConnectAsync(configurationOptions);
        }

        /// <summary>
        /// This method generates a ConfigurationOptions object from the instance
        /// </summary>
        /// <returns></returns>
        private ConfigurationOptions generateConfigurationOptions()
        {
            // Define our configuration options
            ConfigurationOptions configurationOptions = new ConfigurationOptions();
            // Set the AllowAdmin flag into the configuration options
            configurationOptions.AllowAdmin = AllowAdmin;
            // Set the UseSSL flag into the configuration options
            configurationOptions.Ssl = UseSsl;
            // Check the UNIX Domain Socket flag and add the endpoint to the configuration options
            if (_isUnixDomainSocket)
                configurationOptions.EndPoints.Add(new UnixDomainSocketEndPoint(Host));
            else
                configurationOptions.EndPoints.Add($"{Host}:{Port.Value.ToString()}");
            // Check for an authentication password and add it to the configuration options
            if (!string.IsNullOrEmpty(Password) && !string.IsNullOrWhiteSpace(Password))
                configurationOptions.Password = Password;
            // Check for an authentication username and add it to the configuration options
            if (!string.IsNullOrEmpty(Username) && !string.IsNullOrWhiteSpace(Password))
                configurationOptions.User = Username;
            // We're done, return the configuration options
            return configurationOptions;
        }

        /// <summary>
        /// This method retrieves a database interface from the Redis service
        /// </summary>
        /// <param name="databaseNumber"></param>
        /// <returns></returns>
        public virtual IDatabase Database(int databaseNumber)
        {
            // Set the database into the instance
            WithDatabaseAtIndex(databaseNumber);
            // Make sure we're connected
            ensureConnection();
            // We're done, return the database
            return _connection.GetDatabase(databaseNumber);
        }

        /// <summary>
        /// This method retrieves a database interface from the Redis service using the last used database number
        /// </summary>
        /// <returns></returns>
        public virtual IDatabase Database() =>
            Database(DatabaseIndex);

        /// <summary>
        /// This method asynchronously retrieves a database interface from the Redis service
        /// </summary>
        /// <param name="databaseNumber"></param>
        /// <returns></returns>
        public virtual async Task<IDatabase> DatabaseAsync(int databaseNumber)
        {
            // Set the database number into the instance
            WithDatabaseAtIndex(databaseNumber);
            // Make sure we're connected
            await ensureConnectionAsync();
            // We're done, return the database
            return _connection.GetDatabase(databaseNumber);
        }

        /// <summary>
        /// This method asynchronously retrieves a database interface from the Redis service using the last used database number
        /// </summary>
        /// <returns></returns>
        public virtual Task<IDatabase> DatabaseAsync() =>
            DatabaseAsync(DatabaseIndex);

        /// <summary>
        /// This method returns a value from Redis and will throw an exception
        /// if the value is empty and <paramref name="allowEmpty"/> is <code>false</code>
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="allowEmpty"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public virtual string Get(RedisKey redisKey, bool allowEmpty = true)
        {
            // Localize the value
            RedisValue redisValue = Database().StringGet(redisKey);
            // Check the value and allow empty flag and throw the exception
            if ((!redisValue.HasValue || redisValue.IsNullOrEmpty) && !allowEmpty)
                throw new Exception($"Redis Value Cannot Be Empty [{redisKey}]");
            // We're done, return the value
            return redisValue.ToString();
        }

        /// <summary>
        /// This method returns a typed value from Redis as <typeparamref name="TValue"/> and will throw
        /// an exception if the value is empty and <paramref name="allowEmpty"/> is <code>false</code>
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="allowEmpty"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public virtual TValue Get<TValue>(RedisKey redisKey, bool allowEmpty = true)
        {
            // Localize the value
            string redisValue = Get(redisKey, allowEmpty);
            // Try to deserialize the value as JSON
            try
            {
                // Deserialize the content and return the typed value
                return JsonConvert.DeserializeObject<TValue>(redisValue);
            }
            catch (JsonSerializationException)
            {
                // Return the converted type
                return (TValue)Convert.ChangeType(redisValue, typeof(TValue));
            }
        }

        /// <summary>
        /// This method loads a POCO from Redis stored as a JSON string using a
        /// <code>RedisKey</code> attribute at the class level
        /// </summary>
        /// <param name="allowEmpty"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public virtual TValue Get<TValue>(bool allowEmpty = true)
        {
            // Define our previous database index
            int? previousDatabase = null;
            // Load the value's type
            Type type = typeof(TValue);
            // Localize the database attribute
            RedisDatabaseAttribute databaseAttribute =
                (type.GetCustomAttributes(typeof(RedisDatabaseAttribute), false).FirstOrDefault() as RedisDatabaseAttribute);
            // Localize the key attribute
            RedisKeyAttribute keyAttribute =
                (type.GetCustomAttributes(typeof(RedisKey), false).FirstOrDefault() as RedisKeyAttribute);
            // Check for a database attribute
            if (databaseAttribute != null && databaseAttribute.Database >= 0)
            {
                // Set the previous database
                previousDatabase = DatabaseIndex;
                // Set the database index into the instance
                DatabaseIndex = databaseAttribute.Database;
            }
            // Make sure we have a key attribute
            if (keyAttribute == null) throw new System.Exception($"{typeof(TValue).FullName} Does Not Contain a RedisKey Attribute");
            // Load the value from Redis
            TValue value = Get<TValue>(keyAttribute.Name, allowEmpty);
            // Check to see if we stored the previous database and reset it into the instance
            if (previousDatabase.HasValue) DatabaseIndex = previousDatabase.Value;
            // We're done, return the value
            return value;
        }

        /// <summary>
        /// This method asynchronously returns a value from Redis and will throw an exception
        /// if the value is empty and <paramref name="allowEmpty"/> is <code>false</code>
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="allowEmpty"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public virtual async Task<string> GetAsync(RedisKey redisKey, bool allowEmpty = true)
        {
            // Localize our database
            IDatabase database = await DatabaseAsync();
            // Localize the value
            RedisValue redisValue = await database.StringGetAsync(redisKey);
            // Check the value and allow empty flag and throw the exception
            if ((!redisValue.HasValue || redisValue.IsNullOrEmpty) && !allowEmpty)
                throw new Exception($"Redis Value Cannot Be Empty [{redisKey}]");
            // We're done, return the value
            return redisValue.ToString();
        }

        /// <summary>
        /// This method asynchronously returns a typed value from Redis as <typeparamref name="TValue"/> and will throw
        /// an exception if the value is empty and <paramref name="allowEmpty"/> is <code>false</code>
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="allowEmpty"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public virtual async Task<TValue> GetAsync<TValue>(RedisKey redisKey, bool allowEmpty = true)
        {
            // Localize the value
            string redisValue = await GetAsync(redisKey, allowEmpty);
            // Try to deserialize the value as JSON
            try
            {
                // Deserialize the content and return the typed value
                return JsonConvert.DeserializeObject<TValue>(redisValue);
            }
            catch (JsonSerializationException)
            {
                // Return the converted type
                return (TValue)Convert.ChangeType(redisValue, typeof(TValue));
            }
        }

        /// <summary>
        /// This method asynchronously loads a POCO from Redis stored as a JSON string using a
        /// <code>RedisKey</code> attribute at the class level
        /// </summary>
        /// <param name="allowEmpty"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public virtual async Task<TValue> GetAsync<TValue>(bool allowEmpty = true)
        {
            // Define our previous database index
            int? previousDatabase = null;
            // Load the value's type
            Type type = typeof(TValue);
            // Localize the database attribute
            RedisDatabaseAttribute databaseAttribute =
                (type.GetCustomAttributes(typeof(RedisDatabaseAttribute), false).FirstOrDefault() as RedisDatabaseAttribute);
            // Localize the key attribute
            RedisKeyAttribute keyAttribute =
                (type.GetCustomAttributes(typeof(RedisKey), false).FirstOrDefault() as RedisKeyAttribute);
            // Check for a database attribute
            if (databaseAttribute != null && databaseAttribute.Database >= 0)
            {
                // Set the previous database
                previousDatabase = DatabaseIndex;
                // Set the database index into the instance
                DatabaseIndex = databaseAttribute.Database;
            }
            // Make sure we have a key attribute
            if (keyAttribute == null) throw new System.Exception($"{typeof(TValue).FullName} Does Not Contain a RedisKey Attribute");
            // Load the value from Redis
            TValue value = await GetAsync<TValue>(keyAttribute.Name, keyAttribute.AllowEmpty);
            // Check to see if we stored the previous database and reset it into the instance
            if (previousDatabase.HasValue) DatabaseIndex = previousDatabase.Value;
            // We're done, return the value
            return value;
        }

        /// <summary>
        /// This method loads and populates an object from Redis using the <code>RedisKeyAttribute</code>
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public virtual dynamic GetObject(Type objectType) =>
            Core.Convert.MapWithValueGetter(objectType,
                attribute => Get(new RedisKey(((RedisKeyAttribute)attribute).Name), ((RedisKeyAttribute)attribute).AllowEmpty), typeof(RedisKeyAttribute));

        /// <summary>
        /// This method loads and populates an object from Redis using the <code>RedisKeyAttribute</code>
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public virtual TValue GetObject<TValue>() =>
            Core.Convert.MapWithValueGetter<TValue, dynamic, RedisKeyAttribute>(attribute =>
                Get<dynamic>(attribute.Name, attribute.AllowEmpty));

        /// <summary>
        /// This method asynchronously loads and populates an object from Redis using the <code>RedisKeyAttribute</code>
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public virtual Task<dynamic> GetObjectAsync(Type objectType) =>
            Core.Convert.MapWithValueGetter<dynamic, dynamic, RedisKeyAttribute>(
                attribute => Get<dynamic>(attribute.Name, attribute.AllowEmpty));

        /// <summary>
        /// This method asynchronously loads and populates an object from Redis using the <code>RedisKeyAttribute</code>
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public virtual Task<TValue> GetObjectAsync<TValue>() =>
            Core.Convert.MapWithValueGetterAsync<TValue, dynamic, RedisKeyAttribute>(attribute =>
                GetAsync<dynamic>(attribute.Name, attribute.AllowEmpty));

        /// <summary>
        /// This method sets a value into Redis
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <returns></returns>
        public virtual IConnection Set(RedisKey redisKey, RedisValue redisValue)
        {
            // Set the value into the database
            Database().StringSet(redisKey, redisValue);
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method sets a typed value into Redis
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public virtual IConnection Set<TValue>(RedisKey redisKey, TValue redisValue)
        {
            // Define our string
            string value = redisValue.ToString();
            // Check the type
            if (typeof(TValue).IsClass && typeof(TValue).IsSerializable)
                value = JsonConvert.SerializeObject(value, _jsonSerializerSettings);
            // We're done, set the value into Redis
            return Set(redisKey, new RedisValue(value));
        }

        /// <summary>
        /// This method saves a POCO to Redis stored as a JSON string using a
        /// <code>RedisKey</code> attribute at the class level
        /// </summary>
        /// <param name="redisValue"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public virtual IConnection Set<TValue>(TValue redisValue)
        {
            // Define our previous database index
            int? previousDatabase = null;
            // Load the value's type
            Type type = typeof(TValue);
            // Localize the database attribute
            RedisDatabaseAttribute databaseAttribute =
                (type.GetCustomAttributes(typeof(RedisDatabaseAttribute), false).FirstOrDefault() as RedisDatabaseAttribute);
            // Localize the key attribute
            RedisKeyAttribute keyAttribute =
                (type.GetCustomAttributes(typeof(RedisKey), false).FirstOrDefault() as RedisKeyAttribute);
            // Check for a database attribute
            if (databaseAttribute != null && databaseAttribute.Database >= 0)
            {
                // Set the previous database
                previousDatabase = DatabaseIndex;
                // Set the database index into the instance
                DatabaseIndex = databaseAttribute.Database;
            }
            // Make sure we have a key attribute
            if (keyAttribute == null) throw new System.Exception($"{typeof(TValue).FullName} Does Not Contain a RedisKey Attribute");
            // Set the value into redis
            Set<TValue>(keyAttribute.Name, redisValue);
            // Check to see if we stored the previous database and reset it into the instance
            if (previousDatabase.HasValue) DatabaseIndex = previousDatabase.Value;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method asynchronously sets a value into Redis
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <returns></returns>
        public virtual async Task<IConnection> SetAsync(RedisKey redisKey, RedisValue redisValue)
        {
            // Localize the database
            IDatabase database = await DatabaseAsync();
            // Set the value into Redis
            await database.StringSetAsync(redisKey, redisValue);
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method asynchronously sets a typed value into Redis
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public virtual Task<IConnection> SetAsync<TValue>(RedisKey redisKey, TValue redisValue)
        {
            // Define our string
            string value;
            // Check the type
            if (typeof(TValue).IsClass && typeof(TValue).IsSerializable)
                value = JsonConvert.SerializeObject(redisValue, _jsonSerializerSettings);
            else
                value = redisValue.ToString();
            // We're done, set the value into Redis
            return SetAsync(redisKey, new RedisValue(value));
        }

        /// <summary>
        /// This method asynchronously saves a POCO to Redis stored as a JSON string using a
        /// <code>RedisKey</code> attribute at the class level
        /// </summary>
        /// <param name="redisValue"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public virtual async Task<IConnection> SetAsync<TValue>(TValue redisValue)
        {
            // Define our previous database index
            int? previousDatabase = null;
            // Load the value's type
            Type type = typeof(TValue);
            // Localize the database attribute
            RedisDatabaseAttribute databaseAttribute =
                (type.GetCustomAttributes(typeof(RedisDatabaseAttribute), false).FirstOrDefault() as RedisDatabaseAttribute);
            // Localize the key attribute
            RedisKeyAttribute keyAttribute =
                (type.GetCustomAttributes(typeof(RedisKey), false).FirstOrDefault() as RedisKeyAttribute);
            // Check for a database attribute
            if (databaseAttribute != null && databaseAttribute.Database >= 0)
            {
                // Set the previous database
                previousDatabase = DatabaseIndex;
                // Set the database index into the instance
                DatabaseIndex = databaseAttribute.Database;
            }
            // Make sure we have a key attribute
            if (keyAttribute == null) throw new System.Exception($"{typeof(TValue).FullName} Does Not Contain a RedisKey Attribute");
            // Set the value into redis
            await SetAsync<TValue>(keyAttribute.Name, redisValue);
            // Check to see if we stored the previous database and reset it into the instance
            if (previousDatabase.HasValue) DatabaseIndex = previousDatabase.Value;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method resets the allow admin flag into the instance
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public virtual IConnection WithAllowAdminFlag(bool flag)
        {
            // Reset the allow admin flag into the instance
            AllowAdmin = flag;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method resets the database index into the instance
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual IConnection WithDatabaseAtIndex(int index)
        {
            // Reset the database index into the instance
            DatabaseIndex = index;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method fluidly resets the host into the instance
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public virtual IConnection WithHost(string host)
        {
            // Check the host for a value and default it to localhost
            if (string.IsNullOrEmpty(host) || string.IsNullOrWhiteSpace(host))
                host = "localhost";
            // Reset the host into the instance
            Host = host;
            // Reset the socket flag
            _isUnixDomainSocket = false;
            // Check the host for a port notation
            if (Host.Contains(':')) WithPort(Host.Split(':').Last()).WithHost(Host.Split(':').First());
            // Check for a socket and reset the port
            if (File.Exists(Host))
            {
                // Reset the socket flag
                WithSocketFlag(true);
                // Reset the port into the instance
                Port = null;
            }
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method resets the socket flag into the instance
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public virtual IConnection WithSocketFlag(bool flag)
        {
            // Reset the socket flag into to the instance
            _isUnixDomainSocket = flag;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method fluidly resets the authentication password into the instance
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public virtual IConnection WithPassword(string password)
        {
            // Reset the password into the instance
            Password = password;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method fluidly resets the port on which the Redis service is listening
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public virtual IConnection WithPort(int? port)
        {
            // Make sure we have a value and reset it into the instance
            if (port.HasValue) Port = port;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method fluidly resets the port on which the Redis service is listening
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public virtual IConnection WithPort(string port)
        {
            // Check to see if the string is empty and reset the port
            if (!string.IsNullOrEmpty(port) && !string.IsNullOrWhiteSpace(port))
                WithPort(Convert.ToInt32(port));
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method sets the JSON serializer settings into the instance
        /// </summary>
        /// <param name="jsonSerializerSettings"></param>
        /// <returns></returns>
        public virtual IConnection WithSerializerSettings(JsonSerializerSettings jsonSerializerSettings)
        {
            // Reset the JSON serializer settings into the instance
            _jsonSerializerSettings = jsonSerializerSettings;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method sets the default JSON serializer settings into the instance
        /// </summary>
        /// <returns></returns>
        public virtual IConnection WithSerializerSettings()
        {
            // Instantiate our serializer settings
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
            // Define our date format handling
            jsonSerializerSettings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFK";
            // Define our formatting
            jsonSerializerSettings.Formatting = Formatting.None;
            // Define our null value handling
            jsonSerializerSettings.NullValueHandling = NullValueHandling.Include;
            // Define our reference loop handling
            jsonSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            // We're done, reset the serializer settings into the instance
            return WithSerializerSettings(jsonSerializerSettings);
        }

        /// <summary>
        /// This method resets the use SSL flag into the instance
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public IConnection WithSslFlag(bool flag)
        {
            // Reset the SSL flag into the instance
            UseSsl = flag;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method fluidly resets the username into the instance
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public virtual IConnection WithUsername(string username)
        {
            // Reset the username into the instance
            Username = username;
            // We're done, return the instance
            return this;
        }
    }
}
