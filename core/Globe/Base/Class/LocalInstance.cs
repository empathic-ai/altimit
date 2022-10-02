using System;

namespace Altimit
{
    [AType]
    //Used as a minimal, typeless storage unit for information concerning the contents of classes
    public struct LocalInstance
    {
        [AProperty]
        public Type Type;

        [AProperty]
        public object Value;

        public LocalInstance(Type type = null, object value = null)
        {
            Type = type;
            Value = value;
        }
    }
}

