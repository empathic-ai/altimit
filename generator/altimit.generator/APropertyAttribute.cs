using System;

namespace Altimit
{

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