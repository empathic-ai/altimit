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
        public static APairedDictionary<Type, Type> DereferencedTypesByReferencedTypes { get; } = new APairedDictionary<Type, Type>();

        public static Type GetReferencedType(this Type type)
        {
            var interfaceType = type.GetGenericInterface(typeof(IDereferencedObject<>));

            if (interfaceType == null)
                OS.Logger.LogError($"Attempted to get referenced type from invalid type {type.GetTypeName()}!");

            return interfaceType.GetGenericArguments()[0];
        }

        public static Type GetDereferencedType(this Type type)
        {
            if (!type.IsStructure() && !type.IsInstanceID())
            {
                OS.Logger.LogError($"Attempted to get local type for non-structure type {type.GetTypeName()}!");
            }

            Type dereferencedType;
            if (!DereferencedTypesByReferencedTypes.TryGetByFirst(type, out dereferencedType))
            {
#if ALTIMIT_DYNAMIC
                localType = typeof(DereferencedObject<>).MakeGenericType(type);
                DereferencedTypesByReferencedTypes[type] = localType;
#else

                // Gets local types genearted using SourceGenerators
                // TODO: Possibly add Mono.Cecil/IL weaving implimentation of this for legacy systems
                dereferencedType = TypeExtensions.GetTypeByName("Dereferenced" + type.Name);


                DereferencedTypesByReferencedTypes[type] = dereferencedType;
#endif
            }
            return dereferencedType;
        }

        // Creates a 'mock' dynamic type at runtime--compatible with AOT platforms (platforms that don't support actual dynamic types)
        // Not used for anything right now, but could eventually be used for creating custom types at runtime using a visual scripting system.
        // Doesn't play nice with other frameworks, so trying to avoid
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

        public static Type GetPropertyLocalType(this Type type)
        {
            if (type.IsInstanceType())
            {
                return typeof(Guid);
            }
            else if (type.IsStructure())
            {
                return type.GetDereferencedType();
            }
            return type;
        }

        public static string GetDereferencedPropertyName(APropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType.IsInstanceType())
                return propertyInfo.Name + "ID";
            return propertyInfo.Name;
        }

        public static bool IsDereferenced(this object o)
        {
            var type = o.GetType();
            if (type.GetInterfaces().Contains(typeof(IDereferencedObject)))
                return true;

            return !type.IsInstanceType() && !type.IsStructure();
        }
    }
}
