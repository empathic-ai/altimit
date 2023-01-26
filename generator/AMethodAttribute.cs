using System;
using System.Linq;

namespace Altimit
{

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
}