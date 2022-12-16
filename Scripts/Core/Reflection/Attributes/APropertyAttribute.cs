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


    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AMethodAttribute : Attribute
    {
        public ProtocolType ProtocolType { get; private set; } = ProtocolType.Sequential;

        public AMethodAttribute()
        {
        }

        public AMethodAttribute(ProtocolType protocolType)
        {
            this.ProtocolType = protocolType;
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class APropertyAttribute : Attribute
    {
        public ObserveType ObserveType { get; private set; } = ObserveType.Mutable;
        public Func<object, object, bool> EqualityCheck;

        public ProtocolType ProtocolType { get; private set; } = ProtocolType.Sequential;

        public APropertyAttribute()
        {
        }

        public APropertyAttribute(ProtocolType protocolType)
        {
            this.ProtocolType = protocolType;
        }

        public APropertyAttribute(ObserveType serializeType)
        {
            this.ObserveType = serializeType;
        }

        public APropertyAttribute(Func<object, object, bool> equalityCheck)
        {
            this.EqualityCheck = equalityCheck;
        }

        public APropertyAttribute(ObserveType serializeType, Func<object, object, bool> equalityCheck)
        {
            this.ObserveType = serializeType;
            this.EqualityCheck = equalityCheck;
        }
        
        /*
        public IDAttribute(string x)
        {
            this.StringKey = x;
        }
        */
    }
}