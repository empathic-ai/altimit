using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit
{
    public interface IContainer<TContainer, TModule> : IDisposable where TModule : IModule where TContainer : IContainer<TContainer, TModule>
    {
        T AddModule<T>(T module) where T : TModule;
        T Get<T>() where T : TModule;
        bool HasModule<T>() where T : TModule;
        bool HasModule(Type type);
        TModule GetModule(Type type);
    }
}
