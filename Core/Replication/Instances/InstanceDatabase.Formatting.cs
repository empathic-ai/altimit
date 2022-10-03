using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;
using Altimit.Serialization;
#if UNITY_64
using UnityEngine;
#endif

namespace Altimit
{
    // Includes methods for 
    public partial class InstanceDatabase
    {
        public async Task<object> AddLocalInstanceAsync(AID instanceID, object localInstance)
        {
            Logger.LogError($"Adding local instance with an ID of {instanceID}!");

            var instance = await AddLocalInstanceAsync(localInstance);
            instance = await SetProperties(instance, localInstance);
            if (!HasInstanceID(instanceID))
                AddInstance(instanceID, instance);
            return await GetInstanceAsync(instanceID);
        }

        public LocalInstanceReference[] GetLocalInstances()
        {
            return GetLocalInstances(GetInstances().ToArray());
        }

        public LocalInstanceReference[] GetLocalInstances(object[] instances)
        {
            var localInstanceReferences = new LocalInstanceReference[instances.Length];

            for (int i = 0; i < instances.Length; i++)
            {
                var instance = instances[i];
                localInstanceReferences[i] = new LocalInstanceReference(instance.GetInstanceID(), instance.Localize());
            }

            return localInstanceReferences;
        }

        public async Task<IEnumerable<object>> AddLocalInstanceGroup(Dictionary<AID, object> localInstancesByID)
        {
            //resolver.Lock();

            var instancesByID = new Dictionary<AID, object>();

            // Construct each local instance into a global instance
            foreach (var localInstanceByID in localInstancesByID)
            {
                var instanceID = localInstanceByID.Key;

                if (HasInstanceID(instanceID))
                    throw new Exception(string.Format("Part of local instance group is already added to the resolver!"));

                if (!instancesByID.ContainsKey(instanceID))
                {
                    await AddLocalInstanceAsync(localInstanceByID.Value, instanceID);
                }
            }

            // Construct each local instance into a global instance
            foreach (var localInstanceByID in localInstancesByID)
            {
                //if (globe.HasInstanceID(localPair.Key))
                //    continue;

                var instance = instancesByID[localInstanceByID.Key];
                await SetProperties(instance, localInstanceByID.Value);
            }

            //resolver.Unlock();
            //TODO: reimpliement
            //resolver.AddInstanceGroup(instancesByID);

            return instancesByID.Select(x => x.Value);
        }

        public async Task<object> AddLocalInstanceAsync(object localObj, AID id = default)
        {
            var type = localObj.GetAType().GetGlobalType();
            var typeInfo = type.GetATypeInfo();

            // Get constructor arguments for the object--if we're dealing with an array, the arguments are the array length
            object[] args;
            if (type.IsCollection())
            {
                if (type.IsArray)
                {
                    args = new object[] { localObj.GetPropertyCount() };
                }
                else
                {
                    args = new object[0];
                }
            }
            else
            {
                var propertyNames = typeInfo.DefaultConstructorInfo.PropertyNames;
                args = new object[propertyNames.Length];
                for (int i = 0; i < propertyNames.Length; i++)
                {
                    //var propertyInfo = typeInfo.GetPropertyInfo(propertyNames[i]);
                    //var index = Array.IndexOf(typeInfo.ReplicatedPropertyInfos, propertyInfo);
                    var property = localObj.GetProperty(propertyNames[i]);  //await resolver.GlobalizeProperty(localObj.GetProperty(propertyNames[i]));

                    // TODO: reimpliment
                    args[i] = await GlobalizeProperty(property); ;// resolver.FilterPropertyIn(propertyInfo, property);
                }
            }

            var instance = type.GetATypeInfo().DefaultConstructorInfo.Construct(args);

            if (!id.IsEmpty())
                AddInstance(id, instance);
            
            return instance;
        }

        public async Task<object> SetProperties(object obj, object localObj)
        {
            foreach (var (propertyInfo, property) in localObj.GetPropertiesByInfo())
            {
                if (obj.GetPropertyInfo(propertyInfo.Name).CanSet)
                    SetProperty(ref obj, propertyInfo.Name, await GlobalizeProperty(property));
            }
            return obj;
        }

        /*
        public static void SetProperty(this IInstanceResolver resolver, ref object obj, string propertyName, object property)
        {
            if (resolver != null)
            {
                resolver.SetProperty(ref obj, propertyName, property);
            }
            else
            {
                obj.SetProperty(propertyName, property);
            }
        }*/

        public async Task<object> GlobalizeStructure(object localObj)
        {
            object obj;
            if (localObj == null || !localObj.IsStructure())
            {
                obj = localObj;
            }
            else
            {
                if (OS.LogFormatting)
                    OS.Log($"Globalizing object of type {localObj.GetAType().GetTypeName()}.");

                obj = await AddLocalInstanceAsync(localObj, AID.Empty);
                obj = await SetProperties(obj, localObj);
            }

            return obj;
        }

        public async Task<object> GlobalizeProperty(object property)
        {
            if (property == null)
                return null;

            if (OS.LogFormatting)
                OS.Logger.Log($"Globalizing property of type {property.GetAType().GetTypeName()} with value {property.LocalToString()}.");

            if (property != null)
            {
                if (property.GetAType().IsInstanceID())
                {
                    AID id = (AID)property;
                    if (OS.LogFormatting)
                        OS.Log($"Object is a reference with an ID of {id}.");
                    property = await GetInstanceAsync(id);
                }
                else if (property.IsStructure())
                {
                    if (OS.LogFormatting)
                        OS.Log($"Object is of type {property.GetAType().GetTypeName()} with a value of {property.LocalToString()}.");
                    property = await GlobalizeStructure(property);
                }
            }
            return property;
        }

        public LocalInstanceReference[] GetLocals()
        {
            var instances = InstancesByID.GetFirst();
            LocalInstanceReference[] locals = new LocalInstanceReference[instances.Count];
            int i = 0;
            foreach (var globalPair in instances)
            {
                var instance = globalPair.Value;

                if (OS.LogFormatting)
                    OS.Logger.Log(string.Format("Serializing {0} with an ID of {1}.", instance.GetType(), instance.GetInstanceID()));
                locals[i] = GetLocalInstanceReference(instance);
                i++;
            }

            return locals;
        }

        public LocalInstanceReference GetLocalInstanceReference(object instance)
        {
            return new LocalInstanceReference(instance.GetInstanceID(), instance.Localize());
        }
    }
}
