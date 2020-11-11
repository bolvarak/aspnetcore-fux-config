using Fux.Core.Attribute;

namespace Fux.Config.DockerHelper.Attribute
{
    /// <summary>
    /// This attributes maintains correspondences between Docker secrets an POCO properties 
    /// </summary>
    public class DockerSecretNameAttribute : FromKeyAttribute
    {
        /// <summary>
        /// This method instantiates the attribute class with a secret name
        /// </summary>
        /// <param name="secretName"></param>
        public DockerSecretNameAttribute(string secretName) : base(secretName) { }
    }
}