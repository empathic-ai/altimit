using System;

namespace Altimit
{
    [Flags]
    //Determines how changes are observed from a property
    public enum ObserveType
    {
        None = 0,
        // Changes are observed incrementally (during the game's update loop)
        Mutable = 1 << 1,
        // Property is set right after object construction and before object is added to a globe
        // Occasionally needed when properties should be set by the time an object is added to a globe
        Construct = 1 << 2,
        Initial = 1 << 3,
        // This property is never serialized
        //NonSerializable = 1 << 4,
        All = ~None
    }
}