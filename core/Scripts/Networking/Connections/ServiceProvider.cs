using Altimit.Serialization;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Altimit.Networking;
using Altimit.UI;

namespace Altimit.Networking
{
    // Represents an endpoint for services, either local or over a network
    public class ServiceProvider
    {
        public ServiceProvider Peer { get; set; }
        public App App { get; set; }
        Dictionary<Type, IConnectionSingleton> ServicesByType = new Dictionary<Type, IConnectionSingleton>();

        public T AddService<T>() where T : IConnectionSingleton
        {
            return (T)AddService((IConnectionSingleton)Activator.CreateInstance(typeof(T)));
        }

        public IConnectionSingleton AddService(Type type)
        {
            return (IConnectionSingleton)AddService((IConnectionSingleton)Activator.CreateInstance(type));
        }

        public T AddService<T>(T module) where T : IConnectionSingleton
        {
            return (T)AddService((IConnectionSingleton)module);
        }

        public IConnectionSingleton AddService(IConnectionSingleton module)
        {
            ServicesByType.Add(module.GetType(), module);
            return module;
        }

        public bool HasModule<T>() where T : IConnectionSingleton
        {
            throw new NotImplementedException();
        }

        public bool HasService(Type type)
        {
            throw new NotImplementedException();
        }
        public IConnectionSingleton GetService(Type type)
        {
            throw new NotImplementedException();
        }

        public T GetService<T>() where T : IConnectionSingleton
        {
            return (T)ServicesByType[typeof(T)];
        }

        public virtual void Dispose()
        {
        }
    }
}
