using System;

namespace Altimit
{
    public struct LocalPropertyPath
    {
        [AProperty]
        public int GlobalID;
        [AProperty]
        public int PropertyID;

        public LocalPropertyPath(int globalID, int propertyID)
        {
            GlobalID = globalID;
            PropertyID = propertyID;
        }
    }
}

