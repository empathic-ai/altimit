using System;
using System.Collections.Generic;

namespace Altimit
{
    public static class DictionaryExtensions
    {
        public static TValue AddOrGet<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key) where TValue : new()
        {
            TValue val;

            if (!dict.TryGetValue(key, out val))
            {
                if (typeof(TValue).GetConstructor(Type.EmptyTypes) != null)
                    val = new TValue();

                dict.Add(key, val);
            }

            return val;
        }

        public static void AddOrSet<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = value;
            }
            else
            {
                dict.Add(key, value);
            }
        }

        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            TValue val;
            if (!dict.TryGetValue(key, out val))
            {
                dict.Add(key, value);
                return true;
            }

            return false;
        }


        public static bool TryRemove<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            TValue val;
            if (dict.TryGetValue(key, out val))
            {
                dict.Remove(key);
                return true;
            }

            return false;
        }
    }
}