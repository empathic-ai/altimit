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
        public static PairedDictionary<Type, Type> LocalTypesByType { get; } = new PairedDictionary<Type, Type>();

        public static Type GetGlobalType(this Type type)
        {
            if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(Local<>))
                OS.Logger.LogError($"Attempted to get global type from non-local type {type.GetTypeName()}!");

            return type.GetGenericArguments()[0]; //LocalTypesByType.GetBySecond(type);
        }

        public static Type GetLocalType(this Type type)
        {
            if (!type.IsStructure() && !type.IsInstanceID() && !typeof(IValue<AID>).IsAssignableFrom(type))
            {
                OS.Logger.LogError($"Attempted to get local type for non-structure type {type.GetTypeName()}!");
            }

            Type localType;
            if (!LocalTypesByType.TryGetByFirst(type, out localType))
            {
                localType = typeof(Local<>).MakeGenericType(type);
                LocalTypesByType[type] = localType;
            }
            return localType;
        }

        // Creates a 'mock' dynamic type at runtime--compatible with AOT platforms (platforms that don't support actual dynamic types)
        // Not used for anything right now, but could eventually be used for creating custom types at runtime using a visual scripting system.
        public static Type CreateAType(string typeName, Tuple<Type, string>[] propertyData, Func<object[], AObject> constructor, Type elementType = null)
        {
            var aType = new AType();
            CreateATypeInfo(aType, typeName, propertyData, constructor, elementType);
            return aType;
        }

        public static void CreateATypeInfo(Type aType, string typeName, Tuple<Type, string>[] propertyData, Func<object[], AObject> constructor, Type elementType = null)
        {
            var propertyInfos = new APropertyInfo[propertyData.Length];
            for (int i = 0; i < propertyData.Length; i++)
            {
                var propertyIndex = i;
                propertyInfos[propertyIndex] = CreateAPropertyInfo(aType, propertyData[i].Item1, propertyData[i].Item2, i);
            }

            RegisterTypeInfo(new ATypeInfo(
                typeName,
                aType, elementType, propertyInfos,
                false,
                new AConstructorInfo(aType, args => constructor(args), new string[] { })));
        }

        // Creates property info for grabbing a property from an AObject
        public static APropertyInfo CreateAPropertyInfo(Type aType, Type propertyType, string propertyName, int propertyIndex)
        {
            return new APropertyInfo(aType, propertyType, propertyName, true,
                    x => {
                        return ((AObject)x).Properties[propertyIndex];
                    }, (ref object x, object y) =>
                    {
                        ((AObject)x).Properties[propertyIndex] = y;
                    });
        }

        public static bool IsLocal(this Type type)
        {
            return LocalTypesByType.ContainsValue(type);
        }
        public static Type GetLocalType(APropertyInfo propertyInfo)
        {
            return GetLocalType(propertyInfo.IsValue, propertyInfo.PropertyType);
        }

        public static Type GetLocalType(bool isReference, Type type)
        {
            if (isReference)
            {
                return typeof(int);
            }
            return type;
        }
    }
}
