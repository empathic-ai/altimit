using System;
using System.Collections.Generic;

namespace Altimit
{
    public static class ObserverExtensions
    {
        public static Dictionary<Type, Type> ObserverTypes = new Dictionary<Type, Type>();
        public static PairedDictionary<object, IInstanceObserver> ObserverTargets = new PairedDictionary<object, IInstanceObserver>();

        static ObserverExtensions()
        {
        }

        public static void RegisterObserverType(Type type)
        {
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            var targetType = type.BaseType.GetGenericArguments()[0];
            ObserverTypes.Add(targetType, type);
        }

        public static bool IsGlobalObserver(Type type)
        {
            return (!type.IsGenericTypeDefinition && (type.BaseType != null && type.BaseType.IsGenericType) && typeof(IInstanceObserver).IsAssignableFrom(type));
        }

        public static IInstanceObserver GetObserver(this object instance)
        {
            IInstanceObserver observer;
            if (!TryGetObserver(instance, out observer))
            {
                Type observerType;
                if (!TryGetObserverType(instance.GetType(), out observerType))
                {
                    observerType = typeof(InstanceObserver);
                }

                observer = (IInstanceObserver)Activator.CreateInstance(observerType, instance);

                ObserverTargets.Add(instance, observer);
            }
            return observer;
        }

        public static bool TryGetObserver(this object global, out IInstanceObserver observer)
        {
            if (global == null)
                throw new ArgumentException("Cannot get mapper for null object!");

            if (global as IInstanceObserver != null)
                throw new ArgumentException("Cannot get mapper for a mapper!");

            if (ObserverTargets.TryGetByFirst(global, out observer))
            {
                return true;
            }

            return false;
        }

        public static bool TryGetObserverType(Type type, out Type observerType)
        {
            return ObserverExtensions.ObserverTypes.TryGetValue(type, out observerType);
        }
    }
}
