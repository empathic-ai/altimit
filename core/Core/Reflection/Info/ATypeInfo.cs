using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Altimit
{

    public class ATypeInfo<TClass> : ATypeInfo
    {
        public ATypeInfo(Type type) : base(type) {
        }

        public void MapProperty<TProperty>(string propertyName, Func<TClass, TProperty> getter = null, SetByRef<TClass, TProperty> setter = null,
            ObserveType observeType = ObserveType.Mutable, Func<TProperty, TProperty, bool> isEqualFunc = null)
        {
            MapProperty<TClass, TProperty>(propertyName, getter, setter, observeType, isEqualFunc);
        }

        public void MapProperty<TProperty>(Expression<Func<TClass, TProperty>> propertyName, Func<TClass, TProperty> getter = null, SetByRef<TClass, TProperty> setter = null,
ObserveType observeType = ObserveType.Mutable, Func<TProperty, TProperty, bool> isEqualFunc = null)
        {
            MapProperty<TClass, TProperty>(propertyName, getter, setter, observeType, isEqualFunc);
        }
    }

    // Used to cache Altimit-related type information
    // TODO: Consider creating IsOverride option for this class's construction. This would only map overrides in PropertyInfos, which could then be easily looked up and weaved into classes post-compilation
    public class ATypeInfo
    {
        public string Name { get; set; }
        public bool ForcePolling { get; set; } = false;
        public bool IsEnum { get; set; } = false;
        public bool IsStructure { get; set; } = false;
        public bool IsCollection { get; protected set; } = false;
        public bool IsInstance { get; protected set; }
        public Type ElementType { get; protected set; }
        public Type Type { get; protected set; }
        public bool IsReplicated { get; protected set; } = false;
        public Dictionary<string, APropertyInfo> PropertyInfosByName { get; set; } = new Dictionary<string, APropertyInfo>();
        public APropertyInfo[] PropertyInfos { get; set; }
        public APropertyInfo[] ReplicatedPropertyInfos { get; set; }
        public APropertyInfo[] MutablePropertyInfos { get; protected set; }
        public AMethodInfo[] MethodInfos { get; protected set; }
        public AMethodInfo[] ReplicatedMethodInfos { get; protected set; }
        public List<AConstructorInfo> ConstructorInfos { get; protected set; } = new List<AConstructorInfo>();
        public AConstructorInfo DefaultConstructorInfo
        {
            get { return ConstructorInfos[0]; }
        }

        public AMethodInfo GetMethodInfo(string methodName, Type[] types)
        {
            
            var methods = MethodInfos.Where(x => x.Name == methodName && x.ParameterTypes.IsEqualContent(types));
            /*
            if (methods.Count() != 1)
            {
                OS.Log(Name);
                OS.Log(methodName);
                foreach (var _type in types)
                {
                    OS.Log(_type.GetTypeName());
                }

                OS.Log(types.Length);
                foreach (var method in MethodInfos)
                {
                    foreach (var type in method.ParameterTypes)
                    {
                        OS.Log(type.GetTypeName());
                    }
                }
                //methods = methods.OrderBy(x => GetStepsFromType(x.BaseMethodInfo));
            }
            */
            return methods.First();
        }

        /*
        int GetStepsFromType(MemberInfo memberInfo)
        {
            var type = Type;
            int steps = 0;
            while (memberInfo.DeclaringType != type) {
                type = type.BaseType;
                return steps;
            }
            return steps;
        }
        */

        public bool TryGetPropertyInfo(string propertyName, out APropertyInfo propertyInfo)
        {
            return PropertyInfosByName.TryGetValue(propertyName, out propertyInfo);
        }

        public APropertyInfo GetPropertyInfo(string propertyName)
        {
            APropertyInfo propertyInfo;
            if (!TryGetPropertyInfo(propertyName, out propertyInfo))
            {
                if (Type.IsCollection())
                {
                    var index = int.Parse(propertyName);
                    return GetPropertyInfoByIndex(index);

                }
                throw new ArgumentException(string.Format("The property {0} could not be found within {1}. Please register it as an AProperty.", propertyName, Name));
            }
            return propertyInfo;
        }

        public APropertyInfo GetPropertyInfoByIndex(int index)
        {
            try
            {
                if (Type.IsCollection())
                {
                    return new APropertyInfo(Type, ElementType,
                        index.ToString(),
                        true,
                        x => x.GetElement(index),
                        (ref object x, object y) => x.SetElement(index, y));
                }
                else
                {
                    return ReplicatedPropertyInfos[index];
                }
            }
            catch
            {
                throw new ArgumentException(string.Format("Failed to find property using an index of {0} within {1}.", index, Name));
            }
        }

        // Used for creating type info for a mock dynamic type--see TypeExtensions.Locals
        public ATypeInfo(string name, Type type, Type elementType, APropertyInfo[] aPropertyInfos, bool isInstance, AConstructorInfo constructorInfo)
        {
            Name = name;
            Type = type;
            ElementType = elementType;
            ReplicatedPropertyInfos = aPropertyInfos;
            PropertyInfosByName = aPropertyInfos.ToDictionary(x => x.Name);
            IsInstance = isInstance;
            IsStructure = true;
            IsCollection = elementType != null;
            ConstructorInfos.Add(constructorInfo);
        }

        public ATypeInfo(Type nativeType)
        {
            try
            {
                Type = nativeType.GetTrueType();
                IsStructure = nativeType.IsNativeStructure();
                ElementType = nativeType.GetCollectionElementType();
                IsCollection = nativeType.IsNativeCollection();
                IsInstance = nativeType.IsNativeInstance();
                Name = Type.GetNativeTypeName();
                IsEnum = Type.IsEnum;

                var aTypeAttribute = nativeType.GetCustomAttribute<ATypeAttribute>(false);

                if (aTypeAttribute != null)
                    ForcePolling = aTypeAttribute.ForcePolling;

                var ignoreAttribute = nativeType.GetCustomAttribute<IgnoreAttribute>(false);

                if (ignoreAttribute == null)
                {
                    if (aTypeAttribute != null)
                    {
                        IsReplicated = true;
                    }

                    var aTypeInfoAttribute = GetType().GetCustomAttribute<ATypeInfoAttribute>();
                    if (aTypeInfoAttribute != null)
                    {
                        IsReplicated = true;
                    }
                }

                var bindingFlags = BindingFlags.NonPublic | BindingFlags.Public |
                       BindingFlags.Instance |
                       BindingFlags.FlattenHierarchy;

                var properties = nativeType.GetProperties(bindingFlags).Cast<MemberInfo>()
                    .Concat(nativeType.GetFields(bindingFlags))
                    .Where(x => x.GetCustomAttribute<APropertyAttribute>() != null);

                foreach (var property in properties)
                {
                    AddProperty(property);
                }

                InheritMaps();
                InheritConstructor();

                PropertyInfos = PropertyInfosByName.Select(x => x.Value).ToArray();
                ReplicatedPropertyInfos = PropertyInfos.Where(x=>x.IsReplicated).OrderBy(x => x.Name).ToArray();
                MutablePropertyInfos = PropertyInfos.Where(x => x.ObserveType.HasFlag(ObserveType.Mutable)).ToArray();
                var nativeMethodInfos = GetMethodsWithFlattenedInterface(Type);
                MethodInfos = nativeMethodInfos.Select(x => new AMethodInfo(Type, x)).ToArray();
                ReplicatedMethodInfos = MethodInfos.Where(x => x.IsReplicated).ToArray();

                for (int i = 0; i < ReplicatedPropertyInfos.Length; i++)
                {
                    ReplicatedPropertyInfos[i].Index = i;
                }

                var constructors = nativeType.GetConstructors();
                foreach (var constructor in constructors)
                {
                    var constructorAttribute = constructor.GetCustomAttribute<AConstructorAttribute>();
                    string[] propertyNames = null;
                    if (constructorAttribute != null)
                        propertyNames = constructorAttribute.PropertyNames;
                    ConstructorInfos.Add(new AConstructorInfo(Type, constructor, propertyNames));
                }

                var customConstructors = nativeType.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).Where(
                    x => x.GetCustomAttribute<AConstructorAttribute>() != null);

                ConstructorInfos.AddRange(customConstructors.Select(x =>
                    new AConstructorInfo(Type, args => x.Invoke(null, new object[] { args }),
                    x.GetCustomAttribute<AConstructorAttribute>().PropertyNames)));

                ConstructorInfos = ConstructorInfos.Where(x => x.PropertyNames != null).OrderByDescending(x => x.PropertyNames.Length).ToList();

                if (ConstructorInfos.Count == 0)
                {
                    ConstructorInfos.Add(new AConstructorInfo(Type, objects =>
                        (nativeType.IsArray ? Activator.CreateInstance(nativeType, objects) :
                            Activator.CreateInstance(nativeType)), new string[0]));
                }
            } catch (Exception e)
            {
                OS.Logger.LogError(new Exception($"Error creating TypeInfo of type {nativeType}.", e));
            }
        }


        public virtual void InheritMaps()
        {
        }
        public virtual void InheritConstructor()
        {
        }

        public APropertyInfo AddProperty(MemberInfo memberInfo)
        {
            var propertyAttribute = memberInfo.GetCustomAttribute<APropertyAttribute>();
            var setter = memberInfo.GetSetter();
            return AddProperty(new APropertyInfo(Type, GetUnderlyingType(memberInfo), memberInfo.Name, propertyAttribute != null && setter != null, memberInfo.GetGetter(),
                setter, propertyAttribute != null ? propertyAttribute.ObserveType : ObserveType.Ignore));
        }

        public static Type GetUnderlyingType(MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                default:
                    throw new ArgumentException
                    (
                        "Input MemberInfo must be an event, field, method, or property."
                    );
            }
        }

        public static List<MethodInfo> GetMethodsWithFlattenedInterface(Type type)
        {
            var methods = new List<MethodInfo>();
            methods.AddRange(type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy));

            if (type.IsInterface)
            {
                foreach (var childInterfaceType in type.GetInterfaces())
                {
                    methods.AddRange(childInterfaceType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy));
                }
            }
            return methods;
        }

        public static D GetMapperProperty<D>(Type mapperType, string name)
        {
            return (D)mapperType.GetProperty(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).GetValue(null, null);
        }

        public void MapConstructor(Func<object[], object> func, params string[] propertyNames)
        {
            ConstructorInfos.Add(new AConstructorInfo(Type, func, propertyNames));
        }
        
        public void MapProperty<TClass, TProperty>(string propertyName, Func<TClass, TProperty> getter = null, SetByRef<TClass, TProperty> setter = null,
            ObserveType observeType = ObserveType.Mutable, Func<TProperty, TProperty, bool> isEqualFunc = null)
        {
            MapProperty(
                propertyName,
                typeof(TProperty),
                ConvertGetter(getter),
                ConvertSetter(setter),
                observeType,
                ConvertEqualFunc(isEqualFunc));
        }

        public void MapProperty<TClass, TProperty>(Expression<Func<TClass, TProperty>> propertyName, Func<TClass, TProperty> getter = null, SetByRef<TClass, TProperty> setter = null,
ObserveType observeType = ObserveType.Mutable, Func<TProperty, TProperty, bool> isEqualFunc = null)
        {
            MapProperty(
                BindingExtensions.GetPropertyName(propertyName),
                typeof(TProperty),
                getter: ConvertGetter(getter),
                setter: ConvertSetter(setter),
                observeType: observeType,
                isEqualFunc: ConvertEqualFunc(isEqualFunc));
        }

        public void MapProperty(string propertyName, Type propertyType = null, Func<object,object> getter = null, SetByRef setter = null,
ObserveType observeType = ObserveType.Mutable, Func<object, object, bool> isEqualFunc = null)
        {
            APropertyInfo propertyInfo;
            // Add property info if it doesn't exist at all yet
            if (!PropertyInfosByName.TryGetValue(propertyName, out propertyInfo))
            {
                MemberInfo memberInfo;
                memberInfo = Type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                if (memberInfo == null)
                {
                    memberInfo = Type.GetField(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                }

                // If there is a property or field with this name and type, add that property
                if (memberInfo != null && (propertyType == null || GetUnderlyingType(memberInfo).Equals(propertyType)))
                {
                    propertyInfo = AddProperty(memberInfo);
                // Otherwise, create a new property info completely
                } else
                {
                    if (propertyType == null)
                        OS.Logger.LogError($"Missing property type for custom mapped property {propertyName}!");
                    if (getter == null)
                        OS.Logger.LogError($"Missing getter for custom mapped property {propertyName}!");
                    if (setter == null)
                        OS.Logger.LogError($"Missing setter for custom mapped property {propertyName}!");

                    propertyInfo = AddProperty(new APropertyInfo(Type, propertyType, propertyName, true));
                }
            }

            // Update the proeprty info based on the arguments given
            if (getter != null)
                propertyInfo.Getter = getter;
            if (setter != null)
                propertyInfo.Setter = setter;
            propertyInfo.IsEqualFunc = isEqualFunc;
            propertyInfo.ObserveType = observeType;
            //propertyInfo.FormatType = formatType;
            propertyInfo.IsReplicated = true;
        }

        public APropertyInfo AddProperty(APropertyInfo propertyInfo)
        {
            PropertyInfosByName.Add(propertyInfo.Name, propertyInfo);
            return propertyInfo;
        }

        /*
        public void CreateProperty<TClass, TProperty>(string propertyName, bool isReplicated, Func<TClass, TProperty> getter, SetByRef<TClass, TProperty> setter,
            FormatType overrideType)
        {
            CreateProperty<TClass, TProperty>(propertyName, isReplicated, getter, setter, ObserveType.Mutable, null, overrideType);
        }

        public void CreateProperty<TClass, TProperty>(string propertyName, bool isReplicated, Func<TClass, TProperty> getter, SetByRef<TClass, TProperty> setter,
            ObserveType observeType = ObserveType.Mutable, Func<TProperty, TProperty, bool> isEqualFunc = null, FormatType overrideType = FormatType.Default)
        {
            CreateProperty(typeof(TProperty), propertyName, isReplicated, ConvertGetter(getter), ConvertSetter(setter), observeType, ConvertEqualFunc(isEqualFunc), overrideType);
        }
        

        void CreateProperty(Type propertyType, string propertyName, bool isReplicated, Func<object,object> getter, SetByRef setter,
            ObserveType observeType = ObserveType.Mutable, Func<object, object, bool> isEqualFunc = null, FormatType overrideType = FormatType.Default)
        {

            CreateProperty(propertyInfo);
        }
        */
        
        public ObserveType GetObserveType(APropertyAttribute idAttribute)
        {
            if (idAttribute != null)
            {
                return idAttribute.ObserveType;
            }
            return ObserveType.Ignore;
        }

        public Func<object, object, bool> GetEqualityCheck(APropertyAttribute idAttribute)
        {
            if (idAttribute != null && idAttribute.EqualityCheck != null)
            {
                return idAttribute.EqualityCheck;
            }
            return null;
        }
        
        public static Func<object, object> ConvertGetter<T1, T2>(Func<T1, T2> action)
        {
            if (action == null)
                return null;
            return (x) => action((T1)x);
        }

        public static SetByRef ConvertSetter<T1, T2>(SetByRef<T1, T2> func)
        {
            if (func == null)
                return null;
            return new SetByRef((ref object x, object y)=> {
                T1 convertX = (T1)x;
                func(ref convertX, (T2)y);
                x = convertX;
            });
        }

        public static Func<T, T, bool> ConvertEqualFunc<T>(Func<object, object, bool> func)
        {
            if (func == null)
                return null;
            return new Func<T, T, bool>((x, y) => { return func(x, y); });
        }
        public static Func<object, object, bool> ConvertEqualFunc<T>(Func<T, T, bool> func)
        {
            if (func == null)
                return null;
            return new Func<object, object, bool>((x, y) => { return func((T)x, (T)y); });
        }
        

    }

    public static class ReflectionExtensions
    {

        public static SetByRef GetSetter(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return GetSetter((FieldInfo)member);
                case MemberTypes.Property:
                    return GetSetter((PropertyInfo)member);
                default:
                    throw new ArgumentException
                    (
                        "Input MemberInfo must be a property or field."
                    );
            }
        }

        public static Func<object,object> GetGetter(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return GetGetter((FieldInfo)member);
                case MemberTypes.Property:
                    return GetGetter((PropertyInfo)member);
                default:
                    throw new ArgumentException
                    (
                        "Input MemberInfo must be a property or field."
                    );
            }
        }

        public static SetByRef GetSetter(PropertyInfo propertyInfo)
        {
            if ((propertyInfo.DeclaringType.ContainsGenericParameters) ||
                (propertyInfo.PropertyType.ContainsGenericParameters)
                || !propertyInfo.CanWrite || propertyInfo.GetIndexParameters().Length > 0)
                return null;

            var methodInfo = propertyInfo.GetSetMethod(true);
            
            var classParameter = Expression.Parameter(typeof(object).MakeByRefType());
            // Create local variable of object's true type and assign value of object
            ParameterExpression variableExpr = Expression.Variable(propertyInfo.DeclaringType, "convObject");
            var exp1 = Expression.Assign(variableExpr, Expression.Convert(classParameter, propertyInfo.DeclaringType));

            // Create local variable of property's true type and assign value of property object
            var propertyParameter = Expression.Parameter(typeof(object));
            var propertyConvParameter = Expression.Convert(propertyParameter, propertyInfo.PropertyType);

            // Call set method
            var exp2 = Expression.Call(variableExpr, methodInfo, propertyConvParameter);

            // Assign value of ref object to modified object of true type
            var exp3 = Expression.Assign(classParameter, Expression.Convert(variableExpr, typeof(object)));

            return Expression.Lambda<SetByRef>(Expression.Block(new[] { variableExpr }, exp1, exp2, exp3),
                classParameter, propertyParameter).Compile();
        }


        public static Func<object, object> GetGetter(PropertyInfo propertyInfo)
        {
            if ((propertyInfo.DeclaringType.ContainsGenericParameters) ||
                (propertyInfo.PropertyType.ContainsGenericParameters) ||
                !propertyInfo.CanRead || propertyInfo.GetIndexParameters().Length > 0)
                return null;

            bool isValueType = propertyInfo.DeclaringType.IsValueType;
            var methodInfo = propertyInfo.GetGetMethod(true);

            var classParameter = Expression.Parameter(typeof(object), "o");

            var exBody = Expression.Call(Expression.Convert(classParameter, propertyInfo.DeclaringType), methodInfo);

            var exBody2 = Expression.Convert(exBody, typeof(object));
            var lambda = Expression.Lambda<Func<object, object>>(exBody2, classParameter);

            var action = lambda.Compile();
            return action;
        }

        //TODO: Refer to two tabs in browser to create a getgetter and getsetter method for fieldinfos
        //use switch statement to get getters and setters for a memberinfo
        //this should fix issues with serializing a vector3 using its x,y, and z fields
        public static SetByRef GetSetter(FieldInfo fieldInfo)
        {
            if ((fieldInfo.DeclaringType.ContainsGenericParameters) ||
                (fieldInfo.FieldType.ContainsGenericParameters) ||
                fieldInfo.IsInitOnly)
                return null;

            var classParameter = Expression.Parameter(typeof(object).MakeByRefType());
            ParameterExpression variableExpr = Expression.Variable(fieldInfo.DeclaringType, "convObject");
            var exp1 = Expression.Assign(variableExpr, Expression.Convert(classParameter, fieldInfo.DeclaringType));
            var fieldMember = Expression.Field(variableExpr, fieldInfo);
            var propertyParameter = Expression.Parameter(typeof(object));
            var propertyConvParameter = Expression.Convert(propertyParameter, fieldInfo.FieldType);
            var exp2 = Expression.Assign(fieldMember, propertyConvParameter);
            var exp3 = Expression.Assign(classParameter, Expression.Convert(variableExpr, typeof(object)));

            return Expression.Lambda<SetByRef>(Expression.Block(new[] { variableExpr }, exp1, exp2, exp3),
                classParameter, propertyParameter).Compile();
        }


        public static Func<object, object> GetGetter(FieldInfo fieldInfo)
        {
            if (fieldInfo.DeclaringType.IsGenericType && fieldInfo.DeclaringType.IsGenericTypeDefinition || fieldInfo.FieldType.IsGenericType && fieldInfo.FieldType.IsGenericTypeDefinition)
                return null;

            var instExp = Expression.Parameter(typeof(object));
            var convInstExp = Expression.Convert(instExp, fieldInfo.DeclaringType);
            var fieldExp = Expression.Field(convInstExp, fieldInfo);
            var convExp = Expression.Convert(fieldExp, typeof(object));

            return Expression.Lambda<Func<object, object>>(convExp, instExp).Compile();
        }
    }

    public delegate void SetByRef(ref object o, object property);

    public delegate void SetByRef<Class, Property>(ref Class o, Property property);

}






