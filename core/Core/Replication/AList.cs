using Altimit;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Altimit
{
    public interface IAList<TElement> : IAList
    {

    }

    public interface IAList : IEnumerable, INotifyPropertyChanged
    {
        int Count { get; }
        /// <summary>
        /// Fired whenever list item has been changed, added or removed or when list has been cleared
        /// </summary>
        //public event ListChangedEventHandler<object> DynamicListChanged;
        /// <summary>
        /// Fired when list item has been removed from the list
        /// </summary>
        event ElementRemovedEventHandler ElementRemoved;
        /// <summary>
        /// Fired when item has been added to the list
        /// </summary>
        event ElementAddedEventHandler ElementAdded;
        /// <summary>
        /// Fired when list is cleared
        /// </summary>
        //public event ListClearedEventHandler ListCleared;
    }

    [AType]
    public class AList<T> : List<T>, IAList<T>, IList
    {
        public event Action OnChanged;
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
            OnChanged?.Invoke();
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