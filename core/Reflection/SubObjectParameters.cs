using System;

namespace Altimit
{
    public struct SubObjectParameters
    {
        public object Value;
        public string Name;

        public bool HasName()
        {
            return Name != null;
        }

        public SubObjectParameters(object value, string name)
        {
            Value = value;
            Name = name;
        }
    }
}
