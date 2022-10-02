using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections;
using System.Text;

namespace Altimit
{
    public static partial class TypeExtensions
    {
        public static object GetElement(this object collection, int index)
        {
            if (collection is AObject)
            {
                return ((AObject)collection).Properties[index];
            } else 
            {
                return (collection as Array).GetValue(index);
            }
        }

        public static void AddElement(this object collection, int index, object element)
        {
            if (collection is IList)
            {
                try
                {
                    if (index == ((IList)collection).Count)
                    {
                        ((IList)collection).Add(element);
                    }
                    else
                    {
                        ((IList)collection).Insert(index, element);
                    }
                } catch (ArgumentOutOfRangeException e)
                {
                    OS.Log(((IList)collection).Count);
                    OS.Log(index);
                    OS.LogError(e);
                }
            } else if (collection is IDictionary)
            {
                var dict = collection as IDictionary;
                dict.Add(element.GetProperty(nameof(KeyValuePair<object, object>.Key)),
                    element.GetProperty(nameof(KeyValuePair<object, object>.Value)));
            } else
            {
                throw new NotImplementedException();
            }
        }

        public static void RemoveElement(this object collection, int index, object element)
        {
            if (collection is IList)
            {
                ((IList)collection).RemoveAt(index);
            }
            else if (collection is IDictionary)
            {
                var dict = collection as IDictionary;
                dict.Remove(element.GetProperty(nameof(KeyValuePair<object, object>.Key)));
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        // Sets an element in a collection

        public static void SetElement(this object collection, int index, object element)
        {
            if (collection is IList)
            {
                var list = ((IList)collection);
                if (index == list.Count)
                {
                    list.Add(element);
                } else if (index > list.Count)
                {
                    OS.Logger.LogError("Attempted to set element at an invalid index in the list!");
                } else
                {
                    list[index] = element;
                }
                /*
                while (list.Count < index)
                {
                    list.Add(Activator.CreateInstance(collection.GetType().GetElementType()));
                }
                */
            } else if (collection is IDictionary)
            {
                var dict = collection as IDictionary;
                var key = element.GetProperty(nameof(KeyValuePair<object, object>.Key));
                var value = element.GetProperty(nameof(KeyValuePair<object, object>.Value));
                dict[key] = value;
            }
            else
            {
                throw new NotImplementedException();
                OS.Log(collection == null);
                OS.Log(collection.GetType());
                OS.Log(collection as IList == null);
                (collection as IList)[index] = element;
//                (collection as Array).SetValue(element, index);
            }
        }
    }
}
