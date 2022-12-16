using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit
{
    [ATypeInfo(typeof(Tuple<,>))]
    public class TupleInfo : TypeTypeInfo {

        public TupleInfo(Type type) : base(type) {
        }

        public override void InheritMaps()
        {
            base.InheritMaps();
            var methodInfo = typeof(Tuple).GetMethods().Single(x => x.Name == nameof(Tuple.Create) && x.GetGenericArguments().Length == 2);
            MapProperty("Item1");
            MapProperty("Item2");
            MapConstructor(x => methodInfo.MakeGenericMethod(Type.GetGenericArguments()).Invoke(null, x), new string[]{ "Item1", "Item2"});
        }
    }
}

