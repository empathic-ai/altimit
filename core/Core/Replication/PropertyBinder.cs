using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Altimit
{
    public class PropertyBinder
    {
        string[] propertyPath;
        object[] propertyInstances;
        Action<object, string, object> action;

        public PropertyBinder(object instance, Expression propExp, Action<object, string, object> action, bool isInit = true)
        {
            propertyPath = BindingExtensions.GetPropertyPath(propExp).ToArray();
            propertyInstances = new object[propertyPath.Length];
            object pathInstance = instance;
            for (int i = 0; i < propertyPath.Length; i++)
            {
                if (pathInstance == null)
                    break;

                var propertyName = propertyPath[i];
                propertyInstances[i] = pathInstance;
                pathInstance.BindProperty(propertyName, OnPropertyChanged);

                if (i == propertyPath.Length - 1)
                {
                    // Initialize from data in last part of path, if isInit is true
                    if (isInit)
                    {
                        action(pathInstance, propertyName, pathInstance.GetProperty(propertyName));
                    }
                    break;
                }

                pathInstance = pathInstance.GetProperty(propertyName);
            }
            this.action = action;
        }

        void OnPropertyChanged(object instance, string propertyName, object oldProperty)
        {
            //OS.Log("Property changed in linked binder! " + instance.GetType() + "." + propertyName);
            var nextInstanceIndex = Array.IndexOf(propertyInstances, instance) + 1;

            // Get the first old property instance that was linked and shouldn't be anymore
            // Unbind that property and unbind all subsequent ones
            object pathInstance;
            string pathPropertyName;
            object pathOldProperty;
            if (nextInstanceIndex < propertyPath.Length)
            {
                pathOldProperty = null;
                pathInstance = propertyInstances[nextInstanceIndex];
                for (int i = nextInstanceIndex; i < propertyPath.Length; i++)
                {
                    if (pathInstance == null)
                        break;

                    pathPropertyName = propertyPath[i];
                    pathInstance.UnbindProperty(pathPropertyName, action);
                    if (i == propertyPath.Length - 1)
                    {
                        pathOldProperty = pathInstance.GetProperty(pathPropertyName);
                        break;
                    }
                    pathInstance = pathInstance.GetProperty(pathPropertyName);
                }

                // Get the first new property instance that wasn't linked and should be now
                // Bind that property and bind all subsequent ones
                pathInstance = instance.GetProperty(propertyName);
                for (int i = nextInstanceIndex; i < propertyPath.Length; i++)
                {
                    if (pathInstance == null)
                        break;

                    pathPropertyName = propertyPath[i];
                    pathInstance.BindProperty(pathPropertyName, action);
                    if (i == propertyPath.Length - 1)
                        break;
                    pathInstance = pathInstance.GetProperty(pathPropertyName);
                }
            }
            else
            {
                pathInstance = instance;
                pathPropertyName = propertyName;
                pathOldProperty = oldProperty;
            }

            // Invoke the action associated with the final binding
            action.Invoke(pathInstance, propertyName, pathOldProperty);
        }
    }
}