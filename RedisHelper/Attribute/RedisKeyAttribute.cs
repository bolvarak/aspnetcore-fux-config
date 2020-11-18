namespace Fux.Config.RedisHelper.Attribute
{
    /// <summary>
    /// This class maintains the RedisKey attribute for classes
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Field | System.AttributeTargets.Property | System.AttributeTargets.Struct)]
    public class RedisKeyAttribute : Fux.Core.Attribute.FromKeyAttribute
    {
        /// <summary>
        /// This property tells Redis whether to throw an exception if the value doesn't exist or is empty
        /// </summary>
        /// <value></value>
        public bool AllowEmpty { get; set; }

        /// <summary>
        /// This method instantiates the attribute class with a key name
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="allowEmpty"></param>
        public RedisKeyAttribute(string redisKey, bool allowEmpty = true) : base(redisKey) =>
            AllowEmpty = allowEmpty;
    }
}
