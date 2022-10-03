using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Altimit;
using Altimit.Networking;

namespace Altimit.Networking
{
    // Adds a database to an app (with long-term storage using LocalDB and short-term storage using InstanceDB)
    public partial class ReplicationAM : AppModule<ReplicationSM>
    {
        public Action OnInitialized;
        public bool IsResolving { get; set; } = false;
        public Guid AppID { get; protected set; } = Guid.NewGuid();
        public InstanceDatabase InstanceDB { get; private set; }
        public LocalInstanceDatabase LocalDB { get; private set; }

        Dictionary<AID, Guid> appIDsByInstance = new Dictionary<AID, Guid>();
        //Dictionary<Guid, HashSet<AID>> instancesByApp = new Dictionary<Guid, HashSet<AID>>();

        HashSet<object> dirtyInstances = new HashSet<object>();

        public bool IsInitialized = false;
        CancellationTokenSource tokenSource = new CancellationTokenSource();

        public ReplicationAM(Guid appID)
        {
            AppID = appID;
        }

        public override async void Init()
        {
            LocalDB = new LocalInstanceDatabase();
            // Set second parameter to false to not reset at startup
            await LocalDB.Connect(Path.Combine(OS.Settings.AssetsPath, $"{AppID}.db"), Logger, true);

            InstanceDB = new InstanceDatabase(true, this.Logger);
            InstanceDB.onInstanceRemoved += OnInstanceRemoved;
            InstanceDB.onPropertyChanged += OnPropertyChanged;
            InstanceDB.onInstanceAdded += OnInstanceAdded;
            InstanceDB.onElementAdded += OnElementAdded;
            InstanceDB.onElementRemoved += OnElementRemoved;
            InstanceDB.onAssetAdded += OnAssetAdded;
            InstanceDB.GetInstanceFuncs.Add(TryFindInstance);

            AutoSave();

            OnInitialized?.Invoke();
            IsInitialized = true;
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
                appInstance.SetApp(App);
            }

            //UpdateInstance(instance);
            SetDirty(instance);
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
                            await UpdateInstance(instance);
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

        async Task UpdateInstance(object instance)
        {
            await LocalDB.Update(instance.GetInstanceID(), instance.Localize());
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
            return GetAppID(instance) == AppID;
        }

        public Guid GetAppID(object instance)
        {
            return GetAppID(instance.GetInstanceID());
        }

        public Guid GetAppID(AID instanceID)
        {
            Guid appID;
            if (!appIDsByInstance.TryGetValue(instanceID, out appID))
            {
                appID = AppID;
                //Logger.LogError($"An app was not found for an instance with an ID of {instanceID} and type {instance.GetType()}.");
            }
            return appID;
        }

        public void SetAppID(Guid appID, AID instanceID)
        {
            appIDsByInstance[instanceID] = appID;
        }

        public override void Dispose()
        {
            base.Dispose();
            tokenSource.Cancel();
            LocalDB?.Dispose();
        }

        public virtual bool HasAuthority(object instance, string propertyName)
        {
            return App.Get<P2PClientAM>().GetSession(GetAppID(instance)).Get<ReplicationSM>().HasAuthority(instance, propertyName);
        }

        /*
        public async Task<SelfSession> ConnectToApp(ConnectionSettings settings) {
            ISocketPeer socketPeer;
            switch (settings.Type)
            {
                case ServerType.WebSocketServer:
                    socketPeer = await new ClientSocketWs(settings.IP, settings.Port).Connect();
                    OS.Log("CONNECTED");
                    break;
                case ServerType.P2PServer:
                    socketPeer = null;
                    break;
                default:
                    socketPeer = null;
                    break;
            }
            var session = new SelfSession(App, socketPeer);

            return session;
            //networkModule.AddSession(session);
            //MasterClientSession = session;
            //return new Response<SelfSession>(session);
        }*/


        /*
        public async Task<SelfSession> AddOrGetSession(Guid appID)
        {

            return null;
        }*/
    }
}