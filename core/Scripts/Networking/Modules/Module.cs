using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit
{
    public class Module<TContainer, TModule> : IModule<TContainer, TModule> where TModule : IModule where TContainer : IContainer<TContainer, TModule>
    {
        //public TaskCompletionSource<bool> CompletionSource { get; set; } = new TaskCompletionSource<bool>();

        protected TContainer container;

        public TType GetModule<TType>() where TType : TModule
        {
            if (container == null)
                throw new Exception("Container not found! Make sure module is added to a container and initialized before calling this method.");
            var module = container.Get<TType>();
            if (module == null)
                throw new Exception("Module not found! Make sure module is added to this module's container before calling this method.");
            return module;
        }

        public virtual async Task OnAdded()
        {
        }

        public void SetContainer(TContainer contianer)
        {
            this.container = contianer;
        }

        public virtual void Dispose()
        {

        }

        public IEnumerable<TModule> GetDependentModuleTypes()
        {
            var requireModuleAttributes = GetType().GetCustomAttributes(true).Where(x => x is RequireModuleAttribute).Select(x => x as RequireModuleAttribute);
            foreach (var requireModuleAttribute in requireModuleAttributes)
            {
                if (!container.HasModule(requireModuleAttribute.Type))
                {
                    OS.LogError($"Missing module {requireModuleAttribute.Type.Name}! Requires this module to open module {GetType().Name}.");
                }
                else
                {
                    OS.Log($"Waiting to open module {requireModuleAttribute.Type.Name} to open module {GetType().Name}.");
                    yield return container.GetModule(requireModuleAttribute.Type);
                }
            }
        }
    }
}
