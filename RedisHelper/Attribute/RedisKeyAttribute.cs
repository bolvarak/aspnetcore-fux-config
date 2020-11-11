namespace Fux.Config.RedisHelper.Attribute
{
    /// <summary>
    /// This class maintains the RedisKey attribute for classes
    /// </summary>
    public class RedisKeyAttribute : Fux.Core.Attribute.FromKeyAttribute 
    {
        /// <summary>
        /// This method instantiates the attribute class with a key name
        /// </summary>
        /// <param name="redisKey"></param>
        public RedisKeyAttribute(string redisKey) : base(redisKey) { }
    }
}