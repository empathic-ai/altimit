using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Altimit
{
    public partial class RuntimeDatabase
    {
        void OnElementAdded(object instance, ElementEventArgs eventArgs)
        {
            OnPropertyAdded(instance, eventArgs.element);
            onElementAdded?.Invoke(instance, eventArgs.index, eventArgs.element);
        }
        void OnElementRemoved(object instance, ElementEventArgs eventArgs)
        {
            OnPropertyRemoved(instance, eventArgs.element);
            onElementRemoved?.Invoke(instance, eventArgs.index, eventArgs.element);
        }
    }
}
