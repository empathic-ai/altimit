using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Altimit
{
    public class AObject : IList
    {
        public Type Type;
        //public object[] Properties;
        public List<object> Properties = new List<object>();
        public int Count => Properties.Count;

        public bool IsFixedSize => Type.IsStructure();

        public bool IsReadOnly => false;

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public object this[int index] { get => Properties[index]; set => Properties[index] = value; }

        public AObject(Type type) {
            Type = type;
            
            foreach (var propertyInfo in Type.GetPropertyInfos())
            {
                Properties.Add(null);
            }
        }

        public IEnumerator GetEnumerator()
        {
            return Properties.GetEnumerator();
        }

        public int Add(object value)
        {
            Properties.Add(value);
            return Count;
        }

        public void Clear()
        {
            Properties.Clear();
        }

        public bool Contains(object value)
        {
            return Properties.Contains(value);
        }

        public int IndexOf(object value)
        {
            return Properties.IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            Properties.Insert(index, value);
        }

        public void Remove(object value)
        {
            Properties.Remove(value);
        }

        public void RemoveAt(int index)
        {
            Properties.RemoveAt(index);
        }

        public void CopyTo(Array array, int index)
        {
            Properties.CopyTo((object[])array, index);
        }
    }
}