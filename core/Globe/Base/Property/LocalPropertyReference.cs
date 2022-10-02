using System;

namespace Altimit
{
    public class LocalPropertyReference
    {
        [AProperty]
        public Guid ID;
        [AProperty]
        public int PropertyID;
        [AProperty]
        public object LocalProperty;

        public LocalPropertyReference()
        {
        }

        public LocalPropertyReference(Guid id, int propertyID, object localProperty)
        {
            ID = id;
            PropertyID = propertyID;
            LocalProperty = localProperty;
        }
    }
}

