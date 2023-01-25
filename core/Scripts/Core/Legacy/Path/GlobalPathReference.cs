namespace Altimit
{
    //Used as a minimal, typeless storage unit for information concerning changes in properties
    public class GlobalPathReference
    {
        [AProperty]
        public int[] IDs;
        [AProperty]
        public object Global;

        public GlobalPathReference()
        {
        }

        public GlobalPathReference(int[] ids, object global)
        {
            IDs = ids;
            Global = global;
        }
    }
}
