using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;
using Altimit;

namespace Altimit
{
    public partial class Database
    {
        public IEnumerable<object> GetInstances()
        {
            return InstancesByID.Select(x => x.Value);
        }
        public AID TryGetInstanceID(object instance)
        {
            AID id;
            if (instance == null)
            {
                id = AID.Empty;
            }
            else if (!InstancesByID.TryGetBySecond(instance, out id))
            {
                id = AID.Empty;
            }
            return id;
        }

        public object GetInstance(AID id)
        {
            if (id.Equals(AID.Empty))
                Logger.LogError("Tried to get an instance with an invalid ID! ID was empty.");

            object instance;
            if (!TryGetInstance(id, out instance))
            {
                Logger.LogError($"Failed to find instance with an ID of {id}!");
            }

            return instance;
        }

        public T CreateInstance<T>(AID id)
        {
            return (T)CreateInstance(typeof(T), id);
        }

        public object CreateInstance(Type type, AID id)
        {
            var instance = Activator.CreateInstance(type);
            return AddInstance(id, instance);
        }

        public async Task<T> GetInstanceAsync<T>(AID id)
        {
            return (T) await GetInstanceAsync(id);
        }

        public async Task<object> GetInstanceAsync(AID id)
        {
            //Logger.Log($"Asynchronously getting instance with an ID of {id}.");

            object instance;
            if (TryGetInstance(id, out instance))
            {
                //Logger.Log($"Asynchronously got instance with an ID of {id}. Already exists within database.");
                return instance;
            }
            else
            {
                foreach (var func in GetInstanceFuncs)
                {
                    instance = await func(id);
                    if (instance != null)
                        return instance;
                }

                if (instance == null)
                    Logger.LogError($"Failed to asynchronously get instance with an ID of {id}!");

                //Logger.Log($"Asynchronously failed to get instance with an ID of {id}.");

                return null;
            }
        }

        public AID GetInstanceID(object instance)
        {
            AID id;
            if (instance == null)
            {
                Logger.LogError($"Attempted to get ID for a null instance!");
                return AID.Empty;
            }
            else if (!TryGetInstanceID(instance, out id))
            {
                var instanceString = instance == null ? "null" : instance.GetType().ToString();
                Logger.LogError($"An ID has not been assigned for an instance of type {instanceString} with a value of {instance.GlobalToString()}! Add the instance to this globe.");
            }
            return id;
        }

        public AID TryAddOrGetInstanceID(object instance)
        {
            AID id;
            if (instance == null)
            {
                id = AID.Empty;
                //throw (new Exception(string.Format("Attempted to add/get ID for a null instance!")));
            }
            else if (!InstancesByID.TryGetBySecond(instance, out id))
            {
                id = GetInstanceID(AddInstance(instance));
            }
            return id;
        }

        public virtual void ClearInstances()
        {
            MaxGlobalID = 0;
            var globalIDs = InstancesByID.GetFirst().Select(x => x.Key).ToArray();
            foreach (var id in globalIDs)
            {
                RemoveInstance(id);
            }
        }

        public AID CreateInstanceID(object instance)
        {
            if (instance is IID)
            {
                return ((IID)instance).ID;
            }
            return AID.New();
        }

        public bool TryAddInstance(object instance, out AID instanceID)
        {
            var wasAdded = TryAddInstance(instance);
            instanceID = GetInstanceID(instance);
            return wasAdded;
        }

        public bool TryAddInstance(object instance)
        {
            return TryAddInstance(CreateInstanceID(instance), instance);
        }

        public bool TryAddInstance(AID instanceID, object instance)
        {
            if (isLocked)
                return false;

            if (instance.IsReplicatedInstance() && !HasInstance(instance))
            {
                AddInstance(instanceID, instance);
                return true;
            }

            return false;
        }

        public object AddOrSetInstance(AID instanceID, object instance)
        {
            object existingGlobal;
            if (TryGetInstance(instanceID, out existingGlobal))
            {
                if (!instance.Equals(existingGlobal))
                {
                    instance.CopyTo(existingGlobal);
                }
                return existingGlobal;
            }
            else
            {
                AddInstance(instanceID, instance);
                return instance;
            }
        }

        public object AddInstance(object instance) {
            return AddInstance(CreateInstanceID(instance), instance);
        }

        /*
        public virtual void TryAddInstanceGroup(IDictionary<Guid, object> instancesByID)
        {
            Dictionary<Guid, object> newInstancesByID = new Dictionary<Guid, object>();
            var instanceGroupCount = 0;

            foreach (var instanceByID in instancesByID)
            {
                if (!HasInstanceID(instanceByID.Key))
                {
                    newInstancesByID.Add(instanceByID.Key, instanceByID.Value);
                    instanceGroupCount++;
                }
            }

            if (instanceGroupCount == 1)
            {
                AddInstanceOrGroup(newInstancesByID.First().Key, newInstancesByID.First().Value);
            }
            else if (instanceGroupCount > 1)
            {
                AddInstanceGroup(newInstancesByID);
            }
        }
        */

        public IEnumerable<object> GetUnaddedInstances(object instance)
        {
            return instance.GetAllProperties(true).Select(x => x).Where(x => x.IsInstance() && !HasInstance(x)).Distinct();
        }

        /*
        public void AddInstanceOrGroup(object instance)
        {
            var instanceGroup = GetUnaddedInstances(instance).ToList();
            if (instanceGroup.Count == 1)
            {
                AddInstance(instanceGroup[0]);
            } else
            {
                AddInstanceGroup(instanceGroup);
            }
        }

        
        public virtual void AddInstanceGroup(IEnumerable<object> instances)
        {
            foreach (var _instance in instances)
            {
                AddInstance(_instance);
            }
            OnInstanceGroupAdded(instances.ToArray());
        }

        public virtual void AddInstanceGroup(IDictionary<AID, object> instancesByID)
        {
            List<object> instanceGroup = instancesByID.Select(x => x.Value).ToList();

            foreach (var instanceByID in instancesByID)
            {
                AddInstance(instanceByID.Key, instanceByID.Value, true);
            }
            OnInstanceGroupAdded(instanceGroup.ToArray());
        }
        */

        /*
        public object AddInstance(AID instanceID, object instance)
        {
            return AddInstance(instanceID, instance, false);
        }*/

        public object AddInstance(AID instanceID, object instance)
        {
            //lock (instance)
           // {
                if (OS.LogInstanceDB)
                    Logger.Log($"Adding instance of type {instance.GetType().ToString()}");

                if (!instance.IsReplicatedInstance())
                {
                    Logger.LogError($"Attempted to add invalid instance with an ID of {instanceID} and type {instance.GetType()}.");
                }

                if (OS.LogInstanceDB)
                    Logger.Log($"Adding instance with an ID of {instanceID} and type {instance.GetType()}.");

                if (!InstancesByID.TryAdd(instanceID, instance))
                {
                    AID otherID;
                    if (InstancesByID.TryGetBySecond(instance, out otherID))
                    {
                        Logger.LogError(new ArgumentException($"Globe already contains instance of type {instance.GetType()} and value ({instance.GlobalToString()}). " +
                            $"Existing instance ID is {otherID}. New instance ID is {instanceID}. {InstancesByID.Count} instances are in use."));
                    }
                    else
                    {
                        Logger.LogError($"Globe already contains instance with an ID of {instanceID}. {InstancesByID.Count} instances are in use.");
                    }
                }

                //if (!isInstanceGroup)
                OnInstanceAdded(instance);
            //}
            return instance;
        }

        public virtual async void OnInstanceAdded(object instance)
        {
            if (IsAssetType(instance.GetType()) && !HasAsset(instance))
                await AddAsset(instance.GetType(), instance);

            if (isObserving)
            {
                if (instance is INotifyMethodCalled)
                {
                    (instance as INotifyMethodCalled).MethodCalled += OnMethodCalled;
                }
                if (instance is IObservableList)
                {
                    var list = instance as IObservableList;
                    list.ElementAdded += OnElementAdded;
                    list.ElementRemoved += OnElementRemoved;
                }
            }

            foreach (var (propertyInfo, property) in TypeExtensions.GetPropertiesByInfo(instance, true))
            {
                OnPropertyAdded(instance, property);
            }
            //instance.GetObserver().onInstanceRemoved += ()=> RemoveInstance(instance.GetInstanceID());

            onInstanceAdded?.Invoke(instance);

            if (isObserving && !instance.IsCollection())
            {
                foreach (var (propertyInfo, property) in TypeExtensions.GetPropertiesByInfo(instance, true))
                {
                    if (propertyInfo.ObserveType.HasFlag(ObserveType.Mutable))
                    {
                        if (OS.LogInstanceDB)
                            Logger.Log($"Observing property named { instance.GetPropertyInfo(propertyInfo.Name)}.");
                        instance.BindProperty(propertyInfo.Name, OnPropertyChanged);
                    }
                }
            }
        }

        /*
        public virtual void OnInstanceGroupAdded(object[] instanceGroup)
        {
            if (OS.LogInstanceDB)
                Logger.Log($"Added {instanceGroup.Length} dependent instances: {instanceGroup.GlobalToString()}.");
            onInstanceGroupAdded?.Invoke(instanceGroup);

            foreach (var instance in instanceGroup)
            {
                OnInstanceAdded(instance);
            }
        }
        */

        public virtual void RemoveInstance(AID instanceID)
        {
            object instance;
            if (InstancesByID.TryGetByFirst(instanceID, out instance))
            {
                //logger?.Log(string.Format("Removing instance with an ID of {0}.", instanceID));
                foreach (var (propertyInfo, property) in instance.GetPropertiesByInfo())
                {
                    if (property.IsInstance() && HasInstance(property)) {
                        OnPropertyRemoved(instance, property);
                    }
                }

                List<object> superInstances;
                if (superInstancesByInstance.TryGetValue(instance, out superInstances))
                {
                    // Clear any references to this instance within other instances (referred to as 'super' instances)
                    foreach (var superInstance in superInstances)
                    {
                        //logger?.Log(string.Format("Removing instance with a super instance of type {0}.", superInstance.GetType()));
                        if (superInstance.GetType().IsNativeCollection())
                        {
                            if (superInstance.GetType().HasGenericInterface(typeof(ICollection<>)))
                            {
                                //OS.Logger.Log("REMOVED ELEMENT OF TYPE " + instance.GetType().Name);
                                superInstance.CallMethod(nameof(ICollection<object>.Remove), new Type[] { instance.GetType() }, instance);
                            }
                        }
                        else
                        {
                            foreach (var propertyInfo in superInstance.GetATypeInfo().ReplicatedPropertyInfos)
                            {
                                if (!propertyInfo.CanSet)
                                    continue;

                                var property = propertyInfo.Get(superInstance);
                                if (property == instance)
                                {
                                    superInstance.SetProperty(propertyInfo.Name, null);
                                }
                            }
                        }
                    }
                }
                InstancesByID.TryRemoveByFirst(instanceID);
                OnInstanceRemoved(instanceID, instance); // possibly move to just prior to removal from database?
            }
            else
            {
                throw new Exception("Unable to remove global! It may have already been removed.");
            }
        }

        public virtual void OnInstanceRemoved(AID instanceID, object global)
        {
            IInstanceObserver observer;
            if (global.TryGetObserver(out observer))
            {
                observer.UnbindAll(OnPropertyChanged);
            }
            onInstanceRemoved?.Invoke(instanceID, global);
        }

        public void TryRemoveInstance(AID instanceID)
        {
            object instance;
            if (TryGetInstance(instanceID, out instance))
                TryRemoveInstance(instance);
        }

        public void TryRemoveInstance(object instance)
        {
            if (HasInstance(instance))
            {
                RemoveInstance(instance);
            }
        }

        public virtual void RemoveInstance(object instance)
        {
            if (!HasInstance(instance))
                Logger.LogError($"Tried to remove an instance of type {instance.GetType()} that doesn't exist in this globe!");

            RemoveInstance(GetInstanceID(instance));
        }

        public bool HasInstance(object instance)
        {
            return (InstancesByID.TryGetBySecond(instance, out _));
        }

        public bool HasInstanceID(AID instanceID)
        {
            return TryGetInstance(instanceID, out _);
        }

        /*
        public T GetInstance<T>(AID id, bool createIfEmpty = false)
        {
            return (T)GetInstance(id, createIfEmpty ? typeof(T) : null);
        }
        */

        public List<T> GetInstancesOfType<T>(bool includeInheritors = false)
        {
            var globals = new List<T>();
            foreach (var item in InstancesByID.GetFirst())
            {
                if ((includeInheritors && item.Value.GetType().IsAssignableFrom(typeof(T))) || (!includeInheritors && item.Value.GetType() == typeof(T)))
                    globals.Add((T)item.Value);
            }
            return globals;
        }

        public bool TryGetInstanceID(object instance, out AID instanceID)
        {
            return (InstancesByID.TryGetBySecond(instance, out instanceID));
        }

        public bool TryGetInstance(AID id, out object instance)
        {
            instance = null;

            if (id.Equals(AID.Empty))
                return false;

            return (InstancesByID.TryGetByFirst(id, out instance));
        }
    }
}
