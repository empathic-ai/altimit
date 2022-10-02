using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Altimit
{
    [ATypeInfo(typeof(CancellationToken))]
    public class CancellationTokenInfo : TypeTypeInfo {

        public CancellationTokenInfo(Type type) : base(type) {
        }
    }

    /*
    public class ArrayInfo<T> : TypeInfo<T[]>
    {

    }*/
}

