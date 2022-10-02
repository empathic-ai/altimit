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

        public static Type GetCollectionElementType(this Type type)
        {
            if (type.IsNativeCollection())
            {
                if (type.IsArray)
                    return type.GetElementType();
                var collectionType = type.GetGenericInterface(typeof(ICollection<>));
                if (collectionType == null)
                    throw new ArgumentException($"Could not find generic interface in type {type}.");
                return collectionType.GetGenericArguments()[0];
            }

            return null;
        }

        public static Type GetTrueType(this Type type)
        {
            if (type.IsTypeType())
                type = typeof(Type);
            return type;
        }

        public static Type GetUnderlyingReturnType(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
                return type.GenericTypeArguments[0];
            return type;
        }

        public static bool IsEqualContent<T>(this IList<T> firstList, IList<T> secondList)
        {
            if (firstList.Count != secondList.Count)
                return false;

            for (int i = 0; i < firstList.Count; i++)
            {
                var equalNulls = EqualNulls(firstList[i], secondList[i]);
                if (equalNulls != null)
                {
                    if (equalNulls == false)
                    {
                        return false;
                    }
                }
                else if (!firstList[i].Equals(secondList[i]))
                {
                    return false;
                }
            }
            return true;
        }
        // Returns true if both are null, false if one is null and the other is not, and null if both are not null and the equality cannot be inferred
        public static bool? EqualNulls(object a, object b)
        {
            //If nullable states don't match
            if ((a != null && b == null) ||
                (a == null && b != null))
            {
                return false;
            }

            //If both are nullable
            if (a == null)
            {
                return true;
            }

            return null;
        }
        public static bool EqualObjects(object a, object b, bool isValue = false)
        {
            bool? equalNulls = TypeExtensions.EqualNulls(a, b);
            if (equalNulls != null)
                return (bool)equalNulls;

            //If types don't match
            if (a.GetAType() != b.GetAType())
                return false;
            
            var type = a.GetAType();

            if (!type.IsInstance() && type.IsStructure())
            {
                var bProperties = b.GetProperties().GetEnumerator();

                foreach (var property in a.GetProperties())
                {
                    if (!bProperties.MoveNext() || !EqualObjects(property, bProperties.Current))
                    {
                        bProperties.Dispose();
                        return false;
                    }
                }
                bProperties.Dispose();
                return true;
            }
            
            return a.Equals(b);
        }

        public static IEnumerable<object> GetAllProperties(this object instance, bool includeSelf = false)
        {
            var includedSubObjects = new HashSet<object>();
            if (includeSelf)
            {
                yield return instance;
                includedSubObjects.Add(instance);
            }
            foreach (var subObjectParameters in instance.GetAllProperties(includedSubObjects))
            {
                yield return subObjectParameters;
            }
        }

        public static IEnumerable<object> GetAllProperties(this object instance, HashSet<object> includedSubObjects)
        {
            foreach (var property in instance.GetProperties())
            {
                if (property != null)
                {
                    yield return property;

                    if (!includedSubObjects.Contains(property))
                    {
                        includedSubObjects.Add(property);

                        foreach (var subObjectSubObjectParameters in property.GetAllProperties(includedSubObjects))
                        {
                            yield return subObjectSubObjectParameters;
                        }
                    }
                }
            }
        }

        public static IEnumerable<object> GetProperties(this object instance, bool isFiltered = false)
        {
            if (instance == null)
                yield break;

            //if (instance.GetType() != structureType)
            //    throw new ArgumentException(string.Format("Type mismatch in serializer. Type of {0} and type of {1} are incompatible. Make sure objects are serialized with correct types.", instance.GetType(), structureType));

            if (instance.GetAType().IsCollection())
            {
                var collection = instance as IEnumerable;
                int i = 0;
                foreach (var element in collection)
                {
                    yield return element;
                    i++;
                }
            }
            else if (instance.GetAType().IsStructure())
            {
                var typeInfo = instance.GetATypeInfo();
                foreach (var propertyInfo in typeInfo.ReplicatedPropertyInfos)
                {
                    yield return instance.GetProperty(propertyInfo.Name, isFiltered);
                }
            }
        }
        public static IEnumerable<APropertyInfo> GetPropertyInfos(this object instance)
        {
            if (instance == null)
                yield break;

            var collection = instance as IEnumerable;
            IEnumerator enumerator = null;
            if (collection != null)
                enumerator = collection.GetEnumerator();
            foreach (var propertyInfo in instance.GetATypeInfo().ReplicatedPropertyInfos)
            {
                if (enumerator != null && !enumerator.MoveNext())
                {
                    break;
                }
                yield return propertyInfo;
            }
        }

        public static IEnumerable<APropertyInfo> GetPropertyInfos(this Type type)
        {
            foreach (var propertyInfo in type.GetATypeInfo().ReplicatedPropertyInfos)
            {
                yield return propertyInfo;
            }
        }

        public static IEnumerable<APropertyInfo> GetPropertyInfos(this Type type, int elementCount)
        {
            for (int i = 0; i < elementCount; i++)
            {
                yield return type.GetATypeInfo().GetPropertyInfoByIndex(i);
            }
        }

        public static IEnumerable<object> GetConstructorProperties(this object instance, bool isFiltered = false)
        {
            return instance.GetATypeInfo().DefaultConstructorInfo.PropertyNames.Select(x => instance.GetProperty(x, isFiltered));
        }

        // Grabs properties or elements from an object, if it's a structure type or collection type respectively
         // Otherwise, grab nothing
        public static IEnumerable<Tuple<APropertyInfo, object>> GetPropertiesByInfo(this object instance, bool isFiltered = false)
        {
            if (instance == null)
                yield break;

            //if (instance.GetType() != structureType)
            //    throw new ArgumentException(string.Format("Type mismatch in serializer. Type of {0} and type of {1} are incompatible. Make sure objects are serialized with correct types.", instance.GetType(), structureType));

            if (instance.IsCollection())
            {
                var collection = instance as IEnumerable;
                int i = 0;
                foreach (var element in collection)
                {
                    yield return new Tuple<APropertyInfo, object>(instance.GetPropertyInfo(i.ToString()), element);
                    i++;
                }
            }
            else if (instance.IsStructure())
            {
                var typeInfo = instance.GetATypeInfo();
                foreach (var propertyInfo in typeInfo.ReplicatedPropertyInfos)
                {
                    yield return new Tuple<APropertyInfo, object>(propertyInfo, instance.GetProperty(propertyInfo.Name, isFiltered));
                }
            }
        }

        public static object[] GetDefaultConstructorArgs(this object instance)
        {
            if (instance.GetType().IsArray)
            {
                return new object[] { instance.GetPropertyCount() };
            } else if (instance.GetType().IsNativeCollection())
            {
                return new object[0];
            }
            var defaultConstructorInfo = instance.GetATypeInfo().DefaultConstructorInfo;
            return instance.GetPropertiesByInfo().Where(x => defaultConstructorInfo.PropertyNames.Contains(x.Item1.Name)).Select(x=>x.Item2).ToArray();
        }

        public static FormatType FilterPropertyFormatType(FormatType formatType, FormatType subFormatType)
        {
            if (formatType.HasFlag(FormatType.Verbose))
                subFormatType |= FormatType.Verbose;

            if (formatType.HasFlag(FormatType.RecursiveValue))
                subFormatType |= FormatType.RecursiveValue;

            return subFormatType;
        }

        public static bool HasGenericInterface(this Type type, Type genericInterface)
        {
            return GetGenericInterface(type, genericInterface) != null;
        }

        public static Type GetGenericInterface(this Type type, Type genericInterface)
        {
            return type.GetInterfaces().SingleOrDefault(x => x.IsGenericType &&
                                                             x.GetGenericTypeDefinition() == genericInterface);
        }

        public static int GetPropertyCount(this object o, bool isVerbose = false)
        {
            Type type = o.GetType();
            if (o is ICollection)
            {
                return (o as ICollection).Count;
            }
            else if (o.GetType().HasGenericInterface(typeof(ICollection<>)))
            {
                return (int)o.GetType().GetProperty(nameof(ICollection.Count)).GetValue(o);
            }
            else if (type.IsStructure())
            {
                var typeInfo = o.GetATypeInfo();
                return isVerbose ? typeInfo.PropertyInfos.Length : typeInfo.ReplicatedPropertyInfos.Length;
            }
            return 0;
        }


    }
}
