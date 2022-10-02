using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit
{
    //Use this to exclude methods from the stack trace
    [AttributeUsage(AttributeTargets.Method)]
    public class StackTraceIgnore : Attribute { }

}
