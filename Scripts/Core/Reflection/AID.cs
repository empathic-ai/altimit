using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Altimit
{
    [AType]
    public interface IValue<T>
    {
        T Value { get; set; }
    }

    [AType]
    public struct AID : IValue<Guid>
    {
        public static AID Empty = new AID(Guid.Empty);

        public static AID New()
        {
            return new AID(Guid.NewGuid());
        }

        [AProperty]
        public Guid Value { get; set; }

        public AID(Guid id)
        {
            Value = id;
        }
        public AID(string id)
        {
            Value = new Guid(id);
        }

        public bool IsEmpty()
        {
            return this.Equals(AID.Empty);
        }

        public override string ToString()
        {
            return Value.ToAbbreviatedString();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is AID))
                return false;
            return Value.Equals(((AID)obj).Value);
        }

        // TODO: possible implement differently to avoid collisions better--use dictionary?
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        //public static implicit operator Guid(AID id) => id.ID;
    }
}






