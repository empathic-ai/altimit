using Altimit;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Altimit
{
    [AType]
    public class ADictionary<TKey, TValue> : Dictionary<TKey, TValue>, IObservableList
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

    public class ElementEventArgs : EventArgs
    {
        public int index;
        public object element;
        public ElementEventArgs(int index, object item)
        {
            this.index = index;
            this.element = item;
        }
    }

    public delegate void ElementAddedEventHandler(object source, ElementEventArgs e);
    public delegate void ElementRemovedEventHandler(object source, ElementEventArgs e);
    public delegate void ListChangedEventHandler(object source, ElementEventArgs e);
    public delegate void ListClearedEventHandler(object source, EventArgs e);

    public delegate void HashsetItemAddedEventHandler<T>(object source, T e);
    public delegate void HashsetItemRemovedEventHandler<T>(object source, T e);
    public delegate void HashsetClearedEventHandler(object source, EventArgs e);

    public interface IObservableList : IEnumerable, INotifyPropertyChanged
    {
        public int Count { get; }
        /// <summary>
        /// Fired whenever list item has been changed, added or removed or when list has been cleared
        /// </summary>
        //public event ListChangedEventHandler<object> DynamicListChanged;
        /// <summary>
        /// Fired when list item has been removed from the list
        /// </summary>
        public event ElementRemovedEventHandler ElementRemoved;
        /// <summary>
        /// Fired when item has been added to the list
        /// </summary>
        public event ElementAddedEventHandler ElementAdded;
        /// <summary>
        /// Fired when list is cleared
        /// </summary>
        //public event ListClearedEventHandler ListCleared;
    }

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

    [AType]
    public class AList<T> : List<T>, IObservableList, IList
    {
        public event Action<T> OnChanged;
        public event Action<T> OnRemoved;
        public event Action<T> OnAdded;

        public event ElementRemovedEventHandler ElementRemoved;
        public event ElementAddedEventHandler ElementAdded;
        public event PropertyChangedEventHandler PropertyChanged;

        public AList() : base()
        {
        }

        protected virtual void OnElementAdded(int index, T e)
        {
            ElementAdded?.Invoke(this, new ElementEventArgs(index, e));
            OnAdded?.Invoke(e);
        }

        protected virtual void OnElementRemoved(int index, T e)
        {
            ElementRemoved?.Invoke(this, new ElementEventArgs(index, e));
            OnRemoved?.Invoke(e);
        }

        protected virtual void OnElementChanged(int index, T element)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(index.ToString(), element));
            OnChanged?.Invoke(element);
        }

        protected virtual void OnListCleared(EventArgs e)
        {
            //ListCleared?.Invoke(this, e);
        }

        public new int IndexOf(T item)
        {
            return base.IndexOf(item);
        }

        void IList.Insert(int index, object item)
        {
            base.Insert(index, (T)item);
            OnElementAdded(index, (T)item);
        }

        public new void Insert(int index, T item)
        {
            base.Insert(index, item);
            OnElementAdded(index, item);
            //OnElementChanged(index, item);
        }

        public new void RemoveAt(int index)
        {
            T item = this[index];
            Remove(item);
        }

        public new T this[int index]
        {
            get { return base[index]; }
            set
            {
                if (!base[index].Equals(value))
                {
                    var oldValue = base[index];
                    base[index] = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(index.ToString(), oldValue));
                }
            }
        }

        int IList.Add(object value)
        {
            Add((T)value);
            return Count;
        }

        //[AMethod]
        public new void Add(T item)
        {
            base.Add(item);
            OnElementAdded(IndexOf(item), item);
        }

        public new void AddRange(IEnumerable<T> list)
        {
            foreach (var e in list)
            {
                Add(e);
            }
        }

        public new void Clear()
        {
            while (Count > 0)
            {
                Remove(this[0]);
            }
            //internalList.Clear();
            OnListCleared(new EventArgs());
        }

        public void Reset(IEnumerable<T> list)
        {
            Clear();
            AddRange(list);
        }

        public new bool Remove(T item)
        {
            var contains = Contains(item);
            if (contains)
                InternalRemove(item);
            return contains;
        }

        //[AMethod]
        public void InternalRemove(T item)
        {
            int index = IndexOf(item);
            base.Remove(item);
            OnElementRemoved(index, item);
        }
    }
}