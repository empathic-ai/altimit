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
    public struct DereferencedAID : IDereferencedObject<AID>
    {
        [AProperty]
        public Guid Value { get; set; }
        
        public DereferencedAID(Guid id)
        {
            Value = id;
        }
    }


    [AType]
    public struct AID : IValue<Guid>, IComparable<AID>, IEquatable<AID>
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
            return this.Equals(Guid.Empty);
        }

        public override string ToString()
        {
            return Value.ToString();
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

        public int CompareTo(AID other)
        {
            return Value.CompareTo(other.Value);
        }

        public bool Equals(AID other)
        {
            return Value.Equals(other.Value);
        }

        public static bool operator ==(AID a, AID b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(AID a, AID b)
        {
            return !a.Equals(b);
        }
    }
}
