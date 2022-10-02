using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Altimit
{
    public partial class InstanceDatabase : IInstanceResolver
    {
        // TODO: allow multiple filters for these three
        public Func<APropertyInfo, object, object> FilterPropertyIn = (x, y) => y;
        public Func<APropertyInfo, object, object> FilterPropertyOut = (x, y) => y;

        public List<Func<AID, Task<object>>> GetInstanceFuncs = new List<Func<AID, Task<object>>>();

        public Action<object, string, object> onPropertyChanged;
        public Action<object, string, Type[], object[]> onMethodCalled;
        public Action<object> onInstanceAdded;
        public Action<object> onGroupInstanceAdded;
        public Action<object, object> onSubInstancePassed;
        public Action<AID, object> onInstanceRemoved;


        public Action<object, int, object> onElementAdded;
        public Action<object, int, object> onElementRemoved;

        public Action<object, object> onSubObjectAdded;
        public Action<object, object> onSubObjectRemoved;

        public Action<AID, byte[]> onAssetAdded;
        public Action<AID, byte[]> onAssetUpdated;

        Dictionary<object, List<object>> subInstancesByInstance = new Dictionary<object, List<object>>();
        Dictionary<object, List<object>> superInstancesByInstance = new Dictionary<object, List<object>>();

        public PairedDictionary<AID, object> InstancesByID = new PairedDictionary<AID, object>();
        public static int Version = 0;
        public int MaxGlobalID = 0;
        public Func<object, bool> IsValidInstanceFunc;

        public static List<InstanceDatabase> Databases = new List<InstanceDatabase>();

        bool isLocked = false;
        bool isObserving = true;

        public ILogger Logger;

        /*
         public ReplicationDatabase(bool isObserving, ReplicationDatabase parentGlobe = null)
         {
             this.isObserving = isObserving;
             this.ParentGlobe = parentGlobe;
             Init();
         }*/

        public InstanceDatabase(bool isObserving, ILogger logger)
        {
            this.isObserving = isObserving;
            this.Logger = logger;
            Init();
        }

        public virtual void Init()
        {
            Databases.Add(this);
        }

        ~InstanceDatabase()
        {
            Databases.Remove(this);
        }

        public void Lock()
        {
            isLocked = true;
        }

        public void Unlock()
        {
            isLocked = false;
        }

        #region SUBINSTANCES ----------------------------------------------------------
        public virtual void OnPropertyAdded(object instance, object property)
        {
            TryAddProperty(property);

            if (property.IsInstance())
            {
                if (OS.LogInstanceDB)
                    Logger.Log($"Registering instance relationship: {TryGetInstanceID(instance)} contains {TryGetInstanceID(property)}.");

                if (!superInstancesByInstance.ContainsKey(property))
                    superInstancesByInstance[property] = new List<object>();
                superInstancesByInstance[property].Add(instance);

                if (!subInstancesByInstance.ContainsKey(instance))
                    subInstancesByInstance[instance] = new List<object>();
                subInstancesByInstance[instance].Add(property);

                OnSubInstancePassed(instance, property);
            }
            onSubObjectAdded?.Invoke(instance, property);
        }

        public virtual void OnPropertyRemoved(object instance, object property)
        {
            if (property.IsInstance())
            {
                //TODO: add back in tracking of super instances and sub instances
                /*
                List<object> superInstances;
                if (!superInstancesByInstance.TryGetValue(property, out superInstances))
                {
                    //Logger.LogError($"Failed to get super instances from instance with an ID of {TryGetInstanceID(property)} and type {property.GetType()} with a super instance with an ID of {TryGetInstanceID(instance)}.");
                }

                if (!superInstances.TryRemove(instance))
                {
                    //Logger.LogError($"Failed to remove super instance with an ID of {GetInstanceID(instance)} from instance with an ID of {GetInstanceID(property)}.");
                }

                if (superInstances.Count == 0)
                    superInstances.Remove(property);

                subInstancesByInstance[instance].Remove(property);
                if (subInstancesByInstance[instance].Count == 0)
                    subInstancesByInstance.Remove(instance);
                */
            }
            onSubObjectRemoved?.Invoke(GetInstanceID(instance), property);
        }

        // Called when a property is set, an array element is added or a method argument is passed
        public virtual void OnSubInstancePassed(object instance, object subInstance)
        {
            onSubInstancePassed?.Invoke(instance, subInstance);
        }
        #endregion

        #region PROPERTIES ----------------------------------------------------------

        public virtual bool TryAddProperty(object obj)
        {
            if (obj == null)
                return false;

            if (obj.GetType().IsInstance())
            {
                return TryAddInstance(obj);
            }

            if (obj.GetType().IsNativeStructure())
            {
                foreach (var property in TypeExtensions.GetProperties(obj, true))
                {
                    TryAddProperty(property);
                }
            }
               
            return false;
        }

        public void OnPropertyChanged(object instance, string propertyName, object oldProperty)
        {
            var propertyInfo = instance.GetPropertyInfo(propertyName);
            if (propertyInfo.IsReplicated)
            {
                var property = GetInstanceProperty(instance, propertyInfo.Name);
                if (property.IsInstance())
                    OnSubInstancePassed(instance, property);

                OnPropertyRemoved(instance, oldProperty);

                OnPropertyAdded(instance, property);

                onPropertyChanged?.Invoke(instance, propertyName, oldProperty);
            }
        }
        #endregion


        public void AddRange(InstanceDatabase snapshot)
        {
            foreach (var globalPair in snapshot.InstancesByID.GetFirst())
            {
                TryAddInstance(globalPair.Value);
            }
        }
       
        public object GetInstanceProperty(object instance, string propertyName)
        {
            var propertyInfo = instance.GetPropertyInfo(propertyName);
            var property = propertyInfo.Get(instance);
            if (FilterPropertyOut != null)
                property = FilterPropertyOut(propertyInfo, property);
            return property;
        }

        /*
        public void SetLocalProperty(AID instanceID, string propertyName, object localProperty)
        {
            var instance = GetInstance(instanceID);
            SetProperty(ref instance, propertyName, this.GlobalizeProperty(instance.GetPropertyInfo(propertyName), localProperty));
        }*/

        public void SetProperty(object instance, string propertyName, object value)
        {
            SetProperty(ref instance, propertyName, value);
        }

        public void SetProperty(ref object instance, string propertyName, object value)
        {
            //logger?.Log(string.Format("Setting property {0} with a value of {1}.", instance.GetPropertyInfo(propertyID), value));
            var propertyInfo = instance.GetPropertyInfo(propertyName);
            if (FilterPropertyIn != null)
                value = FilterPropertyIn(propertyInfo, value);
            propertyInfo.Set(ref instance, value);
        }

        async Task<object> IInstanceResolver.GetInstance(AID instanceID)
        {
            return GetInstance(instanceID);
        }

        /*
        public bool HasProperty(LocalPropertyReference reference)
        {
            object global;
            if (TryGetInstance(reference.ID, out global))
            {
                var propertyInfo = global.GetPropertyInfo(reference.PropertyID);
                var isPropertyReference = propertyInfo.PropertyType.IsReferenceType();
                if (!isPropertyReference || (isPropertyReference && HasInstance(reference.PropertyID)))
                {
                    //SetProperty(ref global, reference.PropertyID, FormatterExtensions.GlobalizeProperty(this, propertyInfo, reference.LocalProperty));
                    return true;
                }
            }
            return false;
        }*/
    }
}
