using Altimit;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Altimit
{
    [AType]
    public class AHashset<T> : HashSet<T>, IAList<T>
    {
        public event HashsetItemRemovedEventHandler<T> ItemRemoved;
        public event HashsetItemAddedEventHandler<T> ItemAdded;

        public event ElementRemovedEventHandler ElementRemoved;
        public event ElementAddedEventHandler ElementAdded;
        public event PropertyChangedEventHandler PropertyChanged;

        [AMethod]
        public new void Add(T item)
        {
            if (base.Add(item))
                OnItemAdded(item);
        }

        public new bool Remove(T item)
        {
            var contains = Contains(item);
            InternalRemove(item);
            return contains;
        }

        [AMethod]
        public void InternalRemove(T item)
        {
            base.Remove(item);
            OnItemRemoved(item);
        }

        protected virtual void OnItemAdded(T e)
        {
            ElementAdded?.Invoke(this, new ElementEventArgs(-1, e));
            ItemAdded?.Invoke(this, e);
        }

        protected virtual void OnItemRemoved(T e)
        {
            ElementRemoved?.Invoke(this, new ElementEventArgs(-1, e));
            ItemRemoved?.Invoke(this, e);
        }

    }
}