using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Altimit;
using Altimit.Serialization;
using System.Linq;

namespace Altimit
{
    public partial class App// : IInstanceResolver
    {

        /*
        public static IEnumerable<object> AddLocalInstances(IEnumerable<LocalInstanceReference> localInstances)
        {

        }*/

        /*
        public T AddInstance<T>(T instance) where T : class
        {
            Dictionary<Guid, object> instancesByID = new Dictionary<Guid, object>();
            foreach (var _instance in InstanceDB.GetUnaddedInstances(instance))
            {
                var id = Guid.New();
                RegisterInstance(AppID, id);
                instancesByID.Add(id, _instance);
            }
            InstanceDB.AddInstanceGroup(instancesByID);
            return instance;
        }

        public object TryAddInstance(object instance)
        {
            if (InstanceDB.HasInstance(instance))
                return instance;

            return AddInstance(instance);
        }
        */

        public async void TryRemoveInstance(object instance)
        {
            if (!InstanceDB.HasInstance(instance))
                return;

            await RemoveInstance(instance);
        }

        public async Task RemoveInstance(object instance)
        {
            InstanceDB.RemoveInstance(instance);
            await PersistentDatabase.Delete(instance.GetType().GetDereferencedType(), instance.GetInstanceID());
            //UnregisterInstance(instance.GetInstanceID());
        }

        // Tries finding instance from local db
        // TODO: possibly refactor to structure codebase better
        // Tries finding instance from local db
        // TODO: possibly refactor to structure codebase better
        public async Task<object> TryFindInstance(AID instanceID)
        {
            var dereferencedInstance = await PersistentDatabase.FindOne(instanceID);
            if (dereferencedInstance == null)
            {
                //Logger.Log($"Failed to find instance with an ID of {instanceID} in local database! Returning null.");
                return null;
            }

            SetAppID(ID, instanceID);

            return await InstanceDB.AddDereferencedInstanceAsync(instanceID, dereferencedInstance);
        }

        public async Task<IEnumerable<T>> Find<T>(Func<QueryBuilder<T>, QueryNode> queryFunc) where T : class
        {
            await Save(typeof(T));

            var query = queryFunc(new QueryBuilder<T>());

            var instances = new List<T>();
            var dereferencedInstances = await PersistentDatabase.Find<T>(query.CompileToLocalBson());

            foreach (var dereferencedInstance in dereferencedInstances)
            {
                if (InstanceDB.HasInstanceID(dereferencedInstance.GetInstanceID()))
                {
                    instances.Add((T)InstanceDB.GetInstance(dereferencedInstance.GetInstanceID()));
                }
                else
                {
                    var instance = await InstanceDB.AddDereferencedInstanceAsync(dereferencedInstance);
                    instances.Add((T)instance);
                }
            }
            return instances;
        }
        public async Task<T> FindOrAddOne<T>(AID instanceID) where T : class, new()
        {
            if (InstanceDB.HasInstanceID(instanceID))
                return (T)InstanceDB.GetInstance(instanceID);

            await Save(typeof(T));

            var dereferencedInstance = await PersistentDatabase.FindOne(instanceID);

            if (dereferencedInstance == null)
            {
                return InstanceDB.CreateInstance<T>(instanceID);
            }

            return (T)await InstanceDB.AddDereferencedInstanceAsync(instanceID, dereferencedInstance);
        }


        // Finds a single instance matching search criteria
        public async Task<T> FindOne<T>(Func<QueryBuilder<T>, QueryNode> queryFunc) where T : class
        {
            await Save(typeof(T));

            var query = queryFunc(new QueryBuilder<T>());
            var dereferencedInstance = await PersistentDatabase.FindOne<T>(query.CompileToLocalBson());
            if (dereferencedInstance == null)
            {
                // OS.Log($"Couldn't find an instance of type {typeof(T)} matching search criteria, returning null!");
                return null;
            }
            if (InstanceDB.HasInstanceID(dereferencedInstance.GetInstanceID()))
                return (T)InstanceDB.GetInstance(dereferencedInstance.GetInstanceID());

            return (T)await InstanceDB.AddDereferencedInstanceAsync(dereferencedInstance.GetInstanceID(), dereferencedInstance);
        }

#if LEGACY
        public async Task<int> DeleteMany<T>(Func<QueryBuilder<T>, QueryNode> queryFunc) where T : class
        {
            await Save(typeof(T));

            var query = queryFunc(new QueryBuilder<T>());
            int deletedCount = 0;
            var locals = await LocalDB.Find<T>(query.CompileToLocalBson());
            foreach (var (instanceID, instance) in locals)
            {
                await RemoveInstance(instanceID);
                deletedCount++;
            }
            return deletedCount;
        }


        /*
        public async Task<object> AddLocalInstance(Guid id, object localInstance)
        {
            //var localsByID = new Dictionary<Guid, object>();

            return await this.ConstructInstance(localInstance, id);
            // Resolves properties, if any are missing
            //await ResolveSubInstances(id, localInstance, localsByID);

            // TODO: reimpliement
            //InstanceDB.AddLocalInstanceGroup(localsByID);
            //return InstanceDB.GetInstance(id);
        }*/


        
        /*
        public async Task ResolveSubInstances(AID instanceID, object localInstance, Dictionary<Guid, object> resolvedSubInstances)
        {
            if (OS.LogResolver)
                Logger.Log($"Resolving sub instances in local instance with an ID of {instanceID} and type {localInstance.GetType()}. Resolved instances: {resolvedSubInstances.LocalToString()}.");

            resolvedSubInstances.Add(instanceID, localInstance);

            var missingSubInstanceIDs = localInstance.GetAllProperties().Where(x => x.GetType().Equals(typeof(Guid))).
    Select(x => ((Guid)x)).Where(x => !x.IsEmpty() && !resolvedSubInstances.ContainsKey(x) && !InstanceDB.HasInstanceID(x)).Distinct().ToList();

            foreach (var missingSubInstanceID in missingSubInstanceIDs)
            {
                if (OS.LogResolver)
                    Logger.Log($"Dependent instance ID: {missingSubInstanceID}.");
                await GetInstance(missingSubInstanceID, resolvedSubInstances);
            }
        }
        */

        public bool HasInstanceID(AID instanceID)
        {
            return InstanceDB.HasInstanceID(instanceID);
        }

        public void SetProperty(ref object obj, string propertyName, object value)
        {
            obj.GetPropertyInfo(propertyName).Set(ref obj, value);
        }

        public object AddInstance(AID instanceID, object instance)
        {
            return InstanceDB.AddInstance(instanceID, instance);
        }

        public object AddInstance(object instance)
        {
            return InstanceDB.AddInstance(instance);
        }

        /*
        public async Task<object> ConstructInstance(Type localType, object localInstance)
        {
           object[] args = new object[localArgs.Length];
           for (int i = 0; i < args.Length; i++)
           {
               args[i] = this.GlobalizeProperty(localArgs[i]);
           }
           this.ConstructInstance()
           var constructorInfo = localType.GetGlobalType().GetATypeInfo().DefaultConstructorInfo;
           return constructorInfo.Construct(localArgs);
        }
        */
#endif
    }
}
