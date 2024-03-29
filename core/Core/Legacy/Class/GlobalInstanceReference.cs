﻿using System;

namespace Altimit
{
    //Used as a minimal, typeless storage unit for information concerning a global
    public class GlobalInstanceReference
    {
        [AProperty]
        public Guid ID = Guid.Empty;
        [AProperty]
        public object Value;

        public GlobalInstanceReference()
        {
        }

        public GlobalInstanceReference(Guid id, object global)
        {
            ID = id;
            Value = global;
        }
    }
}