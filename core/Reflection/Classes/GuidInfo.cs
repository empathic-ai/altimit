﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit
{
    [ATypeInfo(typeof(Guid))]
    public class GuidInfo : ATypeInfo
    {
        public GuidInfo(Type type) : base(type) { }
        public override void InheritMaps()
        {
            base.InheritMaps();
        }
    }
}
