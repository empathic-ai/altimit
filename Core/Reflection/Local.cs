using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Altimit
{
    [AType]
    public class Local<T> : AObject
    {
        public Local() : base(typeof(Local<T>))
        {
        }
    }
}