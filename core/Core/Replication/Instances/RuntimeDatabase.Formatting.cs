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
    public partial class RuntimeDatabase
    {
        public async Task<object> AddDereferencedInstanceAsync(AID instanceID, object localInstance)
        {
            Logger.LogError($"Adding local instance with an ID of {instanceID}!");

            var instance = await AddDereferencedInstanceAsync(localInstance);
            instance = await SetProperties(instance, localInstance);
            if (!HasInstanceID(instanceID))
                AddInstance(instanceID, instance);
            return await GetInstanceAsync(instanceID);
        }

        public IDereferencedObject[] GetDereferencedInstance()
        {
            return GetDereferencedInstanceGroup(GetInstances().ToArray());
        }

        public IDereferencedObject[] GetDereferencedInstanceGroup(object[] instances)
        {
            var localInstanceReferences = new IDereferencedObject[instances.Length];

            for (int i = 0; i < instances.Length; i++)
            {
                var instance = instances[i];
                localInstanceReferences[i] = instance.DereferenceInstance();
            }

            return localInstanceReferences;
        }

        /*
        public async Task<IEnumerable<object>> AddDereferencedInstanceGroup(Dictionary<Guid, object> localInstancesByID)
        {
            //resolver.Lock();

            var instancesByID = new Dictionary<Aid, object>();

            // Construct each local instance into a global instance
            foreach (var localInstanceByID in localInstancesByID)
            {
                var instanceID = localInstanceByID.Key;

                if (HasInstanceID(instanceID))
                    throw new Exception(string.Format("Part of local instance group is already added to the resolver!"));

                if (!instancesByID.ContainsKey(instanceID))
                {
                    await AddDereferencedInstanceAsync(localInstanceByID.Value, instanceID);
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
        */

        public async Task<object> AddDereferencedInstanceAsync(object dereferencedObj, AID id = default)
        {
            var type = dereferencedObj.GetAType().GetReferencedType();
            var typeInfo = type.GetATypeInfo();

            // Get constructor arguments for the object--if we're dealing with an array, the arguments are the array length
            object[] args;
            if (type.IsCollection())
            {
                if (type.IsArray)
                {
                    args = new object[] { dereferencedObj.GetPropertyCount() };
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
                    var propertyInfo = dereferencedObj.GetPropertyInfo(propertyNames[i]);  //await resolver.GlobalizeProperty(localObj.GetProperty(propertyNames[i]));

                    // TODO: reimpliment
                    args[i] = await ReferenceProperty(propertyInfo.Get(dereferencedObj)); ;// resolver.FilterPropertyIn(propertyInfo, property);
                }
            }

            var instance = type.GetATypeInfo().DefaultConstructorInfo.Construct(args);

            if (!id.Equals(AID.Empty))
                AddInstance(id, instance);
            
            return instance;
        }

        public async Task<object> SetProperties(object obj, object dereferencedObj)
        {
            foreach (var (propertyInfo, property) in dereferencedObj.GetPropertiesByInfo())
            {
                if (propertyInfo.CanSet)
                    SetProperty(ref obj, propertyInfo.Name, await ReferenceProperty(property));
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

        public async Task<object> ReferenceStructure(object dereferencedObj)
        {
            object obj;
            if (dereferencedObj == null || !dereferencedObj.IsStructure())
            {
                obj = dereferencedObj;
            }
            else
            {
                if (OS.LogFormatting)
                    OS.Log($"Globalizing object of type {dereferencedObj.GetAType().GetTypeName()}.");

                obj = await AddDereferencedInstanceAsync(dereferencedObj, AID.Empty);
                obj = await SetProperties(obj, dereferencedObj);
            }

            return obj;
        }

        public async Task<object> ReferenceProperty(object property)
        {
            if (property == null)
                return null;

            if (OS.LogFormatting)
                OS.Logger.Log($"Globalizing property of type {property.GetAType().GetTypeName()} with value {property.ToNestedString()}.");

            if (property != null)
            {
                if (property.GetType().Equals(typeof(AID)))
                {
                    AID id = (AID)property;
                    if (OS.LogFormatting)
                        OS.Log($"Object is a reference with an ID of {id}.");
                    property = await GetInstanceAsync(id);
                }
                else if (property.IsStructure())
                {
                    if (OS.LogFormatting)
                        OS.Log($"Object is of type {property.GetAType().GetTypeName()} with a value of {property.ToNestedString()}.");
                    property = await ReferenceStructure(property);
                }
            }
            return property;
        }

        /*
        public LocalInstanceReference[] GetDereferencedInstances()
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
            return new LocalInstanceReference(instance.GetInstanceID(), instance.Dereference());
        }
        */
    }
}
