using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit
{
    public enum FormatType
    {
        // Treat this object as a value or reference based on its type
        Default = 0,
        // Flatten this object if its a reference
        //Value = 1 << 1,
        // Flatten this object if its a reference, and also do the same for or its sub-objects
        RecursiveValue = 1 << 2,
        // Don't localize or globalize
        Ignore = 1 << 3,
        // Include properties of this object that don't have IDs
        Verbose = 1 << 4
    }
}
