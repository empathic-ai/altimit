using System;

namespace Altimit
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.ReturnValue | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class IgnoreAttribute : Attribute
    {
        //public bool IsRecursive { get; }

        public IgnoreAttribute()
        {
        }

        /*
        public ForceValueAttribute(bool isRecursive = false)
        {
            IsRecursive = isRecursive;
        }
        */
    }
}