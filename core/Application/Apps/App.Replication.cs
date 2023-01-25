using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Altimit.UI;
using Altimit.Networking;
using Meridian;
using Altimit.Application.Apps;

namespace Altimit
{
    // Adds a database to an app (with long-term storage using LocalDB and short-term storage using InstanceDB)
    public partial class App// : AppModule<AppConnection>
    {
        public Node Root;
        public bool IsResolving { get; set; } = false;
        public AID ID { get; protected set; } = AID.New();
        public RuntimeDatabase InstanceDB { get; private set; }
        public PersistentDatabase PersistentDatabase { get; private set; }

        Dictionary<AID, AID> appIDsByInstance = new Dictionary<AID, AID>();
        //Dictionary<Guid, HashSet<Guid>> instancesByApp = new Dictionary<Guid, HashSet<Guid>>();

        HashSet<object> dirtyInstances = new HashSet<object>();

        //public bool IsInitialized = false;
        CancellationTokenSource tokenSource = new CancellationTokenSource();

        /*
        public ReplicationModule(AID appID)
        {

        }
        */

        public virtual async Task Init()
        {
            PersistentDatabase = new PersistentDatabase();
            // Set second parameter to false to not reset at startup
            await PersistentDatabase.Connect(Path.Combine(OS.Settings.AssetsPath, $"{ID}.db"), Logger, true);

            InstanceDB = new RuntimeDatabase(true, this.Logger);
            InstanceDB.onInstanceRemoved += OnInstanceRemoved;
            InstanceDB.onPropertyChanged += OnPropertyChanged;
            InstanceDB.onInstanceAdded += OnInstanceAdded;
            InstanceDB.onElementAdded += OnElementAdded;
            InstanceDB.onElementRemoved += OnElementRemoved;
            InstanceDB.onAssetAdded += OnAssetAdded;

            // TODO: Uncomment
            //AutoSave();

            InstanceDB.GetInstanceFuncs.Add(TryFindInstance);

            //if (OS.LogApps)
            //    Logger.Log("Connecting to master server...");

            // Establish connection to the master server socket over Websockets
            /*
            masterClientSocket = new ClientSocketWs(OS.Settings.MasterURL, OS.Settings.MasterPort);
            var masterPeerSocket = await masterClientSocket.ConnectAsync();
            cancellationToken.ThrowIfCancellationRequested();

            masterServer = DynamicMethodCallerFactory.CreateNotifyMethodCalledOfType<IReplicationServer>();
            var masterServerRPC = new RPCConnection<IReplicationClient, IReplicationServer>(new ReplicationClient(), masterServer, masterPeerSocket, Logger);

            InstanceDB.GetInstanceFuncs.Add(GetInstance);

            OnInitialized?.Invoke();
            */
            //clientSocket = networkModule.AddSocket(new ClientSocketWs(MasterURL, Settings.MasterPort));

            //await clientSocket.Connect(IP, Settings.UserPort, ctSource.Token);
            //clientSocket.PeerConnected += OnConnected;
            //clientSocket.PeerDisconnected += OnDisconnected;

            //App.InstanceDB.onInstanceAdded += OnInstanceAdded;


            // Automatically add session with self
            /*
            var selfServiceProvider = new SelfServiceProvider(App);
            selfServiceProvider.AddService(new ReplicationService(App, selfServiceProvider));
            selfServiceProvider.Peer = selfServiceProvider;
            AddSession(replicationAM.AppID, selfServiceProvider);
            */

            // Create P2P client socket over Websockets
            //p2pClientSocket = ;
            //p2pClientSocket.PeerConnected += OnConnected;
            //p2pClientSocket.PeerDisconnected += OnDisconnected;

            //var connection = new Connection<IP2PClientService, IP2PServerService>(this, p2pClientSocketPeer);

            // Add service providers for this and server with basic replication service to communicate with the P2P websocket server

            // Used to receive peer connections
            //P2PSocket
            // Temporarily removed in favor of relaying due to lack of proper functioning of WebRTC in Godot
            // TODO: Add back in once WebRTC support is reliable, but keep relaying as a fallback
            //new ServerSocketWebRTC(App.Get<ReplicationAM>().AppID, OnNegotiationNeeded, OnIceCandidate, App.Logger);
            //P2PSocket.PeerConnected += OnPeerConnected;

            // Add P2PClientSM and let peer know about this app's ID

            //throw new NotImplementedException();
            //await selfP2PServiceProvider.GetService<IP2PServerService>().SetAppID(replicationAM.AppID);

            Root = OS.Settings.GetRootNode();
            AddInstance(Root);

            CompletionSource.SetResult(true);
            OnInitialized?.Invoke();

            if (OS.LogApps)
                Logger.Log("App core initialized.");
        }

        async void AutoSave()
        {
            try
            {
                while (true)
                {
                    await Task.Delay(5);
                    tokenSource.Token.ThrowIfCancellationRequested();
                    await Save(token: tokenSource.Token);
                }
            }
            catch (OperationCanceledException) { }
        }

        public virtual void OnPropertyChanged(object instance, string propertyName, object oldProperty)
        {
            SetDirty(instance);
        }

        public virtual void OnInstanceRemoved(AID instanceID, object instance)
        {
            SetDirty(instance, false);
        }

        public virtual void OnAssetAdded(AID instanceID, byte[] bytes)
        {
        }

        public virtual void OnInstanceAdded(object instance)
        {
            var appInstance = instance as AppInstance;
            if (appInstance != null)
            {
                appInstance.SetApp(this);
            }

            //UpdateInstance(instance);
            SetDirty(instance);

            // TODO: Uncomment
            /*
            if (IsOwner(instance))
            {
                if (OS.LogReplication)
                {
                    Logger.Log($"Setting app ID for {instance.GetInstanceID()} on replication server.");
                }
                masterServer.SetAppID(instance.GetInstanceID());
            }
            */
        }

     
        public void SetDirty(object instance, bool value = true)
        {
            if (value)
            {
                if (!dirtyInstances.Contains(instance))
                    dirtyInstances.Add(instance);
            }
            else
            {
                dirtyInstances.Remove(instance);
            }
        }

        public bool IsDirty(object instance)
        {
            return dirtyInstances.Contains(instance);
        }

        public async Task Save(Type type = null, CancellationToken token = default)
        {
            try
            {
                int savedInstancesCount = 0;
                foreach (var instance in dirtyInstances.ToList())
                {
                    if (IsOwner(instance) && (type == null || instance.GetType() == type))
                    {
                        if (InstanceDB.HasInstance(instance))
                        {
                            instance.GetObserver().Update();
                            await SaveInstance(instance);
                            token.ThrowIfCancellationRequested();
                            savedInstancesCount++;
                        }
                        SetDirty(instance, false);
                    }
                }

                if (OS.LogApps && savedInstancesCount > 0)
                {
                    Logger.Log($"Saved {savedInstancesCount} instances.");
                }
            }
            catch (Exception e)
            {
                //UnityEngine.Debug.LogException(e);
                Logger.LogError(e);
            }
        }

        async Task SaveInstance(object instance)
        {
            await PersistentDatabase.Update(instance.DereferenceInstance());
        }

        public virtual void OnElementAdded(object instance, int index, object element)
        {
            SetDirty(instance);
        }

        public virtual void OnElementRemoved(object instance, int index, object element)
        {
            SetDirty(instance);
        }

        public bool IsOwner(object instance)
        {
            return GetAppID(instance).Equals(ID);
        }

        public AID GetAppID(object instance)
        {
            return GetAppID(instance.GetInstanceID());
        }

        public AID GetAppID(AID instanceID)
        {
            AID appID;
            if (!appIDsByInstance.TryGetValue(instanceID, out appID))
            {
                appID = ID;
                //Logger.LogError($"An app was not found for an instance with an ID of {instanceID} and type {instance.GetType()}.");
            }
            return appID;
        }

        public void SetAppID(AID appID, AID instanceID)
        {
            appIDsByInstance[instanceID] = appID;
        }

        public async ValueTask DisposeAsync()
        {
            //base.Dispose();
            tokenSource.Cancel();
            if (PersistentDatabase != null)
                await PersistentDatabase.DisposeAsync();
        }

        public object AddInstance(object instance)
        {
            return InstanceDB.AddInstance(instance);
        }
    }
}