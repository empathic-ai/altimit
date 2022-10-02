using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class RequireModuleAttribute : Attribute
    {
        public Type Type { get; }

        public RequireModuleAttribute(Type type)
        {
            Type = type;
        }
    }
}
