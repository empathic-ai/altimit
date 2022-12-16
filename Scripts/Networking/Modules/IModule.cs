using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit
{
    public interface IModule<TContainer, TModule> : IModule where TModule : IModule where TContainer : IContainer<TContainer, TModule>
    {
        void SetContainer(TContainer contianer);
        TType GetModule<TType>() where TType : TModule;
    }

    public interface IModule : IDisposable
    {
        //public TaskCompletionSource<bool> CompletionSource { get; }
        void Init();
    }
}
