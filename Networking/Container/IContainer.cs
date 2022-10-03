using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit
{
    public interface IContainer<TContainer, TModule> : IDisposable where TModule : IModule where TContainer : IContainer<TContainer, TModule>
    {
        public T AddModule<T>(T module) where T : TModule;
        public T Get<T>() where T : TModule;
        public bool HasModule<T>() where T : TModule;
        public bool HasModule(Type type);
        public TModule GetModule(Type type);
    }
}
