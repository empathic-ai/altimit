using System;
using System.Collections.Generic;

namespace Altimit
{
    [ATypeInfo(typeof(KeyValuePair<,>))]
    public class KeyValuePairInfo : ATypeInfo //<KeyValuePair<TKey, TValue>>
    {
        public KeyValuePairInfo(Type type) : base(type) { }

        public override void InheritMaps()
        {
            base.InheritMaps();
            MapProperty(nameof(KeyValuePair<object,object>.Key));
            MapProperty(nameof(KeyValuePair<object, object>.Value));
            MapConstructor(Constructor, nameof(KeyValuePair<object, object>.Key), nameof(KeyValuePair<object, object>.Value));
        }

        public object Constructor(params object[] args)
        {
            return Activator.CreateInstance(Type, args[0], args[1]);
        }
    }
}



