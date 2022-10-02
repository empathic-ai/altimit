using System;

namespace Altimit
{
    public struct PropertyChange
    {
        [AProperty]
        public object Instance;
        [AProperty]
        public string PropertyName;
        [AProperty]
        public object Value;

        public PropertyChange(object instance, string propertyName, object value)
        {
            Instance = instance;
            PropertyName = propertyName;
            Value = value;
        }
    }
}

