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
        public static void CallMethod(this object instance, string methodName, Type[] types, params object[] args)
        {
            instance.GetType().GetMethodInfo(methodName, types).Invoke(instance, args);
        }

        public static AMethodInfo GetMethodInfo(this object instance, string methodName, Type[] types)
        {
            return instance.GetType().GetMethodInfo(methodName, types);
        }

        public static AMethodInfo GetMethodInfo(this Type type, string methodName, Type[] types)
        {
            try
            {
                return type.GetATypeInfo().GetMethodInfo(methodName, types);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Failed to find method named {0} within type {1}!", methodName, type), e);
            }
        }
    }
}
