using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Altimit.Serialization;
using System.Linq;
using System.Threading;

namespace Altimit.Networking
{
    public partial class ReplicationSM
    {

        TaskCompletionDictionary<object, bool> instanceOnboarding = new TaskCompletionDictionary<object, bool>();
        public Action<object> onInstanceTracked;
        public Action<object> onInstanceUntracked;

        // Called from peer session
        public async Task AddOrTrackInstance(AID instanceID)
        {
            if (OS.LogReplication)
                Logger.Log($"Tracking instance with an ID of {instanceID}.");
            var instance = await replicationAM.InstanceDB.GetInstanceAsync(instanceID);
            Lock();
            TryTrackInstance(instance);
            Unlock();
        }

        // Called from peer session
        /*
        public virtual void AddOrTrackInstance(AID instanceID, Type instanceType, object[] constructorArgs)
        {
            Lock();
            if (OS.LogApps)
                Logger.Log($"Constructing peer instance with an ID of {instanceID}, type {instanceType}, and constructor arguments {constructorArgs.GlobalToString()}.");

            var instance = instanceType.GetATypeInfo().DefaultConstructorInfo.Construct(constructorArgs);
            TryAddAndTrackInstance(P2PPeer.PeerAppID, instanceID, instance);
            Unlock();
        }
        */

        // Attempts to add an instance to this app's database and then track it in this session
        public bool TryAddAndTrackInstance(object instance)
        {
            if (!instance.IsInstance())
                return false;

            if (!replicationAM.InstanceDB.HasInstance(instance))
                replicationAM.InstanceDB.AddInstance(instance);
                //App.Replication.InstanceDB.AddInstanceOrGroup(instance);

            if (!replicationAM.InstanceDB.HasInstance(instance))
            {
                throw new Exception("Attempted to add an instance to a scope that could not be added to the scope's referenced database!");
            }

            return TryTrackInstance(instance);
        }

        /*
        public bool TryAddAndTrackInstance(Guid appID, AID instanceID, object instance)
        {
            App.Base.InstanceDB.TryAddInstance(instanceID, instance);
            if (App.Base.InstanceDB.HasInstance(instance) && App.Base.InstanceDB.GetInstanceID(instance).Equals(instanceID))
            {
                return TryTrackInstance(instance);
            }
            else
            {
                throw new Exception("Attempted to add an instance to a scope that could not be found with the same ID in the database!");
            }
        }
        */
        /*
        public bool TryAddAndTrackInstanceGroup(IDictionary<Guid, object> instancesByID)
        {
            return false;
        }
        */
        public async Task EnsureTracked(object o, CancellationToken cancellationToken)
        {
            await EnsureTracked(o, new HashSet<object>(), cancellationToken);
        }

        // Recursively runs through objects and tracks anything needed. Returns true if something was tracked--false if nothing was
        public async Task EnsureTracked(object o, HashSet<object> triedObjects, CancellationToken cancellationToken)
        {
            if (o == null)
                return;

            if (TypeExtensions.IsReplicatedInstance(o))
            {
                InstanceDB.TryAddInstance(o);
                if (!IsTrackedInstance(o))
                {
                    if (!instanceOnboarding.ContainsKey(o))
                    {
                        instanceOnboarding.AddTask(o);
                        // go through async process of making sure peer has instance
                        await Peer.AddOrTrackInstance(o.GetInstanceID());
                        cancellationToken.ThrowIfCancellationRequested();

                        instanceOnboarding.SetResult(o, true);
                    }
                    else
                    {
                        await instanceOnboarding.AddOrGetTask(o);
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                    TryTrackInstance(o);
                }
            }
            else
            {
                if (o.IsStructure())
                {
                    triedObjects.Add(o);

                    foreach (var property in TypeExtensions.GetProperties(o))
                    {
                        if (!triedObjects.Contains(property)) {
                            await EnsureTracked(property, triedObjects, cancellationToken);
                            cancellationToken.ThrowIfCancellationRequested();
                        }
                    }
                }
            }
        }

        bool TryTrackInstance(object instance)
        {
            if (IsTrackedInstance(instance))
                return false;

            TrackInstance(instance);
            return true;
        }

        /*
        // Attempts to add the object to this app's database and then track it in this session
        public bool TryAddAndTrackObject(object o)
        {
            return TryAddAndTrackObject(o, new HashSet<object>());
        }

        public bool TryAddAndTrackObject(object o, HashSet<object> triedObjects)
        {
            if (o == null)
                return false;

            if (o.IsInstance())
            {
                return TryAddAndTrackInstance(o);
            }
            else
            {
                if (o.IsStructure())
                {
                    triedObjects.Add(o);

                    foreach (var property in TypeExtensions.GetProperties(o))
                    {
                        if (!triedObjects.Contains(property))
                            TryAddAndTrackObject(property, triedObjects);
                    }
                }
                return false;
            }
        }

        public bool TryTrackObject(object o)
        {
            if (o == null)
                return false;

            if (o.IsInstance())
            {
                return TryTrackInstance(o);
            }
            else
            {
                foreach (var property in o.GetProperties())
                {
                    TryTrackObject(property);
                }
                return false;
            }  
        }*/

        // Registers an instance as relevant to this session. Lets the session's peer know about it as well.
        void TrackInstance(object instance)
        {
            /*
            if (isLocked)
            {
                TrackSubInstance(instance);
                return;
            }
            */
            //logger.Log(instanceDatabase.GetInstanceID(instance));
            //logger.Log(instanceDatabase.HasInstance(instance));
            if (OS.LogReplication)
                Logger.Log($"Tracking instance with an ID of { instance.GetInstanceID()}, letting peer know.");
            //Peer.AddOrTrackInstance(App.Base.InstanceDB.GetInstanceID(instance));

            TrackedInstances.Add(instance);
            OnInstanceTracked(instance);
            // Fires construction, method call, and property change events in an appropriate order for re-assembling newly tracked instances from events

            /*
            // For this instance and any untracked subInstances--mark them as tracked and construct them
            List<object> subInstances = new List<object>();
            foreach (var subObjectParameters in TypeExtensions.GetAllSubObjects(instance, true))
            {
                TryTrackSubObject(subObjectParameters.SubObject, subInstances, new HashSet<object>());
            }

            // Set properties that were not included in construction
            foreach (var subInstance in subInstances)
            {
                var typeInfo = subInstance.GetTypeInfo();
                foreach (var subObjectParameters in TypeExtensions.GetSubObjects(subInstance))
                {
                    if (subInstance.GetType().IsArray)
                    {
                        // Will require something here, or removal of arrays as acceptable types
                        throw new NotImplementedException();
                        //PeerSession.CallMethod(subInstance, subInstance.GetMethodID("Insert", new Type[] { typeof(int), subInstance.GetType().GetElementType() }), subObjectParameters.SubObject);
                    }
                    else if (subInstance.GetType().IsCollectionType())
                    {
                        PeerSession.CallMethod(subInstance, subInstance.GetMethodID("Add", new Type[] { subInstance.GetType().GetCollectionElementType() }), subObjectParameters.SubObject);
                    }
                    else
                    {
                        if (!typeInfo.DefaultConstructorInfo.IDs.Contains(subObjectParameters.PropertyID))
                        {
                            if (OS.LogSessions)
                                OS.LogFormat("Setting initial value for property named {0} to {1}.",subInstance.GetPropertyInfo(subObjectParameters.PropertyID),
                                    subObjectParameters.SubObject.ToFormattedString());
                            PeerSession.ChangeProperty(subInstance, subObjectParameters.PropertyID, subObjectParameters.SubObject);
                        }
                    }
                    OnInstanceTracked(subInstance);
                }
            }
            */
        }
        /*
        void TryTrackSubObject(object subObject, List<object> subInstances, HashSet<object> pendingInstances)
        {
            if (instanceDatabase.IsValidInstance(subObject) && !IsTrackedInstance(subObject))
            {
                pendingInstances.Add(subObject);
                subInstances.Add(subObject);
                TrackSubInstance(subObject);
                
                foreach (var constructorArg in subObject.GetDefaultConstructorArgs())
                {
                    TryTrackSubObject(constructorArg, subInstances, pendingInstances);
                    if (pendingInstances.Contains(constructorArg))
                    {
                        logger.LogErrorFormat("A circular dependency occurred between two instance's constructor arguments! The first instance's ID was {0} and the second instance's ID was {1}.",
                            instanceDatabase.GetInstanceID(constructorArg), instanceDatabase.GetInstanceID(subObject));
                    }
                }

                if (OS.LogSessions)
                   logger.LogFormat("Sending construction of instance of type {0} with constructor arguments {1}.", subObject.GetType(), subObject.GetDefaultConstructorArgs().ToFormattedString());

                if (instanceDatabase.IsOwner(subObject))
                {
                    PeerSession.AddOrTrackInstance(instanceDatabase.GetInstanceID(subObject), subObject.GetType(), subObject.GetDefaultConstructorArgs());
                } else
                {
                    PeerSession.AddOrTrackInstance(instanceDatabase.GetAppByInstance(subObject), instanceDatabase.GetInstanceID(subObject));
                }
                pendingInstances.Remove(subObject);
            }
        }
        */
        /*
        void TryTrackSubInstance(object instance)
        {
            if (!IsTrackedInstance(instance))
                TrackSubInstance(instance);
        }

        void TrackSubInstance(object instance)
        {
            //if (OS.LogApps)
            //    logger.LogFormat("Tracking instance with an ID of {0}.", instanceID);

        }
        */

        void UntrackInstance(AID instanceID, object instance)
        {
            // If this instance is one owned by the peer
            if (replicationAM.GetAppID(instanceID).Equals(PeerAppID))
                InstanceDB.TryRemoveInstance(instance);

            TrackedInstances.Remove(instance);
            onInstanceUntracked?.Invoke(instance);
        }

        public virtual void OnInstanceTracked(object instance)
        {
            onInstanceTracked?.Invoke(instance);
        }

        public bool IsTrackedInstance(object instance)
        {
            return TrackedInstances.Contains(instance);
        }
    }
}
