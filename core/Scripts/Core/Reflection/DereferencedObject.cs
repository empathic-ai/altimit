using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Altimit
{
    // Represents a dynamically generated dereferened version of type T, compatible with Altimit's reflection system
    // Has largely been replaced with SourceGenerators that generate dereferenced types
    //[AType]
    public class DereferencedObject<TReferencedType> : AObject, IDereferencedObject<TReferencedType> where TReferencedType : class
    {
        public AID ID { get; set; }

        public DereferencedObject() : base(typeof(DereferencedObject<TReferencedType>))
        {
        }
    }
}