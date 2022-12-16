using System;
using System.Linq;

namespace Altimit
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple = false, Inherited = true)]
    public class AConstructorAttribute : Attribute
    {
        public string[] PropertyNames { get; private set; }

        public AConstructorAttribute(params string[] propertyNames)
        {
            this.PropertyNames = propertyNames;
        }
    }
}