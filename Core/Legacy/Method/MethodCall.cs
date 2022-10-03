using System;

namespace Altimit
{
    public struct MethodCall
    {
        [AProperty]
        public object Instance;
        [AProperty]
        public Guid MethodID;
        [AProperty]
        public object[] Args;

        public MethodCall(object instance, Guid methodID, object[] args)
        {
            Instance = instance;
            MethodID = methodID;
            Args = args;
        }
    }
}

