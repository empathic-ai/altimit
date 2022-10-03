using System;
using System.Linq;

namespace Altimit
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple = false, Inherited = true)]
    public class ConstructorAttribute : Attribute
    {
        public string[] PropertyNames { get; private set; }

        public ConstructorAttribute(params string[] propertyNames)
        {
            this.PropertyNames = propertyNames;
        }
    }
}