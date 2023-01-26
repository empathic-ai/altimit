using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Altimit
{
    public interface IDereferencedObject
    {
    }

    public interface IDereferencedObject<TReferencedObject> : IDereferencedObject
    {
    }
}