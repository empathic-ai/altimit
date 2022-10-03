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
        public static bool IsEnum(this Type type)
        {
            return type.GetATypeInfo().IsEnum;
        }

        public static bool IsTypeType(this Type type)
        {
            return type == typeof(Type) || type == typeof(Type).GetType();
        }

        public static bool IsObjectType(this AType type)
        {
            if (!type.IsNativeType())
                return false;

            return ((Type)type).IsObjectType();
        }

        public static bool IsObjectType(this Type type)
        {
            return type == typeof(object);
        }

        public static bool IsValueType(this Type type)
        {
            return !type.IsInstance();
        }

        public static bool IsInstanceID(this Type type)
        {
            return type.Equals(typeof(AID));
        }

        public static bool IsReplicatedInstance(this object obj)
        {
            if (obj == null)
                return false;

            return obj.IsInstance() && obj.GetAType().IsReplicatedType();
        }

        public static bool IsInstance(this object obj)
        {
            if (obj == null)
                return false;

            return obj.GetAType().IsInstance();
        }

        public static bool IsInstance(this Type type)
        {
            return type.GetATypeInfo().IsInstance;
        }

        public static bool IsNativeInstance(this Type type)
        {
            return type.IsClass && type.IsNativeStructure() && !type.IsSystemDictionary() && !type.IsArray;
        }

        public static bool IsSystemDictionary(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Dictionary<,>));
        }

        public static bool IsCollection(this object obj)
        {
            return obj.GetATypeInfo().IsCollection;
        }

        public static bool IsNativeStructure(this Type type)
        {
            return !type.Equals(typeof(string)) && !type.IsObjectType() && !type.IsEnum && !type.Equals(typeof(Guid)) && !type.IsTypeType() && ((!type.IsPrimitive) || IsNativeCollection(type));
        }

        public static bool IsNativeCollection(this Type type)
        {
            return HasGenericInterface(type, typeof(ICollection)) || HasGenericInterface(type, typeof(ICollection<>)) || type.IsArray;
        }

        public static bool IsCollection(this Type type)
        {
            return type.GetATypeInfo().IsCollection;
        }

        public static bool IsStructure(this Type type)
        {
            return type.GetATypeInfo().IsStructure;
        }

        public static bool IsStructure(this object obj)
        {
            return obj.GetATypeInfo().IsStructure;
        }
        public static bool IsValueType(this FormatType formatType)
        {
            return formatType.HasFlag(FormatType.RecursiveValue);
        }
    }
}
