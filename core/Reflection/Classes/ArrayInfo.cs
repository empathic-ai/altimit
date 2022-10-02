using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit
{
    [ATypeInfo(typeof(Array<>))]
    public class ArrayInfo : TypeTypeInfo {

        public ArrayInfo(Type type) : base(type) {
        }
    }

    /*
    public class ArrayInfo<T> : TypeInfo<T[]>
    {

    }*/
}

