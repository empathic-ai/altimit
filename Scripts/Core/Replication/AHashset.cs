using Altimit;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Altimit
{
    [AType]
    public class AHashset<T> : HashSet<T>
    {
        public event HashsetItemRemovedEventHandler<T> ItemRemoved;
        public event HashsetItemAddedEventHandler<T> ItemAdded;

        public event HashsetItemRemovedEventHandler<object> DynamicItemRemoved;
        public event HashsetItemAddedEventHandler<object> DynamicItemAdded;
        public event ListClearedEventHandler ListCleared;

        [AMethod]
        public new void Add(T item)
        {
            base.Add(item);
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
            DynamicItemAdded?.Invoke(this, e);
            ItemAdded?.Invoke(this, e);
        }

        protected virtual void OnItemRemoved(T e)
        {
            DynamicItemRemoved?.Invoke(this, e);
            ItemRemoved?.Invoke(this, e);
        }

    }
}