using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit
{
    [ATypeInfo(typeof(byte))]
    public class ByteInfo : ATypeInfo
    {
        public ByteInfo(Type type) : base(type) { }
        public override void InheritMaps()
        {
            base.InheritMaps();
        }
    }
}
