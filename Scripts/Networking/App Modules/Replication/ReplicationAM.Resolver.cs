using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Altimit;
using Altimit.Serialization;
using System.Linq;

namespace Altimit.Networking
{
    public partial class ReplicationAM// : IInstanceResolver
    {

        /*
        public static IEnumerable<object> AddLocalInstances(IEnumerable<LocalInstanceReference> localInstances)
        {

        }*/

        /*
        public T AddInstance<T>(T instance) where T : class
        {
            Dictionary<AID, object> instancesByID = new Dictionary<AID, object>();
            foreach (var _instance in InstanceDB.GetUnaddedInstances(instance))
            {
                var id = AID.New();
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
            await LocalDB.Delete(instance.GetType().GetLocalType(), instance.GetInstanceID());
            //UnregisterInstance(instance.GetInstanceID());
        }

        // Tries finding instance from local db
        // TODO: possibly refactor to structure codebase better
        public async Task<object> TryFindInstance(AID instanceID)
        {
            throw new NotImplementedException();
        }


#if LEGACY
        // Tries finding instance from local db
        // TODO: possibly refactor to structure codebase better
        public async Task<object> TryFindInstance(AID instanceID)
        {
            var (_, localInstance) = await LocalDB.FindOne(instanceID);
            if (localInstance == null)
            {
                //Logger.Log($"Failed to find instance with an ID of {instanceID} in local database! Returning null.");
                return null;
            }

            SetAppID(AppID, instanceID);

            return await InstanceDB.AddLocalInstanceAsync(instanceID, localInstance);
        }

        public async Task<IEnumerable<T>> Find<T>(Func<QueryBuilder<T>, QueryNode> queryFunc) where T : class
        {
            await Save(typeof(T));

            var query = queryFunc(new QueryBuilder<T>());

            var instances = new List<T>();
            var localInstances = await LocalDB.Find<Local<T>>(query.CompileToLocalBson());
            foreach (var (instanceID, localInstance) in localInstances)
            {
                if (InstanceDB.HasInstanceID(instanceID))
                {
                    instances.Add((T)InstanceDB.GetInstance(instanceID));
                }
                else
                {
                    var instance = await InstanceDB.AddLocalInstanceAsync(instanceID, localInstance);
                    instances.Add((T)instance);
                }
            }
            return instances;
        }

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

        public async Task<T> FindOrAddOne<T>(AID instanceID) where T : class, new()
        {
            if (InstanceDB.HasInstanceID(instanceID))
                return (T)InstanceDB.GetInstance(instanceID);

            await Save(typeof(T));

            var (_, localInstance) = await LocalDB.FindOne(instanceID);

            if (localInstance == null)
            {
                return InstanceDB.CreateInstance<T>(instanceID);
            }

            return (T)await InstanceDB.AddLocalInstanceAsync(instanceID, localInstance);
        }


        // Finds a single instance matching search criteria
        public async Task<T> FindOne<T>(Func<QueryBuilder<T>, QueryNode> queryFunc) where T : class
        {
            await Save(typeof(T));

            var query = queryFunc(new QueryBuilder<T>());
            var (instanceID, localInstance) = await LocalDB.FindOne<Local<T>>(query.CompileToLocalBson());
            if (localInstance == null)
            {
               // OS.Log($"Couldn't find an instance of type {typeof(T)} matching search criteria, returning null!");
                return null;
            }
            if (InstanceDB.HasInstanceID(instanceID))
                return (T)InstanceDB.GetInstance(instanceID);

            return (T)await InstanceDB.AddLocalInstanceAsync(instanceID, localInstance);
        }

        /*
        public async Task<object> AddLocalInstance(AID id, object localInstance)
        {
            //var localsByID = new Dictionary<AID, object>();

            return await this.ConstructInstance(localInstance, id);
            // Resolves properties, if any are missing
            //await ResolveSubInstances(id, localInstance, localsByID);

            // TODO: reimpliement
            //InstanceDB.AddLocalInstanceGroup(localsByID);
            //return InstanceDB.GetInstance(id);
        }*/


        
        /*
        public async Task ResolveSubInstances(AID instanceID, object localInstance, Dictionary<AID, object> resolvedSubInstances)
        {
            if (OS.LogResolver)
                Logger.Log($"Resolving sub instances in local instance with an ID of {instanceID} and type {localInstance.GetType()}. Resolved instances: {resolvedSubInstances.LocalToString()}.");

            resolvedSubInstances.Add(instanceID, localInstance);

            var missingSubInstanceIDs = localInstance.GetAllProperties().Where(x => x.GetType().Equals(typeof(AID))).
    Select(x => ((AID)x)).Where(x => !x.IsEmpty() && !resolvedSubInstances.ContainsKey(x) && !InstanceDB.HasInstanceID(x)).Distinct().ToList();

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
