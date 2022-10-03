using System;
using System.Collections.Generic;

namespace Altimit
{
    // TODO: Add custom observers for built-in types, such as Transform. Subscribe to events on those types instead of resorting to polling... which is evil!
    public class InstanceObserver : Updateable, IInstanceObserver
    {
        public object Instance { get; }
        public Dictionary<string, Action<object, string, object>> propertyHandlers = new Dictionary<string, Action<object, string, object>>();
        public Action onInstanceRemoved { get; set; }
        bool wasInstanceRemoved = false;

        private ATypeInfo typeInfo { get; }
        private bool isPolling = false;
        private object[] oldProperties { get; }

        public InstanceObserver(object instance) : base()
        {
            typeInfo = instance.GetATypeInfo();
            Instance = instance;
            foreach (var propertyInfo in typeInfo.MutablePropertyInfos)
            {
                propertyHandlers[propertyInfo.Name] = null;
            }
            // Observe properties by subscribing to INotifyPropertyChanged.PropertyChanged event
            if (instance is INotifyPropertyChanged)
            {
                //OS.Log(instance.GetType() + " has INotifyPropertyChanged!");
                (instance as INotifyPropertyChanged).PropertyChanged += OnPropertyChanged;
            } else
            {
                // Observe properties by polling
                isPolling = true;
                oldProperties = new object[typeInfo.MutablePropertyInfos.Length];
                for (int i = 0; i < typeInfo.MutablePropertyInfos.Length; i++)
                {
                    oldProperties[i] = typeInfo.MutablePropertyInfos[i].Get(Instance);
                }
                //OS.Logger.LogErrorFormat("Unable to observe instance of type {0} because it doesn't implement INotifyPropertyChanged! Implement it manually or add an AType attribute to this type.", instance.GetType());
            }
        }

        public override void Update()
        {
            if (Instance == null)
            {
                if (!wasInstanceRemoved)
                {
                    wasInstanceRemoved = true;
                    propertyHandlers.Clear();
                    onInstanceRemoved?.Invoke();
                }
                return;
            }
            if (isPolling)
            {
                for (int i = 0; i < typeInfo.MutablePropertyInfos.Length; i++)
                {
                    object oldProperty = oldProperties[i];
                    var propertyInfo = typeInfo.MutablePropertyInfos[i];
                    var propertyHandler = propertyHandlers[propertyInfo.Name];

                    //If something is subscribed to this property (via propertyHandler)
                    if (propertyHandler != null)
                    {
                        var property = propertyInfo.Get(Instance);

                        if (!EqualProperties(propertyInfo, property, oldProperty))
                        {
                            OnPropertyChanged(Instance, propertyInfo.Name, oldProperty);
                            oldProperty = property;
                        }
                    }

                    oldProperties[i] = oldProperty;
                }
            }
        }

        public bool EqualProperties(APropertyInfo propertyInfo, object a, object b)
        {
            bool? equalNulls = TypeExtensions.EqualNulls(a, b);
            if (equalNulls != null)
                return (bool)equalNulls;

            var isEqualFunc = propertyInfo.IsEqualFunc;
            if (isEqualFunc != null)
                return isEqualFunc(a, b);

            return TypeExtensions.EqualObjects(a, b, propertyInfo.IsValue);
        }
        

        void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            OnPropertyChanged(sender, args.PropertyName, args.OldProperty);
        }

        void OnPropertyChanged(object instance, string propertyName, object oldProperty)
        {
            if (OS.LogObservations)
                OS.Log($"Property named {instance.GetPropertyInfo(propertyName)} changed to {instance.GetProperty(propertyName)}.");

            Action<object, string, object> propertyHandler;
            if (propertyHandlers.TryGetValue(propertyName, out propertyHandler))
                propertyHandler?.Invoke(instance, propertyName, oldProperty);
        }

        public void BindAll(Action<object, string, object> propertyChanged)
        {
            foreach(var propertyInfo in typeInfo.MutablePropertyInfos)
            {
                BindProperty(propertyInfo.Name, propertyChanged);
            }
        }

        public void UnbindAll(Action<object, string, object> propertyChanged)
        {
            foreach (var propertyInfo in typeInfo.MutablePropertyInfos)
            {
                UnbindProperty(propertyInfo.Name, propertyChanged);
            }
        }

        public void BindProperty(string propertyName, Action<object, string, object> propertyChanged)
        {
            Action<object, string, object> handler;
            if (propertyHandlers.TryGetValue(propertyName, out handler))
            {
                handler -= propertyChanged;
                handler += propertyChanged;
                propertyHandlers[propertyName] = handler;
            } else
            {
                OS.Logger.LogError($"Failed to bind property named {propertyName} in type {Instance.GetType().GetTypeName()}. Make sure it has an AProperty attribute!");
            }
        }

        public void UnbindProperty(string propertyName, Action<object, string, object> propertyChanged)
        {
            propertyHandlers[propertyName] -= propertyChanged;
        }
    }
}