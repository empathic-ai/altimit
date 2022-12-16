using System;
using System.Collections;
using System.Collections.Generic;

namespace Altimit
{
    [System.Serializable]
    public class DictionaryList<TFirst, TSecond> : IDictionary<TSecond, TFirst>
    {

        public IDictionary<TFirst, List<TSecond>> firstToSecond = new Dictionary<TFirst, List<TSecond>>();
        public IDictionary<TSecond, TFirst> secondToFirst = new Dictionary<TSecond, TFirst>();

        #region Exception throwing methods

        /// <summary>
        /// Tries to add the pair to the dictionary.
        /// Throws an exception if either element is already in the dictionary
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        public void Add(TFirst first, TSecond second)
        {
            if (secondToFirst.ContainsKey(second))
                throw new ArgumentException("Duplicate second");

            if (!firstToSecond.AddOrGet(first).TryAdd(second))
                throw new ArgumentException("Duplicate first");

            secondToFirst.Add(second, first);
        }

        public virtual IDictionary<TFirst, List<TSecond>> GetFirst()
        {
            return firstToSecond;
        }

        public virtual IDictionary<TSecond, TFirst> GetSecond()
        {
            return secondToFirst;
        }

        /// <summary>
        /// Find the TSecond corresponding to the TFirst first
        /// Throws an exception if first is not in the dictionary.
        /// </summary>
        /// <param name="first">the key to search for</param>
        /// <returns>the value corresponding to first</returns>
        public virtual List<TSecond> GetByFirst(TFirst first)
        {
            List<TSecond> second;
            if (!firstToSecond.TryGetValue(first, out second))
                throw new ArgumentException("Failed to retrieve second type " + typeof(TSecond).Name +
                    " from first type " + typeof(TFirst).Name + " with value " + first.ToString() + ".");

            return second;
        }

        /// <summary>
        /// Find the TFirst corresponing to the Second second.
        /// Throws an exception if second is not in the dictionary.
        /// </summary>
        /// <param name="second">the key to search for</param>
        /// <returns>the value corresponding to second</returns>
        public virtual TFirst GetBySecond(TSecond second)
        {
            TFirst first;
            if (!secondToFirst.TryGetValue(second, out first))
                throw new ArgumentException("Failed to retrieve first type " + typeof(TFirst).Name +
                    " from second type " + typeof(TSecond).Name + " with value " + second.ToString() + ".");

            return first;
        }


        /// <summary>
        /// Remove the record containing first.
        /// If first is not in the dictionary, throws an Exception.
        /// </summary>
        /// <param name="first">the key of the record to delete</param>
        public void RemoveByFirst(TFirst first)
        {
            List<TSecond> secondList;
            if (!firstToSecond.TryGetValue(first, out secondList))
                throw new ArgumentException("first");

            firstToSecond.Remove(first);
            foreach (var second in secondList)
                secondToFirst.Remove(second);
        }

        /// <summary>
        /// Remove the record containing second.
        /// If second is not in the dictionary, throws an Exception.
        /// </summary>
        /// <param name="second">the key of the record to delete</param>
        public void RemoveBySecond(TSecond second)
        {
            TFirst first;
            if (!secondToFirst.TryGetValue(second, out first))
                throw new ArgumentException("second");

            secondToFirst.Remove(second);
            List<TSecond> secondList = firstToSecond.AddOrGet(first);
            secondList.Remove(second);
            if (secondList.Count == 0)
                firstToSecond.Remove(first);
        }

        #endregion

        #region Try methods

        /// <summary>
        /// Tries to add the pair to the dictionary.
        /// Returns false if either element is already in the dictionary        
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns>true if successfully added, false if either element are already in the dictionary</returns>
        public Boolean TryAdd(TFirst first, TSecond second)
        {
            if (firstToSecond.ContainsKey(first) || secondToFirst.ContainsKey(second))
                return false;

            firstToSecond.AddOrGet(first).TryAdd(second);
            secondToFirst.Add(second, first);
            return true;
        }


        /// <summary>
        /// Find the TSecond corresponding to the TFirst first.
        /// Returns false if first is not in the dictionary.
        /// </summary>
        /// <param name="first">the key to search for</param>
        /// <param name="second">the corresponding value</param>
        /// <returns>true if first is in the dictionary, false otherwise</returns>
        public Boolean TryGetByFirst(TFirst first, out List<TSecond> second)
        {
            return firstToSecond.TryGetValue(first, out second);
        }

        /// <summary>
        /// Find the TFirst corresponding to the TSecond second.
        /// Returns false if second is not in the dictionary.
        /// </summary>
        /// <param name="second">the key to search for</param>
        /// <param name="first">the corresponding value</param>
        /// <returns>true if second is in the dictionary, false otherwise</returns>
        public Boolean TryGetBySecond(TSecond second, out TFirst first)
        {
            return secondToFirst.TryGetValue(second, out first);
        }

        /// <summary>
        /// Remove the record containing first, if there is one.
        /// </summary>
        /// <param name="first"></param>
        /// <returns> If first is not in the dictionary, returns false, otherwise true</returns>
        public Boolean TryRemoveByFirst(TFirst first)
        {
            List<TSecond> secondList;
            if (!firstToSecond.TryGetValue(first, out secondList))
                return false;

            firstToSecond.Remove(first);
            foreach (var second in secondList)
                secondToFirst.Remove(second);
            return true;
        }

        /// <summary>
        /// Remove the record containing second, if there is one.
        /// </summary>
        /// <param name="second"></param>
        /// <returns> If second is not in the dictionary, returns false, otherwise true</returns>
        public Boolean TryRemoveBySecond(TSecond second)
        {
            TFirst first;
            if (!secondToFirst.TryGetValue(second, out first))
                return false;

            secondToFirst.Remove(second);
            List<TSecond> secondList = firstToSecond.AddOrGet(first);
            secondList.Remove(second);
            if (secondList.Count == 0)
                firstToSecond.Remove(first);
            return true;
        }

        #endregion

        /// <summary>
        /// The number of pairs stored in the dictionary
        /// </summary>
        public Int32 Count
        {
            get { return firstToSecond.Count; }
        }

        public bool IsReadOnly => secondToFirst.IsReadOnly;

        public ICollection<TSecond> Keys => throw new NotImplementedException();

        public ICollection<TFirst> Values => throw new NotImplementedException();

        public TFirst this[TSecond key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// Removes all items from the dictionary.
        /// </summary>
        public void Clear()
        {
            firstToSecond.Clear();
            secondToFirst.Clear();
        }

        public void Add(KeyValuePair<TSecond, TFirst> item)
        {
            Add(item.Value, item.Key);
        }

        public bool Contains(KeyValuePair<TSecond, TFirst> item)
        {
            return secondToFirst.Contains(item);
        }

        public void CopyTo(KeyValuePair<TSecond, TFirst>[] array, int arrayIndex)
        {
            for (int i = arrayIndex; i < array.Length; i++)
            {
                var item = array[i];
                Add(item.Value, item.Key);
            }
        }

        public bool Remove(KeyValuePair<TSecond, TFirst> item)
        {
            return TryRemoveByFirst(item.Value);
        }

        public IEnumerator<KeyValuePair<TSecond, TFirst>> GetEnumerator()
        {
            return secondToFirst.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return secondToFirst.GetEnumerator();
        }

        public void Add(TSecond key, TFirst value)
        {
            Add(value, key);
        }

        public bool ContainsKey(TSecond key)
        {
            return secondToFirst.ContainsKey(key);
        }

        public bool Remove(TSecond key)
        {
            return TryRemoveBySecond(key);
        }

        public bool TryGetValue(TSecond key, out TFirst value)
        {
            return TryGetBySecond(key, out value);
        }
    }
}