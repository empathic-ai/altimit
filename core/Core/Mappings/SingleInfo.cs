using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit
{
    [ATypeInfo(typeof(Single))]
    public class SingleInfo : ATypeInfo
    {
        public SingleInfo(Type type) : base(type) { }
        public override void InheritMaps()
        {
            base.InheritMaps();
        }
    }
}
