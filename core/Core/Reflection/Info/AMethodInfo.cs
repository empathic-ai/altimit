using System;
using System.Linq;
using System.Reflection;

namespace Altimit
{
    public class AMethodInfo
    {
        public Type ClassType { get; }
        public Type ReturnType { get; set; }
        public bool IsReturnForceValue { get; set; }
        public string Name { get; set; }
        public bool IsReplicated { get; private set; }
        public Type[] ParameterTypes { get; }
        public bool[] ParameterForceValues { get; }
        public Func<object, object[], object> InvokeFunc { get; set; }
        public ProtocolType ProtocolType { get; set; } = ProtocolType.None;
        public MethodInfo BaseMethodInfo { get; }
        public FormatType ReturnFormatType { get; }
        public bool HasResponse { get; }

        public AMethodInfo(Type classType, System.Reflection.MethodInfo methodInfo)
        {
            ClassType = classType;
            Name = methodInfo.Name;
            ReturnType = methodInfo.ReturnType;
            ParameterTypes = methodInfo.GetParameters().Select(x => x.ParameterType).ToArray();
            var baseMethodInfo = GetBaseMethodInfo(classType, methodInfo);
            IsReturnForceValue = baseMethodInfo.ReturnParameter.GetCustomAttributes(typeof(ForceValueAttribute), true).Length > 0;
            ParameterForceValues = baseMethodInfo.GetParameters().Select(x => x.GetCustomAttribute<ForceValueAttribute>() != null).ToArray();
            HasResponse = baseMethodInfo.ReturnParameter != null && baseMethodInfo.ReturnTypeCustomAttributes.GetCustomAttributes(typeof(IgnoreAttribute), true).Length == 0;

            var idAttribute = baseMethodInfo.GetCustomAttribute<AMethodAttribute>();
            if (idAttribute == null)
                idAttribute = methodInfo.GetCustomAttribute<AMethodAttribute>();

            IsReplicated = idAttribute != null;
            ProtocolType = idAttribute != null ? idAttribute.ProtocolType : ProtocolType.Sequential;

            InvokeFunc = methodInfo.Invoke;

            BaseMethodInfo = methodInfo;
        }

        // Used to get information related to this method from the place it's originally declared
        // If this method is based on an interface, return the MethodInfo of this method from that interface
        MethodInfo GetBaseMethodInfo(Type type, MethodInfo methodInfo)
        {
            foreach (var interfaceType in type.GetInterfaces())
            {
                var interfaceMethodInfo = interfaceType.GetMethod(Name, ParameterTypes);
                if (interfaceMethodInfo != null)
                    return GetBaseMethodInfo(interfaceType, interfaceMethodInfo);
            }
            return methodInfo;
        }

        public object Invoke(object target, params object[] args)
        {
            return InvokeFunc(target, args);
        }

        public override string ToString()
        {
            return ClassType.ToString() + "." + Name.ToString()+"()";
        }
    }
}
