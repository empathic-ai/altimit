using System;
using System.Collections.Generic;

namespace Altimit
{
    [ATypeInfo(typeof(Dictionary<,>))]
    public class DictionaryInfo : ATypeInfo
    {
        public DictionaryInfo(Type type) : base(type) { }

        public override void InheritMaps()
        {
            base.InheritMaps();
        }
    }
}



