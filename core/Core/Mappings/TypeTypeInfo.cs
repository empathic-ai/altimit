using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit
{
    [ATypeInfo(typeof(Type))]
    public class TypeTypeInfo : ATypeInfo
    {
        public TypeTypeInfo(Type type) : base(type)
        {
        }

        public override void InheritMaps()
        {
            base.InheritMaps();
        }
    }
}
