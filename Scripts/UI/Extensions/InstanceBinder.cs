using System.ComponentModel;

namespace Altimit.UI
{
    [AType]
    public class InstanceBinder
    {
        [AProperty]
        public object Instance { get => instance; set => Set(value); }
        object instance;

        protected virtual void Set(object instance)
        {
            this.instance = instance;
        }
    }
}