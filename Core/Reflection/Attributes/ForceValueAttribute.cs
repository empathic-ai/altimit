using System;

namespace Altimit
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = true)]
    public class ForceValueAttribute : Attribute
    {
        public ForceValueAttribute()
        {
        }
    }
}