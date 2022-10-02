using System;
using System.Reflection;
using System.Linq;

namespace Altimit
{
    public class AConstructorInfo
    {
        public Type Type { get; }
        public string[] PropertyNames { get; }
        private Func<object[], object> Constructor { get; }

        public AConstructorInfo(Type type, Func<object[], object> constructor, string[] propertyNames)
        {
            Type = type;
            this.Constructor = constructor;
            this.PropertyNames = propertyNames;
        }

        public AConstructorInfo(Type type, System.Reflection.ConstructorInfo constructorInfo, string[] propertyNames = null)
        {
            Type = type;
            PropertyNames = propertyNames;
            Constructor = x => { return Activator.CreateInstance(Type, x);};
        }

        public object Construct(object[] args)
        {
            return Constructor.Invoke(args);
        }
    }
}
