using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Altimit
{
    public partial class InstanceDatabase
    {
        private void OnMethodCalled(object instance, MethodCalledEventArgs e)
        {
            foreach (var methodArg in e.MethodArgs)
            {
                TryAddInstance(methodArg);
                if (methodArg.IsInstance())
                {
                    OnSubInstancePassed(instance, methodArg);
                    /*
                    if (instance.GetType().IsCollectionType() && e.MethodName == nameof(ICollection<object>.Add))
                    {
                        OnElementAdded(instance, methodArg);
                    }
                    else
                    {

                    }
                    */
                }
            }
            onMethodCalled?.Invoke(instance, e.MethodName, e.MethodTypes, e.MethodArgs);
        }

    }
}
