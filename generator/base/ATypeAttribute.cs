using System;

namespace Altimit
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ATypeInfoAttribute : Attribute
    {
        public Type Type { get; }

        public ATypeInfoAttribute(Type type)
        {
            Type = type;
        }
    }

    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
    public class ATypeAttribute : Attribute
    {
        public bool ForcePolling = false;
        public ATypeAttribute()
        {
        }

        public ATypeAttribute(bool forcePolling = false)
        {
            ForcePolling = true;
        }
    }
}