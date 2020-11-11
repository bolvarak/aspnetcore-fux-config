using Fux.Core.Attribute;

namespace Fux.Config.EnvironmentHelper.Attribute
{
    public class EnvironmentVariableAttribute : FromKeyAttribute
    {
        /// <summary>
        /// This method instantiates our attribute with a variable name
        /// </summary>
        /// <param name="variableName"></param>
        public EnvironmentVariableAttribute(string variableName) : base(variableName) { }
    }
}