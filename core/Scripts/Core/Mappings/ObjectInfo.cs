using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit
{
    [ATypeInfo(typeof(object))]
    public class ObjectInfo : ATypeInfo
    {
        public ObjectInfo(Type type) : base(type) { }
        public override void InheritMaps()
        {
            base.InheritMaps();
        }
    }
}
