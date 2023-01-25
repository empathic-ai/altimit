using System;

namespace Altimit
{
    [AType]
    public struct NetworkMethodCall
    {
        //[AProperty]
        //public string TypeName { get; set; }
        [AProperty]
        public string MethodName { get; set; }
        [AProperty]
        public Type[] MethodTypes { get; set; }
        [AProperty]
        public int OrderID { get; set; }
        [AProperty]
        public int TaskID { get; set; }
        [AProperty]
        public bool IsResponse { get; set; }
        [AProperty(ObserveType.Mutable)]
        public object[] Args { get; set; }

        [AConstructor(nameof(NetworkMethodCall.OrderID), nameof(NetworkMethodCall.TaskID), nameof(NetworkMethodCall.MethodName), nameof(NetworkMethodCall.MethodTypes), nameof(NetworkMethodCall.IsResponse), nameof(NetworkMethodCall.Args))]
        public NetworkMethodCall(int orderID, int taskID, string methodName, Type[] methodTypes, bool isReponse, object[] args)
        {
            //TypeName = typeName;
            OrderID = orderID;
            IsResponse = isReponse;
            MethodName = methodName;
            MethodTypes = methodTypes;
            TaskID = taskID;
            Args = args;
        }
    }
}
