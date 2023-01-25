using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit
{
    public abstract class ModuleContainer<TContainer, TModule> : IContainer<TContainer, TModule> where TModule : class, IModule<TContainer, TModule> where TContainer : class, IContainer<TContainer, TModule>
    {
        protected abstract TContainer container { get; }

        [AProperty]
        public AList<TModule> Modules = new AList<TModule>();

        public Action<TModule> OnModuleAdded;
        public Action<TModule> OnModuleRemoved;

        public T AddModule<T>() where T : TModule
        {
            return (T)AddModule(typeof(T));
        }

        public TModule AddModule(Type type)
        {
            return AddModule((TModule)Activator.CreateInstance(type));
        }

        public virtual T AddModule<T>(T module) where T : TModule
        {
            Modules.Add(module);
            module.SetContainer(container);
            module.OnAdded();
            OnModuleAdded?.Invoke(module);
            return module;
        }

        public virtual void RemoveModule<T>(T module) where T : TModule
        {
            module.Dispose();
            Modules.Remove(module);
        }

        public TModule GetModule(Type type)
        {
            var module = Modules.SingleOrDefault(x => type.IsAssignableFrom(x.GetType()));
            if (module == null)
            {
                OS.Logger.LogError($"Failed to locate module of type {type.GetNativeTypeName()}!");
            }
            return module;
        }

        public T AddOrGetModule<T>() where T : TModule
        {
            if (HasModule<T>())
                return (T)GetModule(typeof(T));
            return AddModule<T>();
        }

        public T Get<T>() where T : TModule
        {
            return (T)GetModule(typeof(T));
        }

        public T TryGetModule<T>() where T : TModule
        {
            if (HasModule<T>())
                return Get<T>();
            return default(T);
        }

        public bool HasModule<T>() where T : TModule
        {
            return HasModule(typeof(T));
        }
        public bool HasModule(Type type)
        {
            return Modules.Any(x => x.GetType().Equals(type));
        }

        public virtual void Dispose()
        {
            while(Modules.Count > 0)
            {
                var lastIndex = Modules.Count - 1;
                RemoveModule(Modules[lastIndex]);
            }
        }
    }
}
