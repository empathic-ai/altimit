namespace Altimit
{
    [AType]
    public struct LocalPropertyChange
    {
        [AProperty]
        public int ClassID;
        
        [AProperty]
        public int PropertyID;

        [AProperty]
        public object Local;

        [AConstructor(nameof(LocalPropertyChange.ClassID),
            nameof(LocalPropertyChange.PropertyID),
            nameof(LocalPropertyChange.Local))]
        public LocalPropertyChange(int classID, int propertyID, object local)
        {
            ClassID = classID;
            PropertyID = propertyID;
            Local = local;
        }
    }
}
