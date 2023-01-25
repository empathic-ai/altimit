using Altimit;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Altimit
{
    [AType]
    public class ADictionary<TKey, TValue> : Dictionary<TKey, TValue>, IAList<KeyValuePair<TKey, TValue>>
    {
        public event ElementAddedEventHandler ElementAdded;
        public event ElementRemovedEventHandler ElementRemoved;
        public event PropertyChangedEventHandler PropertyChanged;

        public new TValue this[TKey key] {
            get => base[key];
            set {
                if (!base[key].Equals(value))
                {
                    var oldValue = base[key];
                    base[key] = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("", oldValue));
                }
            }
        }

        public new void Add(TKey key, TValue value)
        {
            base.Add(key, value);
            ElementAdded?.Invoke(this, new ElementEventArgs(-1, new KeyValuePair<TKey, TValue>(key, value)));
        }

        public new bool Remove(TKey key)
        {
            if (ContainsKey(key))
            {
                var value = this[key];
                base.Remove(key);
                ElementRemoved?.Invoke(this, new ElementEventArgs(-1, new KeyValuePair<TKey, TValue>(key, value)));
                return true;
            }
            return false;
        }
    }
}