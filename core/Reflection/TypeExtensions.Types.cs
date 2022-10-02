using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Altimit
{
    public static partial class TypeExtensions
    {
        public static PairedDictionary<string, Type> TypesByName { get; } = new PairedDictionary<string, Type>();
        public static Dictionary<Type, ATypeInfo> TypeInfosByType { get; } = new Dictionary<Type, ATypeInfo>();
        //public static Dictionary<Type, Type> GenericTypeInfoTypesByType { get; } = new Dictionary<Type, Type>();

        const string AltimitCoreAssemblyName = "Altimit.Core";

        static TypeExtensions()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GetName().Name == AltimitCoreAssemblyName || assembly.GetReferencedAssemblies().SingleOrDefault(x => x.Name == AltimitCoreAssemblyName) != null)
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (type != typeof(ATypeInfo) && typeof(ATypeInfo).IsAssignableFrom(type))
                        {
                            //Register custom type info
                            var aTypeInfoAttribute = type.GetCustomAttribute<ATypeInfoAttribute>();
                            if (aTypeInfoAttribute != null)
                            {
                                //throw new Exception(string.Format("Assign ATypeInfoAttribute to {0}!", type.Name));

                                var _type = type;
                                if (type.ContainsGenericParameters)
                                {
                                    _type = type.MakeGenericType(aTypeInfoAttribute.Type);
                                }
                                RegisterTypeInfo((ATypeInfo)Activator.CreateInstance(_type, aTypeInfoAttribute.Type));
                            }
                        }
                    }
                }
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GetName().Name == AltimitCoreAssemblyName || assembly.GetReferencedAssemblies().SingleOrDefault(x => x.Name == AltimitCoreAssemblyName) != null)
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        var aTypeAttribute = type.GetCustomAttribute<ATypeAttribute>();
                        if (aTypeAttribute != null)
                        {
                            // Load type info for any class using the ATypeAttribute, so that its ID is registered
                            TryRegister(type);
                        }
                    }
                }
            }
        }

        public static Type GetAType(this object obj)
        {
            if (obj is AObject)
                return (obj as AObject).Type;

            return obj.GetType().GetTrueType();
        }

        public static bool IsNativeType(this Type type)
        {
            return !(type is AType);
        }

        public static ATypeInfo GetATypeInfo(this object target)
        {
            return target.GetAType().GetATypeInfo();
        }

        public static int GetIndexFromName(this ATypeInfo classInfo, string propertyName)
        {
            return Array.IndexOf(classInfo.ReplicatedPropertyInfos, classInfo.GetPropertyInfo(propertyName));
        }

        public static Type GetType(string typeName)
        {
            try
            {
                Type type;
                if (!TypesByName.TryGetByFirst(typeName, out type))
                {
                    var firstBracketIndex = typeName.IndexOf("[");
                    if (firstBracketIndex != -1)
                    {
                        var lastBracketIndex = typeName.LastIndexOf("]");
                        var innerText = typeName.Substring(firstBracketIndex + 1, lastBracketIndex - (firstBracketIndex + 1));
                        var outerTypeName = typeName.Substring(0, firstBracketIndex);
                        var innerTypeNames = Regex.Split(innerText, @"(?![^)(]*\([^)(]*?\)\)),(?![^\[]*\])");

                        var genericType = TypesByName[outerTypeName + "`" + innerTypeNames.Length];
                        Type[] typeArgs = innerTypeNames.Select(x => GetType(x)).ToArray();
                        return genericType.MakeGenericType(typeArgs);
                    }
                    else
                    {
                        OS.Logger.LogError($"Failed to find type from name {typeName}!");
                        return null;
                    }
                }
                return type;
            } catch (Exception e)
            {
                throw new Exception($"Failed to get type for type named {typeName}!", e);
            }
        }

        public static bool IsReplicatedType(this Type type)
        {
            return type.GetATypeInfo().IsReplicated;
        }

        public static ATypeInfo GetATypeInfo<T>()
        {
            return GetATypeInfo(typeof(T));
        }

        // TODO: Simplify how custom TypeInfos are grabbed for native types--this code is more verbose than it has to be!
        public static ATypeInfo GetATypeInfo(this Type type)
        {
            type = type.GetTrueType();
            if (type.IsNativeType())
            {
                ATypeInfo typeInfo;
                lock (TypeInfosByType)
                {
                    Type tempType = type;
                    do
                    {
                        if (TypeInfosByType.TryGetValue(tempType, out typeInfo))
                        {
                            if (tempType == type)
                            {
                                return typeInfo;
                            }
                            else
                            {
                                typeInfo = (ATypeInfo)Activator.CreateInstance(typeInfo.GetType(), type);
                                break;
                            }
                        }
                        else if (tempType.IsGenericType && TypeInfosByType.TryGetValue(tempType.GetGenericTypeDefinition(), out typeInfo))
                        {
                            typeInfo = (ATypeInfo)Activator.CreateInstance(typeInfo.GetType(), type);
                            break;
                        }
                        tempType = tempType.BaseType;
                    } while (tempType != null);

                    if (typeInfo == null)
                    {
                        typeInfo = (ATypeInfo)Activator.CreateInstance(typeof(ATypeInfo), type);
                    }
                    RegisterTypeInfo(typeInfo);
                    return typeInfo;
                }
            }
            else
            {
                ATypeInfo typeInfo;
                if (!TypeInfosByType.TryGetValue(type, out typeInfo))
                {
                    typeInfo = (ATypeInfo)Activator.CreateInstance(typeof(ATypeInfo), type);
                    RegisterTypeInfo(typeInfo);
                }
                return typeInfo;
            }
        }

        public static void RegisterTypeInfo(ATypeInfo typeInfo)
        {
            TypeInfosByType.Add(typeInfo.Type, typeInfo);
            TryRegister(typeInfo.Type);
        }

        public static void TryRegister(Type type)
        {
            TypesByName.TryAdd(type.GetATypeInfo().Name, type);
            //OS.Log($"Registered name for type {type.GetTypeName()}.");

            /*
            if (!TypesByName.TryAdd(id, type))
            {
                if (TypesByName.ContainsKey(id))
                {
                    if (TypesByName[id] != type)
                        throw new Exception(string.Format("Attempted to map ID {0} onto type {1} but type {2} is already using this ID. Please assign a different ID to this type",
                            id, type, TypesByName[id]));
                }
                else
                {
                    throw new Exception(string.Format("Attempted to map ID {0} onto type {1} but this type already has been assigned an ID of {2}.",
                        id, type, TypesByName.GetBySecond(type)));
                }
            }
            */
        }

        public static bool IsTypeInfo(this Type type)
        {
            return typeof(ATypeInfo).IsAssignableFrom(type);
        }

        public static T Copy<T>(this T source)
        {
            return source.CopyTo(Activator.CreateInstance<T>());
        }

        public static T CopyTo<T>(this T source, T target, bool useReplication = true, List<string> excludedNames = null)
        {
            var sourceInfo = typeof(T).GetATypeInfo();
            foreach (var propertyInfo in useReplication ? sourceInfo.ReplicatedPropertyInfos : sourceInfo.PropertyInfos)
            {
                if (useReplication && excludedNames != null && excludedNames.Contains(propertyInfo.Name))
                    continue;

                //Debug.Log(propertyInfo.Name + ", " + propertyInfo.Get(source));
                var newTarget = (object)target;
                propertyInfo.Set(ref newTarget, propertyInfo.Get(source));
            }
            return target;
        }

        public static IEnumerable<Type> GetParentTypes(this Type type)
        {
            // is there any base type?
            if (type == null)
            {
                yield break;
            }

            // return all implemented or inherited interfaces
            foreach (var i in type.GetInterfaces())
            {
                yield return i;
            }

            // return all inherited types
            var currentBaseType = type.BaseType;
            while (currentBaseType != null)
            {
                yield return currentBaseType;
                currentBaseType = currentBaseType.BaseType;
            }
        }

        public static bool IsSubclassOfRawGeneric(this Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }
    }
}
