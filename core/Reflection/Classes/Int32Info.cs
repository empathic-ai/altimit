using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit
{
    [ATypeInfo(typeof(Int32))]
    public class Int32Info : ATypeInfo
    {
        public Int32Info(Type type) : base(type) { }
        public override void InheritMaps()
        {
            base.InheritMaps();
        }
    }
}
