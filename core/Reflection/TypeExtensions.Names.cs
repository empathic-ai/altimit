using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Altimit
{
    public static partial class TypeExtensions
    {

        public static string GetTypeName(this Type type)
        {
            return type.GetATypeInfo().Name;
        }

        // Gets the ID associated with a given type--does not account for types with defined generic arguments
        // This method is purely used for serialization--it shouldn't be used at runtime, as several types cannot be represented with a single integer value
        public static string GetNativeTypeName(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.GetFullName();
        }

        public static string GetFullName(this Type t)
        {
            if (!t.IsGenericType)
                return t.FullName;

            if (t.IsGenericTypeDefinition)
            {
                return t.GetGenericTypeDefinition().Name;
            }

            string genericTypeName = t.GetGenericTypeDefinition().Name;
            genericTypeName = genericTypeName.Substring(0,
                genericTypeName.IndexOf('`'));

            string genericArgs = string.Join(",",
                t.GetGenericArguments()
                    .Select(ta => GetFullName(ta)).ToArray());
            return genericTypeName + "[" + genericArgs + "]";
        }

        private static void _buildClassNameRecursiv(Type type, StringBuilder classNameBuilder, int genericParameterIndex = 0)
        {
            if (type.IsGenericParameter)
                classNameBuilder.AppendFormat("`{0}", genericParameterIndex + 1);

            else if (type.IsGenericType)
            {
                classNameBuilder.Append(GetNestedTypeName(type) + "[");
                int subIndex = 0;
                foreach (Type genericTypeArgument in type.GetGenericArguments())
                {
                    if (subIndex > 0)
                        classNameBuilder.Append(",");

                    _buildClassNameRecursiv(genericTypeArgument, classNameBuilder, subIndex++);
                }
                classNameBuilder.Append("]");
            }
            else
                classNameBuilder.Append(type.GetNestedTypeName());
        }

        public static string GetNestedTypeName(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (!type.IsNested)
                return GetNonGenericTypeName(type);

            StringBuilder nestedName = new StringBuilder();
            while (type != null)
            {
                if (nestedName.Length > 0)
                    nestedName.Insert(0, '.');

                nestedName.Insert(0, GetNonGenericTypeName(type));

                type = type.DeclaringType;
            }
            return nestedName.ToString();
        }

        public static string GetNonGenericTypeName(this Type type)
        {
            return type.IsGenericType ? type.FullName.Split('`')[0] : type.FullName;
        }


    }
}
