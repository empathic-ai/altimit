using Altimit.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;

namespace Altimit.Networking
{
    public static class SessionExtensions
    {
        public static Type GetPeerSessionType(Type type)
        {
            var sessionModuleType = type.GetInterfaces().SingleOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ISessionModule<>));
            if (sessionModuleType == null)
                OS.Logger.LogError($"Failed to find ISessionModule interface for type {type}.");
            return sessionModuleType.GenericTypeArguments[0];
        }
    }

    public class ProxySMFactory
    {
        static Dictionary<Type, Type> GeneratedPeerSessionModules = new Dictionary<Type, Type>();

        public static T CreateModule<T>() where T : ISessionModule
        {
            return (T)CreateModule(typeof(T));
        }

        public static ISessionModule CreateModule(Type sessionModuleType)
        {
            Type generatedType;
            if (!GeneratedPeerSessionModules.TryGetValue(sessionModuleType, out generatedType))
            {
                generatedType = GeneratePeerSessionModuleType(sessionModuleType);
                GeneratedPeerSessionModules.Add(sessionModuleType, generatedType);
            }
            return (ISessionModule)Activator.CreateInstance(generatedType);
        }

        // Used to make network calls from this application to another application
        // Generates a session module with methods defined in a session module interface
        // The generated session module converts standard method calls into network messages to send out
        // and converts received network messages into standard method calls
        public static Type GeneratePeerSessionModuleType(Type interfaceType)
        {
            string name = "DefineMethodOverrideExample";
            AssemblyName asmName = new AssemblyName(name);
            AssemblyBuilder ab = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
            ModuleBuilder mb = ab.DefineDynamicModule(name);

            TypeBuilder tb;

            var peerSessionType = SessionExtensions.GetPeerSessionType(interfaceType);

            tb = mb.DefineType("Peer" + interfaceType.Name.TrimStart('I'), TypeAttributes.Public, typeof(PeerSessionModule<>).MakeGenericType(peerSessionType));
            
            tb.AddInterfaceImplementation(interfaceType);

            var peerSessionField = typeof(PeerSessionModule<>).GetField("container", BindingFlags.NonPublic | BindingFlags.Instance);

            // TODO: Re-implement this in a better way so it doesn't broadcast a warning when types without problems are passed through
            // Appears to be detecting extra methods in methodinfos that are non-problematic
            //if (interfaceType.GetATypeInfo().MethodInfos.Length != interfaceType.GetATypeInfo().ReplicatedMethodInfos.Length)
            //    OS.Log($"Tried to generate an interface of type {interfaceType.Name} using non-replicated methods!");

            foreach (var methodInfo in interfaceType.GetATypeInfo().ReplicatedMethodInfos)
            {
                MethodBuilder mbIM = tb.DefineMethod(methodInfo.Name,
                    MethodAttributes.Public | MethodAttributes.Virtual, methodInfo.ReturnType, methodInfo.ParameterTypes);
                ILGenerator il = mbIM.GetILGenerator();
                // il.Emit(OpCodes.Ldstr,"Calling method named " + method.Name);
                //il.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine",
                //    new Type[] { typeof(string) }));

                var typeNames = il.DeclareLocal(typeof(Type[]));

                il.Emit(OpCodes.Ldc_I4_S, methodInfo.ParameterTypes.Length);
                il.Emit(OpCodes.Newarr, typeof(System.String));
                il.Emit(OpCodes.Stloc, typeNames);

                var paramValues = il.DeclareLocal(typeof(object[]));

                il.Emit(OpCodes.Ldc_I4_S, methodInfo.ParameterTypes.Length);
                il.Emit(OpCodes.Newarr, typeof(System.Object));
                il.Emit(OpCodes.Stloc, paramValues);

                for (int i = 0; i < methodInfo.ParameterTypes.Length; i++)
                {
                    var type = methodInfo.ParameterTypes[i];

                    il.Emit(OpCodes.Ldloc, typeNames);
                    il.Emit(OpCodes.Ldc_I4, i);
                    il.Emit(OpCodes.Ldstr, TypeExtensions.GetNativeTypeName(type));
                    il.Emit(OpCodes.Stelem, typeof(string));

                    il.Emit(OpCodes.Ldloc, paramValues);
                    il.Emit(OpCodes.Ldc_I4, i);
                    il.Emit(OpCodes.Ldarg, i + 1);
                    if (type.IsValueType)
                    {
                        il.Emit(OpCodes.Box, type);
                    }
                    il.Emit(OpCodes.Stelem, typeof(object));
                }
                var taskFunc = il.DeclareLocal(typeof(object[]));

                var isGeneric = methodInfo.ReturnType.IsGenericType && methodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>);
                var isAsync = (methodInfo.ReturnType == typeof(Task) || isGeneric);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, peerSessionField);
                il.Emit(OpCodes.Ldstr, TypeExtensions.GetNativeTypeName(interfaceType).ToString());
                il.Emit(OpCodes.Ldstr, methodInfo.Name);
                il.Emit(OpCodes.Ldloc, typeNames);
                il.Emit(OpCodes.Ldloc, paramValues);
                var sendMethodInfo = typeof(ProxyPeerSession).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).
                    Single(x => x.Name == (isAsync ? "SendAsync" : "Send") && x.ContainsGenericParameters == isGeneric
                        && x.GetParameters().Select(x=>x.ParameterType).ToArray().IsEqualContent(new Type[]{ typeof(string), typeof(string), typeof(string[]), typeof(object[]) }));

                if (isGeneric)
                    sendMethodInfo = sendMethodInfo.MakeGenericMethod(methodInfo.ReturnType.GetGenericArguments()[0]);
                il.Emit(OpCodes.Call, sendMethodInfo);
                
                il.Emit(OpCodes.Ret);
                tb.DefineMethodOverride(mbIM, methodInfo.BaseMethodInfo);
            }

            return tb.CreateTypeInfo().AsType();
        }
    }
}
